using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveKinematic : MonoBehaviour
{
    [SerializeField] float speed;

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
        Vector2 target = playerBody.transform.position + (Vector3)playerBody.velocity.normalized * Time.fixedDeltaTime;

        body.velocity = target - (Vector2)transform.position;
        body.velocity = body.velocity.normalized * speed;
    }

    void OnDrawGizmos()
    {
        if (!isRunning) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)body.velocity);
    }
}
