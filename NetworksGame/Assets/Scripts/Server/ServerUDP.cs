using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;  // For scene checking

public class ServerUDP : MonoBehaviour
{
    private Socket socket;
    private Thread receiveThread;
    private Dictionary<IPEndPoint, PlayerState> playerStates = new Dictionary<IPEndPoint, PlayerState>();

    public GameObject UItextObj;
    private TextMeshProUGUI UItext;
    private string serverText;

    void Start()
    {
        // Only initialize the server logic if we are in the ServerScene
        if (SceneManager.GetActiveScene().name == "ServerScene")
        {
            UItext = UItextObj.GetComponent<TextMeshProUGUI>();
            StartServer();
        }
        else
        {
            // Disable the script or show a message if not in the correct scene
            serverText = "This is not the server scene.";
        }
    }

    public void StartServer()
    {
        serverText = "Starting UDP Server...";
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);

        // Start the receive thread
        receiveThread = new Thread(Receive);
        receiveThread.Start();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remoteEndPoint = sender;

        while (true)
        {
            int recv = socket.ReceiveFrom(data, ref remoteEndPoint);

            string message = Encoding.ASCII.GetString(data, 0, recv);
            serverText = "Message received: " + message;

            // Handle PlayerState data
            PlayerState receivedState = DeserializePlayerState(data);

            // Update or add player state
            if (playerStates.ContainsKey((IPEndPoint)remoteEndPoint))
            {
                playerStates[(IPEndPoint)remoteEndPoint] = receivedState;
            }
            else
            {
                playerStates.Add((IPEndPoint)remoteEndPoint, receivedState);
            }

            // Send updated player positions to all clients
            BroadcastPlayerStates();
        }
    }

    void BroadcastPlayerStates()
    {
        foreach (var entry in playerStates)
        {
            byte[] data = SerializePlayerState(entry.Value);
            socket.SendTo(data, data.Length, SocketFlags.None, entry.Key);
        }
    }

    byte[] SerializePlayerState(PlayerState state)
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, state);
        return stream.ToArray();
    }

    PlayerState DeserializePlayerState(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryFormatter formatter = new BinaryFormatter();
        return (PlayerState)formatter.Deserialize(stream);
    }
}
