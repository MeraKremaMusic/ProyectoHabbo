using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class FurniturePreview3DUI :
    MonoBehaviour
{
    private const float RetrasoHover =
        0.14f;

    private GameObject prefab;
    private RawImage imagen;
    private RenderTexture textura;

    private bool listo;
    private bool hoverActivo;

    private Coroutine rutinaHover;

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
        if (!TienePreview)
            return;

        hoverActivo =
            true;

        CancelarRutinaHover();

        // No arrancamos la cámara compartida inmediatamente.
        // Esto evita que al cruzar muchas tarjetas rápidamente
        // los modelos puedan verse durante un instante en otra card.
        rutinaHover =
            StartCoroutine(
                ActivarRotacionConRetraso()
            );
    }

    public void DesactivarHover()
    {
        hoverActivo =
            false;

        CancelarRutinaHover();

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

    private IEnumerator ActivarRotacionConRetraso()
    {
        float transcurrido =
            0f;

        while (
            transcurrido <
            RetrasoHover
        )
        {
            if (
                !hoverActivo ||
                !isActiveAndEnabled
            )
            {
                rutinaHover =
                    null;

                yield break;
            }

            transcurrido +=
                Time.unscaledDeltaTime;

            yield return null;
        }

        rutinaHover =
            null;

        if (
            !hoverActivo ||
            !TienePreview ||
            FurniturePreviewRenderer
                .Instance == null
        )
        {
            yield break;
        }

        FurniturePreviewRenderer
            .Instance
            .IniciarRotacion(
                prefab,
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

    private void CancelarRutinaHover()
    {
        if (rutinaHover == null)
            return;

        StopCoroutine(
            rutinaHover
        );

        rutinaHover =
            null;
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

    private void OnDisable()
    {
        DesactivarHover();
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
