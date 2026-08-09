using UnityEngine;
using UnityEngine.UI;

public sealed class FurniturePreview3DUI :
    MonoBehaviour
{
    private GameObject prefab;
    private RawImage imagen;
    private RenderTexture textura;

    private bool listo;

    public bool TienePreview =>
        listo &&
        prefab != null &&
        textura != null;

    public void Inicializar(
        GameObject prefabMueble)
    {
        prefab =
            prefabMueble;

        imagen =
            gameObject.GetComponent<
                RawImage>();

        if (imagen == null)
        {
            imagen =
                gameObject.AddComponent<
                    RawImage>();
        }

        imagen.color =
            Color.white;

        imagen.raycastTarget =
            false;

        if (prefab == null)
        {
            imagen.enabled =
                false;

            listo =
                false;

            return;
        }

        textura =
            new RenderTexture(
                320,
                210,
                16,
                RenderTextureFormat.ARGB32
            );

        textura.name =
            "InventoryPreview_" +
            prefab.name;

        textura.antiAliasing =
            2;

        textura.filterMode =
            FilterMode.Bilinear;

        textura.wrapMode =
            TextureWrapMode.Clamp;

        textura.Create();

        imagen.texture =
            textura;

        listo =
            true;

        SolicitarRenderInicial();
    }

    public void ActivarHover()
    {
        if (
            !TienePreview ||
            FurniturePreviewRenderer
                .Instance == null
        )
        {
            return;
        }

        FurniturePreviewRenderer
            .Instance
            .IniciarRotacion(
                prefab,
                textura
            );
    }

    public void DesactivarHover()
    {
        if (
            textura == null ||
            FurniturePreviewRenderer
                .Instance == null
        )
        {
            return;
        }

        FurniturePreviewRenderer
            .Instance
            .DetenerRotacion(
                textura
            );
    }

    private void SolicitarRenderInicial()
    {
        if (
            !TienePreview ||
            FurniturePreviewRenderer
                .Instance == null
        )
        {
            return;
        }

        FurniturePreviewRenderer
            .Instance
            .RenderizarUnaVez(
                prefab,
                textura
            );
    }

    private void OnEnable()
    {
        if (
            listo &&
            textura != null
        )
        {
            SolicitarRenderInicial();
        }
    }

    private void OnDestroy()
    {
        DesactivarHover();

        if (textura != null)
        {
            textura.Release();

            Destroy(
                textura
            );

            textura =
                null;
        }
    }
}
