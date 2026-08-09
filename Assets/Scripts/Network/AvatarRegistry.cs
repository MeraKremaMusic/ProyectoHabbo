using UnityEngine;

public static class AvatarRegistry
{
    public const string PersonajeBase =
        "personaje_base";

    public static GameObject CargarPrefab(
        string avatarId)
    {
        string ruta =
            ObtenerRuta(
                avatarId
            );

        if (
            string.IsNullOrWhiteSpace(
                ruta
            )
        )
        {
            Debug.LogError(
                "Avatar desconocido: " +
                avatarId
            );

            return null;
        }

        GameObject prefab =
            Resources.Load<GameObject>(
                ruta
            );

        if (prefab == null)
        {
            Debug.LogError(
                "No se encontro el prefab del avatar: " +
                ruta
            );
        }

        return prefab;
    }

    private static string ObtenerRuta(
        string avatarId)
    {
        switch (avatarId)
        {
            case PersonajeBase:

                return
                    "Characters/PersonajePreview";

            default:

                return null;
        }
    }
}