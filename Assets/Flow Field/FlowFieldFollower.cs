using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldFollower : MonoBehaviour {

    FlowField.Node currentNode_;

    FlowField flowField_;

    Rigidbody2D body_;

    [SerializeField] float speed_ = 5;
    [SerializeField] float stoppingDistance_ = 0.1f;
    
    // Start is called before the first frame update
    void Start() {
        flowField_ = FindObjectOfType<FlowField>();

        currentNode_ = flowField_.GetClosestNode(transform.position);

        body_ = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNode_.child != null) {

            if (Vector3.Distance(transform.position, currentNode_.child.position) < stoppingDistance_) {
                currentNode_ = currentNode_.child;
            } else {

                Vector3 dir = (currentNode_.child.position - transform.position).normalized;

                body_.velocity = dir * speed_;
            }
        } else {
            body_.velocity = Vector2.zero;
        }
    }

    void OnDrawGizmos() {
        if (currentNode_ != null) {
            Gizmos.DrawWireSphere(currentNode_.position, 1);
        }
    }
}
