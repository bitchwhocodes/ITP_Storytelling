using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour
{

    private Skybox skybox;

	// Use this for initialization
	void Start ()
    {
        // reference components
        
        skybox = GetComponent<Skybox>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIT ENTER");
        skybox.material.color = new Color(0.0f, 1.0f, 0.0f);
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("HIT EXIT");
        skybox.material.color = new Color(1.0f, 0.0f, 0.0f);
    }
}
