using UnityEngine;
using System.Collections;
using System;
using System.IO;
using proto;

public class EchoTest2 : MonoBehaviour
{
    WebSocket socket;
    int i;
    byte[] sent;
    
    // Use this for initialization
    IEnumerator Start ()
    {
        socket = new WebSocket (new Uri ("ws://188.242.130.83:3000/echo"));
        yield return StartCoroutine (socket.Connect ());
        
        var package = new DebugPackage("гро", "123");
        var message = package.encode();
        Debug.LogWarning("encoding: t: " + message.Length.ToString());
        socket.Send(message);
    }
    
    void Update()
    {
        byte[] reply = socket.Recv ();
        if (reply != null) {
            Debug.Log ("[" + DateTime.UtcNow.ToString() + "] Received: " + reply.Length);
            var zz = new DebugPackage(reply);
            Debug.Log(zz);
        }
        if (socket.Error != null) {
            Debug.LogError ("Error: " + socket.Error);
        }
    }
}
