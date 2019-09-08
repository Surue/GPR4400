using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata3D : MonoBehaviour
{

    [Range(0, 1000)][SerializeField] int size = 10;
    [Range(0, 100)][SerializeField] int iteration = 10;

    [SerializeField] GameObject cubePrefab;

    struct Cell
    {
        public bool isAlive;
        public bool futureState;
    }

    Cell[,,] cells;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        //Create array
        cells = new Cell[size, size, size];

        //Fill array by random
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    float f = Random.Range(0f, 1f);
                    cells[x, y, z].isAlive = f > 0.5f;
                }
            }
        }
        
        StartCoroutine(WorldGeneration());
    }

    IEnumerator WorldGeneration()
    {
        isRunning = true;
        //Cellular automata
        for (int i = 0; i < iteration; i++) {
            Cellular();
            yield return null;
        }

        isRunning = false;

        //Cut cube
        CutCube();

        //Generate cube
        GenerateCube();
    }

    void Cellular()
    {
        BoundsInt bounds = new BoundsInt(-1, -1, -1, 3, 3, 3);

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {

                    int neighboursAlive = 0;

                    //Check neighbours
                    foreach (Vector3Int b in bounds.allPositionsWithin) {
                        if (b.x == 0 && b.y == 0 && b.z == 0) continue;
                        if (x + b.x < 0 || x + b.x >= size) continue;
                        if (y + b.y < 0 || y + b.y >= size) continue;
                        if (z + b.z < 0 || z + b.z >= size) continue;

                        if (cells[x + b.x, y + b.y, z + b.z].isAlive) {
                            neighboursAlive++;
                        }
                    }

                    //Apply rules
                    if (cells[x, y, z].isAlive && (neighboursAlive >= 13 && neighboursAlive <= 26)) {
                        cells[x, y, z].futureState = true;
                    } else if (!cells[x, y, z].isAlive &&
                               ((neighboursAlive >= 13 && neighboursAlive <= 14) ||
                                (neighboursAlive >= 17 && neighboursAlive <= 19))) {
                        cells[x, y, z].futureState = true;
                    } else {
                        cells[x, y, z].futureState = false;
                    }
                }
            }
        }

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    cells[x, y, z].isAlive = cells[x, y, z].futureState;
                }
            }
        }
    }

    void CutCube()
    {
        for(int x = 0; x < size; x++) {
            for (int y = size - 10; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    cells[x, y, z].isAlive = true;
                }
            }
        }
    }

    void GenerateCube()
    {
        for(int x = 0;x < size;x++) {
            for(int y = 0;y < size;y++) {
                for(int z = 0;z < size;z++) {
                    if (!cells[x, y, z].isAlive) {
                        continue;
                    }
                    
                    GameObject instance = Instantiate(cubePrefab);

                    instance.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}
