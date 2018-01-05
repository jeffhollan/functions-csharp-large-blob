using System;
using System.IO;
using System.Threading.Tasks;

namespace CSV_Generator
{
    class Program
    {
        private const int totalLines = 100000000;
        static void Main(string[] args)
        {
            string[] lines = new string[totalLines];
            string line;
            for(int x = 0; x < totalLines; x++)
            {
                line = $"{x},{x}";
                lines[x] = line;
             //   Console.WriteLine(line);
            }
            File.WriteAllLines(@"C:\Users\Public\Documents\large.csv", lines);
        }
    }
}
