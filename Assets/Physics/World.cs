using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour {
    List<Body> bodies_;
    
    void Start() {
        bodies_ = FindObjectsOfType<Body>().ToList();
    }

    void FixedUpdate()
    {
        for (int i = 0; i < bodies_.Count; i++) {

            Body body1 = bodies_[i];
            
            for (int j = 0; j < bodies_.Count; j++) {
                Body body2 = bodies_[j];

                if (body1.Overlap(body2)) {
                    Debug.DrawLine(body1.transform.position, body2.transform.position);
                }    
            } 
        }
    }
}
