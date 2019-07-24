using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planet : MonoBehaviour
{
    [SerializeField] Rigidbody2D sun;

    Rigidbody2D _Body;

    float G = 6.574f * Mathf.Pow(10, -10);

    // Start is called before the first frame update
    void Start()
    {
        _Body = GetComponent<Rigidbody2D>();

        _Body.velocity = new Vector2(Random.Range(0, 5), Random.Range(0, 5));
    }

    void FixedUpdate()
    {
        float force = G * (_Body.mass * sun.mass) / Vector2.Distance(transform.position, sun.position);

        _Body.velocity = _Body.velocity + (force / _Body.mass * (sun.position - (Vector2)transform.position));
    }
}
