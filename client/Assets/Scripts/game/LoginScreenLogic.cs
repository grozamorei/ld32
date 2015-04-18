
using UnityEngine;

namespace game
{

    [RequireComponent(typeof(MainProxy))]
    public class LoginScreenLogic : MonoBehaviour
    {
        private MainProxy _game;
        
        void Start()
        {
            _game = gameObject.GetComponent<MainProxy>();
            showWaiter();
        }
        
        public void showWaiter()
        {
            Debug.Log("LoginScreen.ShowWaiter");
        }
        
        public void showWelcomeData(string availableName, string randomWorld)
        {
            Debug.Log("LoginScreen.ShowWelcome : " + availableName + "; " + randomWorld);
        }
    }
}
