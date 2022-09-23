// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Text;

namespace BinaryToTCDecoder
{
    enum BitsQuant
    {
        BIT8 = 1,
        BIT16 = 2,
        BIT32 = 4,
        BIT64 = 8,
    }

    class TCConverter
    {
        public static string Usage()
        {
            return "Program Usage: [input] [output] <bits 8|16|32|64 (optional)>";
        }
        public static void Main(String[] args)
        {
            // Thanks Tim Lloyd
            void Reverse(BitArray array)
            {
                int length = array.Length;
                int mid = (length / 2);

                for (int i = 0; i < mid; i++)
                {
                    bool bit = array[i];
                    array[i] = array[length - i - 1];
                    array[length - i - 1] = bit;
                }
            }

            try
            {
                string input = args[0];
                string output = args[1];
            }
            catch (IndexOutOfRangeException)
            {
                throw (new IndexOutOfRangeException("Not Enough Arguments. " + Usage()));
            }

            FileStream file = File.OpenRead(args[0]);
            FileStream outFile = File.OpenWrite(args[1]);
            outFile.SetLength(0);
            outFile.Close();
            outFile = File.OpenWrite(args[1]);

            BitsQuant quant = BitsQuant.BIT8;

            if (args.Length > 2)
            {
                int bits = int.Parse(args[2]);
                switch (bits)
                {
                    case 8:
                        break;
                    case 16:
                        quant = BitsQuant.BIT16;
                        break;
                    case 32:
                        quant = BitsQuant.BIT32;
                        break;
                    case 64:
                        quant = BitsQuant.BIT64;
                        break;
                    default:
                        throw (new InvalidDataException("NOT SUPPORTED QUANTITY OF BITS PER INSTRUCTION"));
                }
            }

            Console.WriteLine("Starting conversion");

            for (var i = 0; i < file.Length; i += (int)quant)
            {
                byte[] prepArr = new byte[(int)quant];
                for (var j = 0; j < (int)quant; j++)
                {
                    prepArr[j] = (byte)file.ReadByte();
                }
                var barr = new BitArray(prepArr);
                Reverse(barr);
                var bstr = "0b";
                for (var j = 0; j < barr.Count; j++)
                {
                    bstr += barr.Get(j) ? "1" : "0";
                }
                bstr += "\n";
                Console.Write(bstr);
                var bbstr = Encoding.ASCII.GetBytes(bstr);
                outFile.Write(bbstr, 0, bbstr.Length);
            }

            outFile.Flush();

            Console.WriteLine("Finished work. Enjoy :)");
        }
    }
}