using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace LargeFileSamples
{
    public static class complexfile_sequential
    {
        public static HttpClient client = new HttpClient();

        [FunctionName("complexfile_sequential")]
        public static async Task RunAsync([BlobTrigger("complex/{name}"), Disable()]Stream blob, string name, TraceWriter log,
            [Blob("complexanswers/answersSEQUENTIAL.csv", FileAccess.Write)]Stream answerBlob)
        {
            var lineTasks = new List<Task<int>>();

            log.Info($"COMPLEX SEQUENTIAL Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");

            using (StreamWriter streamWriter = new StreamWriter(answerBlob))
            using (StreamReader streamReader = new StreamReader(blob))
            {
                string line;
                int counter = 0;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    var args = line.Split(',');
                    var result = await client.PostAsJsonAsync(args[2], new { test = "value" });
                    var contentBinary = await result.Content.ReadAsByteArrayAsync();
                    await streamWriter.WriteLineAsync(counter.ToString());
                    counter++;
                }
            }

            log.Info($"COMPLEX SEQUENTIAL Finished");
        }
    }
}
