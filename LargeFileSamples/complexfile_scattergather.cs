using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace LargeFileSamples
{
    public static class complexfile_scattergather
    {
        public static HttpClient client = new HttpClient();

        [FunctionName("file_scattergather_blobTrigger")]
        public static async Task RunAsync(
            [BlobTrigger("complex/{name}")]ICloudBlob blob, string name,
            [OrchestrationClient]DurableOrchestrationClient starter,
            TraceWriter log)
        {
            log.Info($"COMPLEX FANOUT Blob trigger function with Name:{name}");
            
            string instanceId = await starter.StartNewAsync("largefile_scattergather", name);

            log.Info($"Started orchestration with ID = '{instanceId}'.");

        }

        [FunctionName("largefile_scattergather")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            TraceWriter log)
        {
            var outputs = new List<int>();
            var parallelTasks = new List<Task<int>>();
            var blobName = context.GetInput<string>();

            var lines = await context.CallActivityAsync<List<string>>("file_scattergather_getlines", new BlobItem { name = blobName });

            for (int x = 0; x < lines.Count; x++)
            {
                Task<int> task = context.CallActivityAsync<int>("file_scattergather_process", new LineItem {
                    Args = lines[x].Split(','),
                    Index = x
                });
                parallelTasks.Add(task);
            }

            List<int> results = (await Task.WhenAll(parallelTasks)).ToList();
            await context.CallActivityAsync("file_scattergather_write", results);

            log.Info($"COMPLEX FANOUT Finished");
        }

        [FunctionName("file_scattergather_getlines")]
        public static async Task<List<string>> GetLines([ActivityTrigger] BlobItem item, TraceWriter log,
            [Blob("complex/{name}", FileAccess.Read)]Stream blob)
        {
            List<string> lines = new List<string>();
            using (StreamReader streamReader = new StreamReader(blob))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        [FunctionName("file_scattergather_process")]
        public static async Task<int> ProcessLineAsync([ActivityTrigger] LineItem item, TraceWriter log)
        {
            var result = await client.PostAsJsonAsync(item.Args[2], new { test = "value" });
            var contentBinary = await result.Content.ReadAsByteArrayAsync();
            return item.Index;
        }

        [FunctionName("file_scattergather_write")]
        public static async Task WriteToBlob([ActivityTrigger] List<int> results, TraceWriter log,
            [Blob("complexanswers/answersFANOUT.csv", FileAccess.Write)]Stream answerBlob)
        {
            using (StreamWriter streamWriter = new StreamWriter(answerBlob))
            {
                foreach (int result in results)
                {
                    await streamWriter.WriteLineAsync(result.ToString());
                }
            }
        }
    }

    public class BlobItem
    {
        public string name { get; set; }
    }

    public class LineItem
    {
        public string[] Args { get; set; }
        public int Index { get; set; }
    }
}