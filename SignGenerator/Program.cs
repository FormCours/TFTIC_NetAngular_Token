using System;

using System.Security.Cryptography;

namespace SignGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            HMACSHA512 hmac = new HMACSHA512();
            string key = Convert.ToBase64String(hmac.Key);

            Console.WriteLine(key);
        }
    }
}
