using UnityEngine;

public class StartSpriteChange : MonoBehaviour
{
    [SerializeField] private Sprite sprite;

    void Start()
    {

       GetComponent<Renderer>().material.mainTexture = sprite.texture;
    }
}
