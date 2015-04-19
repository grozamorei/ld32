using proto;
using UnityEngine;
using game.board;

namespace game
{
    [RequireComponent(typeof(MainProxy))]
    public class WorldSimulation : MonoBehaviour
    {
        private MainProxy _game;
        private BoardContainer _board;
        private BoardMouseControl _drag;
        
        private WorldData _data;
        
        public void initialize(GameObject cellPrefab, WorldData data)
        {
            _game = gameObject.GetComponent<MainProxy>();
            _board = gameObject.AddComponent<BoardContainer>();
            _board.createBoard(cellPrefab, data.sizeX, data.sizeY);
            _data = data;
            
            _drag = new BoardMouseControl();
            _drag.initialize(Camera.main, _board);
        }
        
        void Update()
        {
            if (_game == null || _game.state != GameState.SIMULATION) return;
            
            _drag.update();
        }
        
        public void attachToDrag(int[] figure)
        {
            //Debug.Log(figure.Length);
            _drag.attach(figure);
        }
    }
}
