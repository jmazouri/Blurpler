using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing.Processors;

public class BlurpleProcessor : IImageProcessor<Rgba32>
{
    static readonly Rgba32 Blurple = new Rgba32(114, 137, 218);
    static readonly Rgba32 DarkBlurple = new Rgba32(78, 93, 148);
    static readonly Rgba32 NotBlack = new Rgba32(35, 39, 42);

    private BlurpleOptions _opt;

    public BlurpleProcessor(BlurpleOptions opt)
    {
        _opt = opt;
    }

    public void Apply(Image<Rgba32> source, Rectangle sourceRectangle)
    {
        int avgPixel = GetAverageColor(source);
        float threshold = _opt.Threshold * avgPixel;

        for (int x = 0; x < source.Width; x++)
        for (int y = 0; y < source.Height; y++)
        {
            var srcPixel = source[x, y];

            var rgba = new Rgba32();
            srcPixel.ToRgba32(ref rgba);

            if (_opt.KeepTransparency && rgba.A <= 5)
            {
                continue;
            }

            int pixelColor = rgba.R + rgba.G + rgba.B;

            if (pixelColor > threshold)
            {
                source[x, y] = Rgba32.White;
            }
            else if (pixelColor > threshold / 2f)
            {
                source[x, y] = Blurple;
            }
            else if (pixelColor > threshold / 3f)
            {
                source[x, y] = DarkBlurple;
            }
            else
            {
                source[x, y] = NotBlack;
            }
        }
    }

    private static int GetAverageColor(Image<Rgba32> source)
    {
        ulong avgR = 0, avgG = 0, avgB = 0;

        for (int x = 0; x < source.Width; x++)
        for (int y = 0; y < source.Height; y++)
        {
            var thisPixel = source[x, y];

            avgR += thisPixel.R;
            avgG += thisPixel.G;
            avgB += thisPixel.B;
        }

        ulong sourceSize = (ulong)(source.Width * source.Height);

        avgR /= sourceSize;
        avgG /= sourceSize;
        avgB /= sourceSize;

        return (int)(avgR + avgG + avgB);
    }
}