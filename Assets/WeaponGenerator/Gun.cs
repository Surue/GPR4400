using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gun : MonoBehaviour {
    SpriteRenderer spriteRenderer_;
    AudioSource audioSource_;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer_ = GetComponent<SpriteRenderer>();

        audioSource_ = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0) {
            audioSource_.Play();
        }
    }

    public void SetSprite(Sprite sprite) {
        spriteRenderer_ = GetComponent<SpriteRenderer>();
        spriteRenderer_.sprite = sprite;
    }
    
    public void SetClip(AudioClip clip) {
        audioSource_ = GetComponent<AudioSource>();
        audioSource_.clip = clip;
    }
}
