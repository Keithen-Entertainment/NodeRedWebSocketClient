<h1>Instructions</h1>

- Add this to the Packages/manifest.json:<br> <code>"com.endel.nativewebsocket": "https://github.com/endel/NativeWebSocket.git#upm"</code>

<h2>Usage in script</h2>
<ul> 
<li>Add the NodeRedWebSocketClient module to your script. Add a property to your script like a following: </li>
<code>public NodeRedWebSocketClient nodeRedWebSocketClient;</code>
<li>Then attach the NodeRedWebSocketClient in Unity to the property from the script.</li>
<li>To listen to the websocket:</li>
<code>void Start()
{
    nodeRedWebSocketClient.OnNodeRedMessage += HandleNodeRedMessage;
}
private void HandleNodeRedMessage(string topic, string msg)
{
    Debug.Log($"Received message from Node-RED: {topic}, {msg}");
} 
</code>
<li>To send a message to the node-red websocket use this: </li>
<code>string jsonMessage = "{\"topic\":\"test\",\"msg\":{\"value\":\"Kaas gegeten\"}}";
nodeRedWebSocketClient.SendJsonToNodeRedWebSocket(jsonMessage);</code>