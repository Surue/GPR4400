using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointNode : MonoBehaviour {
    [SerializeField] WayPointNode nextNeighbors_;
    [SerializeField] WayPointNode previousNeighbors_;
    
    

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (nextNeighbors_ != null) {
            Gizmos.DrawLine(transform.position, nextNeighbors_.transform.position);
        }
        
        if (previousNeighbors_ != null) {
            Gizmos.DrawLine(transform.position, previousNeighbors_.transform.position);
        }
    }

    public WayPointNode GetNextWayPointNode() {
        return nextNeighbors_;
    }
    
    public WayPointNode GetPreviousWayPointNode() {
        return previousNeighbors_;
    }
}
