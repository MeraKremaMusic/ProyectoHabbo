using Nakama;
using UnityEngine;

public class NakamaConnection : MonoBehaviour
{
    public static NakamaConnection Instance
    {
        get;
        private set;
    }

    public IClient Client
    {
        get;
        private set;
    }

    private const string Scheme = "http";
    private const string Host = "127.0.0.1";
    private const int Port = 7350;
    private const string ServerKey = "defaultkey";

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

        CrearCliente();
    }

    private void CrearCliente()
    {
        Client =
            new Client(
                Scheme,
                Host,
                Port,
                ServerKey
            );

        Client.Timeout = 10;

        Debug.Log(
            "Cliente Nakama preparado: " +
            Scheme + "://" +
            Host + ":" +
            Port
        );
    }
}