﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;  // For scene checking

public class ClientUDP : MonoBehaviour
{
    private Socket socket;
    private Thread receiveThread;
    public GameObject UItextObj;
    private TextMeshProUGUI UItext;
    private string clientText;
    private PlayerState myState;

    void Start()
    {
        // Check if we are in the ClientScene
        if (SceneManager.GetActiveScene().name == "ClientScene")
        {
            if (UItextObj != null)
            {
                UItext = UItextObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("UItextObj is not assigned in the Inspector!");
            }

            myState = new PlayerState(0f, 0f, 0f, 0f);  // Initial state: Position (0,0) and velocity (0,0)
            StartClient();
        }
        else
        {
            clientText = "This is not the client scene.";
        }
    }
    
    public void StartClient()
    {
        // Start the main thread to send data to the server
        Thread mainThread = new Thread(Send);
        mainThread.Start();
    }

    void Update()
    {
        if (UItext != null)
        {
            UItext.text = clientText;
        }
        else
        {
            Debug.LogWarning("UItext is null! Ensure UItextObj is assigned.");
        }

        // Only send player state if we are in the ClientScene
        if (SceneManager.GetActiveScene().name == "ClientScene")
        {
            SendPlayerState();
        }
    }

    void SendPlayerState()
    {
        // Update player state based on input
        myState.x += Input.GetAxis("Horizontal") * Time.deltaTime;
        myState.y += Input.GetAxis("Vertical") * Time.deltaTime;

        byte[] data = SerializePlayerState(myState);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);  // Server's IP
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);
    }

    void Send()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Connect(ipep);

        // Send initial handshake message
        byte[] data = Encoding.ASCII.GetBytes("Hello from Client");
        socket.SendTo(data, data.Length, SocketFlags.None, ipep);

        // Start receiving data
        receiveThread = new Thread(Receive);
        receiveThread.Start();
    }

    void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remote = sender;
        byte[] data = new byte[1024];

        while (true)
        {
            int recv = socket.ReceiveFrom(data, ref remote);
            PlayerState state = DeserializePlayerState(data);
            // Update the game with the received player state
            clientText = $"Player state: {state.x}, {state.y}";
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
