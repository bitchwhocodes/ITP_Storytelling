//
//  Created by Jason Walters
//  http://glitchbeam.com
//  http://twitter.com/jasonrwalters
//
//  Kinect Joint IDs http://glitchbeam.com/2015/04/02/kinect-v2-joint-map/
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;

public class BodyParticles : MonoBehaviour
{
    public GameObject joints;
    public bool bodyDetected = false;

    //skybox colors
    private Color colorStart = new Color(0.26F, 0.71F, 0.86F, 0.05F);
    private Color colorEnd = new Color(0.2F, 0.3F, 0.4F, 0.5F);
    private float duration = 1.0F;

    private GameObject[] clone = new GameObject[25];

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame

    //clone items outside the scope of where the body is
	void Update ()
    {
        for (int i = 0; i < BodySourceView.jointObjs.Length; i++)
        {
            if (BodySourceView.bodyTracked)
            {
                Vector3 pos = BodySourceView.jointObjs[i].localPosition;
                Quaternion rot = BodySourceView.jointObjs[i].localRotation;

                if (clone[i] == null)
                {
                    clone[i] = Instantiate(joints, pos, rot) as GameObject;
                    // parent to this gameobject
                    clone[i].transform.parent = transform;
                }
                else
                {
                    clone[i].transform.localPosition = pos;
                    clone[i].transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

                    
                    //original particle system
                    //ParticleSystem pars = clone[i].GetComponentInChildren<ParticleSystem>();
                    //pars.Play();
                }
            }
            else
            {
                if (clone[i] != null)
                {
                    //original particle system
                    //ParticleSystem pars = clone[i].GetComponentInChildren<ParticleSystem>();
                    //pars.Stop();
                }
            }
        }
	}

    //skybox color change modified from http://stackoverflow.com/questions/12551768/unity3d-change-skybox-color-via-script
    //on colliosion enter - http://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log("collision made");
        }
    }

}
