using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMineExplosiveCollider : MonoBehaviour
{
    [SerializeField]
    EnergyMine mine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Energy") {
            mine.EnergizeOnCollision(collider);
        }
    }

    void OnCollisionEnter2D(Collision2D col) { 
        switch (col.collider.tag) {
            default:
                mine.Explode();
                break;
        }
    }
}
