using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                if (args[0] == "-E" && args.Length == 4)
                {
                    JPEGEncode jpg = new JPEGEncode();
                    byte[] encodedData = jpg.Encode(args[2], Convert.ToInt32(args[1].Substring(1, args[1].Length - 1)));

                    BinaryWriter write = new BinaryWriter(File.Open(args[3], FileMode.OpenOrCreate));
                    write.Write(encodedData);
                    write.Close();
                }
                else if (args[0] == "-D")
                {
                    long length = new FileInfo(args[1]).Length;
                    BinaryReader reader = new BinaryReader(File.Open(args[1], FileMode.Open));
                    byte[] data = new byte[length];
                    reader.Read(data, 0, Convert.ToInt32(length));

                    //jpg.DecompressImage(data, args[2]);
                }
                else
                {
                    Console.WriteLine("Check if arguments are ok.");
                }
            }
            else
            {
                Console.WriteLine("Check if arguments are set.");
            }

            //Console.ReadKey();
        }
    }
}
