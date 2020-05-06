using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunGenerator : MonoBehaviour
{
    enum GunType {
        RIFLE,
        HANDGUN,
        SILENCE
    }
    
    [Serializable]
    struct SpritePair {
        public Sprite sprite;
        public GunType gunType;
    }

    [SerializeField] List<SpritePair> sprites_;

    [SerializeField] GameObject gunPrefab_;

    [SerializeField] List<AudioClip> rifleClip_;
    [SerializeField] List<AudioClip> handgunClip_;
    [SerializeField] List<AudioClip> silenceClip_;
    
    
    // Start is called before the first frame update
    void Start() {

        Generate();
    }

    void Generate() {

        Gun gun = Instantiate(gunPrefab_, Vector3.zero, Quaternion.identity).GetComponent<Gun>();
        
        int index = Random.Range(0, sprites_.Count);

        gun.SetSprite(sprites_[index].sprite);

        switch (sprites_[index].gunType) {
            case GunType.RIFLE:
                gun.SetClip(rifleClip_[Random.Range(0, rifleClip_.Count)]);
                break;
            case GunType.HANDGUN:
                gun.SetClip(handgunClip_[Random.Range(0, handgunClip_.Count)]);
                break;
            case GunType.SILENCE:
                gun.SetClip(silenceClip_[Random.Range(0, silenceClip_.Count)]);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Generate();
        }
    }
}
