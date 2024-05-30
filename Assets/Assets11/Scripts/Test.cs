using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    void Start()
    {
        button1.onClick.AddListener(Button1Click);
        button2.onClick.AddListener(Button2Click);
        button3.onClick.AddListener(Button3Click);
        button4.onClick.AddListener(Button4Click);



    }
 



    void Button1Click()
    {
        Debug.Log("1");
        GuideMask.Instance.CreateCircleMaskoffset(button2.gameObject, 0, null);

    }

    void Button2Click()
    {
        Debug.Log("2");
        GuideMask.Instance.CreateCircleMaskoffset(button3.gameObject, 0, null);

    }

    void Button3Click()
    {
        Debug.Log("3");
        GuideMask.Instance.CreateCircleMaskoffset(button4.gameObject, 0, null);

    }

    void Button4Click()
    {
        Debug.Log("4");
        GuideMask.Instance.CloseGuideMask();
    }

}
