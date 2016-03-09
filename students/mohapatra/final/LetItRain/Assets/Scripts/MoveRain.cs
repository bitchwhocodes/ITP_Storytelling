using UnityEngine;
using System.Collections;

public class MoveRain : MonoBehaviour
{
   
	public float speed = 30.0f; 
 
 	private string hAxis = "Horizontal";

    public Vector3 velocity = Vector3.zero;
    

    // Use this for initialization
    void Start () {
       
	
	}
	
	// Update is called once per frame
	void Update () {
        //keep cube in camera view
        //code taken from: http://answers.unity3d.com/questions/799656/how-to-keep-an-object-within-the-camera-view.html

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.15f, 0.85f);
        pos.y = Mathf.Clamp(pos.y, 0.0f, 1.0f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            velocity.x = -speed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            velocity.x = speed;
        }

        transform.position += (velocity * Time.deltaTime);

    }

    //put up two colliders at the edge of the screen

}
