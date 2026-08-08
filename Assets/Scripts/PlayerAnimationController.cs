using UnityEngine;

public class PlayerAnimationController :
    MonoBehaviour
{
    [Header("Referencias")]
    public PlayerState estado;
    public PlayerSitting sitting;
    public Animator animator;

    private static readonly int CaminandoHash =
        Animator.StringToHash(
            "Caminando"
        );

    private static readonly int SentadoHash =
        Animator.StringToHash(
            "Sentado"
        );

    private void Awake()
    {
        if (estado == null)
        {
            estado =
                GetComponent<PlayerState>();
        }

        if (sitting == null)
        {
            sitting =
                GetComponent<PlayerSitting>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null)
            return;

        bool estaSentado =
            sitting != null &&
            sitting.EstaSentado;

        bool caminando =
            !estaSentado &&
            estado != null &&
            estado.estadoActual ==
            PlayerState.Estado.Caminando;

        animator.SetBool(
            CaminandoHash,
            caminando
        );

        animator.SetBool(
            SentadoHash,
            estaSentado
        );
    }
}