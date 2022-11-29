/*
 * GlassFill.cs
 * 
 * Author: Pia Schroeter
 * 
 * Copyright (c) 2022 Pia Schroeter
 * All rights reserved
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassFill : MonoBehaviour
{
    public Image GlassImage;
    public bool done;

    //sets done as false at the beginning 
    private void Start()
    {
        done = false;
    }

    //setter for the amount of water in the glass
    public void SetGlassValue(float value)
    {
        GlassImage.fillAmount = value;
        
    }

    //getter for the amount of water in the glass
    public float GetGlassValue()
    {
        return GlassImage.fillAmount;
    }

    //Function to fill the glass with water
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

    //Function to remove the water in the glass
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

    //function to rotate the glass with the water to the left
    public void Rotate270()
    {
        GlassImage.fillMethod = Image.FillMethod.Radial90;
        GlassImage.fillOrigin = (int)Image.Origin90.TopRight;
        GlassImage.fillClockwise = true;
    }

    //function to rotate the glass with the water to the right
    public void Rotate90()
    {
        GlassImage.fillMethod = Image.FillMethod.Radial90;
        GlassImage.fillOrigin = (int)Image.Origin90.TopLeft;
        GlassImage.fillClockwise = false;

    }

    //Function to rotate the glass to its original orientation 
    public void RotateOriginal()
    {

        GlassImage.fillMethod = Image.FillMethod.Vertical;
        GlassImage.fillOrigin = (int)Image.OriginVertical.Bottom;
        GlassImage.fillClockwise = true;
    }
}
