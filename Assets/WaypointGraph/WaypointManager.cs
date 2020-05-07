using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaypointManager : MonoBehaviour
{
    enum NodeType {
        FREE,
        NOT_FREE
    }
    
    struct Node {
        public Vector2 pos;
        public List<Vector2Int> neighbors;
        public NodeType type;
    }

    Node[,] nodes_;

    [SerializeField] Tilemap obstacleTilemap_;
    [SerializeField] Tilemap groundTilemap_;

    Vector2Int sizeTilemap_;
    
    // Start is called before the first frame update
    void Start() {
        sizeTilemap_ = (Vector2Int) obstacleTilemap_.size;

        nodes_ = new Node[sizeTilemap_.x, sizeTilemap_.y];

        Vector2Int offset = (Vector2Int)obstacleTilemap_.origin;
        
        for (int x = 0 + offset.x; x < sizeTilemap_.x + offset.x; x++) {
            for (int y = 0 + offset.y; y < sizeTilemap_.y + offset.y; y++) {
                Vector2Int nodeIndex = new Vector2Int(x - offset.x, y - offset.y);
                
                nodes_[nodeIndex.x, nodeIndex.y].pos = new Vector2(x, y) + (Vector2)groundTilemap_.cellSize / 2.0f;
                
                nodes_[nodeIndex.x, nodeIndex.y].type = NodeType.NOT_FREE;
                
                if (groundTilemap_.HasTile(new Vector3Int(x, y, 0))) {
                    if (!obstacleTilemap_.HasTile(new Vector3Int(x, y, 0))) {
                        nodes_[nodeIndex.x, nodeIndex.y].type = NodeType.FREE;
                    }
                }
            }
        }
        
        BoundsInt bounds = new BoundsInt(-2, -2, 0, 5, 5, 1);
        
        for (int x = 0; x < sizeTilemap_.x; x++) {
            for (int y = 0 ; y < sizeTilemap_.y; y++) {
                if(nodes_[x, y].type == NodeType.NOT_FREE) continue;
                
                nodes_[x, y].neighbors = new List<Vector2Int>();

                foreach (Vector3Int index in bounds.allPositionsWithin) {
                    if(x + index.x < 0 || x + index.x >= sizeTilemap_.x) continue;
                    if(y + index.y < 0 || y + index.y >= sizeTilemap_.y) continue;
                    
                    if(index.x == 0 && index.y == 0) continue;

                    if (x + index.x == 0 || y + index.y == 0) {
                        if (nodes_[x + index.x, y + index.y].type == NodeType.NOT_FREE) continue;
                        Vector2 dir = nodes_[x + index.x, y + index.y].pos - nodes_[x, y].pos;

                        if (!Physics2D.Raycast(nodes_[x, y].pos, dir.normalized,
                            Vector2.Distance(nodes_[x + index.x, y + index.y].pos, nodes_[x, y].pos))) {
                            nodes_[x, y].neighbors.Add(new Vector2Int(x + index.x, y + index.y));
                        }
                        
                    } else {
                        if (nodes_[x + index.x, y].type == NodeType.FREE &&
                            nodes_[x, y + index.y].type == NodeType.FREE) {
                            if (nodes_[x + index.x, y + index.y].type == NodeType.NOT_FREE) continue;

                            Vector2 dir = nodes_[x + index.x, y + index.y].pos - nodes_[x, y].pos;
                            
                            if (!Physics2D.Raycast(nodes_[x, y].pos, dir.normalized,
                                Vector2.Distance(nodes_[x + index.x, y + index.y].pos, nodes_[x, y].pos))) {
                                nodes_[x, y].neighbors.Add(new Vector2Int(x + index.x, y + index.y));
                            }
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos() {
        if (nodes_ == null) return;
        
        for (int x = 0; x < nodes_.GetLength(0); x++) {
            for (int y = 0; y < nodes_.GetLength(1); y++) {
                if (nodes_[x, y].type == NodeType.FREE) {
                    Gizmos.color = Color.blue;
                } else {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireSphere(nodes_[x, y].pos, 0.1f);

//                Gizmos.color = Color.blue;
                if(nodes_[x, y].neighbors == null) continue;
                
                foreach (Vector2Int index in nodes_[x,y].neighbors) {
                    Gizmos.DrawLine(nodes_[x, y].pos, nodes_[index.x, index.y].pos);
                }
            }
        }
    }
}
