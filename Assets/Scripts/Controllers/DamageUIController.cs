using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUIController : MonoBehaviour
{
    public Text Text;
    public Image ArrowImage;
    private void Start()
    {
        Destroy(gameObject, 1f);
    }
    private void Update()
    {
        transform.position += Vector3.up * Time.deltaTime;
    }
    public void Initialize(float value, Element element, TypeEffectiveness effectiveness)
    {
        Text.text = value.ToString();

        // set text color
        switch (element)
        {
            case Element.Fire:
                Text.color = Color.red;
                break;
            case Element.Ice:
                Text.color = Color.blue;
                break;
            case Element.Wind:
                Text.color = Color.green;
                break;
            default:
                break;
        }

        // set arrow
        switch (effectiveness)
        {
            case TypeEffectiveness.Neutral:
                break;
            case TypeEffectiveness.Weakness:
                ArrowImage.gameObject.SetActive(true);
                ArrowImage.color = Color.green;
                break;
            case TypeEffectiveness.Resistance:
                ArrowImage.gameObject.SetActive(true);
                ArrowImage.color = Color.red;
                ArrowImage.transform.Rotate(180.0f, 0.0f, 0.0f, Space.Self); // invert arrow
                break;
            default:
                break;
        }
    }
}
