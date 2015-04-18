using proto;
using UnityEngine;

namespace game
{
    [RequireComponent(typeof(MainProxy))]
    public class WorldSimulation : MonoBehaviour
    {
        private MainProxy _game;
        public WorldData _data;
        
        public void initialize(WorldData data)
        {
            _game = gameObject.GetComponent<MainProxy>();
            _data = data;
        }
        
        public void pushSnapshot()
        {
            
        }
        
        void Update()
        {
            
        }
    }
}
