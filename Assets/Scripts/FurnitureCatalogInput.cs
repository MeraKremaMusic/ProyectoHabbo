using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureCatalogInput : MonoBehaviour
{
    [Header("Referencias")]
    public FurnitureSpawner spawner;

    private void Update()
    {
        if (
            Keyboard.current == null ||
            spawner == null
        )
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            spawner.CrearMueble(0);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            spawner.CrearMueble(1);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            spawner.CrearMueble(2);
        }
    }
}