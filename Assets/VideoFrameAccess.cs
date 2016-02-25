using System;
using UnityEngine;
using System.Collections;
using Vuforia;
using Object = UnityEngine.Object;

public class PixelData
{
    public Color32[] pixels;
    public int Width
    {
        get;
        private set;
    }
    public int Height
    {
        get;
        private set;
    }

    public float AspectRatio
    {
        get
        {
            return (float)this.Width / (float)this.Height;
        }
    }

    public PixelData(int width, int height)
    {
        Width = width;
        Height = height;
        pixels = new Color32[Width * Height];
    }
}
public class VideoFrameAccess : MonoBehaviour {

    public float VideoHeight { get; private set; }

    public float VideoWidth { get; private set; }

    public float VideoYScale { get; private set; }

    public float VideoXScale { get; private set; }

    private VuforiaBehaviour vuforiaBehaviour;
    private Image.PIXEL_FORMAT pixelFormat;
    private bool formatRegistered;
    private Image image;

    void Awake()
    {
        if (Application.isEditor)
        {
            pixelFormat = Image.PIXEL_FORMAT.GRAYSCALE;
        }
        else
        {
            pixelFormat = Image.PIXEL_FORMAT.RGB888;
        }
        vuforiaBehaviour = VuforiaBehaviour.Instance;
        vuforiaBehaviour.RegisterTrackablesUpdatedCallback(OnTrackableUpdated);
    }

    private void OnTrackableUpdated()
    {
        if (!formatRegistered)
        {
            CameraDevice.Instance.SetFrameFormat(pixelFormat, false);
            if (CameraDevice.Instance.SetFrameFormat(pixelFormat, true))
            {
                formatRegistered = true;
            }
        }
        image = CameraDevice.Instance.GetCameraImage(pixelFormat);
    }

    void OnApplicationPause()
    {
        formatRegistered = false;
    }

    public PixelData GetPixelData()
    {
        if (image == null)
        {
            return null;
        }
        PixelData pixelData = new PixelData(image.Width, image.Height);
        switch (image.PixelFormat)
        {
            case Image.PIXEL_FORMAT.RGB888:
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        pixelData.pixels[i * image.Width + j] = new Color32(image.Pixels[i * image.Width * 3 + j * 3], image.Pixels[i * image.Width * 3 + j * 3 + 1], image.Pixels[i * image.Width * 3 + j * 3 + 2], 255);
                    }
                }
                break;
            case Image.PIXEL_FORMAT.GRAYSCALE:
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        pixelData.pixels[i * image.Width + j] = new Color32(image.Pixels[i * image.Width + j], image.Pixels[i * image.Width + j], image.Pixels[i * image.Width + j], 255);
                    }
                }
                break;
            default:
                return null;
        }

        VideoWidth = (float)image.Width;
        VideoHeight = (float)image.Height;

        float screenAspectRatio = (float)Screen.width / (float)Screen.height;
        float scale = screenAspectRatio / pixelData.AspectRatio;
        if (scale > 1)
        {
            VideoXScale = 1;
            VideoYScale = 1 / scale;
        }
        else
        {
            VideoXScale = scale;
            VideoYScale = 1;
        }

        return pixelData;
    }

    //public PixelData GetPixelData()
    //{
    //    Image image = CameraDevice.Instance.GetCameraImage(pixelFormat);

    //    if (image == null)
    //    {
    //        return null;
    //    }

    //    //OutputTexture.Resize(image.Width, image.Height);
    //    //image.CopyToTexture(OutputTexture);

    //    PixelData pixelData = new PixelData(image.Width, image.Height);

    //    Debug.Log("new pixelData");

    //    switch (image.PixelFormat)
    //    {
    //        case Image.PIXEL_FORMAT.RGB888:
    //            for (int i = 0; i < image.Height; i++)
    //            {
    //                for (int j = 0; j < image.Width; j++)
    //                {
    //                    pixelData.pixels[i * image.Width + j] = new Color32(image.Pixels[i * image.Width * 3 + j * 3], image.Pixels[i * image.Width * 3 + j * 3 + 1], image.Pixels[i * image.Width * 3 + j * 3 + 2], 255);
    //                }
    //            }
    //            break;
    //        case Image.PIXEL_FORMAT.GRAYSCALE:
    //            for (int i = 0; i < image.Height; i++)
    //            {
    //                for (int j = 0; j < image.Width; j++)
    //                {
    //                    pixelData.pixels[i * image.Width + j] = new Color32(image.Pixels[i * image.Width + j], image.Pixels[i * image.Width + j], image.Pixels[i * image.Width + j], 255);
    //                }
    //            }
    //            break;
    //        default:
    //            return null;
    //    }

    //    VideoWidth = (float) image.Width;
    //    VideoHeight = (float) image.Height;

    //    float screenAspectRatio = (float)Screen.width / (float)Screen.height;
    //    float scale = screenAspectRatio / pixelData.AspectRatio;
    //    if (scale > 1)
    //    {
    //        VideoXScale = 1;
    //        VideoYScale = 1 / scale;
    //    }
    //    else
    //    {
    //        VideoXScale = scale;
    //        VideoYScale = 1;
    //    }
    //    //VideoXScale = scale > 1 ? 1 : scale;
    //    //VideoYScale = scale > 1 ? 1/scale : 1;
    //    //float num = (float)Screen.height * pixelData.AspectRatio;
    //    //float num2 = (float)Screen.width / pixelData.AspectRatio;
    //    //if (num > (float)Screen.width)
    //    //{
    //    //    this.VideoYScale = 1f;
    //    //    this.VideoXScale = (float)Screen.width / num;
    //    //}
    //    //else
    //    //{
    //    //    this.VideoXScale = 1f;
    //    //    this.VideoYScale = (float)Screen.height / num2;
    //    //}
    //    return pixelData;
    //}


    //public Vector3 AdjustPoint(Vector3 pnt)
    //{
    //    float num = 0.5f - pnt.x;
    //    float num2 = 0.5f - pnt.y;
    //    return new Vector3(0.5f - num * this.VideoXScale, 0.5f - num2 * this.VideoYScale, pnt.z);
    //}


}
