using UnityEngine;

public class FurnitureInventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelInventario;
    public FurnitureSpawner spawner;

    private void Start()
    {
        Cerrar();
    }

    public void Abrir()
    {
        if (panelInventario != null)
        {
            panelInventario.SetActive(true);
        }
    }

    public void Cerrar()
    {
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }
    }

    public void Alternar()
    {
        if (panelInventario == null)
            return;

        panelInventario.SetActive(
            !panelInventario.activeSelf
        );
    }

    public void SeleccionarMueble(int indice)
    {
        if (spawner == null)
            return;

        spawner.CrearMueble(indice);

        Cerrar();
    }
}