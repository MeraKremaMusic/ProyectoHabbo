using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class NakamaAuthService : MonoBehaviour
{
    public static NakamaAuthService Instance
    {
        get;
        private set;
    }

    public ISession Session
    {
        get;
        private set;
    }

    public string UltimoError
    {
        get;
        private set;
    }

    public string NombreUsuarioActual
    {
        get
        {
            if (Session == null)
                return "";

            return Session.Username;
        }
    }

    public bool EstaAutenticado
    {
        get
        {
            return
                Session != null &&
                !Session.IsExpired;
        }
    }

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );
    }

    public async Task<bool> Registrar(
        string email,
        string password,
        string username)
    {
        UltimoError = "";

        if (
            !ValidarDatos(
                email,
                password,
                username,
                true
            )
        )
        {
            return false;
        }

        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null
        )
        {
            UltimoError =
                "No hay conexion con el servidor.";

            Debug.LogError(
                UltimoError
            );

            return false;
        }

        string emailLimpio =
            email.Trim().ToLowerInvariant();

        string usernameLimpio =
            username.Trim();

        try
        {
            ISession nuevaSesion =
                await NakamaConnection
                    .Instance
                    .Client
                    .AuthenticateEmailAsync(
                        emailLimpio,
                        password,
                        usernameLimpio,
                        true
                    );

            // IMPORTANTE:
            // create=true también puede autenticar
            // una cuenta que ya existía.
            //
            // Created nos dice si realmente
            // acabamos de crear una cuenta nueva.
            if (!nuevaSesion.Created)
            {
                Session = null;

                UltimoError =
                    "Ese correo ya esta registrado.";

                Debug.LogWarning(
                    UltimoError
                );

                return false;
            }

            Session =
                nuevaSesion;

            Debug.Log(
                "Cuenta creada correctamente. " +
                "Usuario: " +
                Session.Username +
                " | ID: " +
                Session.UserId
            );

            return true;
        }
        catch (ApiResponseException e)
        {
            Session = null;

            string mensaje =
                e.Message != null
                    ? e.Message.ToLowerInvariant()
                    : "";

            if (
                mensaje.Contains("username")
            )
            {
                UltimoError =
                    "Ese nombre de usuario ya esta en uso.";
            }
            else
            {
                UltimoError =
                    "Ese correo ya esta registrado " +
                    "o los datos no son validos.";
            }

            Debug.LogWarning(
                UltimoError
            );

            Debug.LogWarning(
                "Nakama: " +
                e.Message
            );

            return false;
        }
        catch (Exception e)
        {
            Session = null;

            UltimoError =
                "No se pudo crear la cuenta.";

            Debug.LogError(
                "Error al registrar: " +
                e.Message
            );

            return false;
        }
    }

    public async Task<bool> IniciarSesion(
        string email,
        string password)
    {
        UltimoError = "";

        if (
            !ValidarDatos(
                email,
                password,
                "",
                false
            )
        )
        {
            return false;
        }

        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null
        )
        {
            UltimoError =
                "No hay conexion con el servidor.";

            Debug.LogError(
                UltimoError
            );

            return false;
        }

        string emailLimpio =
            email.Trim().ToLowerInvariant();

        try
        {
            Session =
                await NakamaConnection
                    .Instance
                    .Client
                    .AuthenticateEmailAsync(
                        emailLimpio,
                        password,
                        null,
                        false
                    );

            Debug.Log(
                "Inicio de sesion correcto. " +
                "Usuario: " +
                Session.Username +
                " | ID: " +
                Session.UserId
            );

            return true;
        }
        catch (ApiResponseException e)
        {
            Session = null;

            UltimoError =
                "Correo o contrasena incorrectos.";

            Debug.LogWarning(
                UltimoError
            );

            Debug.LogWarning(
                "Nakama: " +
                e.Message
            );

            return false;
        }
        catch (Exception e)
        {
            Session = null;

            UltimoError =
                "No se pudo iniciar sesion.";

            Debug.LogError(
                "Error al iniciar sesion: " +
                e.Message
            );

            return false;
        }
    }

    public void CerrarSesionLocal()
    {
        Session = null;
        UltimoError = "";

        Debug.Log(
            "Sesion local cerrada."
        );
    }

    private bool ValidarDatos(
        string email,
        string password,
        string username,
        bool esRegistro)
    {
        if (
            string.IsNullOrWhiteSpace(
                email
            )
        )
        {
            UltimoError =
                "Debes escribir un correo.";

            Debug.LogWarning(
                UltimoError
            );

            return false;
        }

        if (
            !email.Contains("@") ||
            !email.Contains(".")
        )
        {
            UltimoError =
                "Escribe un correo valido.";

            Debug.LogWarning(
                UltimoError
            );

            return false;
        }

        if (
            string.IsNullOrWhiteSpace(
                password
            ) ||
            password.Length < 8
        )
        {
            UltimoError =
                "La contrasena debe tener " +
                "al menos 8 caracteres.";

            Debug.LogWarning(
                UltimoError
            );

            return false;
        }

        if (esRegistro)
        {
            if (
                string.IsNullOrWhiteSpace(
                    username
                )
            )
            {
                UltimoError =
                    "Debes elegir un nombre de usuario.";

                Debug.LogWarning(
                    UltimoError
                );

                return false;
            }

            string usernameLimpio =
                username.Trim();

            if (
                usernameLimpio.Length < 3
            )
            {
                UltimoError =
                    "El nombre debe tener " +
                    "al menos 3 caracteres.";

                Debug.LogWarning(
                    UltimoError
                );

                return false;
            }
        }

        return true;
    }
}