using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class CanonController : MonoBehaviour
{

    [SerializeField] GameObject p_CannonBall;
    [SerializeField] Transform target;
    [SerializeField] float force;

    Vector2 _ForceCannon;

    // Start is called before the first frame update
    void Start()
    {
        _ForceCannon = target.position - transform.position;
        _ForceCannon.Normalize();
        _ForceCannon *= force;
    }

    // Update is called once per frame
    void Update()
    {
        _ForceCannon = target.position - transform.position;
        _ForceCannon.Normalize();
        _ForceCannon *= force;

        if (Input.GetButtonDown("Fire1")) {
            GameObject instance = Instantiate(p_CannonBall);

            instance.transform.position = transform.position;
            instance.GetComponent<Rigidbody2D>().AddForce(_ForceCannon);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.position);
    }
}
