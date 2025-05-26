using UnityEngine;

public class WebcamInput : MonoBehaviour
{
    public Material targetMaterial;
    private WebCamTexture webcamTexture;

    void Start()
    {
        foreach (var device in WebCamTexture.devices)
            Debug.Log($"📸 Device: {device.name}");

        if (WebCamTexture.devices.Length > 0)
        {
            string camName = WebCamTexture.devices[0].name;

            // ✅  NO resolution arguments – let Unity pick a supported mode
            webcamTexture = new WebCamTexture(camName);

            targetMaterial.mainTexture = webcamTexture;
            webcamTexture.Play();

            Debug.Log($"✅ Webcam started: {camName} @ {webcamTexture.width}x{webcamTexture.height}");
        }
        else
        {
            Debug.LogError("🚫 No webcam devices found.");
        }
    }

    public Texture2D GetCurrentFrame()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying) return null;
        var tex = new Texture2D(webcamTexture.width, webcamTexture.height);
        tex.SetPixels32(webcamTexture.GetPixels32());
        tex.Apply();
        return tex;
    }
}

