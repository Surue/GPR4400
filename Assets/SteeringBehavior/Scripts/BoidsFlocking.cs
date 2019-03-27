using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoidsFlocking : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField] float maxSpeed;
    [SerializeField] float maxForce;
    [SerializeField] float viewRadius;

    [Header("weight")]
    [SerializeField] float fleeWeight = 1;
    [SerializeField] float separateWeight = 1;
    [SerializeField] float cohesionWeight = 1;
    [SerializeField] float alignWeight = 1;

    Rigidbody2D body;

    SpriteRenderer spriteRenderer;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        body.velocity = new Vector2(Random.Range(0f, 1) * maxSpeed, Random.Range(0f, 1) * maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, viewRadius);

        Vector2 flee = Flee(colliders);
        Vector2 separate = Separate(colliders);
        Vector2 cohesion = Cohesion(colliders);
        Vector2 align = Align(colliders);

        flee *= fleeWeight;
        separate *= separateWeight;
        cohesion *= cohesionWeight;
        align *= alignWeight;

        body.AddForce(flee);
        body.AddForce(separate);
        body.AddForce(cohesion);
        body.AddForce(align);

        if(body.velocity.magnitude > maxForce) {
            body.velocity = body.velocity.normalized * maxForce;
        }

        body.velocity = new Vector2(body.velocity.x + body.velocity.x * (Random.Range(0f, 1) * 0.5f ), body.velocity.y + body.velocity.y * (Random.Range(0f, 1) * 0.5f));

        // Update sprite
        Vector2 dir = body.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteRenderer.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        spriteRenderer.transform.eulerAngles = new Vector3(0, 0, spriteRenderer.transform.eulerAngles.z - 90);
    }

    Vector2 Flee(Collider2D[] colliders)
    {
        Vector2 fleeForce = Vector2.zero;

        foreach(Collider2D col in colliders) {
            if(col.gameObject.CompareTag("Repulsor")) {
                Vector2 seekVelocity = transform.position - col.transform.position;
                seekVelocity = seekVelocity.normalized * maxSpeed;
                fleeForce += seekVelocity - body.velocity;
            }
        }

        return fleeForce;
    }

    //Avoidance
    Vector2 Separate(Collider2D[] colliders)
    {
        Vector2 separateForce = Vector2.zero;

        foreach(Collider2D col in colliders) {
            if(col.gameObject.CompareTag("Boids")) {
                Vector2 seekVelocity = transform.position - col.transform.position;
                //seekVelocity = seekVelocity.normalized * maxSpeed;
                seekVelocity = seekVelocity.normalized * Mathf.Lerp(0, maxSpeed, seekVelocity.magnitude / viewRadius);
                separateForce += seekVelocity - body.velocity;
            }
        }

        return separateForce;
    }

    Vector2 Cohesion(Collider2D[] colliders)
    {
        Vector2 cohesionForce;

        Vector3 centralPoint = Vector2.zero;
        int nbNeighbours = 0;

        foreach(Collider2D col in colliders) {
            if(col.gameObject.CompareTag("Boids")) {
                nbNeighbours++;
                centralPoint += col.transform.position;
            }
        }

        if (nbNeighbours > 0) {
            centralPoint /= nbNeighbours;
        }

        cohesionForce = centralPoint - transform.position;
        cohesionForce = cohesionForce.normalized * maxSpeed;

        Debug.DrawLine(transform.position, centralPoint);

        return cohesionForce;
    }

    Vector2 Align(Collider2D[] colliders)
    {
        Vector2 aligneForce = Vector2.zero;

        foreach(Collider2D col in colliders) {
            if(col.gameObject.CompareTag("Boids")) {
                Vector2 seekVelocity = col.GetComponent<Rigidbody2D>().velocity;
                aligneForce += seekVelocity - body.velocity;
            }
        }

        return aligneForce;
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
