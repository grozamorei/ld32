using UnityEngine;
using System.Collections.Generic;

namespace game.board
{
    public class BoardMouseControl
    {
        private Camera _cam;
        private BoardContainer _board;
        public delegate void onDeploy(List<int> value);
        private onDeploy _onDeploy;

        public void initialize(Camera cam, BoardContainer board, onDeploy call)
        {
            _cam = cam;
            _board = board;
            _onDeploy = call;
        }
        
        private bool drag = false;
        private bool zooming = false;
        private float[] zoomLevels = new float[]{-4f, -8f, -12f, -16f};
        private int currentZoom = 1;
        
        private Vector3 dragAnchor;
        private Vector3 camAnchor;
        private float scale;
        private int[] figure;
        private Transform tempObject;
        
        public void attach(GameObject cellPrefab, int[] figure)
        {
            if (tempObject != null)
                GameObject.DestroyImmediate(tempObject.gameObject);
            if (figure != null && figure.Length > 0)
            {
                tempObject = new GameObject("tempConteiner").transform;
                tempObject.position = Vector3.zero;
                
                for (int i = 0; i < figure.Length; i+=2)
                {
                    var coord = new Vector3(figure[i] + 0.5f, -figure[i+1] - 0.5f);
                    var c = GameObject.Instantiate(cellPrefab);
                    c.name = figure[i].ToString() + "_" + figure[i+1].ToString();
                    c.transform.parent = tempObject;
                    c.transform.position = coord;
                    
                    c.GetComponent<CellRotator>().setColor(Color.red);
                }
            }
            this.figure = figure;
        }

        public void update ()
        {
            if (zooming)
            {
                Vector3 pos = _cam.transform.position;
                if (Mathf.Abs(pos.z - zoomLevels[currentZoom]) < 0.05f)
                {
                    _cam.transform.position = new Vector3(pos.x, pos.y, zoomLevels[currentZoom]);
                    zooming = false;
                }
                else
                {
                    _cam.transform.position = new Vector3(pos.x, pos.y, Mathf.Lerp(pos.z, zoomLevels[currentZoom], 0.16f));
                    return;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                drag = true;
                dragAnchor = Input.mousePosition;
                camAnchor = _cam.transform.position;
                scale = Screen.width / (Mathf.Abs(_cam.transform.position.z) * 2f);
                return;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                if (Input.mousePosition == dragAnchor)
                {
                    //click
                    if (figure != null)
                    {
                        var w = _cam.ScreenToWorldPoint(Input.mousePosition);
                        w.x = Mathf.Floor(w.x);
                        w.y = Mathf.Abs(Mathf.Ceil(w.y));
                        var t = new List<int>();
                        for (int i = 0; i < figure.Length; i+=2)
                        {
                            int coord = (figure[i+1] + Mathf.FloorToInt(w.y)) * _board.maxX + (figure[i] + Mathf.FloorToInt(w.x));
                            if (coord < 0) break;
                            if (coord >= _board.maxX * _board.maxY) break;
                            t.Add(coord);
                        }
                        _onDeploy(t);
                    }
                }
                drag = false;
            }
            
            if (drag)
            {
                var posDiff = dragAnchor - Input.mousePosition;
                var worldCoordDiff = new Vector3(posDiff.x / scale, posDiff.y / scale);
                _cam.transform.position = new Vector3(camAnchor.x + worldCoordDiff.x, camAnchor.y + worldCoordDiff.y, _cam.transform.position.z);
                return;
            }
            else
            {
                var xy = new Vector2(_cam.transform.position.x, _cam.transform.position.y);
                var s = (Mathf.Abs(_cam.transform.position.z));
                float newX = xy.x;
                float newY = xy.y;
                if (xy.x < s*2)
                {
                    if (Mathf.Abs(xy.x - s*2) < 0.005f)
                        newX = s*2;
                    else
                        newX = Mathf.Lerp(xy.x, s*2, 0.08f);
                }
                else
                if (xy.x > _board.maxX - s*2)
                {
                    if (Mathf.Abs(xy.x - (_board.maxX - s*2)) < 0.005f)
                        newX = _board.maxX - s*2;
                    else
                        newX = Mathf.Lerp(xy.x, _board.maxX - s*2, 0.08f);
                }
                
                if (xy.y > -s)
                {
                    if (Mathf.Abs(xy.y - s) < 0.005f)
                        newY = -s;
                    else
                        newY = Mathf.Lerp(xy.y, -s, 0.08f);
                }
                else
                if (xy.y < -(_board.maxY - s))
                {
                    if (Mathf.Abs(xy.y - (_board.maxY - s)) < 0.005f)
                        newY = -(_board.maxY - s);
                    else
                        newY = Mathf.Lerp(xy.y, -(_board.maxY - s), 0.08f);
                }
                
                _cam.transform.position = new Vector3(newX, newY, _cam.transform.position.z);
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (currentZoom > 0)
                {
                    zooming = true;
                    currentZoom -= 1;
                }
            }
            else
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (currentZoom < 3)
                {
                    zooming = true;
                    currentZoom += 1;
                }
            }
            
            if (figure != null && figure.Length > 0)
            {
                var w = _cam.ScreenToWorldPoint(Input.mousePosition);
                w.x = Mathf.Floor(w.x);
                w.y = Mathf.Ceil(w.y);
                if (tempObject != null)
                {
                    tempObject.position = new Vector3(w.x, w.y);
                }
            }
        }
    }
}
