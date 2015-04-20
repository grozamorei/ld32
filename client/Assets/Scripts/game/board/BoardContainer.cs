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
        private MainProxy _mainP;
        private GameObject seedPrefab;
        
//        private List<GameObject> seedsVisual = new List<GameObject>();
        private Dictionary<int, GameObject> seedsVisual = new Dictionary<int, GameObject>();
        
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
            colors.Add(8, Color.white);

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
                    _fieldVisual[i * maxX + j] = z.GetComponent<CellRotator> ();
                    _fieldVisual[i * maxX + j].instantOff();
                }
            }
            
            _state = BoardState.INITIALIZED;
            _mainP = FindObjectOfType<MainProxy>();
            seedPrefab = _mainP.seedPrefab;
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
                        _fieldVisual[i].off ();
                    } else
                    {
                        _fieldVisual[i].on();
                        _fieldVisual[i].runRotate(colors[_field[i]]);
                    }
                }
            }
        }
        
        public void pushSeed(int location, byte owner)
        {
            var seed = Instantiate(seedPrefab);
            var loc = _fieldVisual[location].transform.position;
            seed.GetComponent<SpriteRenderer>().color = colors[owner];
            seed.transform.position = loc;
            this.seedsVisual.Add(location, seed);
        }

        public void destroySeed (int location)
        {
            foreach (var kv in this.seedsVisual)
            {
                if (kv.Key == location)
                {
                    this.seedsVisual.Remove(kv.Key);
                    GameObject.DestroyImmediate(kv.Value);
                    break;
                }
            }
        }
        
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