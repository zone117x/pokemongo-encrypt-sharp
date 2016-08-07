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
         byte[] output;
         int result;
         int resultOutputLength = 0;

         //32 bit
         if (IntPtr.Size == 4)
         {
            uint outputSize = 0;
            native_encrypt(input, (uint)input.Length, iv, (uint)iv.Length, null, ref outputSize);
            output = new byte[outputSize];
            result = native_encrypt(input, (uint)input.Length, iv, (uint)iv.Length, output, ref outputSize);
            resultOutputLength = (int)outputSize;
         }

         //64 bit
         else if (IntPtr.Size == 8)
         {
            ulong outputSize = 0;
            native_encrypt_x64(input, (ulong)input.Length, iv, (ulong)iv.Length, null, ref outputSize);
            output = new byte[outputSize];
            result = native_encrypt_x64(input, (ulong)input.Length, iv, (ulong)iv.Length, output, ref outputSize);
            resultOutputLength = (int)outputSize;
         }

         else throw new ApplicationException("wtf this running on?");

         if (result != 0)
         {
            throw new ApplicationException($"Failed with result code {result}");
         }

         if (output.Length != resultOutputLength)
         {
            throw new ApplicationException($"Output size discrepancy: actual {output.Length} vs expected {resultOutputLength}");
         }

         return output;
      }

   }
}
