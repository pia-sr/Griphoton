using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //source: http://gyanendushekhar.com/2019/11/17/create-health-bar-unity-3d/
    private Image HealthBarImage;

    /// <summary>
    /// Sets the health bar value
    /// </summary>
    /// <param name="value">should be between 0 to 1</param>
    public void SetHealthBarValue(float value)
    {
        this.gameObject.GetComponent<Image>().fillAmount = value;
        if (this.gameObject.GetComponent<Image>().fillAmount < 0.3f)
        {
            SetHealthBarColor(Color.red);
        }
        else if (this.gameObject.GetComponent<Image>().fillAmount <= 0.5f)
        {
            SetHealthBarColor(Color.yellow);
        }
        else
        {
            SetHealthBarColor(Color.green);
        }
    }

    public float GetHealthBarValue()
    {
        return this.gameObject.GetComponent<Image>().fillAmount;
    }

    /// <summary>
    /// Sets the health bar color
    /// </summary>
    /// <param name="healthColor">Color </param>
    public void SetHealthBarColor(Color healthColor)
    {
        this.gameObject.GetComponent<Image>().color = healthColor;
    }

    /// <summary>
    /// Initialize the variable
    /// </summary>
    private void Start()
    {
        HealthBarImage = GetComponent<Image>();
    }
}
