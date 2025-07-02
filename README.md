<h1>Instructions</h1>

- Add this to the Packages/manifest.json:<br> <code>"com.endel.nativewebsocket": "https://github.com/endel/NativeWebSocket.git#upm"</code>

<h2>Usage in script</h2>
- Add the NodeRedWebSocketClient module to your script. Add a property to your script like a following: <br>
<code>public NodeRedWebSocketClient nodeRedWebSocketClient;</code> <br>
- Then attach the NodeRedWebSocketClient in Unity to the property from the script. <br>
- To listen to the websocket: <br>
<code>
void Start()
{
    nodeRedWebSocketClient.OnNodeRedMessage += HandleNodeRedMessage;
}

private void HandleNodeRedMessage(string topic, string msg)
{
    Debug.Log($"Received message from Node-RED: {topic}, {msg}");
}
</code> <br>
- To send a message to the node-red websocket use this: <br>
<code>
string jsonMessage = "{\"topic\":\"test\",\"msg\":{\"value\":\"Kaas gegeten\"}}";
nodeRedWebSocketClient.SendJsonToNodeRedWebSocket(jsonMessage);
</code>





