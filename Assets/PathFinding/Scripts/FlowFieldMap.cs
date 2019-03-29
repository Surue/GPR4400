using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowFieldMap : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;

    int minX = 0, maxX = 0, minY = 0, maxY = 0;

    [HideInInspector]
    public Node tileStart;
    [HideInInspector]
    public Node tileGoal;
    
    Node[,] graph;

    // Start is called before the first frame update
    void Awake() {
        GenerateGraph();

        BFS();
    }

    void GenerateGraph() {
        minX = tilemap.cellBounds.xMin;
        maxX = tilemap.cellBounds.xMax;

        minY = tilemap.cellBounds.yMin;
        maxY = tilemap.cellBounds.yMax;

        graph = new Node[maxX - minX, maxY - minY];

        for(int x = minX;x < maxX;x++) {
            for(int y = minY;y < maxY;y++) {
                TileBase currentTile = tilemap.GetTile(new Vector3Int(x, y, 0));

                if(currentTile == null) continue;

                Node newNode = new Node {
                    pos = new Vector2(x * tilemap.cellSize.x + tilemap.cellSize.x / 2, y * tilemap.cellSize.y + tilemap.cellSize.y / 2),
                    neighbors = new List<Node>()
                };

                switch(currentTile.name) {
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

        for(int i = 0;i < graph.GetLength(0);i++) {
            for(int j = 0;j < graph.GetLength(1);j++) {
                Node node = graph[i, j];

                if(node == null) continue;
                if(!node.isFree) continue;

                foreach(Vector2Int b in bounds.allPositionsWithin) {
                    if(i + b.x < 0 || i + b.x > maxX - minX || j + b.y <= 0 || j + b.y > maxY - minY) continue;
                    if(b.x == 0 && b.y == 0) continue;

                    if(graph[i + b.x, j + b.y] == null) continue;
                    if(!graph[i + b.x, j + b.y].isFree) continue;

                    node.neighbors.Add(graph[i + b.x, j + b.y]);
                }
            }
        }
    }

    public Node GetNode(Vector2 pos)
    {
        Node returnNode = null;
        float minDistance = float.MaxValue;

        foreach (Node node in graph) {
            if(node == null) continue;
            if(!node.isFree) continue;

            if (Vector2.Distance(node.pos, pos) < minDistance) {
                minDistance = Vector2.Distance(node.pos, pos);
                returnNode = node;
            }
        }

        return returnNode;
    }

    void BFS() {
        Node startingNode = tileStart;

        List<Node> openList = new List<Node>{startingNode};
        List<Node> closedList = new List<Node>();

        int crashValue = 1000;

        while(openList.Count > 0 && --crashValue > 0) {
            Node currentNode = openList[0];
            openList.RemoveAt(0);

            currentNode.hasBeenVisited = true;

            closedList.Add(currentNode);

            
            foreach(Node currentNodeNeighbor in currentNode.neighbors) {
                if(closedList.Contains(currentNodeNeighbor) || openList.Contains(currentNodeNeighbor)) {
                    continue;
                }

                currentNodeNeighbor.cameFrom = currentNode;

                openList.Add(currentNodeNeighbor);
            }
        }

        if(crashValue <= 0) {
            Debug.Log("Nico a fait de la merde");
        }
    }

    void OnDrawGizmos() {
        
    }
}
