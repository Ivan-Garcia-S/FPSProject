using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hit2 : MonoBehaviour
{
    Image marker;
    Color one = Color.white;
    Color two = Color.red;
    void Start()
    {
        marker = gameObject.GetComponent<Image>();
        marker.color = one;
    }

    void Update()
    {
        InvokeRepeating("SetAllMarkers", 0f, 2f);
    }
   
    private void SetAllMarkers(){
        
        Color temp = one;
        marker.color = Color.Lerp(one,two, 1f);
        one = two;
        two = temp;
    }
}