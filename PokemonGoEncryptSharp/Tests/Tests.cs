using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Tests
{
   class Tests
   {
      static void Main(string[] args)
      {
         RunStoredTestData();

         var sampleOutputNative = TestNative();
         var sampleOutputManaged = TestManaged();

         for (var i = 0; i < Math.Min(sampleOutputNative.Length, sampleOutputManaged.Length); i++)
         {
            if (sampleOutputNative[i] != sampleOutputManaged[i])
            {
               throw new Exception($"Output buffer difference at index {i}: native is {sampleOutputNative[i]}, managed is {sampleOutputManaged[i]}");
            }
         }

         if (sampleOutputManaged.Length != sampleOutputNative.Length)
         {
            throw new Exception($"Output length differences: native is {sampleOutputNative.Length}, managed is {sampleOutputManaged.Length}");
         }

         Console.WriteLine("Success! Managed code produces same output as native");

         PerfTest();

         Console.ReadLine();
      }

      static void RunStoredTestData()
      {
         var data = File.ReadAllText("test-data.json");
         int index = 0;
         var failedTests = new List<int>();
         foreach (var entry in JsonConvert.DeserializeAnonymousType(data, new[] { new { iv = "", input = "", output = "" } }))
         {
            var output = PokemonGoEncryptNative.Util.Encrypt(Convert.FromBase64String(entry.input), Convert.FromBase64String(entry.iv));
            if (Convert.ToBase64String(output) != entry.output)
            {
               failedTests.Add(index);
            }
            index++;
         }

         if (failedTests.Any())
         {
            Console.WriteLine("Stored tests failed: " + string.Join(", ", failedTests));
         }
         else
         {
            Console.WriteLine("All stored tests passed");
         }
      }

      static void PerfTest()
      {
         double rounds = 3000;
         Console.WriteLine($"Performing {rounds} encryption operations with both native and managed functions...");

         var sw = new Stopwatch();

         sw.Start();
         for (var i = 0; i < rounds; i++)
         {
            TestNative();
         }
         sw.Stop();
         var nativeTook = sw.Elapsed;

         sw.Restart();
         for (var i = 0; i < rounds; i++)
         {
            TestManaged();
         }
         sw.Stop();
         var managedTook = sw.Elapsed;

         var avgNative = TimeSpan.FromTicks((long)(nativeTook.Ticks / rounds));
         var avgManaged = TimeSpan.FromTicks((long)(managedTook.Ticks / rounds));

         // 16x
         var diffx = managedTook.TotalSeconds / nativeTook.TotalSeconds;

         Console.WriteLine($"native took {Math.Round(avgNative.TotalMilliseconds, 4)} ms per round, total: {Math.Round(nativeTook.TotalMilliseconds, 2)} ms");
         Console.WriteLine($"managed took {Math.Round(avgManaged.TotalMilliseconds, 4)} ms per round, total: {Math.Round(managedTook.TotalMilliseconds, 2)} ms");
         Console.WriteLine($"managed is {Math.Round(diffx, 2)} times slower");
      }

      static byte[] TestNative()
      {
         byte[] input;
         byte[] iv;
         GetSampleData(out input, out iv);

         var output = PokemonGoEncryptNative.Util.Encrypt(input, iv);
         return output;
      }

      static byte[] TestManaged()
      {
         byte[] input;
         byte[] iv;
         GetSampleData(out input, out iv);

         var output = PokemonGoEncryptSharp.Util.Encrypt(input, iv);
         return output;
      }

      static void GetSampleData(out byte[] input, out byte[] iv)
      {
         input = new byte[5000];
         for (var i = 0; i < input.Length; i++)
         {
            input[i] = (byte)i;
         }
         iv = new byte[32];
         for (var i = 0; i < iv.Length; i++)
         {
            iv[i] = (byte)i;
         }
      }
   }
}
