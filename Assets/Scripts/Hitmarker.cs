using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
    //[Header("")]
    private Coroutine displayHitmarker;
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
        //markers = gameObject.GetComponentsInChildren<Image>();
        img = gameObject.GetComponent<Image>();
        img.enabled = true;
        img.color = clear;
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
    public IEnumerator FadeHitmarker(){
        
        float timeElapsedHand = 0;
        while (timeElapsedHand < 1.7)
        {
            img.color = Color.Lerp(white, clear, timeElapsedHand / 1.7f);
            timeElapsedHand += Time.deltaTime;
            yield return null;
        }
        img.color = Color.clear;
        //Invoke("Blah", 2f);
    }

    public void botHit2(){
        Debug.Log("In Bot hit 2");
        if(timeLeft > 0){
            interruptHitmarker = true;
        }
        timeLeft = markerLifespan;
        justHit = true;
        
        if(img.color == Color.clear) img.color = Color.white;
        else img.color = Color.clear;

     //   Debug.Log("color = " + img.color.ToString());
        //xInvoke(nameof(ResetJustHit), markerLifespan);
        //Debug.Log("After setting timeleft = " + timeLeft);
    }

    public void botHit3()
    {
        img.color = white;
        
       // if(displayHitmarker != null)  StopCoroutine(displayHitmarker);
        //displayHitmarker = StartCoroutine(HitmarkerCountdown());

        if(displayHitmarker != null)  StopCoroutine(displayHitmarker);
        displayHitmarker = StartCoroutine(FadeHitmarker());
    }

    IEnumerator HitmarkerCountdown()
    {
        yield return new WaitForSeconds(1.75f);
        img.enabled = false;
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
