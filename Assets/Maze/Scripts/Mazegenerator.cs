using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    enum RuleState
    {
        ALWAYS_TRUE,
        ALWAYS_FALSE,
        DONT_CARE
    }

    struct Rule
    {
        public RuleState up;
        public RuleState down;
        public RuleState left;
        public RuleState right;
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
    }

    int step = 10;

    void GenerateChunkChild(Cell cell)
    {
        step--;

        if (step < 0) {
            return;
        }

        if (cell.chunk.down && cells[cell.x, cell.y - 1].chunk == null) {
            Rule rule = GetRulesForNextChunk(cell.x, cell.y - 1);

            GameObject newChunk = SelectChunk(rule);

            if (newChunk != null) {
                GameObject instance = GameObject.Instantiate(SelectChunk(rule));

                instance.name = "Chunk (" + cell.x.ToString() + ", " + (cell.y - 1).ToString() + ")";
                instance.transform.position =
                    new Vector2(sizeX * (cell.x) + sizeX / 2f, sizeY * (cell.y - 1) + sizeY / 2f);

                cells[cell.x, cell.y - 1].chunk = instance.GetComponent<Chunck>();

                GenerateChunkChild(cells[cell.x, cell.y - 1]);
            }
        }

        if (cell.chunk.up && cells[cell.x, cell.y + 1].chunk == null) {
            Rule rule = GetRulesForNextChunk(cell.x, cell.y + 1);

            GameObject newChunk = SelectChunk(rule);

            if (newChunk != null) {
                GameObject instance = GameObject.Instantiate(SelectChunk(rule));
                instance.name = "Chunk (" + cell.x.ToString() + ", " + (cell.y + 1).ToString() + ")";
                instance.transform.position =
                    new Vector2(sizeX * (cell.x) + sizeX / 2f, sizeY * (cell.y + 1) + sizeY / 2f);

                cells[cell.x, cell.y + 1].chunk = instance.GetComponent<Chunck>();

                GenerateChunkChild(cells[cell.x, cell.y + 1]);
            }
        }

        if (cell.chunk.left && cells[cell.x - 1, cell.y].chunk == null) {
            Rule rule = GetRulesForNextChunk(cell.x - 1, cell.y);

            GameObject newChunk = SelectChunk(rule);

            if (newChunk != null) {
                GameObject instance = GameObject.Instantiate(SelectChunk(rule));
                instance.name = "Chunk (" + (cell.x - 1).ToString() + ", " + (cell.y).ToString() + ")";
                instance.transform.position =
                    new Vector2(sizeX * (cell.x - 1) + sizeX / 2f, sizeY * (cell.y) + sizeY / 2f);

                cells[cell.x - 1, cell.y].chunk = instance.GetComponent<Chunck>();

                GenerateChunkChild(cells[cell.x - 1, cell.y]);
            }
        }

        if (cell.chunk.right && cells[cell.x + 1, cell.y].chunk == null) {
            Rule rule = GetRulesForNextChunk(cell.x + 1, cell.y);

            GameObject newChunk = SelectChunk(rule);

            if (newChunk != null) {
                GameObject instance = GameObject.Instantiate(SelectChunk(rule));
                instance.name = "Chunk (" + (cell.x + 1).ToString() + ", " + (cell.y).ToString() + ")";
                instance.transform.position =
                    new Vector2(sizeX * (cell.x + 1) + sizeX / 2f, sizeY * (cell.y) + sizeY / 2f);

                cells[cell.x + 1, cell.y].chunk = instance.GetComponent<Chunck>();

                GenerateChunkChild(cells[cell.x + 1, cell.y]);
            }
        }

        //Pour chaque porte on vérifie les voisins
    }

    Rule GetRulesForNextChunk(int x, int y)
    {
        Rule rule;
        rule.up = RuleState.DONT_CARE;
        rule.left = RuleState.DONT_CARE;
        rule.right = RuleState.DONT_CARE;
        rule.down = RuleState.DONT_CARE;

        //down
        if (y > 0) {
            if (cells[x, y - 1].chunk != null) {
                rule.down = cells[x, y - 1].chunk.up ? RuleState.ALWAYS_TRUE : RuleState.ALWAYS_FALSE;
            }
        } else {
            rule.down = RuleState.ALWAYS_FALSE;
        }

        //left
        if (x > 0) {
            if (cells[x - 1, y].chunk != null) {
                rule.left = cells[x - 1, y].chunk.right ? RuleState.ALWAYS_TRUE : RuleState.ALWAYS_FALSE;
            }
        } else {
            rule.left = RuleState.ALWAYS_FALSE;
        }

        //right
        if (x < cellNbX - 1) {
            if (cells[x + 1, y].chunk != null) {
                rule.right = cells[x + 1, y].chunk.left ? RuleState.ALWAYS_TRUE : RuleState.ALWAYS_FALSE;
            }
        } else {
            rule.right = RuleState.ALWAYS_FALSE;
        }

        //up
        if (y < cellNbY - 1) {
            if (cells[x, y + 1].chunk != null) {
                rule.up = cells[x, y + 1].chunk.down ? RuleState.ALWAYS_TRUE : RuleState.ALWAYS_FALSE;
            }
        } else {
            rule.up = RuleState.ALWAYS_FALSE;
        }

        return rule;
    }

    GameObject SelectChunk(Rule rule)
    {
        List<GameObject> possibleNewChunk = (from t in chunksAvailable
            let currentChunk = t.GetComponent<Chunck>()
            where rule.down != RuleState.ALWAYS_TRUE || currentChunk.down
            where rule.down != RuleState.ALWAYS_FALSE || !currentChunk.down
            where rule.up != RuleState.ALWAYS_TRUE || currentChunk.up
            where rule.up != RuleState.ALWAYS_FALSE || !currentChunk.up
            where rule.left != RuleState.ALWAYS_TRUE || currentChunk.left
            where rule.left != RuleState.ALWAYS_FALSE || !currentChunk.left
            where rule.right != RuleState.ALWAYS_TRUE || currentChunk.right
            where rule.right != RuleState.ALWAYS_FALSE || !currentChunk.right
            select t).ToList();

        return possibleNewChunk.Count == 0 ? null : possibleNewChunk[Random.Range(0, possibleNewChunk.Count)];
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < cellNbX; x++) {
            for (int y = 0; y < cellNbY; y++) {
                Gizmos.DrawWireCube(new Vector3(x * sizeX + sizeX / 2f, y * sizeY + sizeY / 2f),
                    new Vector3(sizeX, sizeY));
            }
        }
    }
}