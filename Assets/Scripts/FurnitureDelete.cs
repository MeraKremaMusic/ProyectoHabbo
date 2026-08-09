using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureDelete :
    MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (
            Keyboard.current
                .deleteKey
                .wasPressedThisFrame
        )
        {
            EliminarSeleccionado();
        }
    }


    public void EliminarSeleccionado()
    {
        Debug.LogWarning(
            "Eliminar muebles esta deshabilitado. " +
            "Usa Recoger. El reciclaje se implementara por separado."
        );
    }
}