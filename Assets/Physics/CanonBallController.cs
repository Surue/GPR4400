using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBallController : MonoBehaviour
{
    [SerializeField] float elapsedTime;

    double _Timer;

    List<Vector2> _GizmoPoints;

    // Start is called before the first frame update
    void Start() {
        _GizmoPoints = new List<Vector2>();

        StartCoroutine(Draw());
    }

    // Update is called once per frame
    void Update()
    {
        //_Timer += Time.deltaTime;

        //if (_Timer > elapsedTime) {
        //    _GizmoPoints.Add(transform.position);
        //    _Timer = 0;
        //}
    }

    IEnumerator Draw()
    {
        while (true) {
            _GizmoPoints.Add(transform.position);
            yield return new WaitForSeconds(elapsedTime);
        }
    }

    void OnDrawGizmos()
    {
        foreach (Vector2 vector2 in _GizmoPoints) {
            Gizmos.color = Color.red;
            //Gizmos.DrawSphere(vector2, 0.5f);
        }
    }
}
