using UnityEngine;

public class Groups : MonoBehaviour
{
    [SerializeField] ComputeShader shader;

    void Start()
    {
        ComputeBuffer buffer = new ComputeBuffer(8 * 8 * 5 * 5, sizeof(int));

        int kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "Result", buffer);
        shader.Dispatch(kernel, 5, 5, 1);

        int[] data = new int[8 * 8 * 5 * 5];
        buffer.GetData(data);

        for (int i = 0; i < 8 * 8 * 5 * 5; i++)
            Debug.Log(data[i]);

        buffer.Release();

    }
}
