using proto;
using UnityEngine;
using game.board;

namespace game
{
    [RequireComponent(typeof(MainProxy))]
    public class WorldSimulation : MonoBehaviour
    {
        private MainProxy _game;
        private BoardContainer _boardAss;
        
        private WorldData _data;
        
        public void initialize(GameObject cellPrefab, WorldData data)
        {
            _game = gameObject.GetComponent<MainProxy>();
            _boardAss = gameObject.AddComponent<BoardContainer>();
            _boardAss.createBoard(cellPrefab, data.sizeX, data.sizeY);
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
