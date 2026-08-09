using System.Collections.Generic;
using UnityEngine;

public static class UIRoundedSpriteFactory
{
    private static readonly Dictionary<int, Sprite>
        cache = new Dictionary<int, Sprite>();

    public static Sprite Obtener(float radio)
    {
        int clave = Mathf.Max(2, Mathf.RoundToInt(radio));

        if (cache.TryGetValue(clave, out Sprite existente))
            return existente;

        const int tamano = 64;

        Texture2D textura = new Texture2D(
            tamano,
            tamano,
            TextureFormat.RGBA32,
            false
        );

        textura.name = "RoundedUI_" + clave;
        textura.wrapMode = TextureWrapMode.Clamp;
        textura.filterMode = FilterMode.Bilinear;

        Color32 transparente =
            new Color32(255, 255, 255, 0);

        Color32 blanco =
            new Color32(255, 255, 255, 255);

        float r = Mathf.Clamp(
            clave,
            2f,
            tamano * 0.45f
        );

        Vector2[] centros =
        {
            new Vector2(r, r),
            new Vector2(tamano - 1 - r, r),
            new Vector2(r, tamano - 1 - r),
            new Vector2(
                tamano - 1 - r,
                tamano - 1 - r
            )
        };

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                bool dentroCentroX =
                    x >= r &&
                    x <= tamano - 1 - r;

                bool dentroCentroY =
                    y >= r &&
                    y <= tamano - 1 - r;

                bool visible =
                    dentroCentroX ||
                    dentroCentroY;

                if (!visible)
                {
                    Vector2 punto =
                        new Vector2(x, y);

                    foreach (Vector2 centro in centros)
                    {
                        if (
                            Vector2.Distance(
                                punto,
                                centro
                            ) <= r
                        )
                        {
                            visible = true;
                            break;
                        }
                    }
                }

                textura.SetPixel(
                    x,
                    y,
                    visible
                        ? blanco
                        : transparente
                );
            }
        }

        textura.Apply();

        Sprite sprite = Sprite.Create(
            textura,
            new Rect(
                0f,
                0f,
                tamano,
                tamano
            ),
            new Vector2(0.5f, 0.5f),
            100f,
            0u,
            SpriteMeshType.FullRect,
            new Vector4(r, r, r, r)
        );

        sprite.name =
            "RoundedSprite_" + clave;

        cache[clave] =
            sprite;

        return sprite;
    }
}
