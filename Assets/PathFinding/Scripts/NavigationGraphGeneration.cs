using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node {
    public List<Node> neighbors;
    public Vector2 pos;
    public Vector2Int index;

    public bool isFree;
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
                    index = new Vector2Int(x, y),
                    pos = new Vector2(x + tilemap.cellSize.x / 2, y + tilemap.cellSize.y / 2),
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
                        break;
                }

                graph[x - minX, y - minY] = newNode;
            }
        }

        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        for (int i = 0; i < maxX - minX; i++) {
            for (int j = 0; j < maxY - minY; j++) {
                Node node = graph[i, j];

                if (node == null) continue;
                if (!node.isFree) continue;;

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

            Gizmos.DrawWireSphere(node.pos, 0.25f);

            foreach (Node nodeNeighbor in node.neighbors) {
                Gizmos.DrawLine(node.pos, nodeNeighbor.pos);
            }
        }
    }
}
