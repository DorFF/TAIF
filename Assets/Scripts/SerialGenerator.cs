using System.Collections.Generic;
 using System.Security.Cryptography;
 using System.Linq;
 
 public static class SerialGenerator
 {
     static RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider();
     static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
 
     public enum SerialResult
     {
         Valid,
         Invalid,
         Expired
     }
 
     public static byte[] GenerateSerial(System.DateTime aExpires)
     {
         long date = aExpires.Ticks;
         byte[] bytes = System.BitConverter.GetBytes(date);
         if (System.BitConverter.IsLittleEndian)
             System.Array.Reverse(bytes); 
         byte[] randomData = new byte[8];
         rnd.GetBytes(randomData);
         bytes = bytes.Concat(randomData).ToArray();
         byte[] hash1 = md5.ComputeHash(bytes);
         byte[] hash2 = md5.ComputeHash(hash1);
         byte[] result = new byte[32];
         for (int i = 0; i < 16; i++)
             result[i] = hash1[i];
         for (int i = 0; i < 16; i++)
             result[i + 16] = (byte)(bytes[i] ^ hash2[i]);
         return result;
     }
     public static System.TimeSpan T;
     public static SerialResult ValidateSerial(byte[] aSerial)
     {
         long t = long.MinValue;
         
         if (aSerial.Length != 32)
             return SerialResult.Invalid;
         byte[] hash2 = md5.ComputeHash(aSerial,0,16);
         byte[] data = new byte[16];
         for(int i = 0; i < 16; i++)
             data[i] = (byte)(aSerial[16+i] ^ hash2[i]);
         byte[] bytes = new byte[8];
         System.Array.Copy(data, bytes, 8);
         if (System.BitConverter.IsLittleEndian)
             System.Array.Reverse(bytes);
         long date = System.BitConverter.ToInt64(bytes,0);
         byte[] hash1 = md5.ComputeHash(data);
         for(int i = 0; i < 16; i++)
             if (aSerial[i] != hash1[i])
                 return SerialResult.Invalid;
         if (System.DateTime.UtcNow.Ticks > date)
             return SerialResult.Expired;
         return SerialResult.Valid;
     }
     public static string GenerateStringSerial(System.DateTime aExpires)
     {
         var serial = GenerateSerial(aExpires);
         return System.Convert.ToBase64String(serial);
     }
     public static SerialResult ValidateStringSerial(string aSerial)
     {
         try
         {
             var data = System.Convert.FromBase64String(aSerial);
             return ValidateSerial(data);
         }
         catch
         {
             return SerialResult.Invalid;
         }
     }
 }