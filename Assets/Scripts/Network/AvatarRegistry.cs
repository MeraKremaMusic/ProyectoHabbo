using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AvatarDefinition
{
    public string Id { get; private set; }
    public string Nombre { get; private set; }
    public string Ruta { get; private set; }
    public GameObject Prefab { get; private set; }

    public AvatarDefinition(
        string id,
        string nombre,
        string ruta,
        GameObject prefab)
    {
        Id = id;
        Nombre = nombre;
        Ruta = ruta;
        Prefab = prefab;
    }
}

public static class AvatarRegistry
{
    public const string PersonajeBase =
        "personaje_base";

    private const string CarpetaResources =
        "Characters";

    private static AvatarDefinition[] cache;

    public static AvatarDefinition[] ObtenerAvatares()
    {
        AsegurarCache();

        return cache;
    }

    public static AvatarDefinition ObtenerAvatar(
        string avatarId)
    {
        AsegurarCache();

        if (string.IsNullOrWhiteSpace(avatarId))
            return null;

        return cache.FirstOrDefault(
            avatar =>
                string.Equals(
                    avatar.Id,
                    avatarId,
                    StringComparison.OrdinalIgnoreCase
                )
        );
    }

    public static GameObject CargarPrefab(
        string avatarId)
    {
        AvatarDefinition avatar =
            ObtenerAvatar(
                avatarId
            );

        if (avatar == null)
        {
            Debug.LogError(
                "Avatar desconocido: " +
                avatarId
            );

            return null;
        }

        if (avatar.Prefab == null)
        {
            Debug.LogError(
                "El avatar " +
                avatarId +
                " no tiene prefab."
            );

            return null;
        }

        return avatar.Prefab;
    }

    private static void AsegurarCache()
    {
        if (cache != null)
            return;

        GameObject[] prefabs =
            Resources.LoadAll<GameObject>(
                CarpetaResources
            );

        List<AvatarDefinition> encontrados =
            new List<AvatarDefinition>();

        foreach (
            GameObject prefab
            in prefabs.OrderBy(
                p => p.name
            )
        )
        {
            if (prefab == null)
                continue;

            string id;
            string nombre;

            // Compatibilidad con el avatar
            // que ya guardamos en Nakama.
            if (
                string.Equals(
                    prefab.name,
                    "PersonajePreview",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                id =
                    PersonajeBase;

                nombre =
                    "Personaje base";
            }
            else
            {
                id =
                    NormalizarId(
                        prefab.name
                    );

                nombre =
                    CrearNombreVisible(
                        prefab.name
                    );
            }

            if (
                encontrados.Any(
                    avatar =>
                        avatar.Id == id
                )
            )
            {
                Debug.LogWarning(
                    "AvatarRegistry: ID duplicado: " +
                    id
                );

                continue;
            }

            encontrados.Add(
                new AvatarDefinition(
                    id,
                    nombre,
                    CarpetaResources +
                    "/" +
                    prefab.name,
                    prefab
                )
            );
        }

        cache =
            encontrados
                .OrderBy(
                    avatar =>
                        avatar.Id ==
                        PersonajeBase
                            ? 0
                            : 1
                )
                .ThenBy(
                    avatar =>
                        avatar.Nombre
                )
                .ToArray();

        Debug.Log(
            "AvatarRegistry: " +
            cache.Length +
            " avatar(es) disponible(s)."
        );
    }

    private static string NormalizarId(
        string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "avatar";

        StringBuilder resultado =
            new StringBuilder();

        bool ultimoFueSeparador =
            false;

        foreach (
            char caracter
            in nombre.Trim()
        )
        {
            if (
                char.IsLetterOrDigit(
                    caracter
                )
            )
            {
                resultado.Append(
                    char.ToLowerInvariant(
                        caracter
                    )
                );

                ultimoFueSeparador =
                    false;
            }
            else if (!ultimoFueSeparador)
            {
                resultado.Append('_');

                ultimoFueSeparador =
                    true;
            }
        }

        return
            resultado
                .ToString()
                .Trim('_');
    }

    private static string CrearNombreVisible(
        string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "Avatar";

        return nombre
            .Replace("_", " ")
            .Replace("-", " ");
    }
}