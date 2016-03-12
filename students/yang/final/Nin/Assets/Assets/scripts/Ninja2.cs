using UnityEngine;
using System.Collections;

public class Ninja2 : MonoBehaviour {

    private Animator anim;
	// Use this for initialization
	void Start () {
	   anim = GetComponent<Animator>();
	}
    
    public void ResetAnimation(){
        anim.SetInteger("state",0);
    }
	
	// Update is called once per frame
    // Showing how to call animations using the Play
	void Update () {

	   if(Input.GetKeyDown(KeyCode.RightArrow)){
           anim.SetInteger("state",1);
       }
       if(Input.GetKeyUp(KeyCode.LeftArrow)){
           anim.SetInteger("state",2);
       }
       
       if(Input.GetKey(KeyCode.Space)){
          anim.SetInteger("state",3);
       }
       
       if(Input.GetKeyUp(KeyCode.RightArrow)){
          anim.SetInteger("state",0);
       }
       
       
       
	}
}
