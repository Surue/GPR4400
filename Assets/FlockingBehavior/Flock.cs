using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Flock : MonoBehaviour {

    [SerializeField] GameObject prefabAgent_;
    [SerializeField] int numberToSpawn_ = 300;
    [SerializeField] float densitySpawn_ = 0.5f;
    
    List<Agent> agents_;

    [SerializeField] float neighborDetection_ = 1;
    [SerializeField] SteeringBehavior steeringBehavior_;
    
    // Start is called before the first frame update
    void Start()
    {
        agents_ = new List<Agent>();
        
        for (int i = 0; i < numberToSpawn_; i++) {
            Agent instance = Instantiate(prefabAgent_, densitySpawn_ * numberToSpawn_ * Random.insideUnitCircle, Quaternion.identity).GetComponent<Agent>();
            agents_.Add(instance);
            instance.name = "Boid " + i;
            instance.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Agent agent in agents_) {
            List<Transform> neighbor = GetNeighbor(agent);

            agent.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.black, neighbor.Count / 24.0f); //THIS IF FOR DEBUG PURPUSE NEVER DO THAT AGAIN
            
            agent.Move(steeringBehavior_.CalculateMove(agent, neighbor));
        }
    }

    List<Transform> GetNeighbor(Agent boid) {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(boid.transform.position, neighborDetection_, Vector2.zero);
        
        List<Transform> neighbor = new List<Transform>();

        foreach (RaycastHit2D hit in hits) {
            if (hit.collider != boid.Collider2D) {
                neighbor.Add(hit.transform);
            }
        }

        return neighbor;
    }
}
