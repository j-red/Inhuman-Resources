using UnityEngine; // https://forum.unity.com/threads/need-some-help-writing-a-simple-scanlines-shader.375868/

[ExecuteInEditMode]
public class ScanlineEffect : MonoBehaviour {
    public Material material;
    // Use this for initialization
    void Start()
    {
        material = new Material(Shader.Find("Hidden/CRT"));
    }
 
    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        material.SetTexture("_MainTex", source);
        Graphics.Blit(source, destination, material);
    }
}