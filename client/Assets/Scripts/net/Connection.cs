using UnityEngine;
using System.Collections;
using System;
using System.IO;
using proto;
using util;
using game;

namespace net
{
    public enum ConnectionState
    {
        UNKNOWN,
        CONNECTED,
        WELCOMED,
        AUTHORIZING,
        AUTHORIZED
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
        
        IEnumerator Start ()
        {
            _game = gameObject.GetComponent<MainProxy>();
            
            _state = ConnectionState.UNKNOWN;
            _socket = new WebSocket (new Uri ("ws://188.242.130.83:3000/echo"));
            yield return StartCoroutine (_socket.Connect ());
            _state = ConnectionState.CONNECTED;
        }
        
        void Update()
        {
            if (_state == ConnectionState.UNKNOWN)
                return;

            byte[] data = _socket.Recv ();
            
            if (data == null || data.Length == 0) return;
            
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
                Debug.LogWarning("message on authorizing : " + data[4].ToString());
            }
        }
        
        public void tryAuthorize(string name, string world)
        {
            if (_state != ConnectionState.WELCOMED)
                throw new UnityException("cannot call authorize in that state : " + _state.ToString());
                
            var auth = new RequestEnterWorld(name, world);
            _socket.Send(auth.encode());
            _state = ConnectionState.AUTHORIZING;
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