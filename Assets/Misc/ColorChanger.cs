using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public Material m1, m2;

    public static ColorChanger Instance;

    private void Awake()
    {
        Instance = this;
    }

    [RemoteEvent("change_color_to_custom")]
    public static void ChangeToCustom(string col)
    {
        if(ColorUtility.TryParseHtmlString(col, out Color color))
        {
            Instance.m1.color = Instance.m2.color = color;
        }
        else
        {
            Debug.Log("Could not parse the given color!");
        }
    }

    [RemoteEvent("change_color_to_green")]
    public static void ChangeToGreen()
    {
        Instance.m1.color = Instance.m2.color = Color.green;
    }

    [RemoteEvent("change_color_to_blue")]
    public static void ChangeToBlue()
    {
        Instance.m1.color = Instance.m2.color = Color.blue;
    }


}
