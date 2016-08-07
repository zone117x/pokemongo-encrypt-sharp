using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGoHashNative
{
    public class Util
    {

      [DllImport("encryptx64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "encrypt")]
      static extern int native_encrypt_x64(byte[] input, ulong inputSize, byte[] iv, ulong ivSize, byte[] output, ref ulong outputSize);

      [DllImport("encrypt", CallingConvention = CallingConvention.Cdecl, EntryPoint = "encrypt")]
      static extern int native_encrypt(byte[] input, uint inputSize, byte[] iv, uint ivSize, byte[] output, ref uint outputSize);


      public static byte[] Hash(byte[] input, byte[] iv)
      {
         byte[] output = new byte[ushort.MaxValue];

         int result;
         int outputLength;

         //32 bit
         if (IntPtr.Size == 4)
         {
            var outL = (uint)output.Length;
            result = native_encrypt(input, (uint)input.Length, iv, (uint)iv.Length, output, ref outL);
            outputLength = (int)outL;
         }
         //64 bit
         else if (IntPtr.Size == 8)
         {
            var outL = (ulong)output.Length;
            result = native_encrypt_x64(input, (ulong)input.Length, iv, (ulong)iv.Length, output, ref outL);
            outputLength = (int)outL;
         }
         else throw new ApplicationException("wtf this running on?");

         if (result != 0)
         {
            throw new ApplicationException($"Failed with result code {result}");
         }

         var truncatedOutput = new byte[outputLength];
         Buffer.BlockCopy(output, 0, truncatedOutput, 0, outputLength);
         return truncatedOutput;
      }

   }
}
