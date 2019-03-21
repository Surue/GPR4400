using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityScript.Steps;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int cellNbX = 2;
    [SerializeField] int cellNbY = 7;

    int sizeX = 5;
    int sizeY = 5;

    struct Cell
    {
        public int x;
        public int y;
        public Chunck chunk;
    }

    [SerializeField] GameObject[] chunksAvailable;

    Cell[,] cells;

    enum Direction
    {
        UP = 0x0,
        DOWN = 0x1,
        LEFT,
        RIGHT
    }

    struct Rule
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public Direction parentDirection;
    }

    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    void Generate()
    {
        if (sizeX > 0 & sizeY > 0) {
            cells = new Cell[cellNbX, cellNbY];

            for (int x = 0; x < cellNbX; x++) {
                for (int y = 0; y < cellNbY; y++) {
                    cells[x, y].x = x;
                    cells[x, y].y = y;
                }
            }

            GenerateStartChunk();
        }
    }

    void GenerateStartChunk()
    {
        GameObject newChunk = chunksAvailable[1];

        List<GameObject> possibleNewChunk = new List<GameObject>();

        for (int i = 0; i < chunksAvailable.Length; i++) {
            Chunck currentChunk = chunksAvailable[i].GetComponent<Chunck>();

            if ((currentChunk.right || currentChunk.up) && !currentChunk.left && !currentChunk.down) {
                possibleNewChunk.Add(chunksAvailable[i]);
            }
        }

        newChunk = possibleNewChunk[Random.Range(0, possibleNewChunk.Count)];

        GameObject startChunk = GameObject.Instantiate(newChunk);
        startChunk.name = "StartChunk";
        startChunk.transform.position = new Vector2(0 + sizeX / 2f, 0 + sizeY / 2f);

        cells[0, 0].chunk = startChunk.GetComponent<Chunck>();

        GenerateChunkChild(cells[0, 0]);
    }

    void GenerateLevel()
    {
        int flag = 0;

        flag = 1 << 1;
        flag = 1 << 2;

        switch (flag) {
            case 0x0000:
                break;
        }
    }

    void GenerateChunkChild(Cell cell)
    {
        //Parcourir tout les portes possible
        if (cell.chunk.down) {

        }


        if (cell.chunk.up) {
            Rule rule = CheckNewCellUp(cell.x, cell.y + 1);

            GameObject newChunk = GameObject.Instantiate(SelectChunk(rule));
            newChunk.name = "Chunk";
            newChunk.transform.position = new Vector2(sizeX * (cell.x) + sizeX / 2f, sizeY *(cell.y + 1) + sizeY / 2f);

            cells[cell.x, cell.y + 1].chunk = newChunk.GetComponent<Chunck>();
        }

        if (cell.chunk.left) {

        }

        if(cell.chunk.right) {
            Rule rule = CheckNewCellRight(cell.x + 1, cell.y);

            GameObject newChunk = GameObject.Instantiate(SelectChunk(rule));
            newChunk.name = "Chunk";
            newChunk.transform.position = new Vector2(sizeX * (cell.x + 1) + sizeX / 2f, sizeY * (cell.y) + sizeY / 2f);

            cells[cell.x + 1, cell.y].chunk = newChunk.GetComponent<Chunck>();
        }

        //Pour chaque porte on vérifie les voisins
    }

    Rule CheckNewCellUp(int x, int y)
    {
        Rule rule;

        rule.down = true;
        rule.parentDirection = Direction.DOWN;

        //left
        if (x > 0 && cells[x - 1, y].chunk == null) {
            rule.left = true;
        } else {
            rule.left = false;
        }

        //right
        if(x < cellNbX && cells[x + 1, y].chunk == null) {
            rule.right = true;
        } else {
            rule.right = false;
        }

        //up
        if(y < cellNbY && cells[x, y + 1].chunk == null) {
            rule.up = true;
        } else {
            rule.up = false;
        }

        return rule;
    }

    Rule CheckNewCellRight(int x, int y) {
        Rule rule;

        rule.left = true;
        rule.parentDirection = Direction.LEFT;

        //down
        if(y > 0 && cells[x, y - 1].chunk == null) {
            rule.down = true;
        } else {
            rule.down = false;
        }

        //right
        if(x < cellNbX && cells[x + 1, y].chunk == null) {
            rule.right = true;
        } else {
            rule.right = false;
        }

        //up
        if(y < cellNbY && cells[x, y + 1].chunk == null) {
            rule.up = true;
        } else {
            rule.up = false;
        }

        return rule;
    }

    GameObject SelectChunk(Rule rule)
    {
        List<GameObject> possibleNewChunk = new List<GameObject>();
        for(int i = 0;i < chunksAvailable.Length;i++) {
            Chunck currentChunk = chunksAvailable[i].GetComponent<Chunck>();

            switch (rule.parentDirection) {
                case Direction.UP:
                    if(!currentChunk.up) continue;

                    //100
                    if (rule.down && !rule.left && !rule.right) {
                        if (currentChunk.down && !currentChunk.left && !currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //110
                    if(rule.down && rule.left && !rule.right) {
                        if(currentChunk.down && currentChunk.left && !currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //111
                    if(rule.down && rule.left && rule.right) {
                        if(currentChunk.down && currentChunk.left && currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //101
                    if(rule.down && !rule.left && rule.right) {
                        if(currentChunk.down && !currentChunk.left && currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //011
                    if(!rule.down && rule.left && rule.right) {
                        if(!currentChunk.down && currentChunk.left && currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //001
                    if(!rule.down && !rule.left && rule.right) {
                        if(!currentChunk.down && !currentChunk.left && !currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //000
                    if(!rule.down && !rule.left && !rule.right) {
                        if(!currentChunk.down && !currentChunk.left && !currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //010
                    if(!rule.down && rule.left && !rule.right) {
                        if(!currentChunk.down && currentChunk.left && !currentChunk.right) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }
                    
                    break;
                case Direction.DOWN:
                    if(!currentChunk.down) continue;
                    break;
                case Direction.LEFT:
                    if(!currentChunk.left) continue;
                    break;
                case Direction.RIGHT:
                    if(!currentChunk.right) continue;

                    //100
                    if(rule.down && !rule.left && !rule.up) {
                        if(currentChunk.down && !currentChunk.left && !currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //110
                    if(rule.down && rule.left && !rule.up) {
                        if(currentChunk.down && currentChunk.left && !currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //111
                    if(rule.down && rule.left && rule.up) {
                        if(currentChunk.down && currentChunk.left && currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //101
                    if(rule.down && !rule.left && rule.up) {
                        if(currentChunk.down && !currentChunk.left && currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //011
                    if(!rule.down && rule.left && rule.up) {
                        if(!currentChunk.down && currentChunk.left && currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //001
                    if(!rule.down && !rule.left && rule.up) {
                        if(!currentChunk.down && !currentChunk.left && !currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //000
                    if(!rule.down && !rule.left && !rule.up) {
                        if(!currentChunk.down && !currentChunk.left && !currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }

                    //010
                    if(!rule.down && rule.left && !rule.up) {
                        if(!currentChunk.down && currentChunk.left && !currentChunk.up) {
                            possibleNewChunk.Add(chunksAvailable[i]);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return possibleNewChunk[Random.Range(0, possibleNewChunk.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < cellNbX; x++) {
            for (int y = 0; y < cellNbY; y++) {
                Gizmos.DrawWireCube(new Vector3(x * sizeX + sizeX / 2f, y * sizeY + sizeY / 2f), new Vector3(sizeX, sizeY));
            }
        }
    }
}
