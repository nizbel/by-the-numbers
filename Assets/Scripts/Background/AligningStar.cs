using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AligningStar : MonoBehaviour
{
    Vector3 destination;

    Color targetColor;

    float speed;

    float initialScale;

    SpriteRenderer sprite;

    float direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(0.12f, 0.15f);
        if (GameController.RollChance(50)) {
            direction *= -1;
        }
        initialScale = transform.localScale.x;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difference = destination - transform.position;
        transform.position = Vector3.Lerp(transform.position, destination + new Vector3(-difference.y, difference.x, 0) * direction, Time.deltaTime * speed);

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * initialScale * 1.5f, Time.deltaTime * speed / 5);

        sprite.color = Color.Lerp(sprite.color, targetColor, Time.deltaTime * speed / 5);
    }

    public void SetDestination(Vector3 destination) {
        this.destination = destination;
    }

    public void SetTargetColor(Color targetColor) {
        this.targetColor = targetColor;
    }
}
