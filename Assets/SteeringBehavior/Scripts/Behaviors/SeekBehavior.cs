using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior : SteeringBehavior
{
    public override Vector2 Steer(Rigidbody2D body, Collider2D col, float maxSpeed)
    {
        Vector2 seekVelocity = Vector2.zero;
        
        if(col.gameObject.CompareTag("Attractor")) {
            seekVelocity = col.transform.position - body.transform.position;
            seekVelocity = seekVelocity.normalized * maxSpeed;
            seekVelocity += seekVelocity - body.velocity;
        }

        return seekVelocity;
    }
}
