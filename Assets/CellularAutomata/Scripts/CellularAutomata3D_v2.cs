using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata3D_v2 : MonoBehaviour {

    [Header("Settings")] 
    [Range(0, 1000)][SerializeField] int size = 10;
    [Range(0, 100)][SerializeField] int iteration = 10;

    [SerializeField] GameObject cubePrefab;
    
    struct Cell {
        public int aliveNeighbors;
        public bool isAlive;
        public bool futureState;
        public int[] neighborsIndex;
        public GameObject cube;
    }

    Cell[] cells;
    
    // Start is called before the first frame update
    void Start()
    {
        //Setup base array
        cells = new Cell[size * size * size];
        
        GenerateGameObjects();

        SetNeighbors();

        RandomBirth();
    }

    // Update is called once per frame
    void Update()
    {
        if (iteration <= 0) {
            return;
        }

        foreach (Cell cell in cells) {
            if (cell.isAlive && cell.aliveNeighbors < 26) {
                cell.cube.gameObject.SetActive(true);
            }
            else {
                cell.cube.gameObject.SetActive(false);
            }
        }

        CellularStep();
        iteration--;
    }

    void GenerateGameObjects() {
        int index = 0;
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    cells[index] = new Cell {cube = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity)};

                    index++;
                }
            }
        }
    }

    void SetNeighbors() {
        BoundsInt bounds = new BoundsInt(-1, -1, -1, 3, 3, 3);
        int index = 0;
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                for (int z = 0; z < size; z++) {
                    List<int> tmpNeighbors = new List<int>();
                    
                    foreach (Vector3Int b in bounds.allPositionsWithin) {
                        if (b.x == 0 && b.y == 0 && b.z == 0) continue;
                        if (x + b.x < 0 || x + b.x >= size) continue;
                        if (y + b.y < 0 || y + b.y >= size) continue;
                        if (z + b.z < 0 || z + b.z >= size) continue;
                            
                        tmpNeighbors.Add(CoordToLinearIndex(new Vector3Int(x + b.x, y + b.y, z + b.z)));
                    }
                    
                    cells[index].neighborsIndex = new int[tmpNeighbors.Count];
                    cells[index].neighborsIndex = tmpNeighbors.ToArray();
                    index++;
                }
            }
        }
    }

    void RandomBirth() {
        for (int index = 0; index < cells.Length; index++) {
            int i = Random.Range(0, 2);

            if (i == 0) {
                cells[index].isAlive = true;
                foreach (int t in cells[index].neighborsIndex) {
                    cells[t].aliveNeighbors++;
                }
            }
            else {
                cells[index].isAlive = false;
            }
        }
    }

    void CellularStep() {
        for (int i = 0; i < cells.Length; i++) {
            //Apply rules
            if (cells[i].isAlive && (cells[i].aliveNeighbors >= 13 && cells[i].aliveNeighbors <= 26)) {
                cells[i].futureState = true;
            } else if (!cells[i].isAlive &&
                       ((cells[i].aliveNeighbors >= 13 && cells[i].aliveNeighbors <= 14) ||
                        (cells[i].aliveNeighbors >= 17 && cells[i].aliveNeighbors <= 19))) {
                cells[i].futureState = true;
            } else {
                cells[i].futureState = false;
            }
        }

        for (int i = 0; i < cells.Length; i++) {
            if (cells[i].futureState && !cells[i].isAlive) {
                cells[i].isAlive = true;

                foreach (int t in cells[i].neighborsIndex) {
                    cells[t].aliveNeighbors++;
                }
            }else if (!cells[i].futureState && cells[i].isAlive) {
                cells[i].isAlive = false;

                foreach (int t in cells[i].neighborsIndex) {
                    cells[t].aliveNeighbors--;
                }
            }
        }
    }

    int CoordToLinearIndex(Vector3Int pos) {
        return (pos.x * size * size) + (pos.y * size) + pos.z;
    }
}
