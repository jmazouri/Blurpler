using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Linq;
using System;
using System.Net.Http;
using System.Diagnostics;

namespace Blurpler
{
    public static class ImageTransformer
    {
        [FunctionName("ImageTransformer")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]BlurpleOptions opt, TraceWriter log)
        {
            log.Info($"Image blurple request recieved.");

            try
            {
                opt.Validate();

                Stopwatch watch = new Stopwatch();
                watch.Start();

                var mem = MakeBlurpleFromStream(opt);
                var result = new FileStreamResult(mem, "image/png");

                watch.Stop();

                log.Info($"Converted image, took {watch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (NotSupportedException ex)
            {
                log.Error(ex.Message, ex);
                return new BadRequestObjectResult("Image format not supported.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                log.Error(ex.Message, ex);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return new BadRequestObjectResult("There was an error of some kind.");
        }

        public static MemoryStream MakeBlurpleFromStream(BlurpleOptions opt)
        {
            Image<Rgba32> img = Image.Load(opt.SourceImage);
            img.Mutate(x => 
            {
                x.GaussianBlur(opt.Preblur <= 0 ? 0.00001f : opt.Preblur)
                .ApplyProcessor(new BlurpleProcessor(opt));
            });

            MemoryStream imgPngData = new MemoryStream();
            img.SaveAsPng(imgPngData);

            imgPngData.Seek(0, SeekOrigin.Begin);

            return imgPngData;
        }
        
    }
}
