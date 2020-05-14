using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathFollower : MonoBehaviour {
    WayPointNode currentWayPointNode_;

    WayPointNodesManager wayPointNodesManager_;

    [SerializeField] bool clockwise_ = true;
    [SerializeField] float stoppingDistance_ = 0.1f;

    enum MovementType {
        TRANSFORM,
        BODY_VELOCITY,
        BODY_FORCE,
        SEEK_BEHAVIOR,
        PATH_BEHAVIOR,
    }

    [SerializeField] MovementType movementType_ = MovementType.TRANSFORM;
    [SerializeField] float speed_;
    [SerializeField] float maxForce_ = 4;
    [SerializeField] float arrivalDistance_ = 3.5f;

    [SerializeField] Rigidbody2D body_;
    
    // Start is called before the first frame update
    void Start() {
        wayPointNodesManager_ = FindObjectOfType<WayPointNodesManager>();

        currentWayPointNode_ = wayPointNodesManager_.GetClosestWayPointNode(transform.position);

        body_ = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        float distanceToTarget = Vector3.Distance(transform.position, currentWayPointNode_.transform.position);
        
        if (distanceToTarget <= stoppingDistance_) {
            if (clockwise_) {
                currentWayPointNode_ = currentWayPointNode_.GetNextWayPointNode();
            } else {
                currentWayPointNode_ = currentWayPointNode_.GetPreviousWayPointNode();
            }
        } else {
            Vector3 dir = (currentWayPointNode_.transform.position - transform.position).normalized;
            
            switch (movementType_) {
                case MovementType.TRANSFORM: {
                    transform.position += Time.deltaTime * speed_ * dir;
                }
                    break;
                case MovementType.BODY_VELOCITY: {
                    body_.velocity = dir * speed_;
                }
                    break;
                case MovementType.BODY_FORCE: {
                    body_.AddForce(dir);

                    if (body_.velocity.magnitude > speed_) {
                        body_.velocity = body_.velocity.normalized * speed_;
                    }
                }
                    break;
                case MovementType.SEEK_BEHAVIOR: {
                    Vector2 desiredVelocity =
                        (currentWayPointNode_.transform.position - transform.position).normalized * speed_;
                    Vector2 steering = desiredVelocity - body_.velocity;

                    if (steering.magnitude > maxForce_) {
                        steering = steering.normalized * maxForce_;
                    }

                    steering /= body_.mass;

                    body_.AddForce(steering);

                    if (body_.velocity.magnitude > speed_) {
                        body_.velocity = body_.velocity.normalized * speed_;
                    }
                }
                    break;
                case MovementType.PATH_BEHAVIOR: {
                    Vector2 desiredVelocity =
                        (currentWayPointNode_.transform.position - transform.position).normalized * speed_;
                    Vector2 steering = desiredVelocity - body_.velocity;

                    if (steering.magnitude > maxForce_) {
                        steering = steering.normalized * maxForce_;
                    }

                    steering /= body_.mass;

                    body_.AddForce(steering);

                    float maxSpeed = speed_;

                    if (distanceToTarget < arrivalDistance_) {
                        maxSpeed = Mathf.Lerp(0, maxSpeed, distanceToTarget / arrivalDistance_);
                    }

                    if (body_.velocity.magnitude > maxSpeed) {
                        body_.velocity = body_.velocity.normalized * maxSpeed;
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    void OnDrawGizmos() {
//        Gizmos.DrawLine(transform.position, (Vector2)transform.position + body_.velocity.normalized * body_.velocity.magnitude);

        if (currentWayPointNode_ == null) return;

        switch (movementType_) {
            case MovementType.TRANSFORM:
                break;
            case MovementType.BODY_VELOCITY:
                break;
            case MovementType.BODY_FORCE: {
                Vector3 dir = (currentWayPointNode_.transform.position - transform.position).normalized;

                Gizmos.color = Color.red;
                DrawArrow(transform.position, (Vector2) transform.position + body_.velocity);
                Gizmos.color = Color.blue;
                DrawArrow(transform.position, transform.position + dir * speed_);
            }
                break;
            case MovementType.SEEK_BEHAVIOR: {
                Vector3 dir = (currentWayPointNode_.transform.position - transform.position).normalized;

                Gizmos.color = Color.red;
                DrawArrow(transform.position, (Vector2) transform.position + body_.velocity);
                Gizmos.color = new Color(1, 0, 1, 1);
                DrawArrow(transform.position, transform.position + dir * speed_);
                
                Gizmos.color = Color.green;
                DrawArrow((Vector2) transform.position + body_.velocity, transform.position + dir * speed_);
            }
                break;
            case MovementType.PATH_BEHAVIOR: {
                Gizmos.color = new Color(1, 0, 1, 1);
                DrawArrow(transform.position, transform.position + (Vector3)body_.velocity);
                
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(currentWayPointNode_.transform.position, arrivalDistance_);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void DrawArrow(Vector2 start, Vector2 end) {
        Gizmos.DrawLine(start, end);

        Vector2 dir = (end - start).normalized;
        
        Gizmos.DrawLine(end, end - dir * 0.2f + new Vector2(dir.y, - dir.x) * 0.25f);
        Gizmos.DrawLine(end, end - dir * 0.2f + new Vector2(- dir.y, dir.x) * 0.25f);
    }
}
