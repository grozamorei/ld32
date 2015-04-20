using UnityEngine;
using proto;
using net;
using game.board;
using System.Collections.Generic;
using game.ui;

namespace game
{
    
    public enum GameState
    {
        AWAITING_CONNECT,
        AUTH_SCREEN,
        AWAITING_AUTH,
        SIMULATION,
        LOST_SCREEN
    }
    
    public class MainProxy : MonoBehaviour
    {
        [EditorReadOnly]
        public GameState state;
        
        [EditorReadOnly]
        public string myName;
        [EditorReadOnly]
        public byte myId;
        [EditorReadOnly]
        public string myWorld;
        
        public GameObject cellPrefab;
        
        private Connection _connection;
        private LoginScreenLogic _loginMenu;
        private CheatMenuLogic _cheatMenu;
        private WorldSimulation _simulation;
        private AbilitiesMenuHook _abil;
        
        void Start()
        {
            state = GameState.AWAITING_CONNECT;
            
            _connection = gameObject.AddComponent<Connection>();
            _loginMenu = gameObject.AddComponent<LoginScreenLogic>();
            _cheatMenu = gameObject.AddComponent<CheatMenuLogic>();
            _simulation = gameObject.AddComponent<WorldSimulation>();
            _abil = FindObjectOfType<AbilitiesMenuHook>();
            _abil.hide ();
        }
        
        public void showWelcomeData(Welcome data)
        {
            state = GameState.AUTH_SCREEN;
            Debug.Log("MainProxy: showWelcome data: " + data.availableName + "; " + data.randomWorld); 
            _loginMenu.showWelcomeData(data.availableName, data.randomWorld);
        }
        
        public void showWelcomeData()
        {
            state = GameState.AUTH_SCREEN;
            Debug.Log("MainProxy: showWelcome data "); 
            _loginMenu.showWelcomeData();
        }
        
        public void tryAuthorize(string name, string world)
        {
            state = GameState.AWAITING_AUTH;
            Debug.Log("MainProxy: try authorize with: " + name + "; " + world); 
            _connection.tryAuthorize(name, world);
        }
        
        public void rememberMe(string name, byte id, string world)
        {
            myName = name;
            myId = id;
            myWorld = world;
        }
        
        public void initializeWorld(WorldData data)
        {
            state = GameState.SIMULATION;
            _simulation.initialize(cellPrefab, data);
            _cheatMenu.initialize(data.sizeX, data.sizeY);
            _abil.initialize(this);
            _abil.show();
        }
        
        public void deployDebugConfig(List<int> config)
        {
            _connection.debugDeploy(config);
        }
        
        public void deployBomb(int location)
        {
            _connection.deployBomb(location);
        }
        
        public void pushSnapshot(byte[] snapshot)
        {
            _simulation.pushSnapshot(snapshot);
        }
    }
}
