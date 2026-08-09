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
        if (!ValidarDatos(
            email,
            password,
            username,
            true))
        {
            return false;
        }

        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null
        )
        {
            Debug.LogError(
                "Nakama no esta conectado."
            );

            return false;
        }

        try
        {
            Session =
                await NakamaConnection
                    .Instance
                    .Client
                    .AuthenticateEmailAsync(
                        email.Trim(),
                        password,
                        username.Trim(),
                        true
                    );

            Debug.Log(
                "Cuenta creada correctamente. " +
                "Usuario ID: " +
                Session.UserId
            );

            return true;
        }
        catch (Exception e)
        {
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
        if (!ValidarDatos(
            email,
            password,
            "",
            false))
        {
            return false;
        }

        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null
        )
        {
            Debug.LogError(
                "Nakama no esta conectado."
            );

            return false;
        }

        try
        {
            // create = false:
            // si la cuenta no existe,
            // NO crea una nueva accidentalmente.
            Session =
                await NakamaConnection
                    .Instance
                    .Client
                    .AuthenticateEmailAsync(
                        email.Trim(),
                        password,
                        null,
                        false
                    );

            Debug.Log(
                "Inicio de sesion correcto. " +
                "Usuario ID: " +
                Session.UserId
            );

            return true;
        }
        catch (Exception e)
        {
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
        if (string.IsNullOrWhiteSpace(email))
        {
            Debug.LogWarning(
                "Debes escribir un correo."
            );

            return false;
        }

        if (
            string.IsNullOrWhiteSpace(password) ||
            password.Length < 8
        )
        {
            Debug.LogWarning(
                "La contraseña debe tener " +
                "al menos 8 caracteres."
            );

            return false;
        }

        if (
            esRegistro &&
            string.IsNullOrWhiteSpace(username)
        )
        {
            Debug.LogWarning(
                "Debes elegir un nombre de usuario."
            );

            return false;
        }

        return true;
    }
}