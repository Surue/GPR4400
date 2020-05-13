using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowField : MonoBehaviour {

    [SerializeField] Tilemap groundTilemap_;
    [SerializeField] Tilemap wallTilemap_;
    
    public class Node {
        public List<Node> neighbors;
        public Node child;
        public Vector3 position;
    }

    Node[,] nodes_;

    Vector2Int sizeTilemap_;

    Vector3 target_;
    
    // Start is called before the first frame update
    void Awake()
    {
        sizeTilemap_ = (Vector2Int) wallTilemap_.size;

        nodes_ = new Node[sizeTilemap_.x, sizeTilemap_.y];

        Vector2Int offset = (Vector2Int)wallTilemap_.origin;
        
        for (int x = 0 + offset.x; x < sizeTilemap_.x + offset.x; x++) {
            for (int y = 0 + offset.y; y < sizeTilemap_.y + offset.y; y++) {
                Vector2Int nodeIndex = new Vector2Int(x - offset.x, y - offset.y);

                if (!wallTilemap_.HasTile(new Vector3Int(x, y, 0))) {
                    nodes_[nodeIndex.x, nodeIndex.y] = new Node {
                        position = new Vector2(x, y) + (Vector2) groundTilemap_.cellSize / 2.0f
                    };
                }

            }
        }

        BoundsInt bounds = new BoundsInt(-1, - 1, 0, 3, 3 , 1);
        
        for (int x = 0; x < nodes_.GetLength(0); x++) {
            for (int y = 0; y < nodes_.GetLength(1); y++) {

                if (nodes_[x, y] == null) continue;
                nodes_[x, y].neighbors = new List<Node>();
                    
                foreach (Vector3Int vector3Int in bounds.allPositionsWithin) {
                    if (x + vector3Int.x < 0 || x + vector3Int.x >= nodes_.GetLength(0)) continue;
                    if (y + vector3Int.y < 0 || y + vector3Int.y >= nodes_.GetLength(1)) continue;
                    
                    if(vector3Int.x == 0 && vector3Int.y == 0) continue;

                    if (nodes_[x + vector3Int.x, y + vector3Int.y] != null) {
                        nodes_[x, y].neighbors.Add(nodes_[x + vector3Int.x, y + vector3Int.y]);
                    }
                }
            }
        }
    }

    float DistanceManhattan(Vector3 pos1, Vector3 pos2) {
        return Mathf.Abs(pos2.x - pos1.x) + Mathf.Abs(pos2.y - pos1.y);
    }

    void GenerateFlowField() {
        //1. find closest node to target
        Node startNode = GetClosestNode(target_);
        startNode.child = null;
        
        //2. find closest node to start point (if A*)
        
        //3. Build flow field
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        
        openList.Add(startNode);

        while (openList.Count > 0) {
            Node currentNode = openList[0];
            
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            foreach (Node neighbor in currentNode.neighbors) {

                if (!openList.Contains(neighbor) && !closedList.Contains(neighbor)) {

                    neighbor.child = currentNode;
                    openList.Add(neighbor);
                }

            }
        }
    }

    public Node GetClosestNode(Vector3 position) {
        float minDistance = Mathf.Infinity;
        Node startNode = null;
        
        foreach (Node node in nodes_) {
            if(node == null) continue;
            
            float distance = DistanceManhattan(position, node.position);

            if (distance > minDistance) continue;
            minDistance = distance;
            startNode = node;
        }

        return startNode;
    }

    public void SetTarget(Vector3 target) {
        target.z = 0;
        target_ = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            
            GenerateFlowField();
        }
    }

    void OnDrawGizmos() {
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target_, 1);

        if (nodes_ == null) return;
        
        Gizmos.color = Color.cyan;
        for (int x = 0; x < nodes_.GetLength(0); x++) {
            for (int y = 0; y < nodes_.GetLength(1); y++) {
                if (nodes_[x, y] == null) continue;
                Gizmos.DrawWireSphere(nodes_[x, y].position, 0.25f);

                if (nodes_[x, y].child != null) {
                    Gizmos.DrawLine(nodes_[x, y].position, nodes_[x, y].child.position);
                }
                
            }
        }
    }
}
