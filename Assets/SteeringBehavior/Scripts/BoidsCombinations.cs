using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoidsCombinations : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField] float maxSpeed;
    [SerializeField] float maxForce;
    [SerializeField] float viewRadius;

    [SerializeField] float separateRadius;

    Rigidbody2D body;
    Vector2 desiredVelocity;

    SpriteRenderer spriteRenderer;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        isRunning = true;
    }

    // Update is called once per frame
    void Update() {
        desiredVelocity = Vector2.zero;

        Vector2 seekForce = Seek();
        Vector2 separateForce = Separate();

        seekForce *= 1f;
        separateForce *= 1f;

        body.AddForce(seekForce);
        body.AddForce(separateForce);

        //Update sprite
        Vector2 dir = body.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteRenderer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        spriteRenderer.transform.eulerAngles = new Vector3(0, 0, spriteRenderer.transform.eulerAngles.z - 90);
    }

    Vector2 Seek() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, viewRadius);

        Vector2 seekForce = Vector2.zero;

        float minDistance = float.MaxValue;

        Vector2 minPos = Vector2.zero;

        foreach(Collider2D col in colliders) {
            if (!col.gameObject.CompareTag("Attractor")) {
                continue;
            }

            Vector2 seekVelocity = col.transform.position - transform.position;
            if (!(seekVelocity.magnitude < minDistance)) {
                continue;
            }

            minDistance = seekVelocity.magnitude;

            seekVelocity = seekVelocity.normalized * maxSpeed;
            seekForce += seekVelocity - body.velocity;

            minPos = col.transform.position;
        }

        Debug.DrawLine(transform.position, minPos);

        if(seekForce.magnitude > maxForce) {
            seekForce = seekForce.normalized * maxForce;
        }

        return seekForce;
    }

    Vector2 Separate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, separateRadius);

        Vector2 separateForce = Vector2.zero;

        foreach(Collider2D col in colliders) {
            if(col.gameObject.CompareTag("Repulsor")) {
                Vector2 seekVelocity = transform.position - col.transform.position;
                seekVelocity = seekVelocity.normalized * maxSpeed;
                separateForce += seekVelocity - body.velocity;
            }
        }

        if(separateForce.magnitude > maxForce) {
            separateForce = separateForce.normalized * maxForce;
        }

        return separateForce;
    }

    void OnDrawGizmos() {
        if(!isRunning) return;

        Vector3 position = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + (Vector3)body.velocity);
    }
}
