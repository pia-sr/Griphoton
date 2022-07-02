using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassFill : MonoBehaviour
{
    public Image GlassImage;
    public bool done;

    /// <summary>
    /// Sets the health bar value
    /// </summary>
    /// <param name="value">should be between 0 to 1</param>
    public void SetGlassValue(float value)
    {
        GlassImage.fillAmount = value;
        
    }

    public float GetGlassValue()
    {
        return GlassImage.fillAmount;
    }

    

    /// <summary>
    /// Initialize the variable
    /// </summary>
    private void Start()
    {
        //GlassImage = GetComponent<Image>();
        done = false;
    }

    public IEnumerator FillUp(float value, float steps)
    {
        while(GetGlassValue() != value)
        {
            if(GetGlassValue() + steps > value)
            {
                float dif = value - GetGlassValue();
                SetGlassValue(GetGlassValue() + dif);
            }
            else
            {
                SetGlassValue(GetGlassValue() + steps);
            }
            yield return new WaitForSeconds(0.1f);
        }
        done = true;

    }

    public IEnumerator DryUp(float value, float steps)
    {
        while(GetGlassValue() != value)
        {
            if(GetGlassValue() - steps < value)
            {
                float dif = GetGlassValue() - value;
                SetGlassValue(GetGlassValue() - dif);
            }
            else
            {
                SetGlassValue(GetGlassValue() - steps);
            }
            yield return new WaitForSeconds(0.1f);
        }
        done = true;

    }

    public void Rotate270()
    {
        GlassImage.fillMethod = Image.FillMethod.Radial90;
        GlassImage.fillOrigin = (int)Image.Origin90.TopRight;
    }

    public void Rotate90()
    {
        GlassImage.fillMethod = Image.FillMethod.Radial90;
        GlassImage.fillOrigin = (int)Image.Origin90.BottomRight;

    }
    public void RotateOriginal()
    {

        GlassImage.fillMethod = Image.FillMethod.Vertical;
        GlassImage.fillOrigin = (int)Image.OriginVertical.Bottom;
    }
}
