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
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _cam.orthographicSize -= 0.5f;
            }
            else
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _cam.orthographicSize += 0.5f;
            }
            
            if (figure != null && figure.Length > 0)
            {
                var w = _cam.ScreenToWorldPoint(Input.mousePosition);
                w.x = Mathf.Floor(w.x);
                w.y = Mathf.Ceil(w.y);
                //Debug.Log(w);
                if (tempObject != null)
                {
                    tempObject.position = new Vector3(w.x, w.y);
                }
            }
        }
    }
}
