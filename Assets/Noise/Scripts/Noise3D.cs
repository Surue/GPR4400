using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise3D : MonoBehaviour
{
    [SerializeField] int mapSize = 10;
    [SerializeField] int seed = 0;
    [SerializeField] float scale = 37;
    [SerializeField] int octave = 4;
    [SerializeField] float persistance = 2.3f;
    [SerializeField] float lacunarity = 4;
    [SerializeField] float height = 2;

    float[,] map;

    bool isRunning = false;

    Vector2 offset = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        isRunning = true;

        Generate();
    }

    public void Generate()
    {
        map = new float[mapSize, mapSize];
        map = Noise.GenerateNoiseMap(mapSize, mapSize, seed, scale, octave, persistance, lacunarity, offset);
    }

    void Update()
    {
        offset += Vector2.one * Time.deltaTime;
        Generate();
    }

    void OnDrawGizmos()
    {
        if (isRunning) {
            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {
                    Gizmos.color = new Color(map[x, y], map[x, y], map[x, y]);
                    Gizmos.DrawCube(new Vector3(x, map[x, y] * height, y), Vector3.one);
                }
            }
        }
    }
}

public static class Noise {

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves,
        float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octavesOffset = new Vector2[octaves];
        for(int i = 0;i < octaves;i++) {
            float offsetX = prng.Next(-10000, 10000) + offset.x;
            float offsetY = prng.Next(-10000, 10000) + offset.y;
            octavesOffset[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth * 0.5f;
        float halfHeight = mapHeight * 0.5f;

        for(int y = 0;y < mapHeight;y++) {
            for(int x = 0;x < mapWidth;x++) {

                float amplitude = 1;
                float frequency = 1;

                float noiseHeight = 0;

                for(int i = 0;i < octaves;i++) {
                    float sampleX = (x - halfWidth) / scale * frequency + octavesOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octavesOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Normalize map
        for(int y = 0;y < mapHeight;y++) {
            for(int x = 0;x < mapWidth;x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}

