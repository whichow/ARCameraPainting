using System;
using UnityEngine;
using System.Collections;
using Vuforia;


public class TexturePicker : MonoBehaviour, ITrackableEventHandler
{
    public Texture2D pickedTexture { get; private set; }
    public bool IsTracked { get; private set; }
    public ImageTargetBehaviour imageTarget;
    public Camera arCamera;
    public VideoFrameAccess videoFrame;
    public Action<Texture2D> TexturePicked;
    public TrackableBehaviour trackableBehaviour;

    private bool isPicked = false;
    private Vector3[] cornerPoints;
    private float targetWidth;
    private float targetHeight;
    //private Vector3 targetCenter;
    private Texture2D videoTexture;
    private Vector3[,] map;

    //public GameObject planeGameObject;

    void Awake()
    {
        //trackableBehaviour = FindObjectOfType<TrackableBehaviour>();
        if (trackableBehaviour!= null)
        {
            trackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    // Use this for initialization
	void Start ()
	{
	    targetWidth = imageTarget.GetSize().x;
	    targetHeight = imageTarget.GetSize().y;
	    var targetCenter = imageTarget.transform.position;

        cornerPoints = new Vector3[4];
        cornerPoints[0] = targetCenter + new Vector3(-targetWidth / 2f, 0f, targetHeight / 2f);
        cornerPoints[1] = targetCenter + new Vector3(targetWidth / 2f, 0f, targetHeight / 2f);
        cornerPoints[2] = targetCenter + new Vector3(targetWidth / 2f, 0f, -targetHeight / 2f);
        cornerPoints[3] = targetCenter + new Vector3(-targetWidth / 2f, 0f, -targetHeight / 2f);

        pickedTexture = new Texture2D((int)targetWidth, (int)targetHeight, TextureFormat.RGBA32, false);

        IsTracked = false;

	}

    // Update is called once per frame
    void Update ()
	{
        if (IsTracked)
        {
            if (!isPicked && AreAllPointVisible(cornerPoints))
            {
                PickTexture();
                isPicked = true;                
            }
        }
        else
        {
            isPicked = false;
        }
	}


    private bool AreAllPointVisible(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            var vpPoint = arCamera.WorldToViewportPoint(points[i]);
            if (!IsViewportPointVisible(vpPoint))
            {
                return false;
            }
        }
        return true;
    }


    private bool IsViewportPointVisible(Vector3 vpPoint)
    {
        return vpPoint.x >= 0f && vpPoint.x <= 1f && vpPoint.y >= 0f && vpPoint.y <= 1f && vpPoint.z >= arCamera.nearClipPlane && vpPoint.z <= arCamera.farClipPlane;
    }

    private void PickTexture()
    {
        PixelData pixelData = videoFrame.GetPixelData();
        if (videoTexture == null)
        {
            videoTexture = new Texture2D(pixelData.Width, pixelData.Height, TextureFormat.RGBA32, false);
        }
        videoTexture.SetPixels32(pixelData.pixels);
        //videoTexture = (Texture2D)planeGameObject.GetComponent<Renderer>().material.mainTexture;
        MapTexture(videoTexture, pickedTexture);
        TextureFlipY(pickedTexture);
        pickedTexture.Apply();
        if (TexturePicked != null)
        {
            TexturePicked(pickedTexture);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            IsTracked = true;
        }
        else
        {
            IsTracked = false;
        }
    }

    private Vector3[,] ScreenPointMap(int w, int h)
    {
        float videoWidth = videoTexture.width;
        float videoHeight = videoTexture.height;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector3[,] map = new Vector3[w, h];
        Vector3 v1 = (cornerPoints[1] - cornerPoints[0])/w;
        Vector3 v2 = (cornerPoints[3] - cornerPoints[0])/h;

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                var p = cornerPoints[0] + v1*j + v2*i;
                p = arCamera.WorldToViewportPoint(p);
                p = new Vector3(p.x * screenWidth, (1 - p.y) * screenHeight);
                float scale = screenWidth / screenHeight > videoWidth / videoHeight ? screenWidth / videoWidth : screenHeight / videoHeight;
                //float scale;
                //if (screenWidth / screenHeight > videoWidth / videoHeight)
                //{
                //    scale = screenWidth / videoWidth;
                //}
                //else
                //{
                //    scale = screenHeight / videoHeight;
                //}
                var delta = (p - new Vector3(screenWidth / 2f, screenHeight / 2f)) / scale;
                map[j, i] = delta + new Vector3(videoWidth / 2f, videoHeight / 2f);
            }
        }
        return map;
    }

    private void MapTexture(Texture2D source, Texture2D dest)
    {
        map = ScreenPointMap((int)targetWidth, (int)targetHeight);

        for (int i = 0; i < dest.height; i++)
        {
            for (int j = 0; j < dest.width; j++)
            {
                dest.SetPixel(j, i, source.GetPixel(Mathf.RoundToInt(map[j, i].x), Mathf.RoundToInt(map[j, i].y)));
            }
        }
        //dest.Apply();
    }

    private void TextureFlipY(Texture2D texture)
    {
        Color temp;
        for (int i = 0; i < texture.height / 2; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                temp = texture.GetPixel(j, i);
                texture.SetPixel(j, i, texture.GetPixel(j, texture.height - i));
                texture.SetPixel(j, texture.height - i, temp);
            }
        }
        //texture.Apply();
    }
}
