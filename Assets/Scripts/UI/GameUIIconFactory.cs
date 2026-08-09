using System.Collections.Generic;
using UnityEngine;

public static class GameUIIconFactory
{
    public enum Tipo
    {
        Buscar,
        Cerrar,
        Mueble,
        Colocar
    }

    private static readonly Dictionary<Tipo, Sprite>
        cache =
            new Dictionary<Tipo, Sprite>();

    public static Sprite Obtener(Tipo tipo)
    {
        if (
            cache.TryGetValue(
                tipo,
                out Sprite existente
            )
        )
        {
            return existente;
        }

        const int tamano = 64;

        Texture2D textura =
            new Texture2D(
                tamano,
                tamano,
                TextureFormat.RGBA32,
                false
            );

        textura.name =
            "UIIcon_" + tipo;

        textura.wrapMode =
            TextureWrapMode.Clamp;

        textura.filterMode =
            FilterMode.Bilinear;

        Color32 transparente =
            new Color32(
                255,
                255,
                255,
                0
            );

        Color32 blanco =
            new Color32(
                255,
                255,
                255,
                255
            );

        Color32[] pixels =
            new Color32[
                tamano * tamano
            ];

        for (
            int i = 0;
            i < pixels.Length;
            i++
        )
        {
            pixels[i] =
                transparente;
        }

        textura.SetPixels32(
            pixels
        );

        switch (tipo)
        {
            case Tipo.Buscar:
                DibujarCirculo(
                    textura,
                    27,
                    35,
                    15,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    38,
                    24,
                    50,
                    12,
                    5,
                    blanco
                );
                break;

            case Tipo.Cerrar:
                DibujarLinea(
                    textura,
                    18,
                    18,
                    46,
                    46,
                    5,
                    blanco
                );

                DibujarLinea(
                    textura,
                    46,
                    18,
                    18,
                    46,
                    5,
                    blanco
                );
                break;

            case Tipo.Mueble:
                DibujarLinea(
                    textura,
                    14,
                    38,
                    32,
                    48,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    32,
                    48,
                    50,
                    38,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    14,
                    38,
                    14,
                    20,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    50,
                    38,
                    50,
                    20,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    14,
                    20,
                    32,
                    10,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    32,
                    10,
                    50,
                    20,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    32,
                    48,
                    32,
                    29,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    14,
                    38,
                    32,
                    29,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    50,
                    38,
                    32,
                    29,
                    4,
                    blanco
                );
                break;

            case Tipo.Colocar:
                DibujarLinea(
                    textura,
                    32,
                    49,
                    32,
                    22,
                    5,
                    blanco
                );

                DibujarLinea(
                    textura,
                    20,
                    34,
                    32,
                    22,
                    5,
                    blanco
                );

                DibujarLinea(
                    textura,
                    44,
                    34,
                    32,
                    22,
                    5,
                    blanco
                );

                DibujarLinea(
                    textura,
                    15,
                    14,
                    49,
                    14,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    15,
                    14,
                    15,
                    23,
                    4,
                    blanco
                );

                DibujarLinea(
                    textura,
                    49,
                    14,
                    49,
                    23,
                    4,
                    blanco
                );
                break;
        }

        textura.Apply();

        Sprite sprite =
            Sprite.Create(
                textura,
                new Rect(
                    0f,
                    0f,
                    tamano,
                    tamano
                ),
                new Vector2(
                    0.5f,
                    0.5f
                ),
                100f
            );

        sprite.name =
            "UIIconSprite_" + tipo;

        cache[tipo] =
            sprite;

        return sprite;
    }

    private static void DibujarCirculo(
        Texture2D textura,
        int centroX,
        int centroY,
        int radio,
        int grosor,
        Color32 color)
    {
        int radioExterior =
            radio;

        int radioInterior =
            Mathf.Max(
                0,
                radio - grosor
            );

        int exterior2 =
            radioExterior *
            radioExterior;

        int interior2 =
            radioInterior *
            radioInterior;

        for (
            int y =
                centroY - radioExterior;
            y <=
                centroY + radioExterior;
            y++
        )
        {
            for (
                int x =
                    centroX - radioExterior;
                x <=
                    centroX + radioExterior;
                x++
            )
            {
                int dx =
                    x - centroX;

                int dy =
                    y - centroY;

                int distancia2 =
                    dx * dx +
                    dy * dy;

                if (
                    distancia2 <= exterior2 &&
                    distancia2 >= interior2
                )
                {
                    Pintar(
                        textura,
                        x,
                        y,
                        color
                    );
                }
            }
        }
    }

    private static void DibujarLinea(
        Texture2D textura,
        int x0,
        int y0,
        int x1,
        int y1,
        int grosor,
        Color32 color)
    {
        int dx =
            Mathf.Abs(x1 - x0);

        int dy =
            Mathf.Abs(y1 - y0);

        int pasos =
            Mathf.Max(dx, dy);

        if (pasos == 0)
            pasos = 1;

        for (
            int i = 0;
            i <= pasos;
            i++
        )
        {
            float t =
                i / (float)pasos;

            int x =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        x0,
                        x1,
                        t
                    )
                );

            int y =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        y0,
                        y1,
                        t
                    )
                );

            int mitad =
                Mathf.Max(
                    1,
                    grosor / 2
                );

            for (
                int py =
                    y - mitad;
                py <= y + mitad;
                py++
            )
            {
                for (
                    int px =
                        x - mitad;
                    px <= x + mitad;
                    px++
                )
                {
                    Pintar(
                        textura,
                        px,
                        py,
                        color
                    );
                }
            }
        }
    }

    private static void Pintar(
        Texture2D textura,
        int x,
        int y,
        Color32 color)
    {
        if (
            x < 0 ||
            y < 0 ||
            x >= textura.width ||
            y >= textura.height
        )
        {
            return;
        }

        textura.SetPixel(
            x,
            y,
            color
        );
    }
}
