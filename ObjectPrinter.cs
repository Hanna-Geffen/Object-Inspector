using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Object_Inspector
{
    public interface ObjectVisitor
    {
        public void Primitive(Object obj);

        public void ObjectStart(string name);

        public void ObjectField(string name);

        public void OnInfiniteLoop(string name);

        public void ObjectEnd();

        public void CollectionStart(bool isArray);

        public void CollectionItem();

        public void CollectionInterval(bool isLast);

        public void CollectionEnd(bool isArray);

        public void OnException(Exception e);
    }

    public class ConsoleObjectPrinter : ObjectVisitor
    {
        private string indent = "";

        public void Primitive(Object obj)
        {
            Console.WriteLine(obj == null ? "null" : obj);
        }

        public void ObjectStart(string name)
        {
            Console.WriteLine("Object of Class \"" + name + "\"");
            Console.WriteLine(indent + "------------------------");
            IncrementIndent();
        }

        public void ObjectField(string name)
        {
            Console.Write(indent + name + " = ");
        }

        public void ObjectEnd()
        {
            DecrementIndent();
        }

        public void CollectionStart(bool isArray)
        {
            IncrementIndent();
            Console.WriteLine(isArray ? "[" : "{");
        }

        public void CollectionItem()
        {
            Console.Write(indent);
        }

        public void CollectionInterval(bool isLast)
        {

        }

        public void CollectionEnd(bool isArray)
        {
            DecrementIndent();
            Console.Write(indent);
            Console.WriteLine(isArray ? "]" : "}");
        }

        public void OnException(Exception e)
        {
            Console.WriteLine("Something went wrong...\n" + e);
        }

        public void OnInfiniteLoop(string name)
        {
            Console.WriteLine("Infinite loop detected.");
            Console.WriteLine(indent + "Object of Class \"" + name + "\"");
            Console.WriteLine(indent + "------------------------");
        }

        private void IncrementIndent()
        {
            indent += ".\t";
        }

        private void DecrementIndent()
        {
            indent = indent.Remove(indent.Length - 2);
        }
    }

    public class ObjectInspector
    {
        private ObjectVisitor visitor;
        HashSet<int> ObjectSet = new HashSet<int>();

        public ObjectInspector(ObjectVisitor visitor)
        {
            this.visitor = visitor;
        }

        public void Accept(Object obj)
        {
            try
            {
                if (obj == null)
                {
                    visitor.Primitive(null);
                    return;
                }

                Type type = obj.GetType();

                if (IsPrimitive(type))
                {
                    visitor.Primitive(obj);
                }
                else if (IsCollection(type))
                {
                    visitor.CollectionStart(type.IsArray);

                    IEnumerable collection = (IEnumerable)obj;

                    int collectionLength = 0;
                    foreach (object item in collection)
                    {
                        collectionLength++;
                    }

                    bool isLast = false;
                    int i = 0;
                    foreach (object item in collection)
                    {
                        visitor.CollectionItem();
                        Accept(item);
                        if (i == collectionLength - 1)
                        {
                            isLast = true;
                        }
                        visitor.CollectionInterval(isLast);
                        i++;
                    }

                    visitor.CollectionEnd(type.IsArray);
                }
                else
                {
                    if (ObjectSet.Contains(obj.GetHashCode()))
                    {
                        visitor.OnInfiniteLoop(type.Name);
                        return;
                    }

                    ObjectSet.Add(obj.GetHashCode());

                    visitor.ObjectStart(type.Name);

                    FieldInfo[] fields = type.GetFields();

                    foreach (FieldInfo field in fields)
                    {
                        visitor.ObjectField(field.Name);
                        object fieldObject = field.GetValue(obj);
                        Accept(fieldObject);
                    }

                    ObjectSet.Remove(obj.GetHashCode());
                    visitor.ObjectEnd();
                }
            }
            catch (Exception e)
            {
                visitor.OnException(e);
            }
        }

        private static bool IsPrimitive(Type type)
        {
            return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);
        }

        private static bool IsCollection(Type type)
        {
            if (type.IsArray)
            {
                return true;
            }

            Type[] interfaces = type.GetInterfaces();

            foreach (Type iface in interfaces)
            {
                if (iface.IsAssignableFrom(typeof(IEnumerable<>)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}