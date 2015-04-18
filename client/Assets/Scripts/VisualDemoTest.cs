using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualDemoTest : MonoBehaviour
{

    public GameObject cellSprite;
    public int minX = -5;
    public int maxX = 5;
    public int minY = -4;
    public int maxY = 4;
    
    private bool[,] field;
    private bool[,] field_temp;
    private List<CellRotator> visuals = new List<CellRotator>();

    int wLen;
    int hLen;
    void Start ()
    {
        wLen = maxX - minX;
        hLen = maxY - minY;
        field = new bool[wLen,hLen];
        field_temp = new bool[wLen,hLen];
        int some = 0;
        for (int j = maxY; j > minY; j--) {
            for (int i = minX; i < maxX; i++) {
                var z = Instantiate (cellSprite);
                z.transform.position = new Vector2 (i + 0.5f, j - 0.5f);
                z.transform.parent = transform;
                visuals.Add(z.GetComponent<CellRotator>());
                some ++;
            }
        }
    }
    
    float currentTime = 0;
    float targetTime = 0.7f;
    bool running = false;
    
    int calcNeighbours(int x, int y)
    {
        int n = 0;
        for (int i = x-1; i <= x+1; i++)
        {
            for (int j = y-1; j <= y+1; j++)
            {
                if (i == x && j == y) continue;
                int newI = i == -1 ? wLen - 1 : i;
                newI = i == wLen ? 0 : newI;
                int newJ = j == -1 ? hLen - 1 : j;
                newJ = j == hLen ? 0 : newJ;
                
                if (field[newI, newJ])
                    n++;
            }
        }
        
        return n;
    }
    
    void Update ()
    {
        if (running)
        {
            currentTime += Time.smoothDeltaTime;
            if (currentTime >= targetTime)
            {
                currentTime = 0;
                
                for (int i = 0; i < wLen; i++)
                {
                    for (int j = 0; j < hLen; j++)
                    {
                        int neighbours = calcNeighbours(i, j);
                        if (field[i, j])
                        {
                            if (neighbours < 2 || neighbours > 3)
                            {
                                field_temp[i, j] = false;
                                visuals[wLen * i + j].runRotate();
                            }
                        }
                        else
                        {
                            if (neighbours == 3)
                            {
                                field_temp[i, j] = true;
                                visuals[wLen * i + j].runRotate();
                            }
                        }
                    }
                }
                
                for (int i = 0; i < wLen; i++)
                {
                    for (int j = 0; j < hLen; j++)
                    {
                        field[i, j] = field_temp[i, j];
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonDown (0)) {
            RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null) {
                var t = hit.collider.gameObject.transform;
//                Debug.Log(t.position + "::" + t.GetComponent<CellRotator> ());
                var r = t.GetComponent<CellRotator> ();
                if (r != null) {
                    r.runRotate ();
                    int idx = visuals.IndexOf(r);
                    int col = Mathf.FloorToInt(idx / wLen);
                    int row = idx - col * wLen;
                    //Debug.Log(col + " :: " + row);
                    field[col, row] = !field[col, row];
                    field_temp[col, row] = field[col, row];
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            running = !running;
            Debug.Log("run");
        }
    }
}
