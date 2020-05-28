using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Body : MonoBehaviour {
    BS bs_;

    BoxCollider2D collider2D_;
    
    void Start() {
        collider2D_ = GetComponent<BoxCollider2D>();

//        aabb_.bottomLeft = collider2D_.bounds.min;
//        aabb_.topRight = collider2D_.bounds.max;
    }

    void FixedUpdate() {
//        aabb_.bottomLeft = collider2D_.bounds.min;
//        aabb_.topRight = collider2D_.bounds.max;

        bs_.center = transform.position;
        bs_.radius = Mathf.Max(collider2D_.bounds.extents.x, collider2D_.bounds.extents.y);
    }

    public bool Overlap(Body other) {
        return bs_.Overlap(other.bs_);
    }

    void OnDrawGizmos() {
//        Vector2 extent = aabb_.topRight - aabb_.bottomLeft;
//        Vector2 center = aabb_.bottomLeft + extent / 2.0f;
//        Gizmos.DrawWireCube(center, extent);

        Gizmos.DrawWireSphere(bs_.center, bs_.radius);
    }
}
