

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
        
        public GameObject target;
        public Color targetColor;

        private MainProxy _game;
        private GameObject _targetInstance;
        private WorldSimulation _sim;
        
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
            
            _targetInstance = Instantiate(target);
            _targetInstance.transform.position = new Vector3(-100f, 100f);
            _sim = FindObjectOfType<WorldSimulation>();
        }
        
        public void onPointerEnter()
        {
            buttonSkin.color = new Color(1f, 1f, 1f, 0.8f);
        }
        
        public void onPoinerExit()
        {
            buttonSkin.color = new Color(1f, 1f, 1f, 0.3f);
        }
        
        public void onBombDeployed()
        {
            //Debug.Log("deployBombHere");
            // launch cooldowns
            buttonText.text = "Bomb ready";
            state = AbilState.WAITING;
            _sim.attachToDrag2(null);
            _targetInstance.transform.position = new Vector3(-100f, 100f);
        }
        
        public void onClick()
        {
            switch(state)
            {
                case AbilState.WAITING:
                    buttonText.text = "Cancel deploy";
                    state = AbilState.BOMB_DEPLOY;
                    _sim.attachToDrag2(_targetInstance);
                    break;
                case AbilState.BOMB_DEPLOY:
                    buttonText.text = "Bomb ready";
                    state = AbilState.WAITING;
                    _sim.attachToDrag2(null);
                    _targetInstance.transform.position = new Vector3(-100f, 100f);
                    break;
                case AbilState.RECHARGE:
                    Debug.LogWarning("recharge");
                    break;
            }
        }
    }
}
