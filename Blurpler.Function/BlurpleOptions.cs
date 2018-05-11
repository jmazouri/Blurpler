using System;
using Microsoft.AspNetCore.Http;

public class BlurpleOptions
{
    public float Threshold { get; set; } = 3;
    public float Preblur { get; set; } = 0.00001f;

    public bool KeepTransparency { get; set; } = true;
    public string ImageBase64 { get; set; }

    private byte[] _sourceImage;
    public byte[] SourceImage => _sourceImage ?? (_sourceImage = Convert.FromBase64String(ImageBase64));

    public void Validate()
    {
        if (Threshold > 10 || Threshold < 0)
        {
            throw new ArgumentOutOfRangeException("Threshold", "Threshold must be between 10 and 0");
        }

        if (Preblur > 10 || Preblur < 0.00001)
        {
            throw new ArgumentOutOfRangeException("Preblur", "Preblur must be between 10 and 0.00001");
        }

        if (SourceImage.Length > 100000)
        {
            throw new ArgumentOutOfRangeException("ImageBase64", "Image, must be less than 100000 bytes");
        }
    }
}