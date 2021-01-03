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

    void OnTriggerEnter2D(Collider2D collider) {
        switch (collider.tag) {
            case "Energy":
                mine.EnergizeOnCollision(collider);
                break;

            case "Energy Strike":
                mine.Explode();
                break;
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
