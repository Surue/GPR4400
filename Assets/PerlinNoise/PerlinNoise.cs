using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PerlinNoise : MonoBehaviour {
    [SerializeField] GameObject pCube;
    [SerializeField] int gridSize = 50;

    [SerializeField] float factor = 0.05f;
    
    GameObject[,] grid;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                grid[x, y] = new GameObject();
                
                float height = Mathf.PerlinNoise(x * factor + Time.time, y * factor + Time.time);

                grid[x, y].transform.localScale = new Vector3(1, height, 1);
                grid[x, y].transform.position = new Vector3(x, height / 2.0f, y);
                
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
//        for (int x = 0; x < gridSize; x++) {
//            for (int y = 0; y < gridSize; y++) {
//                float height = Mathf.PerlinNoise(x * factor + Time.time, y * factor + Time.time);
//
//                grid[x, y].transform.localScale = new Vector3(1, height, 1);
//                grid[x, y].transform.position = new Vector3(x, height / 2.0f, y);
//            }
//        }
    }

    void OnDrawGizmos() {
        if (grid == null) return; 
        
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                float height = grid[x, y].transform.localScale.y;
                if (height < 0.3) {
                    Gizmos.color = Color.blue;
                }else if (height >= 0.3 && height < 0.4) {
                    Gizmos.color = Color.yellow;
                } else if (height >= 0.4 && height < 0.7) {
                    Gizmos.color = Color.green;
                } else {
                    Gizmos.color = Color.white;
                }
                
                Gizmos.DrawCube(grid[x, y].transform.position, grid[x, y].transform.localScale);
            }
        }
    }
}
