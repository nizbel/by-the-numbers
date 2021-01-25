using UnityEngine;
using UnityEngine.UI;

public class EnergyBarInfluence : MonoBehaviour
{
    [SerializeField]
    [ColorUsage(true,true)]
    [Tooltip("Color to target for positive influence")]
    private Color positiveTargetColor;

    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color to target for positive influence")]
    private Color negativeTargetColor;

    [SerializeField]
    int valuePosition;

    [SerializeField]
    float animationSpeed = 0.8f;

    [SerializeField]
    bool isIntensifying = true;

    [SerializeField]
    Color targetColor;

    Image image;

    void Start() {
        image = GetComponent<Image>();
        image.material = Instantiate<Material>(image.material);

        if (isIntensifying) {
            image.material.SetFloat("_DissolveAmount", 1);
            // Define target color
            if (valuePosition > 0) {
                targetColor = positiveTargetColor;
            } else {
                targetColor = negativeTargetColor;
            }

            Color newColor = image.color;
            newColor.a = 0;
            image.color = newColor;
        } else {
            image.material.SetFloat("_DissolveAmount", 0);
            // Define target color
            if (valuePosition > 0) {
                targetColor = negativeTargetColor;
            }
            else {
                targetColor = positiveTargetColor;
            }

            Color newColor = image.color;
            newColor.a = 1;
            image.color = newColor;
        }
        //Debug.Break();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if should intensify into filling the energy bar or remove fill
        if (isIntensifying) {
            float dissolveAmount = Mathf.MoveTowards(image.material.GetFloat("_DissolveAmount"), 0, animationSpeed * Time.deltaTime);
            image.material.SetFloat("_DissolveAmount", dissolveAmount);
            
            // Change color (proportional to inverse the dissolution)
            Color newColor = image.color;
            newColor = Color.Lerp(newColor, targetColor, 1 - dissolveAmount)*3.5f;
            newColor.a = 1 - dissolveAmount;
            image.color = newColor;

            // Remove influence
            if (dissolveAmount == 0) {
                Destroy(gameObject);
            }
        } else {
            float dissolveAmount = Mathf.MoveTowards(image.material.GetFloat("_DissolveAmount"), 1, animationSpeed * Time.deltaTime);
            image.material.SetFloat("_DissolveAmount", dissolveAmount);

            // Change color (proportional to the dissolution)
            Color newColor = image.color;
            newColor = Color.Lerp(newColor, targetColor, dissolveAmount);
            newColor.a = 1 - dissolveAmount;
            image.color = newColor;

            // Remove influence
            if (dissolveAmount == 1) {
                Destroy(gameObject);
            }
        }
    }

    /*
     * Getters and Setters
     */
    public bool GetIsIntensifying() {
        return isIntensifying;
    }

    public void SetIsIntensifying(bool isIntensifying) {
        this.isIntensifying = isIntensifying;
    }

    public int GetValuePosition() {
        return valuePosition;
    }

    public void SetValuePosition(int newValue) {
        this.valuePosition = newValue;
    }
}
