using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadingUIElement : MonoBehaviour {
    private const int IMAGE_TYPE = 1;
    private const int TEXT_TYPE = 2;
    private const int TEXT_MESH_PRO_TYPE = 3;

    Image image = null;
    Text text = null;
    TextMeshPro textMeshPro = null;

    // Define which element to fade
    int type;

    // Start is called before the first frame update
    void Start() {
        if (GetComponent<Image>() != null) {
            image = GetComponent<Image>();
            type = IMAGE_TYPE;
        } else if (GetComponent<Text>() != null) {
            text = GetComponent<Text>();
            type = TEXT_TYPE;
        } else if (GetComponent<TextMeshPro>() != null) {
            textMeshPro = GetComponent<TextMeshPro>();
            type = TEXT_MESH_PRO_TYPE;
        }

        // Always start invisible
        SetAlpha(0);
    }

    public void SetAlpha(float alpha) { 
        Color color;
        switch (type) {
            case IMAGE_TYPE:
                color = image.color;
                color.a = alpha;
                image.color = color;
                break;
            case TEXT_TYPE:
                color = text.color;
                color.a = alpha;
                text.color = color;
                break;
            case TEXT_MESH_PRO_TYPE:
                color = textMeshPro.color;
                color.a = alpha;
                textMeshPro.color = color;
                break;
        }
    }
}
