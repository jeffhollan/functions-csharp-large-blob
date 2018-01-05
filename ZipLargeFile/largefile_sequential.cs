using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ZipLargeFile
{
    public static class largefile_sequential
    {
        private static List<int> answers = new List<int>();
        [FunctionName("largefile_sequential")]
        public static async Task RunAsync([BlobTrigger("largecsv/{name}")]Stream blob, string name, TraceWriter log, [Blob("largecsvanswers/{name}", FileAccess.Write)]Stream answerBlob)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");

            using (StreamWriter streamWriter = new StreamWriter(answerBlob))
            using (StreamReader streamReader = new StreamReader(blob))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var args = line.Split(',');
                    int lineSum = int.Parse(args[0]) + int.Parse(args[1]);
                    await streamWriter.WriteLineAsync(lineSum.ToString());
                }
            }
        }
    }
}
