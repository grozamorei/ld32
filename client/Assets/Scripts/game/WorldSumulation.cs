using proto;
using UnityEngine;
using game.board;
using System.Collections.Generic;

namespace game
{
    [RequireComponent(typeof(MainProxy))]
    public class WorldSimulation : MonoBehaviour
    {
        private MainProxy _game;
        private BoardContainer _board;
        private BoardMouseControl _drag;
        
        private WorldData _data;
        private GameObject _cellPrefab;
        
        public void initialize(GameObject cellPrefab, WorldData data)
        {
            _game = gameObject.GetComponent<MainProxy>();
            _board = gameObject.AddComponent<BoardContainer>();
            _board.createBoard(cellPrefab, data.sizeX, data.sizeY);
            _data = data;
            
            _drag = new BoardMouseControl();
            _drag.initialize(Camera.main, _board, deployDebugConfig);
            _cellPrefab = cellPrefab;
        }
        
        void Update()
        {
            if (_game == null || _game.state != GameState.SIMULATION) return;
            
            _drag.update();
        }
        
        public void attachToDrag(int[] figure)
        {
            _drag.attach(_cellPrefab, figure);
        }
        
        private void deployDebugConfig(List<int> config)
        {
            _game.deployDebugConfig(config);
        }
    }
}
