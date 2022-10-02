using System;
using System.IO;
using System.Text;

namespace Bitmap
{
    class Program
    {
        static void Main()
        {
            // you should point at a valid .bmp file on your machine/drive
            string fileName = @"C:\Users\Alex\Downloads\sps.bmp";

            // choosing a suitable size of buffer
            const int bufferSize = 16;
            byte[] buffer = new byte[bufferSize];

            long sz = new FileInfo(fileName).Length;

            long sizeInt = BytesToDecimal(fileName, 2, 5);
            long width = BytesToDecimal(fileName, 18, 21);
            long height = BytesToDecimal(fileName, 22, 25);

            
            int sizeCounted = PrintAndCount(fileName, buffer, false);

            Console.WriteLine("File size {0} bits from bitmap, .Length returns {1} bits, number counted is {2} bits", sizeInt, sz, sizeCounted);
            Console.WriteLine("Which is {0} kilobytes / {1} kibibytes (from .Length calculation)", sz / 1000, (sz * 8) / 1024);
            Console.WriteLine("width: {0}, height: {1}", width, height);


            int blackPixels = CountPixels(fileName, false); // why are there 4 extra bytes? Padding?
            Console.WriteLine(blackPixels);


        }

        static string LocateByte(string fileName, int pos)
        {
            using (BinaryReader temp = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                temp.BaseStream.Seek(pos, SeekOrigin.Begin);
                byte dByte = temp.ReadByte();  

                return dByte.ToString("X");
            }
        }

        static long BytesToDecimal(string fileName, int start, int end  = 0)
        {
            string sizeH = "";
            
            if (end == 0) { return Int64.Parse(LocateByte(fileName, start), System.Globalization.NumberStyles.HexNumber); }

            for (int i = end-1; i >= start; --i)
            {
                sizeH += LocateByte(fileName, i);
            }

            long numInt = Int64.Parse(sizeH, System.Globalization.NumberStyles.HexNumber);

            return numInt;
        }

        static int PrintAndCount(string fileName, byte[] buffer, bool print = true)
        {
            int count;
            int bufferSize = buffer.Length;
            int sizeCounted = 0;


            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))

            {
                // loop through the file, the read command returns the number of characters read, up to the limit in the third argument
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    for (int i = 0; i < bufferSize; i++)
                    {
                        if (i < count )
                        {
                            if (print == true) { Console.Write("{0:x2} ", buffer[i]); }
                            if (i == bufferSize - 1 && print == true) { Console.WriteLine(); }
                            sizeCounted++;
                        }
                    }
                }
            }

            return sizeCounted;
        }

        static int CountPixels(string fileName, bool print = true)
        {
            byte[] pixelBuffer = new byte[4];
            int count;

            List<byte[]> uColours = new List<byte[]>();

            int blackCount = 0;

            int totalPix = 0;

            long pixelsStart = BytesToDecimal(fileName, 14);

            byte[] empty = new byte[4];
            empty[0] = empty[1] = empty[2] = empty[3]  = 00;

            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))

            {
                reader.BaseStream.Seek(pixelsStart, SeekOrigin.Begin);

                // loop through the file, the read command returns the number of characters read, up to the limit in the third argument
                while ((count = reader.Read(pixelBuffer, 0, pixelBuffer.Length)) != 0)
                {
                    for (int i = 0; i < pixelBuffer.Length; i++)
                    {
                        if (i < count)
                        {
                            if (print == true) { Console.Write("{0:x2} ", pixelBuffer[i]); }
                            if (i == pixelBuffer.Length - 1  && print == true) { Console.WriteLine(); }
                        }
                    }

                    totalPix++;

                    if (Enumerable.SequenceEqual(pixelBuffer, empty)) { blackCount++; }

                    
                    bool contains = (uColours.Any(x => x.SequenceEqual(pixelBuffer))); //{ uColours.Add(pixelBuffer); }
                    Console.WriteLine(contains); //how to see if a list of arrays contains an array?

                }
            }

            Console.WriteLine(uColours.Count());

            Console.WriteLine(totalPix);
            return blackCount;
        }


    }
}