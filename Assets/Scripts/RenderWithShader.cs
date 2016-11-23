using UnityEngine;
using System.Collections;

public class RenderWithShader : MonoBehaviour {
    public Shader shader;
    private Material material;

    void Start() {
        material = new Material(shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination,material);
    }
}
