using UnityEngine;

public class FollowerSpawner : MonoBehaviour {

    [SerializeField] GameObject prefabFollower_;

    [SerializeField] int nbFollower_;
    [SerializeField] Vector2 size_;
    [SerializeField] Vector2 offset_;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < nbFollower_; i++) {
            Instantiate(prefabFollower_, 
                new Vector3(
                    offset_.x + Random.Range(-size_.x * 0.5f, size_.x * 0.5f), 
                    offset_.y + Random.Range(-size_.y * 0.5f, size_.y * 0.5f), 
                    0), 
                Quaternion.identity);
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(new Vector3(offset_.x, offset_.y, 0), size_);
    }
}
