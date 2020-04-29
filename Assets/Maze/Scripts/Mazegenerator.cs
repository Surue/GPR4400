using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int cellNbX = 2;
    [SerializeField] int cellNbY = 7;
    
    [SerializeField][Range(0, 10)] float sizeCell = 5;

    [SerializeField] bool useRandomNeighborsOrder = false;

    enum GenerationType {
        BFS,
        DFS,
        BACKTRACE
    }

    [SerializeField] GenerationType generationType;
    
    enum CellType {
        START,
        END,
        PASSAGE,
        WALL
    }
    
    struct Cell {
        public List<int> neighborsIndices;
        public CellType cellType;
    }

    Cell[] cells_;
    
    // Start is called before the first frame update
    void Start()
    {
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        cells_ = new Cell[cellNbX * cellNbY];
        
        for (int x = 0; x < cellNbX; x++) {
            for(int y = 0; y < cellNbY; y++) {

                int index = PosToIndex(x, y);
                
                //Default type is wall
                cells_[index].cellType = CellType.WALL;
                
                //Get neighbors indices
                cells_[index].neighborsIndices = new List<int>(4);

                foreach (Vector3Int pos in bounds.allPositionsWithin) {
                    //Check if is bounds or array
                    if(x + pos.x < 0 || x + pos.x >= cellNbX) continue;
                    if(y + pos.y < 0 || y + pos.y >= cellNbY) continue;
                    
                    //Ignore self
                    if(pos.x == 0 && pos.y == 0) continue;
                    
                    //Taking only the cross
                    if(pos.x != 0 && pos.y != 0) continue;
                    
                    //Add neighbors
                    cells_[index].neighborsIndices.Add(PosToIndex(x + pos.x, y + pos.y));
                }
                
                if(useRandomNeighborsOrder)
                    cells_[index].neighborsIndices.Shuffle();
            }
        }

        if (generationType == GenerationType.BACKTRACE) {
            StartCoroutine(BuildMazeBacktrace());
        } else {
            StartCoroutine(BuildMaze());
        }
    }
    
    IEnumerator BuildMazeBacktrace() {
        //Generate starting pos
        cells_[0].cellType = CellType.START;
        yield return new WaitForSeconds(1);

        //Link case together
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        List<int> openList = new List<int> {0};
        List<int> closedList = new List<int>();

        while (openList.Count > 0) {
            int indexToSelectFrom = 0;

            int index = openList[indexToSelectFrom];
            
//          closedList.Add(openList[indexToSelectFrom]);
            openList.RemoveAt(indexToSelectFrom);

            foreach (int neighborsIndex in cells_[index].neighborsIndices) {
                bool canBeAddedToPassage = true;
                
                //Check if the neighbors of the current neighbors are all wall (we don't want loop)
                foreach (int neighborsIndex2 in cells_[neighborsIndex].neighborsIndices) {
                    
                    //Ignore the current cell
                    if(neighborsIndex2 == index) continue;
                    
                    if (cells_[neighborsIndex2].cellType == CellType.WALL) continue;
                    canBeAddedToPassage = false;
                    break;
                }

                if (canBeAddedToPassage && !openList.Contains(neighborsIndex) && !closedList.Contains(neighborsIndex)) {
                    openList.Add(neighborsIndex);
                    closedList.Add(neighborsIndex);
                    cells_[neighborsIndex].cellType = CellType.PASSAGE;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        //Select end pos
        cells_[closedList[closedList.Count - 1]].cellType = CellType.END;
    }

    IEnumerator BuildMaze() {
        //Generate starting pos
        cells_[0].cellType = CellType.START;
        yield return new WaitForSeconds(1);

        //Link case together
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        List<int> openList = new List<int> {0};
        List<int> closedList = new List<int>();

        while (openList.Count > 0) {
            int indexToSelectFrom = 0;

            if (generationType == GenerationType.BFS) {
                indexToSelectFrom = 0;
            }else if (generationType == GenerationType.DFS) {
                indexToSelectFrom = openList.Count - 1;
            }
            
            int index = openList[indexToSelectFrom];
            
//            closedList.Add(openList[indexToSelectFrom]);
            openList.RemoveAt(indexToSelectFrom);

            foreach (int neighborsIndex in cells_[index].neighborsIndices) {
                bool canBeAddedToPassage = true;
                
                //Check if the neighbors of the current neighbors are all wall (we don't want loop)
                foreach (int neighborsIndex2 in cells_[neighborsIndex].neighborsIndices) {
                    
                    //Ignore the current cell
                    if(neighborsIndex2 == index) continue;
                    
                    if (cells_[neighborsIndex2].cellType == CellType.WALL) continue;
                    canBeAddedToPassage = false;
                    break;
                }

                if (canBeAddedToPassage && !openList.Contains(neighborsIndex) && !closedList.Contains(neighborsIndex)) {
                    openList.Add(neighborsIndex);
                    closedList.Add(neighborsIndex);
                    cells_[neighborsIndex].cellType = CellType.PASSAGE;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        //Select end pos
        cells_[closedList[closedList.Count - 1]].cellType = CellType.END;
    }

    int PosToIndex(int x, int y) {
        return x * cellNbX + y;
    }

    Vector2 IndexToPos(int index) {
        int x = index / cellNbX;
        int y = index % cellNbX;
        
        return new Vector2(x, y);
    }
    
    Vector2 IndexToWorldPos(int index) {
        int x = index / cellNbX;
        int y = index % cellNbX;
        
        return new Vector2(x * sizeCell + (sizeCell * 0.5f), y * sizeCell + (sizeCell * 0.5f));
    }

    void OnDrawGizmos() {
        if (cells_ == null) return;
        
        for (int i = 0; i < cellNbX * cellNbY; i++) {
            switch (cells_[i].cellType) {
                case CellType.START:
                    Gizmos.color = Color.blue;
                    break;
                case CellType.END:
                    Gizmos.color = Color.red;
                    break;
                case CellType.PASSAGE:
                    Gizmos.color = Color.white;
                    break;
                case CellType.WALL:
                    Gizmos.color = Color.black;
                    break;
                default:
                    Gizmos.color = Color.black;
                    break;
            }
            Gizmos.DrawCube(IndexToWorldPos(i), new Vector3(sizeCell, sizeCell));
        }
    }
}
