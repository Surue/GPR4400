using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetFlowField : MonoBehaviour
{
    Node node;

    Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        FlowFieldMap flowFieldMap = FindObjectOfType<FlowFieldMap>();
        node = flowFieldMap.GetNode(transform.position);

        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance((Vector2)transform.position, node.pos) < 0.5f) {
            node = node.cameFrom;
        }

        if (node == null) {
            Destroy(this);
            body.velocity = Vector2.zero;
            return;
        }

        body.velocity = node.pos - (Vector2)transform.position;
        body.velocity = body.velocity.normalized * 2f;
    }
}
