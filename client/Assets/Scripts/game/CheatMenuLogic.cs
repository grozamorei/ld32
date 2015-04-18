
using UnityEngine;
using game.ui;

namespace game
{
    [RequireComponent(typeof(MainProxy))]
    public class CheatMenuLogic : MonoBehaviour
    {
        private MainProxy _game;
        private CheatMenuHook _uiHook;
        
        void Start()
        {
            _game = gameObject.GetComponent<MainProxy>();
            _uiHook = FindObjectOfType<CheatMenuHook>();
            _uiHook.hideContent();
        }
        
        public void initialize(int w, int h)
        {
            _uiHook.initialize(w, h, onCheatState);
        }
        
        void onCheatState(CheatState state, int[] figure)
        {
            Debug.Log(state);
        }
        
        void Update()
        {
            if (_game == null) return;
            if (_game.state != GameState.SIMULATION) return;
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                _uiHook.toggleContent();
            }
        }
    }
}