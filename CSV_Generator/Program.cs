using System;
using System.IO;
using System.Threading.Tasks;

namespace CSV_Generator
{
    class Program
    {
        private const int totalLines = 1000;
        static void Main(string[] args)
        {
            string[] lines = new string[totalLines];
            string line;
            for(int x = 0; x < totalLines; x++)
            {
                line = $"{x},{x},https://jehollan-scaletestwebapp.azurewebsites.net/api/echo";
                lines[x] = line;
             //   Console.WriteLine(line);
            }
            File.WriteAllLines(@"C:\Users\Public\Documents\complex.csv", lines);
        }
    }
}
