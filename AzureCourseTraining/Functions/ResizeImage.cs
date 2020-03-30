using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Functions
{
    public static class ResizeImage
    {
        [FunctionName("ResizeImage")]
        public static void Run(
            [BlobTrigger("samples-images/{filename}", Connection = "AzureWebJobsStorage")]Stream image,
            [Blob("sample-images-sm/{filename})", FileAccess.Write)] Stream imageSmall,
            string fileName,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{fileName} \n Size: {image.Length} Bytes");

            Console.WriteLine("ResizeImage triggered");

            var img = Image.Load(image);

            img.Mutate(
                i => i.Resize(60, 60));

            img.Save(imageSmall, new PngEncoder());
        }
    }
}
