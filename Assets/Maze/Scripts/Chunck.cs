using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunck : MonoBehaviour
{

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if (up) {
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 2.5f), 1);
        }

        if(down) {
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - 2.5f), 1);
        }

        if(left) {
            Gizmos.DrawWireSphere(new Vector3(transform.position.x - 2.5f, transform.position.y), 1);
        }

        if(right) {
            Gizmos.DrawWireSphere(new Vector3(transform.position.x + 2.5f, transform.position.y), 1);
        }
    }
}
