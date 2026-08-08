using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSelector : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GameObject piso;
    public PlayerMovement jugador;
    public Pathfinding pathfinding;

    private GameObject marcador;

    private void Start()
    {
        CrearMarcador();
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DetectarCasilla();
        }
    }

    private void DetectarCasilla()
    {
        if (
            grid == null ||
            piso == null ||
            jugador == null ||
            pathfinding == null
        )
        {
            return;
        }

        Camera camara = Camera.main;

        if (camara == null)
            return;

        Vector2 posicionMouse =
            Mouse.current.position.ReadValue();

        Ray ray =
            camara.ScreenPointToRay(posicionMouse);

        // Calculamos la altura real de la superficie del piso.
        float alturaPiso = piso.transform.position.y;

        Collider colliderPiso =
            piso.GetComponent<Collider>();

        if (colliderPiso != null)
        {
            alturaPiso =
                colliderPiso.bounds.max.y;
        }

        // Plano invisible exactamente encima del suelo.
        Plane planoPiso =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    alturaPiso,
                    0f
                )
            );

        // Ya NO importa si un mueble está delante.
        if (!planoPiso.Raycast(ray, out float distancia))
            return;

        Vector3 puntoPiso =
            ray.GetPoint(distancia);

        if (
            !grid.ObtenerCasilla(
                puntoPiso,
                out Vector2Int destino
            )
        )
        {
            return;
        }

        if (
            !grid.ObtenerCasilla(
                jugador.transform.position,
                out Vector2Int inicio
            )
        )
        {
            return;
        }

        List<Vector2Int> rutaCasillas =
            pathfinding.EncontrarRuta(
                inicio,
                destino
            );

        if (
            rutaCasillas.Count == 0 &&
            inicio != destino
        )
        {
            Debug.Log(
                "No se puede llegar a esa casilla."
            );

            return;
        }

        MostrarMarcador(
            destino,
            alturaPiso
        );

        List<Vector3> rutaMundo =
            new List<Vector3>();

        foreach (Vector2Int casilla in rutaCasillas)
        {
            Vector3 punto =
                grid.ObtenerCentroCasilla(
                    casilla,
                    jugador.transform.position.y
                );

            rutaMundo.Add(punto);
        }

        jugador.SeguirRuta(rutaMundo);

        Debug.Log(
            "Destino seleccionado: (" +
            destino.x + ", " +
            destino.y + ")"
        );
    }

    private void MostrarMarcador(
        Vector2Int casilla,
        float alturaPiso)
    {
        Vector3 posicion =
            grid.ObtenerCentroCasilla(
                casilla,
                alturaPiso + 0.03f
            );

        marcador.transform.position = posicion;
        marcador.SetActive(true);
    }

    private void CrearMarcador()
    {
        marcador =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        marcador.name =
            "CasillaSeleccionada";

        marcador.transform.localScale =
            new Vector3(
                grid.tamanoCasilla * 0.9f,
                0.03f,
                grid.tamanoCasilla * 0.9f
            );

        Destroy(
            marcador.GetComponent<Collider>()
        );

        Renderer renderer =
            marcador.GetComponent<Renderer>();

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Lit"
            );

        if (shader != null)
        {
            Material material =
                new Material(shader);

            material.SetColor(
                "_BaseColor",
                Color.green
            );

            renderer.material = material;
        }

        marcador.SetActive(false);
    }
}