using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Avoidance")]
public class AvoidanceBehavior : SteeringBehavior {

    [SerializeField] float radiusAvoidance_ = 1;
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbors) {

        Vector2 move = Vector2.zero;
        if (neighbors.Count == 0) {
            return move;
        }

        float sqrRadius = radiusAvoidance_ * radiusAvoidance_;

        int nbAvoidedNeighbor = 0;

        foreach (Transform transform in neighbors) {
            if ((transform.position - agent.transform.position).sqrMagnitude < sqrRadius) {
                nbAvoidedNeighbor++;
                move += (Vector2)(agent.transform.position - transform.position);
            }
        }

        if (nbAvoidedNeighbor > 0) {
            move /= nbAvoidedNeighbor;
        }

        return move;
    }
}
