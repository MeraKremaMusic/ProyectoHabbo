using UnityEngine;
using UnityEngine.UI;

public class CharacterPreview3D : MonoBehaviour
{
    private const int PreviewLayer = 31;

    private GameObject mundoPreview;
    private GameObject modelo;
    private Camera camara;
    private RenderTexture renderTexture;

    private float velocidadRotacion = 18f;

    public void Inicializar(RawImage destino)
    {
        if (destino == null)
        {
            Debug.LogError(
                "CharacterPreview3D: no existe RawImage."
            );

            return;
        }

        GameObject prefab =
            Resources.Load<GameObject>(
                "Characters/PersonajePreview"
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No se encontro Resources/Characters/PersonajePreview.prefab"
            );

            return;
        }

        CrearRenderTexture(destino);
        CrearMundoPreview();
        CrearModelo(prefab);
        CrearCamara();
        CrearIluminacion();
        EncajarPersonaje();

        Debug.Log(
            "Preview 3D del personaje creado."
        );
    }

    private void CrearRenderTexture(
        RawImage destino)
    {
        renderTexture =
            new RenderTexture(
                600,
                700,
                24,
                RenderTextureFormat.ARGB32
            );

        renderTexture.name =
            "CharacterPreviewTexture";

        renderTexture.Create();

        destino.texture =
            renderTexture;

        destino.color =
            Color.white;
    }

    private void CrearMundoPreview()
    {
        mundoPreview =
            new GameObject(
                "CharacterPreviewWorld"
            );

        // Lo ponemos muy lejos de la habitación.
        mundoPreview.transform.position =
            new Vector3(
                1000f,
                1000f,
                1000f
            );
    }

    private void CrearModelo(
        GameObject prefab)
    {
        modelo =
            Instantiate(
                prefab,
                mundoPreview.transform
            );

        modelo.name =
            "PersonajePreview3D";

        modelo.transform.localPosition =
            Vector3.zero;

        modelo.transform.localRotation =
            Quaternion.Euler(
                0f,
                180f,
                0f
            );

        AplicarLayerRecursivo(
            modelo,
            PreviewLayer
        );
    }

    private void CrearCamara()
    {
        GameObject objetoCamara =
            new GameObject(
                "PreviewCamera"
            );

        objetoCamara.transform.SetParent(
            mundoPreview.transform,
            false
        );

        camara =
            objetoCamara.AddComponent<Camera>();

        camara.orthographic =
            true;

        camara.clearFlags =
            CameraClearFlags.SolidColor;

        camara.backgroundColor =
            new Color(
                0f,
                0f,
                0f,
                0f
            );

        camara.cullingMask =
            1 << PreviewLayer;

        camara.targetTexture =
            renderTexture;

        camara.nearClipPlane =
            0.01f;

        camara.farClipPlane =
            100f;
    }

    private void CrearIluminacion()
    {
        GameObject objetoLuz =
            new GameObject(
                "PreviewLight"
            );

        objetoLuz.transform.SetParent(
            mundoPreview.transform,
            false
        );

        Light luz =
            objetoLuz.AddComponent<Light>();

        luz.type =
            LightType.Directional;

        luz.intensity =
            1.5f;

        luz.shadows =
            LightShadows.None;

        luz.cullingMask =
            1 << PreviewLayer;

        objetoLuz.transform.rotation =
            Quaternion.Euler(
                35f,
                -35f,
                0f
            );

        GameObject objetoLuzRelleno =
            new GameObject(
                "PreviewFillLight"
            );

        objetoLuzRelleno.transform.SetParent(
            mundoPreview.transform,
            false
        );

        Light relleno =
            objetoLuzRelleno.AddComponent<Light>();

        relleno.type =
            LightType.Directional;

        relleno.intensity =
            0.7f;

        relleno.shadows =
            LightShadows.None;

        relleno.cullingMask =
            1 << PreviewLayer;

        objetoLuzRelleno.transform.rotation =
            Quaternion.Euler(
                20f,
                140f,
                0f
            );
    }

    private void EncajarPersonaje()
    {
        Renderer[] renderers =
            modelo.GetComponentsInChildren<Renderer>();

        if (
            renderers == null ||
            renderers.Length == 0
        )
        {
            Debug.LogWarning(
                "El personaje no tiene Renderers."
            );

            return;
        }

        bool encontrado =
            false;

        Bounds bounds =
            new Bounds();

        foreach (
            Renderer renderer
            in renderers
        )
        {
            if (
                renderer == null ||
                !renderer.enabled
            )
            {
                continue;
            }

            if (!encontrado)
            {
                bounds =
                    renderer.bounds;

                encontrado =
                    true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds
                );
            }
        }

        if (!encontrado)
            return;

        Vector3 centro =
            bounds.center;

        float altura =
            bounds.size.y;

        camara.orthographicSize =
            Mathf.Max(
                0.5f,
                altura * 0.62f
            );

        Vector3 posicionCamara =
            centro +
            new Vector3(
                0f,
                altura * 0.03f,
                altura * 2f
            );

        camara.transform.position =
            posicionCamara;

        camara.transform.LookAt(
            centro +
            Vector3.up *
            altura *
            0.03f
        );
    }

    private void Update()
    {
        if (modelo == null)
            return;

        modelo.transform.Rotate(
            0f,
            velocidadRotacion *
            Time.deltaTime,
            0f,
            Space.World
        );
    }

    private void AplicarLayerRecursivo(
        GameObject objeto,
        int layer)
    {
        objeto.layer =
            layer;

        foreach (
            Transform hijo
            in objeto.transform
        )
        {
            AplicarLayerRecursivo(
                hijo.gameObject,
                layer
            );
        }
    }

    private void OnDestroy()
    {
        if (mundoPreview != null)
        {
            Destroy(
                mundoPreview
            );
        }

        if (renderTexture != null)
        {
            renderTexture.Release();

            Destroy(
                renderTexture
            );
        }
    }
}