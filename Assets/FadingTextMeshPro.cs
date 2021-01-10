using TMPro;
using UnityEngine;

public class FadingTextMeshPro : MonoBehaviour
{
    TextMeshPro textMesh;

    float fadingDelay = 0.5f;

    float fadingSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingDelay > 0) {
            fadingDelay -= Time.deltaTime;
        } else {
            Color color = textMesh.color;
            color.a -= Time.deltaTime * fadingSpeed;
            textMesh.color = color;
            if (color.a <= 0) {
                Destroy(this);
            }
        }
    }
}
