using UnityEngine;

public class OneDimension : MonoBehaviour
{
    [SerializeField] ComputeShader shader;

    void Start()
    {
        int[] arrayOfInt = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

        ComputeBuffer buffer = new ComputeBuffer(8, sizeof(int));
        buffer.SetData(arrayOfInt);
        

        int kernel = shader.FindKernel("CSMain");
        shader.SetBuffer(kernel, "Result", buffer);
        shader.Dispatch(kernel, 1, 1, 1);

        
        int[] data = new int[8];
        buffer.GetData(data);

        for (int i = 0; i < 8; i++)
            Debug.Log(data[i]);

        buffer.Release();
    }
}
