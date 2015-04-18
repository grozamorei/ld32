using UnityEngine;
using proto;
using net;
using game.board;

namespace game
{
    
    public enum GameState
    {
        AWAITING_AUTH,
        AUTH_SCREEN,
        SIMULATION,
        LOST_SCREEN
    }
    
    public class MainProxy : MonoBehaviour
    {
        [EditorReadOnly]
        public GameState state;
        
        public GameObject cellPrefab;
        
        private Connection _connection;
        private LoginScreenLogic _loginMenu;
        private WorldSimulation _simulation;
        
        void Start()
        {
            state = GameState.AWAITING_AUTH;
            
            _connection = gameObject.AddComponent<Connection>();
            _loginMenu = gameObject.AddComponent<LoginScreenLogic>();
            _simulation = gameObject.AddComponent<WorldSimulation>();
        }
        
        public void showWelcomeData(Welcome data)
        {
            Debug.Log("MainProxy: showWelcome data: " + data.availableName + "; " + data.randomWorld); 
            _loginMenu.showWelcomeData(data.availableName, data.randomWorld);
        }
        
        public void showWelcomeData()
        {
            Debug.Log("MainProxy: showWelcome data "); 
            _loginMenu.showWelcomeData();
        }
        
        public void tryAuthorize(string name, string world)
        {
            Debug.Log("MainProxy: try authorize with: " + name + "; " + world); 
            _connection.tryAuthorize(name, world);
        }
        
        public void initializeWorld(WorldData data)
        {
            _simulation.initialize(cellPrefab, data);
        }
    }
}
