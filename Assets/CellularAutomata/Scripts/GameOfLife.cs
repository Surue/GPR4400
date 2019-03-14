using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOfLife : MonoBehaviour
{
    [Header("Grid")]
    [Range(0, 100)][SerializeField] int sizeX = 50;
    [Range(0, 100)][SerializeField] int sizeY = 50;

    [Header("Cells")]
    [Range(0, 1)] [SerializeField] float probabilityIsAlive = 0.5f;

    bool isRunning = false;

    #region struct


    struct Cell
    {
        public bool currentState;
        public bool futureState;
    }

    Cell[,] cells;
    #endregion

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

        StartCoroutine(Simulate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Simulate()
    {
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

                    if (cells[x, y].currentState && (aliveNeighbours == 2 || aliveNeighbours == 3)) {
                        cells[x, y].futureState = true;
                    } else if (!cells[x, y].currentState && aliveNeighbours == 3) {
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

            yield return new WaitForSeconds(0.1f);
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
