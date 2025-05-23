using Unity.Barracuda;
using UnityEngine;

public class ONNXInference : MonoBehaviour
{
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }

    public string RunInference(Texture2D input)
    {
        var inputTensor = new Tensor(input, channels: 3);
        worker.Execute(inputTensor);
        var output = worker.PeekOutput();
        inputTensor.Dispose();
        return output.ToString();
    }

    void OnDestroy()
    {
        worker?.Dispose();
    }
}

