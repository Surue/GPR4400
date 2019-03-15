using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int cellNbX = 2;
    [SerializeField] int cellNbY = 7;

    int sizeX = 5;
    int sizeY = 5;

    struct Cell
    {
        float posX;
        float posY;

        Chunck chunk;
    }

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
        for (int x = 0; x < cellNbX; x++) {
            for (int y = 0; y < cellNbY; y++) {
                Gizmos.DrawWireCube(new Vector3(x * sizeX + sizeX / 2f, y * sizeY + sizeY / 2f), new Vector3(sizeX, sizeY));
            }
        }
    }
}
