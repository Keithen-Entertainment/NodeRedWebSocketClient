using NativeWebSocket;
using System;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class NodeRedWebSocketClient : MonoBehaviour
{
    [Header("Node-RED WebSocket Settings")]
    [Tooltip("The topic for which the script listens and receives the event.")]
    public string topic = "Unity";
    [Tooltip("The WebSocket URL for Node-RED.")]
    public string webSocketUrl = "ws://192.168.2.221:1880/unity";
    [Tooltip("Delay in seconds before attempting to reconnect to the WebSocket.")]
    public float reconnectDelay = 5f;

    public event Action<string, string> OnNodeRedMessage; // topic, payload

    private WebSocket websocket;
    private bool shouldReconnect = true;

    // --- Logging helpers ---
    private void Log(string message) => Debug.Log($"[{nameof(NodeRedWebSocketClient)}] {message}");
    private void LogWarning(string message) => Debug.LogWarning($"[{nameof(NodeRedWebSocketClient)}] {message}");
    private void LogError(string message) => Debug.LogError($"[{nameof(NodeRedWebSocketClient)}] {message}");

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        InitWebSocket();
    }

    private void InitWebSocket()
    {
        websocket = new WebSocket(webSocketUrl);

        websocket.OnOpen += () =>
        {
            Log("WebSocket Connection open!");
        };

        websocket.OnError += (e) =>
        {
            LogError("WebSocket Error: " + e);
            TryReconnect();
        };

        websocket.OnClose += (e) =>
        {
            Log("WebSocket Connection closed!");
            TryReconnect();
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            Log("Received from Node-RED (WebSocket): " + message);

            try
            {
                var msg = JsonUtility.FromJson<NodeRedMessage>(message);
                if (msg != null && msg.topic == topic)
                {
                    Log($"Message topic matches: {msg.topic}");
                    OnNodeRedMessage?.Invoke(msg.topic, msg.payload);
                }
                else
                {
                    LogWarning($"Message topic does not match. Received: {msg?.topic}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to parse message: {ex.Message}");
            }
        };

        ConnectWebSocket();
    }

    private async void ConnectWebSocket()
    {
        try
        {
            await websocket.Connect();
        }
        catch (Exception ex)
        {
            LogError($"WebSocket connect exception: {ex.Message}");
            TryReconnect();
        }
    }

    private void TryReconnect()
    {
        if (shouldReconnect)
        {
            StopAllCoroutines();
            StartCoroutine(ReconnectCoroutine());
        }
    }

    private IEnumerator ReconnectCoroutine()
    {
        Log($"Attempting to reconnect in {reconnectDelay} seconds...");
        yield return new WaitForSeconds(reconnectDelay);

        Log("Reconnecting to WebSocket...");
        if (websocket != null)
        {
            websocket = null;
        }
        InitWebSocket();
    }

    [Serializable]
    public class NodeRedMessage
    {
        public string topic;
        public string payload;
    }

    public async System.Threading.Tasks.Task SendJsonToNodeRedWebSocket(string json)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Log($"Sending via WebSocket: {json}");
            await websocket.SendText(json);
        }
        else
        {
            LogWarning("WebSocket is not connected. Cannot send message.");
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        shouldReconnect = false;
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}