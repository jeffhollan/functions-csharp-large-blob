using System.IO;
using System.IO.Compression;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ZipLargeFile
{
    public static class zip_file
    {
        [FunctionName("zip_file")]
        public static void Run([BlobTrigger("largeitems/{name}"), Disable()]Stream blob, string name, TraceWriter log, [Blob("largezips/{name}.zip", FileAccess.Write)] Stream outputblob)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");
            using (var archive = new ZipArchive(outputblob, ZipArchiveMode.Create, true))
            {
                var blobEntry = archive.CreateEntry(name);

                using (var entryStream = blobEntry.Open())
                {
                    blob.CopyTo(entryStream);
                }
            }
        }
    }
}
