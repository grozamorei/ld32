using UnityEngine;
using proto;
using net;

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
            _loginMenu.showWelcomeData(data.availableName, data.randomWorld);
        }
    }
}
