using UnityEngine;
using System.Collections;

public class Ninja : MonoBehaviour {

    private Animator anim;
	// Use this for initialization
	void Start () {
	   anim = GetComponent<Animator>();
	}
    
    public void ResetAnimation(){
        anim.Play("idle");
    }
	
	// Update is called once per frame
    // Showing how to call animations using the Play
	void Update () {

	   if(Input.GetKeyDown(KeyCode.RightArrow)){
           anim.Play("running");
       }
       if(Input.GetKeyUp(KeyCode.LeftArrow)){
           anim.Play("throwing");
       }
       
       if(Input.GetKey(KeyCode.Space)){
           anim.Play("dead");
       }
       
       if(Input.GetKeyUp(KeyCode.RightArrow)){
           anim.Play("idle");
       }
       
       
	}
}
