using UnityEngine;
using UnityEngine.UI;

public class CharacterPreview3D :
    MonoBehaviour
{
    private const int PreviewLayer =
        31;

    private GameObject mundoPreview;
    private GameObject modelo;

    private Camera camara;

    private RenderTexture renderTexture;

    private RawImage destino;

    private bool inicializado;

    private float velocidadRotacion =
        18f;

    public string AvatarActual
    {
        get;
        private set;
    }

    public void Inicializar(
        RawImage rawImage,
        string avatarId)
    {
        if (rawImage == null)
        {
            Debug.LogError(
                "CharacterPreview3D: " +
                "RawImage es null."
            );

            return;
        }

        destino =
            rawImage;

        if (!inicializado)
        {
            CrearInfraestructura();

            inicializado =
                true;
        }

        MostrarAvatar(
            avatarId
        );
    }

    public void MostrarAvatar(
        string avatarId)
    {
        if (!inicializado)
        {
            Debug.LogWarning(
                "CharacterPreview3D " +
                "todavia no esta inicializado."
            );

            return;
        }

        GameObject prefab =
            AvatarRegistry.CargarPrefab(
                avatarId
            );

        if (prefab == null)
            return;

        if (modelo != null)
        {
            Destroy(
                modelo
            );

            modelo =
                null;
        }

        modelo =
            Instantiate(
                prefab,
                mundoPreview.transform,
                false
            );

        modelo.name =
            "Preview_" +
            avatarId;

        modelo.transform.localPosition =
            prefab.transform.localPosition;

        // En nuestro modelo actual esto hace
        // que empiece mirando hacia el frente.
        modelo.transform.localRotation =
            prefab.transform.localRotation *
            Quaternion.Euler(
                0f,
                180f,
                0f
            );

        modelo.transform.localScale =
            prefab.transform.localScale;

        AplicarLayerRecursivo(
            modelo,
            PreviewLayer
        );

        Animator animator =
            modelo.GetComponentInChildren<
                Animator>(
                true
            );

        if (animator != null)
        {
            animator.applyRootMotion =
                false;

            animator.Rebind();
            animator.Update(0f);
        }

        AvatarActual =
            avatarId;

        EncajarPersonaje();

        Debug.Log(
            "Preview mostrando avatar -> " +
            avatarId
        );
    }

    private void CrearInfraestructura()
    {
        CrearRenderTexture();
        CrearMundoPreview();
        CrearCamara();
        CrearIluminacion();
    }

    private void CrearRenderTexture()
    {
        renderTexture =
            new RenderTexture(
                700,
                760,
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

        mundoPreview.transform.position =
            new Vector3(
                1000f,
                1000f,
                1000f
            );
    }

    private void CrearCamara()
    {
        GameObject objeto =
            new GameObject(
                "PreviewCamera"
            );

        objeto.transform.SetParent(
            mundoPreview.transform,
            false
        );

        camara =
            objeto.AddComponent<Camera>();

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
            500f;
    }

    private void CrearIluminacion()
    {
        GameObject luzPrincipalObjeto =
            new GameObject(
                "PreviewLight"
            );

        luzPrincipalObjeto
            .transform
            .SetParent(
                mundoPreview.transform,
                false
            );

        Light luzPrincipal =
            luzPrincipalObjeto
                .AddComponent<Light>();

        luzPrincipal.type =
            LightType.Directional;

        luzPrincipal.intensity =
            1.5f;

        luzPrincipal.shadows =
            LightShadows.None;

        luzPrincipal.cullingMask =
            1 << PreviewLayer;

        luzPrincipalObjeto
            .transform
            .rotation =
                Quaternion.Euler(
                    35f,
                    -35f,
                    0f
                );

        GameObject rellenoObjeto =
            new GameObject(
                "PreviewFillLight"
            );

        rellenoObjeto
            .transform
            .SetParent(
                mundoPreview.transform,
                false
            );

        Light relleno =
            rellenoObjeto
                .AddComponent<Light>();

        relleno.type =
            LightType.Directional;

        relleno.intensity =
            0.7f;

        relleno.shadows =
            LightShadows.None;

        relleno.cullingMask =
            1 << PreviewLayer;

        rellenoObjeto
            .transform
            .rotation =
                Quaternion.Euler(
                    20f,
                    140f,
                    0f
                );
    }

    private void EncajarPersonaje()
    {
        if (
            modelo == null ||
            camara == null
        )
        {
            return;
        }

        Renderer[] renderers =
            modelo.GetComponentsInChildren<
                Renderer>(
                true
            );

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
        {
            Debug.LogWarning(
                "El avatar no tiene Renderers."
            );

            return;
        }

        float altura =
            Mathf.Max(
                bounds.size.y,
                0.1f
            );

        float ancho =
            Mathf.Max(
                bounds.size.x,
                0.1f
            );

        Vector3 centro =
            bounds.center;

        float tamanoVertical =
            altura * 0.58f;

        float tamanoHorizontal =
            ancho /
            Mathf.Max(
                0.1f,
                2f * camara.aspect
            ) *
            1.2f;

        camara.orthographicSize =
            Mathf.Max(
                0.5f,
                tamanoVertical,
                tamanoHorizontal
            );

        camara.transform.position =
            centro +
            new Vector3(
                0f,
                altura * 0.03f,
                altura * 2f
            );

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