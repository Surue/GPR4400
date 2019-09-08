using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node {
    public List<Node> neighbors;
    public Vector2 pos;

    public bool isFree;

    public bool hasBeenVisited = false;
    public bool isPath = false;

    public Node cameFrom = null;

    public float cost;
    public float currentCost = -1;
}

public class NavigationGraphGeneration : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;

    int minX = 0, maxX = 0, minY = 0, maxY = 0;

    [HideInInspector]
    public Node tileStart;
    [HideInInspector]
    public Node tileGoal;
    
    Node[,] graph;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGraph();

        StartCoroutine(DFS());
    }

    void GenerateGraph()
    {
        minX = tilemap.cellBounds.xMin;
        maxX = tilemap.cellBounds.xMax;

        minY = tilemap.cellBounds.yMin;
        maxY = tilemap.cellBounds.yMax;

        graph = new Node[maxX - minX, maxY - minY];

        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                TileBase currentTile = tilemap.GetTile(new Vector3Int(x, y, 0));

                if (currentTile == null) continue;

                Node newNode = new Node {
                    pos = new Vector2(x * tilemap.cellSize.x + tilemap.cellSize.x / 2, y * tilemap.cellSize.y + tilemap.cellSize.y / 2),
                    neighbors = new List<Node>()
                };

                switch (currentTile.name) {
                    case "tileFree":
                        newNode.isFree = true;
                        break;
                    case "tileSolid":
                        newNode.isFree = false;
                        break;
                    case "tileStart":
                        newNode.isFree = true;
                        tileStart = newNode;
                        break;
                    case "tileGoal":
                        newNode.isFree = true;
                        tileGoal = newNode;
                        break;
                    default:
                        newNode.isFree = false;
                        Debug.Log("case not handled");
                        break;
                }

                graph[x - minX, y - minY] = newNode;
            }
        }

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        for (int i = 0; i < graph.GetLength(0); i++) {
            for (int j = 0; j < graph.GetLength(1); j++) {
                Node node = graph[i, j];

                if (node == null) continue;
                if (!node.isFree) continue;

                foreach (Vector2Int b in bounds.allPositionsWithin) {
                    if (i + b.x < 0 || i + b.x > maxX - minX || j + b.y <= 0 || j + b.y > maxY - minY) continue;
                    if (b.x == 0 && b.y == 0) continue;

                    if (graph[i + b.x, j + b.y] == null) continue;
                    if (!graph[i + b.x, j + b.y].isFree) continue;
                    
                    node.neighbors.Add(graph[i + b.x, j + b.y]);
                }
            }
        }
    }

    IEnumerator BFS()
    {
        Node startingNode = tileStart;

        List<Node> openList = new List<Node>{startingNode};
        List<Node> closedList = new List<Node>();

        int crashValue = 1000;

        while (openList.Count > 0 && --crashValue > 0) {
            Node currentNode = openList[0];
            openList.RemoveAt(0);

            currentNode.hasBeenVisited = true;

            closedList.Add(currentNode);

            if (currentNode == tileGoal) {
                break;
            } else {
                foreach (Node currentNodeNeighbor in currentNode.neighbors) {
                    if (closedList.Contains(currentNodeNeighbor) || openList.Contains(currentNodeNeighbor)) {
                        continue;
                    }

                    currentNodeNeighbor.cameFrom = currentNode;

                    openList.Add(currentNodeNeighbor);
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        if (crashValue <= 0) {
            Debug.Log("Nico a fait de la merde");
        }


        {
            Node currentNode = tileGoal;

            while (currentNode.cameFrom != null) {
                currentNode.isPath = true;
                currentNode = currentNode.cameFrom;
            }

            currentNode.isPath = true;
        }

    }

    IEnumerator DFS()
    {
        Node startingNode = tileStart;

        List<Node> openList = new List<Node>{startingNode};
        List<Node> closedList = new List<Node>();

        int crashValue = 1000;

        while(openList.Count > 0 && --crashValue > 0) {
            Node currentNode = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count - 1);

            currentNode.hasBeenVisited = true;

            closedList.Add(currentNode);

            if(currentNode == tileGoal) {
                break;
            } else {
                foreach(Node currentNodeNeighbor in currentNode.neighbors) {
                    if(closedList.Contains(currentNodeNeighbor) || openList.Contains(currentNodeNeighbor)) {
                        continue;
                    }

                    currentNodeNeighbor.cameFrom = currentNode;

                    openList.Add(currentNodeNeighbor);
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        if(crashValue <= 0) {
            Debug.Log("Nico a fait de la merde");
        }


        {
            Node currentNode = tileGoal;

            while(currentNode.cameFrom != null) {
                currentNode.isPath = true;
                currentNode = currentNode.cameFrom;
            }

            currentNode.isPath = true;
        }
    }

    void OnDrawGizmos()
    {
        if (minX == maxX || minY == maxY) return;

        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        Gizmos.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY));
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(minX, maxY));
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(minX, minY));

        foreach (Node node in graph) {
            if(node == null) continue;
            
            Gizmos.color = node.isFree ? Color.blue : Color.red;

            if (node.hasBeenVisited) {
                Gizmos.color = Color.yellow;
            }

            if (node.isPath) {
               Gizmos.color = Color.green;
            }
            
            Gizmos.DrawCube(node.pos, Vector3.one * 0.75f); 

            foreach (Node nodeNeighbor in node.neighbors) {
                Gizmos.DrawLine(node.pos, nodeNeighbor.pos);
            }
        }
    }
}
