using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.IO.Compression;

namespace ZipLargeFile
{
    public static class gzip_file
    {
        [FunctionName("gzip_file")]
        public static void Run([BlobTrigger("largeitems/{name}")]Stream blob, string name, TraceWriter log, [Blob("largezips/{name}.gz", FileAccess.Write)] Stream outputblob)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {blob.Length} Bytes");
            using (GZipStream compressionStream = new GZipStream(outputblob, CompressionMode.Compress))
            {
                blob.CopyTo(compressionStream);
            }
        }
    }
}
