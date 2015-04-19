using UnityEngine;

namespace game.board
{
    public class BoardMouseControl
    {
        private Camera _cam;
        private BoardContainer _board;

        public void initialize(Camera cam, BoardContainer board)
        {
            _cam = cam;
            _board = board;
        }
        
        private bool drag = false;
        private Vector3 dragAnchor;
        private Vector3 camAnchor;
        private float scale;

        public void update ()
        {
            if (Input.GetMouseButtonDown(0))
            {
                drag = true;
                dragAnchor = Input.mousePosition;
                camAnchor = _cam.transform.position;
                scale = Screen.width / (_cam.orthographicSize * 2);
                return;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                drag = false;
            }
            
            if (drag)
            {
                var posDiff = dragAnchor - Input.mousePosition;
                var worldCoordDiff = new Vector3(posDiff.x / scale, posDiff.y / scale);
                _cam.transform.position = new Vector3(camAnchor.x + worldCoordDiff.x, camAnchor.y + worldCoordDiff.y, _cam.transform.position.z);
            }
        }
    }
}
