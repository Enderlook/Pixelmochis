using System;
using UnityEngine;

// TODO: Hacer que la escala no dependa de la resolucion del usuario
public class CursorController : MonoBehaviour
{
    [SerializeField] private CursorSet defaultCursorSet;
    private CursorSet currentSet;
    [SerializeField] private float cursorScale = 1.0f; 

    [Serializable]
    private struct CursorSet
    {
        public Sprite normal;
        public Sprite clicked;
    }

    private void Awake()
    {
        currentSet = defaultCursorSet;
        ChangeCursor(currentSet.normal);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) ChangeCursor(currentSet.clicked);
        if (Input.GetMouseButtonUp(0)) ChangeCursor(currentSet.normal);
    }

    private void ChangeCursor(Sprite sprite)
    {
        Texture2D texture = SpriteToTexture(sprite);
        Vector2 hotspot = new Vector2(texture.width / 2f, texture.height / 2f);

        // Aplicar la escala al cursor
        texture = ScaleTexture(texture, cursorScale);

        Cursor.SetCursor(texture, hotspot, CursorMode.ForceSoftware);
    }

    private Texture2D SpriteToTexture(Sprite sprite)
    {
        // Crea una nueva textura con las dimensiones del Sprite
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

        // Obtiene los píxeles del Sprite y los copia a la textura
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                  (int)sprite.textureRect.y,
                                                  (int)sprite.textureRect.width,
                                                  (int)sprite.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    private Texture2D ScaleTexture(Texture2D source, float scaleFactor)
    {
        int newWidth = (int)(source.width * scaleFactor);
        int newHeight = (int)(source.height * scaleFactor);
        Texture2D scaledTexture = new Texture2D(newWidth, newHeight);
        Color[] pixels = source.GetPixels(0, 0, source.width, source.height);
        Color[] scaledPixels = new Color[newWidth * newHeight];

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int index = (int)(y / scaleFactor) * source.width + (int)(x / scaleFactor);
                scaledPixels[y * newWidth + x] = pixels[index];
            }
        }

        scaledTexture.SetPixels(scaledPixels);
        scaledTexture.Apply();

        return scaledTexture;
    }
}
