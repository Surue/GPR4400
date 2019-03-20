using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalRandom : MonoBehaviour
{
    [SerializeField] float size;
    [SerializeField] int nbPoint;

    List<Vector2> points;
    List<Vector2> points2;

    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vector2>();

        for (int i = 0; i < nbPoint; i++) {
            Vector2 p = new Vector2(Random.Range(0, size), Random.Range(0, size));
            points.Add(p);
        }

        points2 = new List<Vector2>();
        for (int i = 0; i < nbPoint; i++) {
            Vector2 p = new Vector2(RandomNormal(size, 1), RandomNormal(size, 1));
            p = new Vector2(p.x - (size / 2), p.y - (size / 2));
            points2.Add(p);
        }

        isRunning = true;
    }

    float RandomNormal(float mean, float dis)
    {
        float a = Random.Range(0.0f, 1);
        float b = Random.Range(0.0f, 1);
        float c = Mathf.Sqrt(-2 * Mathf.Log(a)) * Mathf.Sin(2 * Mathf.PI * b);

        float r = mean + (dis * c);

        return r;
    }

    void OnDrawGizmos()
    {
        if (!isRunning) return;

        foreach (Vector2 v in points) {
            Gizmos.DrawWireSphere(v, 0.1f);
        }

        Gizmos.color = Color.red;
        foreach(Vector2 v in points2) {
            Gizmos.DrawWireSphere(v, 0.1f);
        }
    }
}
