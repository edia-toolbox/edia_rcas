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

    [RemoteEvent("change_color_to_red")]
    public static void ChangeToRed()
    {
        Instance.m1.color = Instance.m2.color = Color.red;
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
