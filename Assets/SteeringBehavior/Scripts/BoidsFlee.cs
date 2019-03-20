using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoidsFlee : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField] float maxSpeed;
    [SerializeField] float maxForce;
    [SerializeField] float viewRadius;

    Rigidbody2D body;
    Vector2 desiredVelocity;

    SpriteRenderer spriteRenderer;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        desiredVelocity = Vector2.zero;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, viewRadius);
        
        foreach (Collider2D col in colliders) {
            if (col.gameObject.CompareTag("Repulsor")) {
                Vector2 seekVelocity = transform.position - col.transform.position;
                seekVelocity = seekVelocity.normalized * maxSpeed;
                desiredVelocity += seekVelocity - body.velocity;
            }
        }

        if (desiredVelocity.magnitude > maxForce) {
            desiredVelocity = desiredVelocity.normalized * maxForce;
        }
        body.AddForce(desiredVelocity);

        //Update sprite
        Vector2 dir = body.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteRenderer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        spriteRenderer.transform.eulerAngles = new Vector3(0, 0, spriteRenderer.transform.eulerAngles.z - 90);
    }

    void OnDrawGizmos()
    {
        if (!isRunning) return;

        Vector3 position = transform.position;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, viewRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(position, position + (Vector3)body.velocity);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(position, position + (Vector3)desiredVelocity);
    }
}
