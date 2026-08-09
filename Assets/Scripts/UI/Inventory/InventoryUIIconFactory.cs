using System.Collections.Generic;
using UnityEngine;

public static class InventoryUIIconFactory
{
    public enum Tipo
    {
        Inventario,
        Todos,
        Sillas,
        Mesas,
        Luces,
        Decoracion,
        Info
    }

    private static readonly Dictionary<
        Tipo,
        Sprite>
        cache =
            new Dictionary<
                Tipo,
                Sprite>();

    public static Sprite Obtener(
        Tipo tipo)
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

        const int tamano =
            64;

        Texture2D textura =
            new Texture2D(
                tamano,
                tamano,
                TextureFormat.RGBA32,
                false
            );

        textura.name =
            "InventoryIcon_" +
            tipo;

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
            case Tipo.Inventario:
                DibujarSofa(
                    textura,
                    blanco
                );
                break;

            case Tipo.Todos:
                DibujarCuadricula(
                    textura,
                    blanco
                );
                break;

            case Tipo.Sillas:
                DibujarSilla(
                    textura,
                    blanco
                );
                break;

            case Tipo.Mesas:
                DibujarMesa(
                    textura,
                    blanco
                );
                break;

            case Tipo.Luces:
                DibujarBombillo(
                    textura,
                    blanco
                );
                break;

            case Tipo.Decoracion:
                DibujarEtiqueta(
                    textura,
                    blanco
                );
                break;

            case Tipo.Info:
                DibujarInfo(
                    textura,
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
            "InventoryIconSprite_" +
            tipo;

        cache[tipo] =
            sprite;

        return sprite;
    }

    private static void DibujarSofa(
        Texture2D t,
        Color32 c)
    {
        Rectangulo(
            t,
            14,
            27,
            50,
            42,
            4,
            c
        );

        Rectangulo(
            t,
            18,
            38,
            46,
            51,
            4,
            c
        );

        Linea(
            t,
            14,
            27,
            10,
            21,
            4,
            c
        );

        Linea(
            t,
            50,
            27,
            54,
            21,
            4,
            c
        );

        Linea(
            t,
            17,
            20,
            17,
            13,
            4,
            c
        );

        Linea(
            t,
            47,
            20,
            47,
            13,
            4,
            c
        );
    }

    private static void DibujarCuadricula(
        Texture2D t,
        Color32 c)
    {
        RellenarRect(
            t,
            14,
            36,
            27,
            49,
            c
        );

        RellenarRect(
            t,
            37,
            36,
            50,
            49,
            c
        );

        RellenarRect(
            t,
            14,
            14,
            27,
            27,
            c
        );

        RellenarRect(
            t,
            37,
            14,
            50,
            27,
            c
        );
    }

    private static void DibujarSilla(
        Texture2D t,
        Color32 c)
    {
        Rectangulo(
            t,
            21,
            34,
            43,
            49,
            4,
            c
        );

        Linea(
            t,
            20,
            34,
            20,
            14,
            4,
            c
        );

        Linea(
            t,
            44,
            34,
            44,
            14,
            4,
            c
        );

        Linea(
            t,
            20,
            29,
            44,
            29,
            4,
            c
        );
    }

    private static void DibujarMesa(
        Texture2D t,
        Color32 c)
    {
        Rectangulo(
            t,
            13,
            34,
            51,
            45,
            4,
            c
        );

        Linea(
            t,
            18,
            32,
            18,
            14,
            4,
            c
        );

        Linea(
            t,
            46,
            32,
            46,
            14,
            4,
            c
        );
    }

    private static void DibujarBombillo(
        Texture2D t,
        Color32 c)
    {
        Circulo(
            t,
            32,
            38,
            14,
            4,
            c
        );

        Linea(
            t,
            25,
            25,
            39,
            25,
            4,
            c
        );

        Linea(
            t,
            27,
            19,
            37,
            19,
            4,
            c
        );

        Linea(
            t,
            32,
            58,
            32,
            54,
            3,
            c
        );

        Linea(
            t,
            10,
            38,
            15,
            38,
            3,
            c
        );

        Linea(
            t,
            49,
            38,
            54,
            38,
            3,
            c
        );
    }

    private static void DibujarEtiqueta(
        Texture2D t,
        Color32 c)
    {
        Linea(
            t,
            17,
            42,
            38,
            53,
            4,
            c
        );

        Linea(
            t,
            38,
            53,
            53,
            38,
            4,
            c
        );

        Linea(
            t,
            53,
            38,
            31,
            16,
            4,
            c
        );

        Linea(
            t,
            31,
            16,
            12,
            35,
            4,
            c
        );

        Linea(
            t,
            12,
            35,
            17,
            42,
            4,
            c
        );

        Circulo(
            t,
            39,
            39,
            4,
            3,
            c
        );
    }

    private static void DibujarInfo(
        Texture2D t,
        Color32 c)
    {
        Circulo(
            t,
            32,
            32,
            22,
            4,
            c
        );

        RellenarRect(
            t,
            30,
            17,
            34,
            34,
            c
        );

        RellenarRect(
            t,
            30,
            42,
            34,
            46,
            c
        );
    }

    private static void Rectangulo(
        Texture2D t,
        int x0,
        int y0,
        int x1,
        int y1,
        int grosor,
        Color32 c)
    {
        Linea(t, x0, y0, x1, y0, grosor, c);
        Linea(t, x1, y0, x1, y1, grosor, c);
        Linea(t, x1, y1, x0, y1, grosor, c);
        Linea(t, x0, y1, x0, y0, grosor, c);
    }

    private static void RellenarRect(
        Texture2D t,
        int x0,
        int y0,
        int x1,
        int y1,
        Color32 c)
    {
        for (
            int y = y0;
            y <= y1;
            y++
        )
        {
            for (
                int x = x0;
                x <= x1;
                x++
            )
            {
                Pintar(
                    t,
                    x,
                    y,
                    c
                );
            }
        }
    }

    private static void Circulo(
        Texture2D t,
        int cx,
        int cy,
        int radio,
        int grosor,
        Color32 c)
    {
        int exterior =
            radio * radio;

        int interiorRadio =
            Mathf.Max(
                0,
                radio - grosor
            );

        int interior =
            interiorRadio *
            interiorRadio;

        for (
            int y =
                cy - radio;
            y <=
                cy + radio;
            y++
        )
        {
            for (
                int x =
                    cx - radio;
                x <=
                    cx + radio;
                x++
            )
            {
                int dx =
                    x - cx;

                int dy =
                    y - cy;

                int d =
                    dx * dx +
                    dy * dy;

                if (
                    d <= exterior &&
                    d >= interior
                )
                {
                    Pintar(
                        t,
                        x,
                        y,
                        c
                    );
                }
            }
        }
    }

    private static void Linea(
        Texture2D t,
        int x0,
        int y0,
        int x1,
        int y1,
        int grosor,
        Color32 c)
    {
        int dx =
            Mathf.Abs(
                x1 - x0
            );

        int dy =
            Mathf.Abs(
                y1 - y0
            );

        int pasos =
            Mathf.Max(
                1,
                Mathf.Max(
                    dx,
                    dy
                )
            );

        for (
            int i = 0;
            i <= pasos;
            i++
        )
        {
            float p =
                i /
                (float)pasos;

            int x =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        x0,
                        x1,
                        p
                    )
                );

            int y =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        y0,
                        y1,
                        p
                    )
                );

            int mitad =
                Mathf.Max(
                    1,
                    grosor / 2
                );

            for (
                int yy =
                    y - mitad;
                yy <=
                    y + mitad;
                yy++
            )
            {
                for (
                    int xx =
                        x - mitad;
                    xx <=
                        x + mitad;
                    xx++
                )
                {
                    Pintar(
                        t,
                        xx,
                        yy,
                        c
                    );
                }
            }
        }
    }

    private static void Pintar(
        Texture2D t,
        int x,
        int y,
        Color32 c)
    {
        if (
            x < 0 ||
            y < 0 ||
            x >= t.width ||
            y >= t.height
        )
        {
            return;
        }

        t.SetPixel(
            x,
            y,
            c
        );
    }
}
