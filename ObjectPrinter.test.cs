using System;
using System.Collections;
using System.Collections.Generic;

namespace Object_Inspector
{
    public class Name
    {
        public string firstName;
        public string lastName;
        public List<string> nicknames;
    }

    public class Person
    {
        public int age;
        public Name name;
        public int height;
        public string[] hobies;
        public Person[] friends;
        public Queue dreams;
    }

    public enum Color
    {
        red,
        yellow,
        green
    }

    public class ObjectPrinterTests
    {
        private static void RunTest(string testName, object obj)
        {
            ConsoleObjectPrinter printer = new ConsoleObjectPrinter();
            ObjectInspector inspector = new ObjectInspector(printer);

            Console.WriteLine("Test: " + testName);
            Console.WriteLine("===================");
            inspector.Accept(obj);
            Console.WriteLine();
        }

        private static void RunTests()
        {
            RunTest("null object", null);

            RunTest("int", 1);

            RunTest("string", "one");

            RunTest("float", 1.01);

            RunTest("bool", true);

            RunTest("enum", Color.green);

            RunTest("Boolean", new Boolean());

            RunTest("int array", new int[] { 0, 1, 2, 3 });

            RunTest("string array", new string[] { "one", "two", "three", "four" });

            RunTest("string list", new List<string>() { "one", "two", "three", "four" });

            RunTest("int matrix", new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 0, 1, 2, 3 }, new int[] { 0, 1, 2, 3 } });

            Name n1 = new Name();
            n1.firstName = "Sarah";
            n1.lastName = "Cohen";
            n1.nicknames = new List<string>() { "Sari", "Sarale" };

            Person sarah = new Person();
            sarah.age = 26;
            sarah.name = n1;
            sarah.height = 175;
            sarah.hobies = new string[] { "dancing" };
            sarah.friends = null;

            Name n2 = new Name();
            n2.firstName = "Bat-Sheva";
            n2.lastName = "Ayoun";
            n2.nicknames = new List<string>() { "Batshevi", "Batchu" };

            Person batsheva = new Person();
            batsheva.age = 24;
            batsheva.name = n2;
            batsheva.height = 160;
            batsheva.hobies = null;
            batsheva.friends = new Person[] { sarah };

            Name n3 = new Name();
            n3.firstName = "Hanna";
            n3.lastName = "Geffen";
            n3.nicknames = null;

            RunTest("Name object", n1);

            Person hanna = new Person();
            hanna.age = 25;
            hanna.name = n3;
            hanna.height = 165;
            hanna.hobies = new string[] { "art", "painting", "music" };
            hanna.dreams = new Queue();
            hanna.dreams.Enqueue("car");
            hanna.dreams.Enqueue(3.14);
            hanna.dreams.Enqueue(26000);
            hanna.dreams.Enqueue("hybrid");
            hanna.friends = new Person[] { sarah, batsheva };

            RunTest("Person object", hanna);

            RunTest("Person List", new List<Person>() { hanna, sarah, batsheva });

            hanna.friends = new Person[] { hanna, hanna, hanna };

            RunTest("Infinite Loop", hanna);
        }

        static void Main(string[] args)
        {
            RunTests();
        }
    }
}