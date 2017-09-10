using UnityEngine;

public class TextureScript : MonoBehaviour
{
    public ComputeShader shader;
    RenderTexture texture;

    void Start()
    {
        texture = new RenderTexture(64, 64, 0);
        texture.enableRandomWrite = true;
        texture.Create();

        shader.SetFloat("width", 64);
        shader.SetFloat("height", 64);
        shader.SetTexture(0, "Result", texture);
        shader.Dispatch(0, texture.width / 8, texture.height / 8, 1);
    }

    void OnGUI()
    {
        int textureSize = 256;
        GUI.DrawTexture(new Rect(0, 0, textureSize, textureSize), texture);
    }

    void OnDisable()
    {
        texture.Release();
    }
}
