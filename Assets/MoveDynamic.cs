using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveDynamic : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;

    Rigidbody2D body;

    Rigidbody2D playerBody;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        playerBody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>();

        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 target = playerBody.transform.position  + (Vector3)playerBody.velocity.normalized * Time.fixedDeltaTime;

        Vector2 force = target - (Vector2)transform.position;

        force = force.normalized * acceleration;

        body.AddForce(force);

        if (body.velocity.magnitude > maxSpeed) {
            body.velocity = body.velocity.normalized * maxSpeed;
        }
    }

    void OnDrawGizmos()
    {
        if(!isRunning) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)body.velocity);
    }
}
