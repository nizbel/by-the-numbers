using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingStar : MonoBehaviour
{
    float fadingFactor = 2.5f;

    float targetScaleX;

    ShinyStar shinyStar = null;

    private void Start() {
        targetScaleX = transform.localScale.x/50;

        shinyStar = GetComponent<ShinyStar>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale/fadingFactor, Time.deltaTime);

        // TODO check if shiny star is not stoping to shine
        if (shinyStar != null) {
            shinyStar.SetDefaultScale(Vector3.Lerp(shinyStar.GetDefaultScale(), shinyStar.GetDefaultScale() / fadingFactor, Time.deltaTime));
        }

        if (transform.localScale.x <= targetScaleX) {
            Destroy(gameObject);
        }
    }
}
