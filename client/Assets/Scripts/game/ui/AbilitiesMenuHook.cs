

using UnityEngine;
using UnityEngine.UI;

namespace game.ui
{
    public enum BombState
    {
        WAITING,
        BOMB_DEPLOY,
        RECHARGE,
    }
    
    public enum SeedState
    {
        WAITING,
        SEED_DEPLOY,
        RECHARGE,
    }
    
    public class AbilitiesMenuHook : MonoBehaviour
    {
        [EditorReadOnly]
        public BombState bombState = BombState.WAITING;
        
        [EditorReadOnly]
        public SeedState seedState = SeedState.WAITING;
        
        public Canvas root;
        
        public Text bombButtonText;
        public Button bombButton;
        public Image bombButtonSkin;
        
        public Text seedButtonText;
        public Button seedButton;
        public Image seedButtonSkin;
        
        public GameObject target;
        public Color targetColor;

        private MainProxy _game;
        private GameObject _targetInstance;
        private WorldSimulation _sim;
        
        private float bombCooldown = float.MaxValue;
        
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
            bombButtonSkin.color = new Color(1f, 1f, 1f, 0.8f);
            seedButtonSkin.color = new Color(1f, 1f, 1f, 0.8f);
        }
        
        public void onPoinerExit()
        {
            bombButtonSkin.color = new Color(1f, 1f, 1f, 0.3f);
            seedButtonSkin.color = new Color(1f, 1f, 1f, 0.3f);
        }
        
        public void onBombDeployed()
        {
            bombButtonText.text = "Bomb ready";
            bombState = BombState.WAITING;
            _sim.attachToDrag2(null);
            _targetInstance.transform.position = new Vector3(-100f, 100f);
            
            bombCooldown = Time.timeSinceLevelLoad + 5f;
            bombState = BombState.RECHARGE;
            bombButtonText.text = "Recharge..";
            bombButton.interactable = false;
        }
        
        public void onBombClick()
        {
            switch(bombState)
            {
                case BombState.WAITING:
                    bombButtonText.text = "Cancel deploy";
                    bombState = BombState.BOMB_DEPLOY;
                    _sim.attachToDrag2(_targetInstance);
                    break;
                case BombState.BOMB_DEPLOY:
                    bombButtonText.text = "Bomb ready";
                    bombState = BombState.WAITING;
                    _sim.attachToDrag2(null);
                    _targetInstance.transform.position = new Vector3(-100f, 100f);
                    break;
                case BombState.RECHARGE:
                    Debug.LogWarning("recharge");
                    break;
            }
        }
        
        public void onSeedClick()
        {
            switch(seedState)
            {
            case SeedState.WAITING:
                seedButtonText.text = "Cancel deploy";
                seedState = SeedState.SEED_DEPLOY;
                //_sim.attachToDrag2(_targetInstance);
                break;
            case SeedState.SEED_DEPLOY:
                seedButtonText.text = "Seed ready";
                seedState = SeedState.WAITING;
                //_sim.attachToDrag2(null);
                //_targetInstance.transform.position = new Vector3(-100f, 100f);
                break;
            case SeedState.RECHARGE:
                break;
            }
        }
        
        void Update()
        {
            if (bombCooldown < Time.timeSinceLevelLoad)
            {
                bombCooldown = float.MaxValue;
                bombState = BombState.WAITING;
                bombButtonText.text = "Bomb ready";
                bombButton.interactable = true;
            }
        }
    }
}
