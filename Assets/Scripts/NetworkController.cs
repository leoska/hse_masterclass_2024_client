using System;
using System.Text;
using UnityEngine;
using NativeWebSocket;

public class NetworkController : MonoBehaviour
{
    [SerializeField] private string _host = "ws://127.0.0.1:8000";
    [SerializeField] private PlayerType _id = PlayerType.Server;
    [SerializeField] private int _freqSend = 20;
    [SerializeField] private PongBall _ball;
    [SerializeField] private GameObject _platformLeft;
    [SerializeField] private GameObject _platformRight;

    private WebSocket _socket;
    private float _timerSend = 0f;

    public static bool isGameStarted = false;
    public static PlayerType Type = PlayerType.Server;
    
    // Start is called before the first frame update
    async void Start()
    {
        Type = _id;
        _socket = new WebSocket($"{_host}/ws/{(int)_id}");
        AddListeners();
        await _socket.Connect();
    }

    private void AddListeners()
    {
        _socket.OnOpen += async () =>
        {
            Debug.Log("Successfully connected to server.");
            Debug.Log("Waiting for other player.");
        };

        _socket.OnError += (e) =>
        {
            Debug.LogError(e);
        };

        _socket.OnMessage += (data) =>
        {
            var msg = Encoding.UTF8.GetString(data);
            var packet = JsonUtility.FromJson<Packet>(msg);

            switch (packet.action)
            {
                case 1:
                    isGameStarted = true;
                    
                    if (_id != PlayerType.Server)
                        return;

                    _ball.StartGame();
                    break;
                
                case 2:
                    _platformLeft.transform.position = packet.platformLeft;
                    _ball.transform.position = packet.ballPos;
                    break;
                
                case 3:
                    _platformRight.transform.position = packet.platformRight;
                    break;
                
                default:
                    throw new Exception($"Undefined action! {msg}");
            }
        };
    }

    // Update is called once per frame
    async void Update()
    {
        Dispatch();
        
        if (_socket.State != WebSocketState.Open)
            return;

        _timerSend += Time.deltaTime;

        if (_timerSend < 1f / _freqSend)
            return;

        _timerSend = 0f;

        var packet = new Packet();
        
        if (_id == PlayerType.Server)
        {
            packet.action = 2;
            packet.ballPos = _ball.transform.position;
            packet.platformLeft = _platformLeft.transform.position;
        }
        else
        {
            packet.action = 3;
            packet.platformRight = _platformRight.transform.position;
        }

        var msg = JsonUtility.ToJson(packet);
        await _socket.SendText(msg);
    }

    private void Dispatch()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (_socket == null)
            return;

        if (_socket.State != WebSocketState.Open)
            return;

        _socket.DispatchMessageQueue();
#endif
    }
}

public enum PlayerType
{
    Server = 1,
    Client = 2,
}
