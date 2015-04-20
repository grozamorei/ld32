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
            _drag.initialize(_game.myId, Camera.main, _board, deployDebugConfig);
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
        
        public void attachToDrag2(GameObject target)
        {
            _drag.attach2(target);
        }
        
        public void attachToDrag3(GameObject target)
        {
            _drag.attach3(target);
        }
        
        private void deployDebugConfig(List<int> config)
        {
            _game.deployDebugConfig(config);
        }
        
        public void pushSnapshot(byte[] snapshot)
        {
            _board.pushSnapshot(snapshot);
        }
        
        public void pushSeed(int location, byte owner_id)
        {
            _board.pushSeed(location, owner_id);
        }

        public void pushBomb (int target)
        {
            //_board.pushBomb(target);
            var w = _board._fieldVisual[target].transform.position;
            w.z += _drag.zoomLevels[_drag.currentZoom] - 1;
            var fat = GameObject.Instantiate(_game.fatmanPrefab, w, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
            fat.gameObject.GetComponent<FatmanHandler>().init(w);
        }

        public void destroySeed (int location)
        {
            _board.destroySeed(location);
        }
    }
}
