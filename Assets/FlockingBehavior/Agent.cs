using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    Collider2D collider2D_;
    [SerializeField] float speed_ = 5;

    public Collider2D Collider2D => collider2D_;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Move(Vector2 velocity) {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime * speed_;
    }
}
