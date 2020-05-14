using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Align")]
public class AlignBehavior : SteeringBehavior {
    
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbors) {

        Vector2 move = Vector2.zero;
        if (neighbors.Count == 0) {
            return move;
        }

        foreach (Transform transform in neighbors) {
            move += (Vector2)transform.up;
        }

        move /= neighbors.Count;

        return move;
    }
}
