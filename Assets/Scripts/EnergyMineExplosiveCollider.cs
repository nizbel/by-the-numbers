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

    void OnCollisionEnter2D(Collision2D col) { 
        switch (col.collider.tag) {
            case "Energy":
                mine.EnergizeOnCollision(col.collider);
                break;

            //case "Mine Force Field":
            //case "Player":
            //    break;

            default:
                mine.Explode();
                break;
        }
    }
}
