using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FurnitureSelection : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;

    [Header("Estado")]
    public GameObject muebleSeleccionado;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        // No seleccionar muebles mientras
        // estamos colocando otro.
        if (
            placement != null &&
            placement.EstaColocando
        )
        {
            return;
        }

        // Ignorar clics sobre la interfaz.
        if (
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()
        )
        {
            return;
        }

        // Desde ahora los muebles se editan
        // con CLIC DERECHO.
        if (
            Mouse.current.rightButton
                .wasPressedThisFrame
        )
        {
            DetectarMueble();
        }
    }

    private void DetectarMueble()
    {
        Camera camara = Camera.main;

        if (camara == null)
            return;

        Vector2 posicionMouse =
            Mouse.current.position.ReadValue();

        Ray ray =
            camara.ScreenPointToRay(
                posicionMouse
            );

        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f
            )
        )
        {
            FurnitureData datos =
                hit.collider
                    .GetComponentInParent<FurnitureData>();

            if (datos != null)
            {
                muebleSeleccionado =
                    datos.gameObject;

                Debug.Log(
                    "Mueble seleccionado: " +
                    muebleSeleccionado.name
                );

                return;
            }
        }

        muebleSeleccionado = null;
    }

    public void Deseleccionar()
    {
        muebleSeleccionado = null;
    }
}