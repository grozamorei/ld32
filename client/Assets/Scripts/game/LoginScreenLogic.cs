
using UnityEngine;
using game.ui;

namespace game
{

    [RequireComponent(typeof(MainProxy))]
    public class LoginScreenLogic : MonoBehaviour
    {
        private MainProxy _game;
        private LoginScreenHook _uiHook;
        
        void Start()
        {
            _game = gameObject.GetComponent<MainProxy>();
            _uiHook = FindObjectOfType<LoginScreenHook>();
            
            showWaiter();
        }
        
        public void showWaiter()
        {
            _uiHook.hideContent();
        }
        
        public void showWelcomeData(string availableName, string randomWorld)
        {
            _uiHook.showContent();
            _uiHook.initialize(availableName, randomWorld, onPlayPressed);
        }
        
        public void showWelcomeData()
        {
            _uiHook.showContent();
        }
        
        private void onPlayPressed()
        {
            _uiHook.hideContent();
            _game.tryAuthorize(_uiHook.userName, _uiHook.worldName);
        }
    }
}
