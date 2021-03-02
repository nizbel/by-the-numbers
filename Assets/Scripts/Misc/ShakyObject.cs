using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakyObject : MonoBehaviour {
    float startTime = 0;

    float duration = 100;

    bool staticRotation = false;

    [SerializeField]
    float speed = 75f; //how fast it shakes
    [SerializeField]
    float amount = 0.02f; //how much it shakes

    public float Duration { get => duration; set => duration = value; }

    // Start is called before the first frame update
    void Start() {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Shake();
        if (Time.time > startTime + duration) {
            Destroy(this.gameObject);
        }
    }

    private void Shake() {
        float amountX = Mathf.Sin(Time.time * speed) * amount;
        float amountY = Mathf.Sin(Mathf.PI / 2 + Time.time * speed) * amount;

        transform.position += new Vector3(amountX, amountY, 0);
        if (!staticRotation) { 
            transform.Rotate(0, 0, Mathf.Sin(Time.time * speed / 3) * amount * 100);
        }
    }
}
