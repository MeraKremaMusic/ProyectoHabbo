using UnityEngine;

public class FurniturePreview : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurniturePlacementValidator validator;

    private MaterialPropertyBlock bloque;
    private Renderer[] renderersActuales;

    private static readonly int BaseColor =
        Shader.PropertyToID("_BaseColor");

    private static readonly int ColorProperty =
        Shader.PropertyToID("_Color");

    private GameObject ultimoMueble;

    private void Awake()
    {
        bloque = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (
            placement == null ||
            validator == null
        )
        {
            return;
        }

        if (!placement.EstaColocando)
        {
            LimpiarPreview();
            return;
        }

        if (ultimoMueble != placement.muebleActual)
        {
            LimpiarPreview();

            ultimoMueble =
                placement.muebleActual;

            renderersActuales =
                ultimoMueble.GetComponentsInChildren<Renderer>();
        }

        bool valido =
            validator.PuedeColocarActual();

        AplicarColor(
            valido ? UnityEngine.Color.green
                   : UnityEngine.Color.red
        );
    }

    private void AplicarColor(
        UnityEngine.Color color)
    {
        if (renderersActuales == null)
            return;

        foreach (Renderer renderer in renderersActuales)
        {
            bloque.Clear();

            bloque.SetColor(
                BaseColor,
                color
            );

            bloque.SetColor(
                ColorProperty,
                color
            );

            renderer.SetPropertyBlock(bloque);
        }
    }

    public void LimpiarPreview()
    {
        if (renderersActuales != null)
        {
            foreach (Renderer renderer in renderersActuales)
            {
                if (renderer != null)
                {
                    renderer.SetPropertyBlock(null);
                }
            }
        }

        renderersActuales = null;
        ultimoMueble = null;
    }
}