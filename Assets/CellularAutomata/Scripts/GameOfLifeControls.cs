using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOfLifeControls : MonoBehaviour
{
    [Header("Grid")]
    [Range(0, 100)][SerializeField] int sizeX = 50;
    [Range(0, 100)][SerializeField] int sizeY = 50;

    [Header("Cells")]
    [Range(0, 1)] [SerializeField] float probabilityIsAlive = 0.5f;

    [Header("Demo")] 
    [Range(0, 10)] [SerializeField] float stepTime = 0;

    [SerializeField] bool s0 = false;
    [SerializeField] bool s1 = false;
    [SerializeField] bool s2 = false;
    [SerializeField] bool s3 = false;
    [SerializeField] bool s4 = false;
    [SerializeField] bool s5 = false;
    [SerializeField] bool s6 = false;
    [SerializeField] bool s7 = false;
    [SerializeField] bool s8 = false;

    [SerializeField] bool b0 = false;
    [SerializeField] bool b1 = false;
    [SerializeField] bool b2 = false;
    [SerializeField] bool b3 = false;
    [SerializeField] bool b4 = false;
    [SerializeField] bool b5 = false;
    [SerializeField] bool b6 = false;
    [SerializeField] bool b7 = false;
    [SerializeField] bool b8 = false;

    bool isRunning = false;

    #region struct
    struct Cell
    {
        public bool currentState;
        public bool futureState;
    }

    Cell[,] cells;
    #endregion

    List<int> ruleS;
    List<int> ruleB;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++) {
            for (int y = 0; y < sizeY; y++) {
                cells[x, y] = new Cell();

                float isAlive = Random.Range(0f, 1f);

                cells[x, y].currentState = isAlive < probabilityIsAlive;
            }
        }

        isRunning = true;

        SetRules();

        StartCoroutine(Simulate());
    }

    // Update is called once per frame
    public bool IsRunning() {
        return isRunning;
    }
    IEnumerator Simulate()
    {
        yield return new WaitForSeconds(stepTime);
        
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        while (true) {

            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    int aliveNeighbours = 0;
                    foreach (Vector2Int b in bounds.allPositionsWithin) {
                        if (b.x == 0 && b.y == 0) continue;
                        if (x + b.x < 0 || x + b.x >= sizeX || y + b.y < 0 || y + b.y >= sizeY) continue;

                        if (cells[x + b.x, y + b.y].currentState) {
                            aliveNeighbours++;
                        }
                    }

                    if (cells[x, y].currentState && ruleS.Contains(aliveNeighbours)) {
                        cells[x, y].futureState = true;
                    } else if (!cells[x, y].currentState && ruleB.Contains(aliveNeighbours)) {
                        cells[x, y].futureState = true;
                    } else {
                        cells[x, y].futureState = false;
                    }
                }
            }

            for (int x = 0; x < sizeX; x++) {
                for (int y = 0; y < sizeY; y++) {
                    cells[x, y].currentState = cells[x, y].futureState;
                }
            }

            yield return new WaitForSeconds(stepTime);
        }
    }

    void SetRules()
    {
        ruleB = new List<int>();
        ruleS = new List<int>();

        if (b0) {
            ruleB.Add(0);
        }
        if(b1) {
            ruleB.Add(1);
        }
        if(b2) {
            ruleB.Add(2);
        }
        if(b3) {
            ruleB.Add(3);
        }
        if(b4) {
            ruleB.Add(4);
        }
        if(b5) {
            ruleB.Add(5);
        }
        if(b6) {
            ruleB.Add(6);
        }
        if(b7) {
            ruleB.Add(7);
        }
        if(b8) {
            ruleB.Add(8);
        }

        if(s0) {
            ruleS.Add(0);
        }
        if(s1) {
            ruleS.Add(1);
        }
        if(s2) {
            ruleS.Add(2);
        }
        if(s3) {
            ruleS.Add(3);
        }
        if(s4) {
            ruleS.Add(4);
        }
        if(s5) {
            ruleS.Add(5);
        }
        if(s6) {
            ruleS.Add(6);
        }
        if(s7) {
            ruleS.Add(7);
        }
        if(s8) {
            ruleS.Add(8);
        }
    }

    void OnDrawGizmos()
    {
        if (!isRunning) return;

        for(int x = 0;x < sizeX;x++) {
            for(int y = 0;y < sizeY;y++) {
                if (cells[x, y].currentState) {
                    DrawAliveCell(new Vector2(x, y));
                } else {
                    DrawDeadCell(new Vector2(x, y));
                }
            }
        }
    }

    void DrawAliveCell(Vector2 pos)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), Vector2.one);
    }

    void DrawDeadCell(Vector2 pos)
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(new Vector3(pos.x, pos.y, 0), Vector2.one);
    }
}
