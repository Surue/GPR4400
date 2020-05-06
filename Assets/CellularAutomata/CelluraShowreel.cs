using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelluraShowreel : MonoBehaviour {

    [SerializeField] List<GameOfLifeControls> gameOfLifeControlses_;

    int index = 0;

    void Start() {
        gameOfLifeControlses_[index].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (index < gameOfLifeControlses_.Count) {
            if (!gameOfLifeControlses_[index].IsRunning()) {
                gameOfLifeControlses_[index].gameObject.SetActive(false);
                index++;
                gameOfLifeControlses_[index].gameObject.SetActive(true);
            }
        }
    }
}
