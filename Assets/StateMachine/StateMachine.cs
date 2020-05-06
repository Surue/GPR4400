using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StateMachine : MonoBehaviour {
    [SerializeField] bool playerSeen;
    [SerializeField] bool playerHeard;

    enum State {
        IDLE,
        PATROL,
        SEARCH,
        PURSUE
    }

    State state_ = State.IDLE;

    Vector3 target_;
    Vector3 guessPosition_;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0) {
            playerHeard = true;
            guessPosition_ = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            guessPosition_.z = 0;
        }
        
        switch (state_) {
            case State.IDLE:
                state_ = State.PATROL;
                break;
            case State.PATROL:
                if (playerHeard) {
                    state_ = State.SEARCH;
                    playerHeard = false;
                } else {
                    if (Vector3.Distance(transform.position, target_) < 0.1f) {
                        target_ = Random.insideUnitCircle * 4;
                    } else {
                        Vector3 dir = target_ - transform.position;
                        transform.position += dir.normalized * 5 * Time.deltaTime;
                    }
                }
                break;
            case State.SEARCH:
                if (playerSeen) {
                    state_ = State.PURSUE;
                } else {
                    if (Vector3.Distance(transform.position, guessPosition_) < 0.1f) {
                        state_ = State.PATROL;
                    } else {
                        Vector3 dir = guessPosition_ - transform.position;
                        transform.position += dir.normalized * 7.5f * Time.deltaTime;
                    }
                }
                break;
            case State.PURSUE:
                if (!playerSeen) {
                    state_ = State.SEARCH;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }    
    }

    void OnDrawGizmos() {
        //Draw self
        switch (state_) {
            case State.IDLE:
            case State.PATROL:
                Gizmos.color = Color.white;
                break;
            case State.SEARCH:
            case State.PURSUE:
                Gizmos.color = Color.red;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Gizmos.DrawCube(transform.position, Vector3.one);
        
        //DrawTarget
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(target_, 0.5f);
        
        //DrawTarget
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(guessPosition_, 0.5f);
    }
}
