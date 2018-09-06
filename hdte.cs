using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace hdt_encryption
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Invalid arugment length");
                return 33;
            }
            var command = args[0];
            var inputPath = args[1];
            var outputPath = args[2];

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("The file doesn't exist");
                return 2;
            }
            var inputBytes = File.ReadAllBytes(inputPath);
            byte[] outputBytes;
            switch (command)
            {
                case "encrypt":
                    outputBytes = ProtectedData.Protect(inputBytes, null, DataProtectionScope.LocalMachine);
                    break;
                case "decrypt":
                    outputBytes =  ProtectedData.Unprotect(inputBytes, null, DataProtectionScope.LocalMachine);
                    break;
                default:
                    Console.WriteLine("The command must be encrypt or decrypt");
                    return 1;
            }
            using (var fs = new FileStream(outputPath, FileMode.Create))
                fs.Write(outputBytes, 0, outputBytes.Length);
            return 0;
        }
    }
}
