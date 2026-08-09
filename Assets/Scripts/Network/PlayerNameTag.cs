using TMPro;
using UnityEngine;

public class PlayerNameTag : MonoBehaviour
{
    [Header("Configuracion")]
    public float separacionCabeza = 0.3f;

    private Canvas canvas;
    private TMP_Text textoNombre;

    private Camera camara;
    private Transform cabeza;

    private string ultimoNombre = "";

    private void Start()
    {
        BuscarCabeza();
        CrearNombre();
        ActualizarNombre();
    }

    private void LateUpdate()
    {
        if (canvas == null)
            return;

        if (camara == null)
        {
            camara =
                Camera.main;
        }

        ActualizarPosicion();
        ActualizarRotacion();
        ActualizarNombre();
    }

    private void BuscarCabeza()
    {
        Animator animator =
            GetComponentInChildren<Animator>();

        if (
            animator != null &&
            animator.isHuman
        )
        {
            cabeza =
                animator.GetBoneTransform(
                    HumanBodyBones.Head
                );
        }
    }

    private void CrearNombre()
    {
        GameObject objetoCanvas =
            new GameObject(
                "NombreJugadorUI"
            );

        objetoCanvas.transform.SetParent(
            transform,
            true
        );

        canvas =
            objetoCanvas.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.WorldSpace;

        canvas.overrideSorting =
            true;

        canvas.sortingOrder =
            200;

        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();

        canvasRect.sizeDelta =
            new Vector2(
                220f,
                50f
            );

        canvasRect.localScale =
            Vector3.one * 0.004f;

        GameObject objetoTexto =
            new GameObject(
                "NombreUsuario",
                typeof(RectTransform)
            );

        objetoTexto.transform.SetParent(
            objetoCanvas.transform,
            false
        );

        textoNombre =
            objetoTexto
                .AddComponent<TextMeshProUGUI>();

        textoNombre.text =
            "Jugador";

        textoNombre.fontSize =
            30f;

        textoNombre.fontStyle =
            FontStyles.Bold;

        textoNombre.color =
            Color.white;

        textoNombre.alignment =
            TextAlignmentOptions.Center;

        textoNombre.raycastTarget =
            false;

        RectTransform textoRect =
            textoNombre.rectTransform;

        textoRect.anchorMin =
            Vector2.zero;

        textoRect.anchorMax =
            Vector2.one;

        textoRect.offsetMin =
            Vector2.zero;

        textoRect.offsetMax =
            Vector2.zero;
    }

    private void ActualizarNombre()
    {
        if (textoNombre == null)
            return;

        string nombre =
            "Jugador";

        if (
            NakamaAuthService.Instance != null &&
            NakamaAuthService.Instance
                .EstaAutenticado &&
            !string.IsNullOrWhiteSpace(
                NakamaAuthService.Instance
                    .NombreUsuarioActual
            )
        )
        {
            nombre =
                NakamaAuthService
                    .Instance
                    .NombreUsuarioActual;
        }

        if (
            nombre ==
            ultimoNombre
        )
        {
            return;
        }

        ultimoNombre =
            nombre;

        textoNombre.text =
            nombre;
    }

    private void ActualizarPosicion()
    {
        if (canvas == null)
            return;

        Vector3 posicion;

        if (cabeza != null)
        {
            posicion =
                cabeza.position +
                Vector3.up *
                separacionCabeza;
        }
        else
        {
            posicion =
                ObtenerParteSuperiorJugador() +
                Vector3.up *
                separacionCabeza;
        }

        canvas.transform.position =
            posicion;
    }

    private Vector3 ObtenerParteSuperiorJugador()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>();

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

        if (encontrado)
        {
            return new Vector3(
                bounds.center.x,
                bounds.max.y,
                bounds.center.z
            );
        }

        return
            transform.position +
            Vector3.up * 2f;
    }

    private void ActualizarRotacion()
    {
        if (
            canvas == null ||
            camara == null
        )
        {
            return;
        }

        canvas.transform.rotation =
            camara.transform.rotation;
    }
}