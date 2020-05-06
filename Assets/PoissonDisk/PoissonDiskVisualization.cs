using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoissonDiskVisualization : MonoBehaviour {
    [SerializeField] Vector2 sampleRegionSize_;
    [SerializeField] float radius_ = 1;
    [SerializeField] int rejectionNumber_ = 30;

    [SerializeField] bool displayGrid_ = true;
    
    int[,] grid_;
    List<Vector2> points_;
    float cellSize_;

    void Start() {
//        cellSize_ = radius_ / Mathf.Sqrt(2);
    }

    void OnValidate() {
        cellSize_ = radius_ / Mathf.Sqrt(2);
        //Build the grid 
        grid_ = new int[Mathf.CeilToInt(sampleRegionSize_.x / cellSize_), Mathf.CeilToInt(sampleRegionSize_.y / cellSize_)];

        points_ = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();
        
        //Center point
        spawnPoints.Add(sampleRegionSize_ / 2);
        while (spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
    
            Vector2 spawnCenter = spawnPoints[spawnIndex];

            bool candidateValid = false;

            for (int i = 0; i < rejectionNumber_; i++) {
                float angle = Random.value * Mathf.PI * 2;
                
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(radius_, 2 * radius_);

                if (!IsValid(candidate)) continue;
                //If the candidate is valid (it's not in a radius of another object)
                points_.Add(candidate);
                spawnPoints.Add(candidate);
                grid_[(int)(candidate.x / cellSize_), (int)(candidate.y / cellSize_)] = points_.Count; //Index of the added point
                candidateValid = true;
                break;
            }
            
            //If the candidate is invalid, then remove the spawn point
            if (!candidateValid) {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
    }

    bool IsValid(Vector2 candidate) {
        if (candidate.x < 0 || candidate.x >= sampleRegionSize_.x || candidate.y < 0 || candidate.y >= sampleRegionSize_.y) return false;
        
        //Cell currentPosition
        int cellX = (int)(candidate.x / cellSize_);
        int cellY = (int)(candidate.y / cellSize_);
    
        //Goes from a 5 by 5 square around the position
        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(cellX + 2, grid_.GetLength(0) - 1);
        
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(cellY + 2, grid_.GetLength(1) - 1);
        
        //Loop through each square adjacent
        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int y = searchStartY; y <= searchEndY; y++) {
                
                int pointIndex = grid_[x, y] - 1;
                
                //If the point index has no assignation yet (by default the value == 0 then minus 1 goes to == -1)
                if (pointIndex == -1) continue;
                
                //Check if the square distance between the point in the grid and the candidate is valid
                float dist = (candidate - points_[pointIndex]).sqrMagnitude;
                    
                if (dist < radius_ * radius_) {
                    return false;
                }
            }
        }
        
        return true;
    }

    void OnDrawGizmos() {
        if (points_ == null || points_.Count <= 0) return;
        
        Gizmos.DrawWireCube(sampleRegionSize_/2.0f, sampleRegionSize_);
            
        foreach (Vector2 vector2 in points_) {
            Gizmos.DrawWireSphere(vector2, radius_ * 0.5f);
        }

        if (!displayGrid_) return;

        Gizmos.color = Color.red;
        for (int x = 0; x < grid_.GetLength(0); x++) {
            for (int y = 0; y < grid_.GetLength(1); y++) {
                if (grid_[x, y] == 0) {
                    Gizmos.DrawWireCube(
                        new Vector3(x * cellSize_ + cellSize_ * 0.5f, y * cellSize_ + cellSize_ * 0.5f, 0),
                        new Vector3(cellSize_, cellSize_, cellSize_));
                    Gizmos.DrawLine(
                        new Vector3(x * cellSize_, y * cellSize_, 0),
                        new Vector3(x * cellSize_ + cellSize_, y * cellSize_ + cellSize_, 0));
                    
                    Gizmos.DrawLine(
                        new Vector3(x * cellSize_ + cellSize_, y * cellSize_, 0),
                        new Vector3(x * cellSize_, y * cellSize_ + cellSize_, 0));
                }
            }
        }
        
        Gizmos.color = Color.white;
        for (int x = 0; x < grid_.GetLength(0); x++) {
            for (int y = 0; y < grid_.GetLength(1); y++) {
                if(grid_[x,y] != 0)
                    Gizmos.DrawWireCube(new Vector3(x * cellSize_ + cellSize_ * 0.5f, y * cellSize_ + cellSize_ * 0.5f, 0), new Vector3(cellSize_, cellSize_, cellSize_));
            }
        }
    }
}
