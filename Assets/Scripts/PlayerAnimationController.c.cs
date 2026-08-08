using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerState estado;
    public Animator animator;

    private static readonly int CaminandoHash =
        Animator.StringToHash("Caminando");

    private void Update()
    {
        if (estado == null || animator == null)
            return;

        bool caminando =
            estado.estadoActual ==
            PlayerState.Estado.Caminando;

        animator.SetBool(
            CaminandoHash,
            caminando
        );
    }
}