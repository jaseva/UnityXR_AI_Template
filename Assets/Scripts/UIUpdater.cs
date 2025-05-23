using UnityEngine;
using TMPro;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI predictionText;
    public ONNXInference inference;

    void Update()
    {
        // Example: simulate output
        string output = "Predicted: Hotdog";
        predictionText.text = output;
    }
}

