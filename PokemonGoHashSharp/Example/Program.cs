using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
   class Program
   {
      static void Main(string[] args)
      {
         Run(Encoding.UTF8.GetBytes("Sample input data.."));
         Console.WriteLine("Enter input to hash...");

         while(true)
         {
            Run(Encoding.UTF8.GetBytes(Console.ReadLine()));
         }
      }

      static void Run(byte[] input)
      {
         byte[] iv = new byte[32];
         new Random().NextBytes(iv);

         byte[] output = PokemonGoHashSharp.Util.Hash(input, iv);

         string hex = BitConverter.ToString(output).Replace("-", "").ToLowerInvariant();
         Console.WriteLine("Output:");
         Console.WriteLine(hex);
      }
   }
}
