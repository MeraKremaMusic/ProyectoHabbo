using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class PlayerAvatarLoader : MonoBehaviour
{
    public bool AvatarCargado
    {
        get;
        private set;
    }

    public string AvatarIdActual
    {
        get;
        private set;
    }

    private void Start()
    {
        CargarAvatarCorrespondiente();
    }

    private void CargarAvatarCorrespondiente()
    {
        string avatarId =
            AvatarRegistry.PersonajeBase;

        // Si existe una sesion real,
        // usamos el avatar guardado en Nakama.
        if (
            NakamaAuthService.Instance != null &&
            NakamaAuthService.Instance.EstaAutenticado &&
            NakamaPlayerProfileService.Instance != null
        )
        {
            PlayerProfileData perfil =
                NakamaPlayerProfileService
                    .Instance
                    .PerfilActual;

            if (
                perfil != null &&
                perfil.personajeCreado &&
                !string.IsNullOrWhiteSpace(
                    perfil.avatarId
                )
            )
            {
                avatarId =
                    perfil.avatarId;

                Debug.Log(
                    "Avatar obtenido desde Nakama: " +
                    avatarId
                );
            }
            else
            {
                Debug.Log(
                    "Perfil sin avatar. " +
                    "Usando personaje_base."
                );
            }
        }
        else
        {
            Debug.Log(
                "Sin sesion activa. " +
                "Usando personaje_base para pruebas."
            );
        }

        CargarAvatar(
            avatarId
        );
    }

    private void CargarAvatar(
        string avatarId)
    {
        GameObject prefab =
            AvatarRegistry.CargarPrefab(
                avatarId
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No se pudo cargar el avatar: " +
                avatarId
            );

            return;
        }

        // Evita duplicados si por alguna razon
        // el cargador se ejecuta mas de una vez.
        Transform avatarExistente =
            transform.Find(
                "AvatarVisual"
            );

        if (avatarExistente != null)
        {
            Destroy(
                avatarExistente.gameObject
            );
        }

        PlayerFacing facing =
            GetComponent<PlayerFacing>();

        PlayerAnimationController
            animationController =
                GetComponent<
                    PlayerAnimationController>();

        GameObject nuevoAvatar =
            Instantiate(
                prefab,
                transform,
                false
            );

        nuevoAvatar.name =
            "AvatarVisual";

        nuevoAvatar.transform.localPosition =
            prefab.transform.localPosition;

        nuevoAvatar.transform.localRotation =
            prefab.transform.localRotation;

        nuevoAvatar.transform.localScale =
            prefab.transform.localScale;

        Animator nuevoAnimator =
            nuevoAvatar.GetComponentInChildren<
                Animator>(
                true
            );

        if (nuevoAnimator == null)
        {
            Debug.LogError(
                "El avatar " +
                avatarId +
                " no tiene Animator."
            );

            Destroy(
                nuevoAvatar
            );

            return;
        }

        nuevoAnimator.applyRootMotion =
            false;

        // El sistema de giro controla
        // el nuevo modelo.
        if (facing != null)
        {
            facing.visual =
                nuevoAvatar.transform;
        }

        // El sistema de animaciones controla
        // el Animator nuevo.
        if (
            animationController != null
        )
        {
            animationController.animator =
                nuevoAnimator;
        }

        nuevoAnimator.Rebind();
        nuevoAnimator.Update(0f);

        AvatarCargado =
            true;

        AvatarIdActual =
            avatarId;

        Debug.Log(
            "AVATAR DINAMICO CARGADO -> " +
            avatarId
        );
    }
}