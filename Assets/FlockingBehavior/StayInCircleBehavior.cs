using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/StayInCircle")]
public class StayInCircleBehavior : SteeringBehavior {

    Vector2 center = Vector2.zero;
    [SerializeField] float radius_;
    [SerializeField] float factorRadius_ = 0.8f;
    
    public override Vector2 CalculateMove(Agent agent, List<Transform> neighbors) {

        Vector2 centerOffset = center - (Vector2)agent.transform.position;

        float t = centerOffset.magnitude / radius_;

        Vector2 move = Vector2.zero;
        
        if (t > factorRadius_) {
            move = centerOffset * t * t;
        }
        
        return move;
    }
}