

using UnityEngine;
using UnityEngine.UI;

namespace game.ui
{
    public enum AbilState
    {
        WAITING,
        BOMB_DEPLOY,
        RECHARGE,
    }
    
    public class AbilitiesMenuHook : MonoBehaviour
    {
        [EditorReadOnly]
        public AbilState state = AbilState.WAITING;
        
        public Canvas root;
        public Text buttonText;
        public Button button;
        public Image buttonSkin;

        private MainProxy _game;
        
        public void hide()
        {
            root.gameObject.SetActive(false);
        }
        
        public void show()
        {
            root.gameObject.SetActive(true);
        }
        
        public void initialize(MainProxy game)
        {
            _game = game;
            onPoinerExit();
        }
        
        public void onPointerEnter()
        {
            buttonSkin.color = new Color(1f, 1f, 1f, 0.8f);
        }
        
        public void onPoinerExit()
        {
            buttonSkin.color = new Color(1f, 1f, 1f, 0.3f);
        }
        
        public void onClick()
        {
            switch(state)
            {
                case AbilState.WAITING:
                    buttonText.text = "Cancel deploy";
                    state = AbilState.BOMB_DEPLOY;
                    break;
                case AbilState.BOMB_DEPLOY:
                    buttonText.text = "Bomb ready";
                    state = AbilState.WAITING;
                    break;
                case AbilState.RECHARGE:
                    Debug.LogWarning("recharge");
                    break;
            }
        }
    }
}
