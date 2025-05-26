using UnityEngine;
using Unity.Barracuda;
using TMPro;

public class ONNXInference : MonoBehaviour
{
    [Header("Model + Input")]
    public NNModel modelAsset;
    public Material displayMaterial;
    public TextMeshProUGUI predictionText;
    public int targetWidth = 32;
    public int targetHeight = 32;

    private Model runtimeModel;
    private IWorker worker;
    private WebCamTexture webcamTexture;
    private Texture2D resizedTexture;

    void Start()
    {
        // Validate model
        if (modelAsset == null)
        {
            Debug.LogError("❌ ONNX model asset is not assigned.");
            return;
        }

        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);

        // Start webcam
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        // Apply webcam to material if assigned
        if (displayMaterial != null)
            displayMaterial.mainTexture = webcamTexture;

        resizedTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
    }

    void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying || webcamTexture.width < 100)
            return;

        // Resize frame
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(webcamTexture, rt);
        RenderTexture.active = rt;
        resizedTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        resizedTexture.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // Create input tensor
        Tensor inputTensor = new Tensor(resizedTexture, channels: 3);

        // Run model
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();

        // Get prediction
        int maxIndex = 0;
        float maxScore = float.MinValue;
        for (int i = 0; i < outputTensor.length; i++)
        {
            if (outputTensor[i] > maxScore)
            {
                maxScore = outputTensor[i];
                maxIndex = i;
            }
        }

        string[] labels = { "Hotdog", "Not Hotdog" };
        string result = (maxIndex >= 0 && maxIndex < labels.Length) ? labels[maxIndex] : $"Class {maxIndex}";

        // Update UI
        if (predictionText != null)
            predictionText.text = $"Predicted:\n{result}";

        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    void OnDisable()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            Destroy(webcamTexture);
        }

        if (worker != null)
            worker.Dispose();
    }
}

