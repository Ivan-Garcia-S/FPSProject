using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
    //public GameObject hitmarker;
    private Image[] markers;
    public Image img;
    public Color white = new Color32(255,255,255,255);
    public Color clear = new Color32(255,255,255,0);
    //private bool justHit = false;
    private float timeLeft = 0;
    private float markerLifespan = 2.25f;
    private bool interruptHitmarker = false;
    public bool justHit;


    void Start()
    {
        markers = gameObject.GetComponentsInChildren<Image>();
        img = gameObject.GetComponent<Image>();
        justHit = false;

        //gameObject.SetActive(true);
       /* foreach(Image marker in markers)
        {
            Debug.Log("Changing " + marker.gameObject.name);
            marker.color = Color.clear;
        } 
        */
        //setAllMarkers(0); 
    }

    void Update()
    {
        Debug.Log("Time left = " + timeLeft);
        
        /*if(timeLeft>= 0){
            Debug.Log("Changing");
            foreach(Image marker in markers) Color.Lerp(Color.white,Color.clear, markerLifespan);
            timeLeft -= Time.deltaTime;
        }
        */

       /* if(justHit) 
        {
            Debug.Log("Lerping");
            img.color = Color.Lerp(Color.white,Color.clear, markerLifespan);
            justHit = false;
        }
        */
       // if (markers[0].color == Color.clear) justHit = false;

/*
        if(timeLeft == markerLifespan){
           // foreach(Image marker in markers) marker.color = Color.white;
            img.color = Color.white;
            timeLeft -= Time.deltaTime;
        }
        if(timeLeft >= 0 && !interruptHitmarker)
        {
            timeLeft -= Time.deltaTime;
            Debug.Log("shifting color");
           // foreach(Image marker in markers) Color.Lerp(Color.white,Color.clear, markerLifespan);
            img.color = Color.Lerp(Color.white, Color.clear, markerLifespan);
        }
        else if (timeLeft >= 0 && interruptHitmarker){
            interruptHitmarker = false;
            timeLeft = markerLifespan;
            //foreach(Image marker in markers) marker.color = Color.white;
        }
        */
        
        
        /*if(timeLeft >= 0){
            timeLeft -= Time.deltaTime;
            setAllMarkers(markers[0].color.a - 10);
        }
        */
    }
    public IEnumerator botHit(){
        setAllMarkers(255);
        
        for (float i = markerLifespan; i >= 0 && !interruptHitmarker; i -= Time.deltaTime)
        {
            timeLeft = i;
            // set color with i as alpha
            setAllMarkers((byte)i);

            yield return null;
        }
        if (interruptHitmarker){
            interruptHitmarker = false;
            setAllMarkers(0);
        }
        yield return null;


       
        //Invoke("Blah", 2f);
    }

    public void botHit2(){
        Debug.Log("In Bot hit 2");
        if(timeLeft > 0){
            interruptHitmarker = true;
        }
        timeLeft = markerLifespan;
        justHit = true;
        
        if(img.color == clear) img.color = white;
        else img.color = clear;

        Debug.Log("color = " + img.color.ToString());
        //xInvoke(nameof(ResetJustHit), markerLifespan);
        //Debug.Log("After setting timeleft = " + timeLeft);
    }

    public float HitmarkerTimeRemaining(){
        return timeLeft;
    }
    public void InterruptHitmarker(){
        interruptHitmarker = true;
    }
    private void ResetJustHit()
    {
        justHit = false;
    }

    private void setAllMarkers(byte alpha){
        Debug.Log("Setting marker to " + alpha);
        foreach(Image marker in markers)
        {
            Debug.Log("Changing " + marker.gameObject.name);
            marker.color = Color.white;//new Color32(255,255,255,255);
        } 
    }
}
