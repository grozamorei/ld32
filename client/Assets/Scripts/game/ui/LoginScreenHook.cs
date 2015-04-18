
using UnityEngine;
using UnityEngine.UI;

namespace game.ui
{
    public class LoginScreenHook : MonoBehaviour
    {
    
        public delegate void PlayCall();
    
        public Canvas canvasHook;
        public InputField inputNameHook;
        public Text worldLabelHook;
        
        private PlayCall _onPlay;
        
        public string userName { get { return inputNameHook.text; } }
        public string worldName { get { return worldLabelHook.text; } }
        
        public void hideContent()
        {
            canvasHook.enabled = false;
        }
        
        public void showContent()
        {
            canvasHook.enabled = true;
        }
        
        public void initialize(string name, string world, PlayCall onPlay)
        {
            inputNameHook.text = name;
            worldLabelHook.text = world;
            _onPlay = onPlay;
        }
        
        public void onNameInputChange(string newString)
        {
//            Debug.Log("input change : " + inputNameHook.text);
        }
        
        public void onNameInputFocusOut(string newString)
        {
//            Debug.Log("focus out : " + newString);
        }
        
        public void onPlayClick()
        {
            _onPlay();
        }
    }
}
