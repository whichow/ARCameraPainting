using UnityEngine;
using System.Collections;

public class RenderAndPlay : MonoBehaviour {

    public TexturePicker texturePicker;
    public GameObject[] renderGameObjects;

	// Use this for initialization
	void Start () {
        texturePicker.TexturePicked += OnTexturePicked;
	}

    void OnTexturePicked(Texture2D texture)
    {
        foreach (var renderGameObject in renderGameObjects)
        {
            foreach (var material in renderGameObject.renderer.materials)
            {
                //material.mainTextureScale = new Vector2(1, -1);
                material.mainTexture = texture;
            }
        }
        if (animation != null)
        {
            animation.Play();
        }
    }
}
