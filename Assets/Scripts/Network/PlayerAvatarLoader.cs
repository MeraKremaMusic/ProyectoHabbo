using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class PlayerAvatarLoader :
    MonoBehaviour
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
        IntentarCargarAvatar();
    }

    private void IntentarCargarAvatar()
    {
        // Si abrimos HabitacionPrincipal
        // directamente desde el editor,
        // conservamos el personaje manual.
        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            Debug.Log(
                "AvatarLoader: no hay sesion. " +
                "Se conserva el avatar de la escena."
            );

            return;
        }

        if (
            NakamaPlayerProfileService
                .Instance == null
        )
        {
            Debug.LogWarning(
                "AvatarLoader: no existe " +
                "NakamaPlayerProfileService."
            );

            return;
        }

        PlayerProfileData perfil =
            NakamaPlayerProfileService
                .Instance
                .PerfilActual;

        if (perfil == null)
        {
            Debug.LogWarning(
                "AvatarLoader: el perfil " +
                "todavia no esta cargado."
            );

            return;
        }

        if (
            !perfil.personajeCreado ||
            string.IsNullOrWhiteSpace(
                perfil.avatarId
            )
        )
        {
            Debug.LogWarning(
                "AvatarLoader: el usuario " +
                "todavia no tiene avatar."
            );

            return;
        }

        CargarAvatar(
            perfil.avatarId
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
            return;

        PlayerFacing facing =
            GetComponent<PlayerFacing>();

        PlayerAnimationController
            animationController =
                GetComponent<
                    PlayerAnimationController>();

        Transform visualAnterior =
            null;

        if (facing != null)
        {
            visualAnterior =
                facing.visual;
        }

        // Ocultamos el modelo antiguo ANTES
        // de crear el nuevo.
        //
        // Así otros scripts como PlayerNameTag
        // encuentran únicamente el Animator nuevo.
        if (
            visualAnterior != null &&
            visualAnterior != transform
        )
        {
            visualAnterior
                .gameObject
                .SetActive(false);
        }

        GameObject nuevoAvatar =
            Instantiate(
                prefab,
                transform,
                false
            );

        nuevoAvatar.name =
            "AvatarVisual";

        // Conservamos exactamente la
        // configuración del prefab.
        nuevoAvatar
            .transform
            .localPosition =
                prefab
                    .transform
                    .localPosition;

        nuevoAvatar
            .transform
            .localRotation =
                prefab
                    .transform
                    .localRotation;

        nuevoAvatar
            .transform
            .localScale =
                prefab
                    .transform
                    .localScale;

        Animator nuevoAnimator =
            nuevoAvatar
                .GetComponentInChildren<
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

            if (
                visualAnterior != null &&
                visualAnterior != transform
            )
            {
                visualAnterior
                    .gameObject
                    .SetActive(true);
            }

            return;
        }

        nuevoAnimator.applyRootMotion =
            false;

        // PlayerFacing ahora gira
        // el avatar cargado.
        if (facing != null)
        {
            facing.visual =
                nuevoAvatar.transform;
        }

        // PlayerAnimationController ahora
        // controla el Animator cargado.
        if (
            animationController != null
        )
        {
            animationController.animator =
                nuevoAnimator;
        }

        nuevoAnimator.Rebind();
        nuevoAnimator.Update(0f);

        // Ya comprobamos que el avatar nuevo
        // funciona. Eliminamos el visual viejo.
        if (
            visualAnterior != null &&
            visualAnterior != transform
        )
        {
            Destroy(
                visualAnterior.gameObject
            );
        }

        AvatarCargado =
            true;

        AvatarIdActual =
            avatarId;

        Debug.Log(
            "AVATAR CARGADO DESDE NAKAMA -> " +
            avatarId
        );
    }
}