using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BS {
    public Vector2 center;
    public float radius;

    public bool Overlap(BS other) {
        return Vector2.Distance(center, other.center) < radius + other.radius;
    }
}
