using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class FurniturePreviewRenderer :
    MonoBehaviour
{
    public static FurniturePreviewRenderer Instance
    {
        get;
        private set;
    }

    private const int PreviewLayer = 31;
    private const float PreviewFps = 24f;

    private Transform escenario;
    private Camera camara;
    private Light luzPrincipal;
    private Light luzRelleno;

    private GameObject modeloActivo;
    private RenderTexture texturaAnimada;
    private GameObject prefabAnimado;

    private float siguienteRender;

    private readonly List<Mesh>
        meshesTemporales =
            new List<Mesh>();

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearAutomaticamente()
    {
        if (Instance != null)
            return;

        GameObject objeto =
            new GameObject(
                "FurniturePreviewRenderer"
            );

        objeto.AddComponent<
            FurniturePreviewRenderer>();

        DontDestroyOnLoad(
            objeto
        );
    }

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );

        CrearEscenario();
    }

    private void Update()
    {
        if (
            modeloActivo == null ||
            texturaAnimada == null
        )
        {
            return;
        }

        modeloActivo.transform.Rotate(
            Vector3.up,
            28f *
            Time.unscaledDeltaTime,
            Space.World
        );

        if (
            Time.unscaledTime <
            siguienteRender
        )
        {
            return;
        }

        siguienteRender =
            Time.unscaledTime +
            1f / PreviewFps;

        RenderizarActual(
            texturaAnimada
        );
    }

    public void RenderizarUnaVez(
        GameObject prefab,
        RenderTexture destino)
    {
        if (
            prefab == null ||
            destino == null
        )
        {
            return;
        }

        LimpiarModelo();

        modeloActivo =
            CrearCopiaVisual(
                prefab
            );

        if (modeloActivo == null)
            return;

        PrepararCamara(
            destino
        );

        RenderizarActual(
            destino
        );

        LimpiarModelo();
    }

    public void IniciarRotacion(
        GameObject prefab,
        RenderTexture destino)
    {
        if (
            prefab == null ||
            destino == null
        )
        {
            return;
        }

        if (
            texturaAnimada == destino &&
            prefabAnimado == prefab &&
            modeloActivo != null
        )
        {
            return;
        }

        LimpiarModelo();

        prefabAnimado =
            prefab;

        texturaAnimada =
            destino;

        modeloActivo =
            CrearCopiaVisual(
                prefab
            );

        if (modeloActivo == null)
        {
            prefabAnimado = null;
            texturaAnimada = null;
            return;
        }

        PrepararCamara(
            destino
        );

        RenderizarActual(
            destino
        );

        siguienteRender =
            0f;
    }

    public void DetenerRotacion(
        RenderTexture destino)
    {
        if (
            texturaAnimada != destino
        )
        {
            return;
        }

        if (
            modeloActivo != null &&
            texturaAnimada != null
        )
        {
            RenderizarActual(
                texturaAnimada
            );
        }

        prefabAnimado =
            null;

        texturaAnimada =
            null;

        LimpiarModelo();
    }

    private void CrearEscenario()
    {
        GameObject objetoEscenario =
            new GameObject(
                "PreviewStage"
            );

        objetoEscenario.transform.SetParent(
            transform,
            false
        );

        objetoEscenario.transform.position =
            new Vector3(
                50000f,
                50000f,
                50000f
            );

        escenario =
            objetoEscenario.transform;

        GameObject objetoCamara =
            new GameObject(
                "PreviewCamera"
            );

        objetoCamara.transform.SetParent(
            transform,
            false
        );

        camara =
            objetoCamara.AddComponent<
                Camera>();

        camara.enabled =
            false;

        camara.clearFlags =
            CameraClearFlags.SolidColor;

        camara.backgroundColor =
            new Color(
                0f,
                0f,
                0f,
                0f
            );

        camara.orthographic =
            true;

        camara.nearClipPlane =
            0.01f;

        camara.farClipPlane =
            100f;

        camara.allowHDR =
            true;

        camara.allowMSAA =
            true;

        camara.cullingMask =
            1 << PreviewLayer;

        CrearLuces();
    }

    private void CrearLuces()
    {
        GameObject principal =
            new GameObject(
                "PreviewKeyLight"
            );

        principal.transform.SetParent(
            transform,
            false
        );

        principal.transform.rotation =
            Quaternion.Euler(
                42f,
                -38f,
                0f
            );

        luzPrincipal =
            principal.AddComponent<
                Light>();

        luzPrincipal.type =
            LightType.Directional;

        luzPrincipal.intensity =
            1.35f;

        luzPrincipal.color =
            new Color(
                1f,
                0.95f,
                0.88f
            );

        luzPrincipal.shadows =
            LightShadows.Soft;

        luzPrincipal.cullingMask =
            1 << PreviewLayer;

        GameObject relleno =
            new GameObject(
                "PreviewFillLight"
            );

        relleno.transform.SetParent(
            transform,
            false
        );

        relleno.transform.rotation =
            Quaternion.Euler(
                28f,
                142f,
                0f
            );

        luzRelleno =
            relleno.AddComponent<
                Light>();

        luzRelleno.type =
            LightType.Directional;

        luzRelleno.intensity =
            0.72f;

        luzRelleno.color =
            new Color(
                0.70f,
                0.82f,
                1f
            );

        luzRelleno.shadows =
            LightShadows.None;

        luzRelleno.cullingMask =
            1 << PreviewLayer;
    }

    private GameObject CrearCopiaVisual(
        GameObject prefab)
    {
        if (prefab == null)
            return null;

        GameObject raiz =
            new GameObject(
                "Preview_" +
                prefab.name
            );

        raiz.layer =
            PreviewLayer;

        raiz.transform.SetParent(
            escenario,
            false
        );

        raiz.transform.localPosition =
            Vector3.zero;

        raiz.transform.localRotation =
            Quaternion.identity;

        raiz.transform.localScale =
            Vector3.one;

        CopiarJerarquia(
            prefab.transform,
            raiz.transform,
            true
        );

        Renderer[] renderers =
            raiz.GetComponentsInChildren<
                Renderer>(
                true
            );

        if (
            renderers == null ||
            renderers.Length == 0
        )
        {
            Destroy(raiz);
            return null;
        }

        return raiz;
    }

    private void CopiarJerarquia(
        Transform origen,
        Transform padreDestino,
        bool esRaiz)
    {
        if (
            origen == null ||
            !origen.gameObject.activeSelf
        )
        {
            return;
        }

        GameObject visual =
            new GameObject(
                origen.name
            );

        visual.layer =
            PreviewLayer;

        visual.transform.SetParent(
            padreDestino,
            false
        );

        visual.transform.localPosition =
            esRaiz
                ? Vector3.zero
                : origen.localPosition;

        visual.transform.localRotation =
            origen.localRotation;

        visual.transform.localScale =
            origen.localScale;

        MeshFilter meshFilterOrigen =
            origen.GetComponent<
                MeshFilter>();

        MeshRenderer meshRendererOrigen =
            origen.GetComponent<
                MeshRenderer>();

        if (
            meshFilterOrigen != null &&
            meshFilterOrigen.sharedMesh != null &&
            meshRendererOrigen != null &&
            meshRendererOrigen.enabled
        )
        {
            MeshFilter meshFilterDestino =
                visual.AddComponent<
                    MeshFilter>();

            meshFilterDestino.sharedMesh =
                meshFilterOrigen.sharedMesh;

            MeshRenderer rendererDestino =
                visual.AddComponent<
                    MeshRenderer>();

            rendererDestino.sharedMaterials =
                meshRendererOrigen
                    .sharedMaterials;

            rendererDestino.shadowCastingMode =
                ShadowCastingMode.On;

            rendererDestino.receiveShadows =
                true;
        }

        SkinnedMeshRenderer skinned =
            origen.GetComponent<
                SkinnedMeshRenderer>();

        if (
            skinned != null &&
            skinned.enabled &&
            skinned.sharedMesh != null
        )
        {
            Mesh mesh =
                new Mesh();

            mesh.name =
                "PreviewBaked_" +
                skinned.sharedMesh.name;

            skinned.BakeMesh(
                mesh
            );

            meshesTemporales.Add(
                mesh
            );

            MeshFilter meshFilterDestino =
                visual.AddComponent<
                    MeshFilter>();

            meshFilterDestino.sharedMesh =
                mesh;

            MeshRenderer rendererDestino =
                visual.AddComponent<
                    MeshRenderer>();

            rendererDestino.sharedMaterials =
                skinned.sharedMaterials;

            rendererDestino.shadowCastingMode =
                ShadowCastingMode.On;

            rendererDestino.receiveShadows =
                true;
        }

        for (
            int i = 0;
            i < origen.childCount;
            i++
        )
        {
            CopiarJerarquia(
                origen.GetChild(i),
                visual.transform,
                false
            );
        }
    }

    private void PrepararCamara(
        RenderTexture destino)
    {
        if (
            modeloActivo == null ||
            camara == null
        )
        {
            return;
        }

        Renderer[] renderers =
            modeloActivo.GetComponentsInChildren<
                Renderer>(
                true
            );

        if (
            renderers == null ||
            renderers.Length == 0
        )
        {
            return;
        }

        Bounds bounds =
            renderers[0].bounds;

        for (
            int i = 1;
            i < renderers.Length;
            i++
        )
        {
            bounds.Encapsulate(
                renderers[i].bounds
            );
        }

        Vector3 centro =
            bounds.center;

        float horizontal =
            Mathf.Max(
                bounds.extents.x,
                bounds.extents.z
            );

        float vertical =
            bounds.extents.y;

        float escala =
            Mathf.Max(
                vertical,
                horizontal * 0.86f
            );

        escala =
            Mathf.Max(
                escala,
                0.08f
            );

        camara.orthographicSize =
            escala * 1.42f;

        camara.aspect =
            destino.width /
            (float)destino.height;

        Vector3 direccion =
            new Vector3(
                1.5f,
                1.15f,
                -1.5f
            ).normalized;

        float distancia =
            Mathf.Max(
                4f,
                bounds.extents.magnitude *
                5f
            );

        camara.transform.position =
            centro +
            direccion *
            distancia;

        camara.transform.LookAt(
            centro +
            Vector3.up *
            vertical *
            0.04f
        );
    }

    private void RenderizarActual(
        RenderTexture destino)
    {
        if (
            camara == null ||
            modeloActivo == null ||
            destino == null
        )
        {
            return;
        }

        RenderTexture anterior =
            camara.targetTexture;

        AmbientMode modoAnterior =
            RenderSettings.ambientMode;

        Color cieloAnterior =
            RenderSettings.ambientSkyColor;

        Color ecuadorAnterior =
            RenderSettings.ambientEquatorColor;

        Color sueloAnterior =
            RenderSettings.ambientGroundColor;

        float intensidadAnterior =
            RenderSettings.ambientIntensity;

        try
        {
            RenderSettings.ambientMode =
                AmbientMode.Trilight;

            RenderSettings.ambientSkyColor =
                new Color(
                    0.72f,
                    0.76f,
                    0.82f
                );

            RenderSettings.ambientEquatorColor =
                new Color(
                    0.42f,
                    0.46f,
                    0.52f
                );

            RenderSettings.ambientGroundColor =
                new Color(
                    0.20f,
                    0.22f,
                    0.27f
                );

            RenderSettings.ambientIntensity =
                1f;

            camara.targetTexture =
                destino;

            camara.Render();
        }
        finally
        {
            camara.targetTexture =
                anterior;

            RenderSettings.ambientMode =
                modoAnterior;

            RenderSettings.ambientSkyColor =
                cieloAnterior;

            RenderSettings.ambientEquatorColor =
                ecuadorAnterior;

            RenderSettings.ambientGroundColor =
                sueloAnterior;

            RenderSettings.ambientIntensity =
                intensidadAnterior;
        }
    }

    private void LimpiarModelo()
    {
        if (modeloActivo != null)
        {
            modeloActivo.SetActive(
                false
            );

            Destroy(
                modeloActivo
            );

            modeloActivo =
                null;
        }

        foreach (
            Mesh mesh
            in meshesTemporales
        )
        {
            if (mesh != null)
                Destroy(mesh);
        }

        meshesTemporales.Clear();
    }

    private void OnDestroy()
    {
        LimpiarModelo();

        if (Instance == this)
            Instance = null;
    }
}
