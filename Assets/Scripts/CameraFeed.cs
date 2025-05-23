using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CameraFeed : MonoBehaviour
{
    private ARCameraManager cameraManager;
    private Texture2D cameraTexture;

    void Start()
    {
        cameraManager = FindObjectOfType<ARCameraManager>();
        cameraManager.frameReceived += OnCameraFrame;
    }

    void OnCameraFrame(ARCameraFrameEventArgs args)
    {
        if (cameraManager.TryAcquireLatestCpuImage(out var image))
        {
            // Placeholder for feeding to ONNX later
            image.Dispose();
        }
    }
}

