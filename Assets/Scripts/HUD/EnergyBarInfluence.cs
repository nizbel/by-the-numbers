using UnityEngine;
using UnityEngine.UI;

public class EnergyBarInfluence : MonoBehaviour, IPooledObject
{
    [SerializeField]
    [ColorUsage(true,true)]
    [Tooltip("Color to target for positive influence")]
    private Color positiveTargetColor;

    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color to target for positive influence")]
    private Color negativeTargetColor;

    int valuePosition;

    float animationSpeed = 1.75f;

    bool isIntensifying = true;

    Color targetColor;

    Image image;

    ElementsEnum poolType = ElementsEnum.ENERGY_INFLUENCE;

    public void StartInfluence() {
        if (image == null) {
            image = GetComponent<Image>();
            image.material = Instantiate<Material>(image.material);
        }

        // Define influence as positive or negative
        if (isIntensifying) {
            DefineInfluence(valuePosition > 0);
        } else {
            DefineInfluence(valuePosition < 0);
        }

        // Set material variables
        image.material.SetFloat("_DissolveAmount", 1);
        image.material.SetVector("_Seed", new Vector4(Random.Range(0,1f), Random.Range(0, 1f), 0, 0));

        // Transparency = 1
        Color newColor = image.material.color;
        newColor.a = 1f;
        image.material.color = newColor;
    }

    // Update is called once per frame
    void Update()
    {
        float dissolveAmount = image.material.GetFloat("_DissolveAmount");
        // Check if should intensify into filling the energy bar or remove fill
        if (dissolveAmount > 0) {
            dissolveAmount = Mathf.MoveTowards(dissolveAmount, 0, animationSpeed * Time.deltaTime);
            image.material.SetFloat("_DissolveAmount", dissolveAmount);
            
            // Change color (proportional to inverse the dissolution)
            Color newColor = image.material.color;
            newColor = Color.Lerp(newColor, targetColor, 1 - dissolveAmount);
            newColor.a = 1f;
            image.material.color = newColor;

        } else {
            // Remove transparency
            Color newColor = image.material.color;
            newColor.a = Mathf.MoveTowards(newColor.a, 0, animationSpeed * Time.deltaTime);
            image.material.color = newColor;

            // Remove influence
            if (newColor.a == 0) {
                OnObjectDespawn();
            }
        }
    }

    void DefineInfluence(bool isPositive) {

        if (isPositive) {
            image.material.SetColor("_DissolveColorOuter", new Color(0, 4f, 8.5f, 1));
            image.material.SetColor("_DissolveColorMiddle", new Color(0, 3f, 6.5f, 1));
            image.material.SetColor("_DissolveColorInner", new Color(0, 2f, 4.5f, 1));
            targetColor = positiveTargetColor;
        }
        else {
            image.material.SetColor("_DissolveColorOuter", new Color(8.5f, 0, 0, 1));
            image.material.SetColor("_DissolveColorMiddle", new Color(6.5f, 3f, 0, 1));
            image.material.SetColor("_DissolveColorInner", new Color(4.5f, 6f, 0, 1));
            targetColor = negativeTargetColor;
        }
    }

    void OnDestroy() {
        Destroy(image.material);
    }

    public void OnObjectSpawn() {

    } 

    public void OnObjectDespawn() {
        ObjectPool.SharedInstance.ReturnPooledObject(poolType, gameObject, false);
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

    public ElementsEnum GetPoolType() {
        return poolType;
    }
    public void SetPoolType(ElementsEnum poolType) {
        this.poolType = poolType;
    }
}
