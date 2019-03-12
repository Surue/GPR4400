using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSeed:MonoBehaviour {

    public float forcedSeed = 0;
    public float curSeed;

    public static float SEED_PROCEDURAL_GENERATION = 12;

    public static float SEED_OTHER = 1;

    //Value for Linear congruential generator NEVER CHANGE THEM!!!
    const long multiplier = 1103515245;
    const long incrementer = 12345;
    static readonly long modulus = (long)Mathf.Pow(2f, 31f);

    // Use this for initialization
    void Start() {
        if(Math.Abs(forcedSeed) < 0.1) {
            SEED_PROCEDURAL_GENERATION = Random.value;
            curSeed = SEED_PROCEDURAL_GENERATION;
        } else {
            SEED_PROCEDURAL_GENERATION = forcedSeed;
        }
    }

    // Use for everything except prcedural generation
    public static float GetRandom() {

        SEED_OTHER = (multiplier * SEED_OTHER + incrementer) % modulus;

        float randomValue = SEED_OTHER / (float)modulus;

        return randomValue;
    }

    // Use only for procedural generation
    public static float GetValue() {

        SEED_PROCEDURAL_GENERATION = (multiplier * SEED_PROCEDURAL_GENERATION + incrementer) % modulus;

        float randomValue = SEED_PROCEDURAL_GENERATION / (float)modulus;

        return randomValue;
    }
}
