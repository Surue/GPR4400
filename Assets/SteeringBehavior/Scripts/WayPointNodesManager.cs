using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayPointNodesManager : MonoBehaviour {
    List<WayPointNode> wayPointNodes_;
    
    // Start is called before the first frame update
    void Start() {
        wayPointNodes_ = GetComponentsInChildren<WayPointNode>().ToList();
    }

    public WayPointNode GetClosestWayPointNode(Vector3 position) {
        int index = 0;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < wayPointNodes_.Count; i++) {
            float distance = (position - wayPointNodes_[i].transform.position).sqrMagnitude;
            
            if(distance > minDistance) continue;
            minDistance = distance;
            index = i;
        }

        return wayPointNodes_[index];
    }
}
