

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
        public GameObject seedTarget;

        private MainProxy _game;
        private GameObject _targetInstance;
        private GameObject _seedTargetInstance;
        private WorldSimulation _sim;
        
        private float bombCooldown = float.MaxValue;
        private float seedCooldown = float.MaxValue;
        
        private bool firstBombClick = true;
        private bool firstBombDeploy = true;
        private bool firstSeedClick = true;
        private bool firstSeedDeploy = true;
        
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
            
            _seedTargetInstance = Instantiate(seedTarget);
            _seedTargetInstance.transform.position = new Vector3(-100f, 100f);
            
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
            if (firstBombDeploy)
            {
                var au = Camera.main.GetComponents<AudioSource>();
                foreach (var a in au)
                {
                    if (a.clip.name != "music")
                    {
                        a.Stop();
                    }
                    if (a.clip.name == "deploy_bomb_launched")
                    {
                        a.Play();
                    }
                }
                firstBombDeploy = false;
            }
            _sim.attachToDrag2(null);
            _targetInstance.transform.position = new Vector3(-100f, 100f);
            
            bombCooldown = Time.timeSinceLevelLoad + 3f;
            bombState = BombState.RECHARGE;
            bombButtonText.text = "Charging";
            bombButton.interactable = false;
        }
        
        public void onSeedDeployed()
        {
            if (firstSeedDeploy)
            {
                var au = Camera.main.GetComponents<AudioSource>();
                foreach (var a in au)
                {
                    if (a.clip.name != "music")
                    {
                        a.Stop();
                    }
                    if (a.clip.name == "deploy_seed_launched")
                    {
                        a.Play();
                    }
                }
                firstSeedDeploy = false;
            }
            _sim.attachToDrag3(null);
            _seedTargetInstance.transform.position = new Vector3(-100f, 100f);
            
            seedCooldown = Time.timeSinceLevelLoad + 6f;
            seedState = SeedState.RECHARGE;
            seedButtonText.text = "Charging";
            seedButton.interactable = false;
        }
        
        public void onBombClick()
        {
            if (firstBombClick)
            {
                var au = Camera.main.GetComponents<AudioSource>();
                foreach (var a in au)
                {
                    if (a.clip.name != "music")
                    {
                        a.Stop();
                    }
                    if (a.clip.name == "deploy_bomb_hint")
                    {
                        a.Play();
                    }
                }
                firstBombClick = false;
            }
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
            if (firstSeedClick)
            {
                var au = Camera.main.GetComponents<AudioSource>();
                foreach (var a in au)
                {
                    if (a.clip.name != "music")
                    {
                        a.Stop();
                    }
                    if (a.clip.name == "deploy_seed_hint")
                    {
                        a.Play();
                    }
                }
                firstSeedClick = false;
            }
            switch(seedState)
            {
            case SeedState.WAITING:
                seedButtonText.text = "Cancel deploy";
                seedState = SeedState.SEED_DEPLOY;
                _sim.attachToDrag3(_seedTargetInstance);
                break;
            case SeedState.SEED_DEPLOY:
                seedButtonText.text = "Seed ready";
                seedState = SeedState.WAITING;
                _sim.attachToDrag3(null);
                _seedTargetInstance.transform.position = new Vector3(-100f, 100f);
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
            else
            if (bombCooldown != float.MaxValue)
            {
                bombButtonText.text = ((int)(bombCooldown - Time.timeSinceLevelLoad)).ToString();
            }
            
            if (seedCooldown < Time.timeSinceLevelLoad)
            {
                seedCooldown = float.MaxValue;
                seedState = SeedState.WAITING;
                seedButtonText.text = "Seed ready";
                seedButton.interactable = true;
            }
            else
            if (seedCooldown != float.MaxValue)
            {
                seedButtonText.text = ((int)(seedCooldown - Time.timeSinceLevelLoad)).ToString();
            }
        }
    }
}
