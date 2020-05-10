using System;
using System.Collections.Generic;
using System.IO;
using DoubleLinkedListSerializer.Lib.Models;

namespace DoubleLinkedListSerializer.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of elements in the list");
            var countStr = Console.ReadLine();
            if (!int.TryParse(countStr, out var count))
            {
                Console.WriteLine("It's not number");
                return;
            }

            var listRandom = new List<(string collection, int indexes)>();
            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"Enter the {i} element");
                var data = Console.ReadLine();
                listRandom.Add((data, rnd.Next(count)));
            }

            var list = new ListRandom(listRandom);
            Console.WriteLine(list.ToString());

            using (var memStream = new MemoryStream())
            {
                Console.WriteLine("Start Serialize list");
                list.Serialize(memStream);
                Console.WriteLine("Finish Serialize list");
                list = new ListRandom(new List<(string collection, int indexes)>());
                Console.WriteLine("Start Deserialize list");
                list.Deserialize(memStream);
                Console.WriteLine("Finish Deserialize list");
            }

            Console.WriteLine(list.ToString());
        }
    }
}
