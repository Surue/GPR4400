using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;

    Rigidbody2D body;

    [System.Serializable]
    enum MoveType
    {
        DYNAMIC,
        KINEMATIC
    }

    [SerializeField]MoveType moveType = MoveType.DYNAMIC;

    Vector2 controlMovement;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        controlMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void FixedUpdate()
    {
        switch (moveType) {
            case MoveType.DYNAMIC:
                controlMovement = controlMovement.normalized * acceleration;

                body.AddForce(controlMovement);

                if (body.velocity.magnitude > maxSpeed) {
                    body.velocity = body.velocity.normalized * maxSpeed;
                }
                break;
            case MoveType.KINEMATIC:
                controlMovement = controlMovement.normalized * maxSpeed;

                body.velocity = controlMovement;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void OnDrawGizmos()
    {
        switch (moveType) {
            case MoveType.DYNAMIC:
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)body.velocity);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)controlMovement);

                break;
            case MoveType.KINEMATIC:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
