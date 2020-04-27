using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Conway : MonoBehaviour {

    [SerializeField] int size_;
    [SerializeField] int iteration_;
    [SerializeField] int nbWallCell_;
    [SerializeField] int minimumCellPerRegion_ = 10;
    
    enum State {
        ALIVE,
        DEAD
    }

    const int nullRegion = -1;

    struct Cell {
        public State currentState;
        public State futureState;
        public int regionIndex;
    }

    int currentRegionIndex = 0;

    Cell[,] cells_;

    enum GenerationState {
        IDLE,
        CELLULAR,
        REGION_KILLER
    }

    GenerationState state_ = GenerationState.CELLULAR;

    // Start is called before the first frame update
    void Start()
    {
        //Allocate memory
        cells_ = new Cell[size_,size_];
        
        
        //Run through the array and fill every cell in a random value
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {
                //If random == 0 => the cell is dead, otherwise the cell is alive
                cells_[x, y].currentState = Random.Range(0, 2) == 0 ? State.DEAD : State.ALIVE;
                
                //Each cell is in a null region at the start
                cells_[x, y].regionIndex = nullRegion;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        //Switch case to check at what states of the generation the algorithm is
        switch (state_) {
            case GenerationState.IDLE:
                break;
            case GenerationState.CELLULAR:
                //Use a maximum number of iteration
                if (iteration_ <= 0) {
                    state_ = GenerationState.REGION_KILLER;
                } else {
                    iteration_--;
                    CellulaAutomat();
                }
                break;
            case GenerationState.REGION_KILLER:
                RegionKiller();
                state_ = GenerationState.IDLE;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void RegionKiller() {
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        //This list contains every region index that are too small and that need to be destroyed
        List<int> regionIndexToKill = new List<int>();
        
        /*
         * Foreach cell, check if the cell is alive,
         * If the cell is alive, check if its region index is null
         * If the region is null, assign the new region to the cell and assign the same value to every neighbors
         */ 
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {

                if (cells_[x, y].currentState != State.ALIVE) continue;
                 
                if (cells_[x, y].regionIndex != nullRegion) continue;

                //This list holds every cell to check 
                List<Vector2Int> openList = new List<Vector2Int>();
                
                openList.Add(new Vector2Int(x, y));
                
                int currentRegionCellNb = 0;

                while (openList.Count > 0) {
                    Vector2Int pos = openList[0];
                    openList.RemoveAt(0);
                    
                    //assign the current region index to the cell
                    cells_[pos.x, pos.y].regionIndex = currentRegionIndex;

                    //increase the number of cell in this region
                    currentRegionCellNb++;
                    
                    foreach (Vector3Int posN in bounds.allPositionsWithin) {
                        //Avoid self
                        if(posN.x == 0 && posN.y == 0) continue; 
                    
                        //Check is not in diagonal (because every cell of a region can be walk from on to the other, which is not the case if is in diagonal)
                        if (posN.x != 0 && posN.y != 0) continue;
                        
                        //Check is in bounds
                        if(pos.x + posN.x < 0 || pos.x + posN.x >= size_) continue;
                        if(pos.y + posN.y < 0 || pos.y + posN.y >= size_) continue;
                    
                        //Count alive neighbors
                        if (cells_[pos.x + posN.x, pos.y + posN.y].currentState == State.ALIVE) {
                            //If the cell's region index is not null, then continue
                            if (cells_[pos.x + posN.x, pos.y + posN.y].regionIndex != nullRegion) continue;
                            //If the cell is already in the open list, then we don't need to add it once more to the list
                            if (openList.Contains(new Vector2Int(pos.x + posN.x, pos.y + posN.y))) continue;
                            openList.Add(new Vector2Int(pos.x + posN.x, pos.y + posN.y));
                        }
                    }
                }

                if (currentRegionCellNb < minimumCellPerRegion_) {
                    regionIndexToKill.Add(currentRegionIndex);
                }

                //At this stage every cell belonging to the current region has been found, so we change  the value of the current region
                currentRegionIndex++;
            }
        }
        
        //Loop through each tile and if it belong to a region that is too small, then kill it
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {
                if(cells_[x, y].currentState != State.ALIVE) continue;

                if (regionIndexToKill.Contains(cells_[x, y].regionIndex)) {
                    cells_[x, y].currentState = State.DEAD;
                }
            }
        }
    }

    void CellulaAutomat() {
        /*
         * Object bounds used to have those value
         * Vector2Int(-1, -1, 0)
         * Vector2Int(0, -1, 0)
         * Vector2Int(1, -1, 0)
         * Vector2Int(-1, 0, 0)
         * Vector2Int(0, 0, 0)
         * Vector2Int(1, 0, 0)
         * Vector2Int(-1, 1, 0)
         * Vector2Int(0, 1, 0)
         * Vector2Int(1, 1, 0)
         */ 
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        //Foreach cell count the number of alive neighbor
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {

                int aliveNeighbors = 0;

                //Check every neighbors
                foreach (Vector3Int pos in bounds.allPositionsWithin) {
                    //Avoid self
                    if(pos.x == 0 && pos.y == 0) continue; 
                    
                    //Check is in bounds
                    if(x + pos.x < 0 || x + pos.x >= size_) continue;
                    if(y + pos.y < 0 || y + pos.y >= size_) continue;
                    
                    //Count alive neighbors
                    if (cells_[x + pos.x, y + pos.y].currentState == State.ALIVE) {
                        aliveNeighbors++;
                    }
                }
                
                //Rules
                if (cells_[x, y].currentState == State.ALIVE && (aliveNeighbors == 1 || (aliveNeighbors >= 4 && aliveNeighbors <= 8))) {
                    cells_[x, y].futureState = State.ALIVE;
                }else if (cells_[x, y].currentState == State.DEAD && (aliveNeighbors >= 5 && aliveNeighbors <= 7)) {
                    cells_[x, y].futureState = State.ALIVE;
                } else {
                    cells_[x, y].futureState = State.DEAD;
                }
            }
        }
        
        //Walled map
        //Make sure the map is walled
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {
                if (x < nbWallCell_ || x > size_ - nbWallCell_) {
                    cells_[x, y].futureState = State.DEAD;
                }
                
                if (y < nbWallCell_ || y > size_ - nbWallCell_) {
                    cells_[x, y].futureState = State.DEAD;
                }
            }
        }

        //Update current states
        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {
                cells_[x, y].currentState = cells_[x, y].futureState ;
            }
        }
    }

    void OnDrawGizmos() {
        if (cells_ == null) return;

        List<Color> colors = new List<Color> {
            Color.gray,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.red,
            Color.yellow
        };

        for (int x = 0; x < size_; x++) {
            for (int y = 0; y < size_; y++) {
                if (cells_[x, y].currentState == State.DEAD) {
                    Gizmos.color = Color.black;
                } else {
                    Gizmos.color = colors[cells_[x, y].regionIndex % colors.Count];
                }
                
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }
    }
}
