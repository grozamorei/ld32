using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace game.board
{
    public enum BoardState
    {
        NONE,
        INITIALIZED,
    }

    [RequireComponent(typeof(MainProxy))]
    public class BoardContainer : MonoBehaviour
    {
        private Dictionary<byte, Color> colors;
    
        private GameObject _cellSprite;
        private byte[] _field;
        private CellRotator[] _fieldVisual;
        
        [EditorReadOnly]
        public BoardState _state = BoardState.NONE;
        
        [EditorReadOnly]
        public int maxX;

        [EditorReadOnly]
        public int maxY;

        public void createBoard (GameObject prefab, int x, int y)
        {
            colors = new Dictionary<byte, Color>();
            colors.Add(0, Color.black);
            colors.Add(1, Color.red);
            colors.Add(2, Color.green);
            colors.Add(3, Color.blue);
            colors.Add(4, Color.gray);
            colors.Add(5, Color.cyan);
            colors.Add(6, Color.magenta);
            colors.Add(7, Color.yellow);
            colors.Add(8, Color.clear);

            _cellSprite = prefab;
            maxX = x;
            maxY = y;
            _field = new byte[x * y];
            _fieldVisual = new CellRotator[x * y];
            for (int i = 0; i < maxX; i++) {
                for (int j = 0; j < maxY; j++) {
                    var z = Instantiate (_cellSprite);
                    z.transform.position = new Vector2 (j + 0.5f, -i - 0.5f);
                    z.transform.parent = transform;
                    z.name = (i * maxX).ToString() + '_' + j.ToString();
                    z.SetActive(false);
                    _fieldVisual[i * maxX + j] = z.GetComponent<CellRotator> ();
                }
            }
            
            _state = BoardState.INITIALIZED;
        }
        
        public void pushSnapshot(byte[] newField)
        {
            for (int i = 0; i < newField.Length; i++)
            {
                if (_field[i] != newField[i])
                {
                    _field[i] = newField[i];
                    if (_field[i] == 0)
                    {
                        _fieldVisual[i].gameObject.SetActive(false);
                    } else
                    {
                        _fieldVisual[i].gameObject.SetActive(true);
                        _fieldVisual[i].runRotate(colors[_field[i]]);
                    }
                }
            }
        }
        
//        float currentTime = 0;
//        float targetTime = 0.7f;
//        bool running = false;
//        
//        int calcNeighbours (int x, int y)
//        {
//            int n = 0;
//            for (int i = x-1; i <= x+1; i++) {
//                for (int j = y-1; j <= y+1; j++) {
//                    if (i == x && j == y)
//                        continue;
//                    int newI = i == -1 ? wLen - 1 : i;
//                    newI = i == wLen ? 0 : newI;
//                    int newJ = j == -1 ? hLen - 1 : j;
//                    newJ = j == hLen ? 0 : newJ;
//                    
//                    if (field [newI, newJ])
//                        n++;
//                }
//            }
//            
//            return n;
//        }
        
        void Update ()
        {
            //        if (running)
            //        {
            //            currentTime += Time.smoothDeltaTime;
            //            if (currentTime >= targetTime)
            //            {
            //                currentTime = 0;
            //                
            //                for (int i = 0; i < wLen; i++)
            //                {
            //                    for (int j = 0; j < hLen; j++)
            //                    {
            //                        int neighbours = calcNeighbours(i, j);
            //                        if (field[i, j])
            //                        {
            //                            if (neighbours < 2 || neighbours > 3)
            //                            {
            //                                field_temp[i, j] = false;
            //                                visuals[wLen * i + j].runRotate();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            if (neighbours == 3)
            //                            {
            //                                field_temp[i, j] = true;
            //                                visuals[wLen * i + j].runRotate();
            //                            }
            //                        }
            //                    }
            //                }
            //                
            //                for (int i = 0; i < wLen; i++)
            //                {
            //                    for (int j = 0; j < hLen; j++)
            //                    {
            //                        field[i, j] = field_temp[i, j];
            //                    }
            //                }
            //            }
            //        }
            //        
            //        if (Input.GetMouseButtonDown (0)) {
            //            RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
            //            
            //            if (hit.collider != null) {
            //                var t = hit.collider.gameObject.transform;
            ////                Debug.Log(t.position + "::" + t.GetComponent<CellRotator> ());
            //                var r = t.GetComponent<CellRotator> ();
            //                if (r != null) {
            //                    r.runRotate ();
            //                    int idx = visuals.IndexOf(r);
            //                    int col = Mathf.FloorToInt(idx / wLen);
            //                    int row = idx - col * wLen;
            //                    //Debug.Log(col + " :: " + row);
            //                    field[col, row] = !field[col, row];
            //                    field_temp[col, row] = field[col, row];
            //                }
            //            }
            //        }
            //        
            //        if (Input.GetKeyDown(KeyCode.S))
            //        {
            //            running = !running;
            //            Debug.Log("run");
            //        }
        }
    }
}