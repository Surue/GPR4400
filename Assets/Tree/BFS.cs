using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour {
    [SerializeField] int size = 10;

    enum NodeType {
        START,
        END,
        NONE,
        CURRENT
    }
    
    struct Node {
        public Vector2Int pos;
        public NodeType type;
        public List<Vector2Int> neighbors;
    }

    Node[,] nodes;
    
    // Start is called before the first frame update
    void Start() {
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        
        nodes = new Node[size,size];
        
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                nodes[x, y].pos = new Vector2Int(x, y);
                nodes[x, y].type = NodeType.NONE;

                nodes[x, y].neighbors = new List<Vector2Int>(8);
                foreach (Vector2Int pos in bounds.allPositionsWithin) {
                    if (pos.x == 0 && pos.y == 0) continue;
                    if (x + pos.x < 0 || x + pos.x >= size) continue;
                    if (y + pos.y < 0 || y + pos.y >= size) continue;
                    
                    nodes[x, y].neighbors.Add(new Vector2Int(x + pos.x, y + pos.y));
                }
            }
        }

        nodes[0, 0].type = NodeType.START;

        StartCoroutine(Dfs());
    }

    IEnumerator Bfs() {
        List<Vector2Int> openList = new List<Vector2Int>();
        List<Vector2Int> closedList = new List<Vector2Int>();
        
        openList.Add(new Vector2Int(0, 0));

        while (openList.Count > 0) {
            Vector2Int currentNode = openList[0];
            closedList.Add(currentNode);

            nodes[currentNode.x, currentNode.y].type = NodeType.CURRENT;
            yield return new WaitForSeconds(0.1f);

            foreach (Vector2Int vector2Int in nodes[currentNode.x, currentNode.y].neighbors) {
                if (closedList.Contains(vector2Int) || openList.Contains(vector2Int)) {
                    continue;
                }
                
                openList.Add(vector2Int);
            }
            openList.RemoveAt(0);
            nodes[currentNode.x, currentNode.y].type = NodeType.END;
            yield return null;
        }
        
    }
    
    IEnumerator Dfs() {
        List<Vector2Int> openList = new List<Vector2Int>();
        List<Vector2Int> closedList = new List<Vector2Int>();
        
        openList.Add(new Vector2Int(0, 0));

        while (openList.Count > 0) {
            Vector2Int currentNode = openList[openList.Count - 1];
            closedList.Add(currentNode);

            nodes[currentNode.x, currentNode.y].type = NodeType.CURRENT;
            yield return new WaitForSeconds(0.1f);

            foreach (Vector2Int vector2Int in nodes[currentNode.x, currentNode.y].neighbors) {
                if (closedList.Contains(vector2Int) || openList.Contains(vector2Int)) {
                    continue;
                }
                
                openList.Add(vector2Int);
            }
            openList.RemoveAt(openList.Count - 1);
            nodes[currentNode.x, currentNode.y].type = NodeType.END;
            yield return null;
        }
        
    }

    void OnDrawGizmos() {
        if (nodes == null) return;

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {

                switch (nodes[x, y].type) {
                    case NodeType.START:
                        Gizmos.color = Color.blue;
                        break;
                    case NodeType.END:
                        Gizmos.color = Color.grey;
                        break;
                    case NodeType.NONE:
                        Gizmos.color = Color.white;
                        break;
                    case NodeType.CURRENT:
                        Gizmos.color = Color.green;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }
    }
}
