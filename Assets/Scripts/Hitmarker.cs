using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitmarker : MonoBehaviour
{
    //public GameObject hitmarker;
    private Image marker;
    //private bool justHit = false;
    private float timeLeft = 0;
    private static float markerLifespan = 2.25f;
    private bool interruptHitmarker = false;


    void Start()
    {
        marker = gameObject.GetComponent<Image>();
        marker.color = new Color(255,255,255,0);
    }

    void Update()
    {
       /* if (justHit){
            for (float i = 3.5f; i >= 0 && ; i -= Time.deltaTime)
            {
                // set color with i as alpha
                marker.color = new Color(255, 255, 255, i);
            }
        }
        */
        if(timeLeft >= 0 && !interruptHitmarker)
        {
            timeLeft -= Time.deltaTime;
            marker.color = new Color(255, 255, 255, timeLeft);
        }
        else if (timeLeft >= 0 && interruptHitmarker){
            interruptHitmarker = false;
            timeLeft = markerLifespan;
            marker.color = new Color(255,255,255,255);
        }

    }
    public IEnumerator botHit(){
        marker.color = new Color(255,255,255,255);
        
        for (float i = markerLifespan; i >= 0 && !interruptHitmarker; i -= Time.deltaTime)
        {
            timeLeft = i;
            // set color with i as alpha
            marker.color = new Color(255, 255, 255, i);

            yield return null;
        }
        if (interruptHitmarker){
            interruptHitmarker = false;
            marker.color = new Color(255, 255, 255, 0);
        }
        yield return null;


       
        //Invoke("Blah", 2f);
    }

    public void botHit2(){
        if(timeLeft > 0){
            interruptHitmarker = true;
        }
        timeLeft = markerLifespan;
    }

    public float HitmarkerTimeRemaining(){
        return timeLeft;
    }
    public void InterruptHitmarker(){
        interruptHitmarker = true;
    }
    
}
