using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class DiceFaceCreator : MonoBehaviour
{
    public static Texture2D CreateDiceFace(int number)
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        Color bgColor = Color.white;
        Color dotColor = Color.black;

        // Fill background
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                texture.SetPixel(x, y, bgColor);

        // Draw rounded border
        DrawBorder(texture, size, dotColor);

        // Draw dots based on number
        switch (number)
        {
            case 1:
                DrawDot(texture, size / 2, size / 2, 12, dotColor);
                break;
            case 2:
                DrawDot(texture, size / 3, size / 3, 10, dotColor);
                DrawDot(texture, 2 * size / 3, 2 * size / 3, 10, dotColor);
                break;
            case 3:
                DrawDot(texture, size / 3, size / 3, 10, dotColor);
                DrawDot(texture, size / 2, size / 2, 10, dotColor);
                DrawDot(texture, 2 * size / 3, 2 * size / 3, 10, dotColor);
                break;
            case 4:
                DrawDot(texture, size / 3, size / 3, 10, dotColor);
                DrawDot(texture, 2 * size / 3, size / 3, 10, dotColor);
                DrawDot(texture, size / 3, 2 * size / 3, 10, dotColor);
                DrawDot(texture, 2 * size / 3, 2 * size / 3, 10, dotColor);
                break;
            case 5:
                DrawDot(texture, size / 3, size / 3, 10, dotColor);
                DrawDot(texture, 2 * size / 3, size / 3, 10, dotColor);
                DrawDot(texture, size / 2, size / 2, 10, dotColor);
                DrawDot(texture, size / 3, 2 * size / 3, 10, dotColor);
                DrawDot(texture, 2 * size / 3, 2 * size / 3, 10, dotColor);
                break;
            case 6:
                DrawDot(texture, size / 3, size / 4, 10, dotColor);
                DrawDot(texture, 2 * size / 3, size / 4, 10, dotColor);
                DrawDot(texture, size / 3, size / 2, 10, dotColor);
                DrawDot(texture, 2 * size / 3, size / 2, 10, dotColor);
                DrawDot(texture, size / 3, 3 * size / 4, 10, dotColor);
                DrawDot(texture, 2 * size / 3, 3 * size / 4, 10, dotColor);
                break;
        }

        texture.Apply();
        return texture;
    }

    static void DrawDot(Texture2D tex, int cx, int cy,
        int radius, Color color)
    {
        for (int x = cx - radius; x <= cx + radius; x++)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                if (x >= 0 && x < tex.width &&
                    y >= 0 && y < tex.height)
                {
                    float dist = Vector2.Distance(
                        new Vector2(x, y), new Vector2(cx, cy));
                    if (dist <= radius)
                        tex.SetPixel(x, y, color);
                }
            }
        }
    }

    static void DrawBorder(Texture2D tex, int size, Color color)
    {
        int thickness = 6;
        for (int x = 0; x < size; x++)
        {
            for (int t = 0; t < thickness; t++)
            {
                tex.SetPixel(x, t, color);
                tex.SetPixel(x, size - 1 - t, color);
                tex.SetPixel(t, x, color);
                tex.SetPixel(size - 1 - t, x, color);
            }
        }
    }
}