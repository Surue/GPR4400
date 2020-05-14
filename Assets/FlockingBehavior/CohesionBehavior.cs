using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Cohesion")]
public class CohesionBehavior : SteeringBehavior {

    Vector2 velocity = Vector2.one;
    
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbors) {

        Vector2 move = Vector2.zero;
        if (neighbors.Count == 0) {
            return move;
        }

        foreach (Transform transform in neighbors) {
            move += (Vector2)transform.position;
        }

        //Center point
        move /= neighbors.Count;

        move -= (Vector2)agent.transform.position;

        move = Vector2.SmoothDamp(agent.transform.up, move, ref velocity, 0.5f);

        return move;
    }
}
