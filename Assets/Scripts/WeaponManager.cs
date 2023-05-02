using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam;
    public float maxRange = 100f;
    private Animator animator;
    public float damage = 20f;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(animator.GetBool("isShooting")){
            //animator.SetBool("isShooting",false);
        }
        */
        if(this.animator.GetCurrentAnimatorStateInfo(0).IsName("FirePistol")){
            animator.SetBool("isShooting", false);
        }
    }

    public void onShoot(InputAction.CallbackContext context){
        //anim.Play();
        
        RaycastHit hit; 
        animator.SetBool("isShooting", true);
        if(context.performed && Physics.Raycast(playerCam.transform.position, transform.forward, out hit, maxRange)){
            Debug.Log("Hit");
            BotManager botManager = hit.transform.GetComponent<BotManager>();
            if(botManager != null){
                botManager.Hit(damage);
            }
        }
        

    }
}
