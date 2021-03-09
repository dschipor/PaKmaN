using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour{

    public int points = 100;//how many points this collectable is worth
    public AudioClip collectSound;

    private PlayerController pc;

    // Start is called before the first frame update
    void Start() {
        pc = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            collected();
        }
    }

    protected virtual void collected()
    {
        pc.addPoints(points);
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        gameObject.SetActive(false);
    }
}
