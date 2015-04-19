using UnityEngine;
using System.Collections;
using System;
using System.IO;
using proto;
using util;
using game;
using System.Collections.Generic;

namespace net
{
    public enum ConnectionState
    {
        UNKNOWN,
        CONNECTED,
        WELCOMED,
        AUTHORIZING,
        AUTHORIZED,
        IN_WORLD,
    }

    [RequireComponent(typeof(MainProxy))]
    public class Connection : MonoBehaviour
    {
        private WebSocket _socket;
        private byte[] sent;
        
        [EditorReadOnly]
        public ConnectionState _state;

        private byte[] _partial_message = null;
        
        private MainProxy _game;
        private string _lastNameAttempt;
        private string _lastWorldAttempt;
        
        IEnumerator Start ()
        {
            _game = gameObject.GetComponent<MainProxy>();
            
            _state = ConnectionState.UNKNOWN;
            _socket = new WebSocket (new Uri ("ws://188.242.130.83:3000/echo"));
            yield return StartCoroutine (_socket.Connect ());
        }
        
        void Update()
        {
            byte[] data = _socket.Recv ();
            
            if (data == null || data.Length == 0) return;
            
            if (_state == ConnectionState.UNKNOWN)
                _state = ConnectionState.CONNECTED;
            
            if (data.Length < 4)
            {
                Debug.Log("partial message!");
                _storeAddPartial(data);
            }
            
            if (_partial_message != null)
            {
                Debug.Log("reconstructing partial message!");
                data = ArrayUtil.sumArrays(_partial_message, data);
                _partial_message = null;
            }
            
            int messageLen = BitConverter.ToInt32(data, 0);
            int realLen = data.Length - 5;
            
            if (realLen < messageLen)
            {
                _storeAddPartial(data);
                return;
            }
            else if (realLen > messageLen)
            {
                Debug.LogError("AHAHAH");
                return;
            }
            
            if (data[4] == DebugPackage.ID)
            {
                var dm = new DebugPackage(data);
                Debug.Log("debug message from: " + dm.sender + "::" + dm.message);
            }
            
            if (_state == ConnectionState.CONNECTED)
            {
                if (data[4] == Welcome.ID)
                {
                    _game.showWelcomeData(new Welcome(data));
                    _state = ConnectionState.WELCOMED;
                }
                else
                {
                    Debug.LogError("expected welcome, got :" + data[4].ToString() + " instead");
                    return;
                }
            }
            else if (_state == ConnectionState.AUTHORIZING)
            {
                if (data[4] == ResponseEnterWorld.ID)
                {
                    var res = new ResponseEnterWorld(data);
                    if (res.status == EnterWorldStatus.ENTER_SUCCESS)
                    {
                        Debug.Log("authorize success at world " + _lastWorldAttempt + 
                        " with name " + _lastNameAttempt + "::" + res.myId.ToString());
                        _state = ConnectionState.AUTHORIZED;
                        _game.rememberMe(_lastNameAttempt, res.myId, _lastWorldAttempt);
                    } 
                    else
                    {
                        Debug.Log("authorize failed!");
                        _game.showWelcomeData();
                        _state = ConnectionState.WELCOMED;
                    }
                }
                else
                {
                    Debug.LogError("expected auth response, got :" + data[4].ToString() + " instead");
                    return;
                }
            }
            else if (_state == ConnectionState.AUTHORIZED)
            {
                if (data[4] == WorldData.ID)
                {
                    Debug.Log("world data received!");
                    var wData = new WorldData(data);
                    _game.initializeWorld(wData);
                    _state = ConnectionState.IN_WORLD;
                    
                    _socket.Send(new DebugPackage(_game.myName, "привет друзья").encode());
                }
                else
                {
                    Debug.LogError("expected world data, got :" + data[4].ToString() + " instead");
                    return;
                }
            }
            else if (_state == ConnectionState.IN_WORLD)
            {
                if (data[4] == WorldSnapshot.ID)
                {
//                    Debug.Log("snapshot received!");
                    var snap = new WorldSnapshot(data);
                    var s = new byte[snap.snapshot.Length];
                    for (int i = 0; i < snap.snapshot.Length; i++)
                    {
                        s[i] = (byte.Parse(snap.snapshot[i].ToString()));
                    }
                    _game.pushSnapshot(s);
                }
            }
        }
        
        public void tryAuthorize(string name, string world)
        {
            if (_state != ConnectionState.WELCOMED)
                throw new UnityException("cannot call authorize in that state : " + _state.ToString());
                
            _lastNameAttempt = name;
            _lastWorldAttempt = world;
            var auth = new RequestEnterWorld(name, world);
            _socket.Send(auth.encode());
            _state = ConnectionState.AUTHORIZING;
        }
        
        public void debugDeploy(List<int> configuration)
        {
            var d = new DebugDeployConfiguration(configuration.ToArray());
            _socket.Send(d.encode());
        }
        
        private void _storeAddPartial(byte[] chunk)
        {
            if (_partial_message != null)
                _partial_message = ArrayUtil.sumArrays(_partial_message, chunk);
            else
                _partial_message = chunk;
        }
    }
}