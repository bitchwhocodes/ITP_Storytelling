using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class SwatCursor : MonoBehaviour
{
    public JointType jointId = JointType.HandLeft;

    public float easing = 0.13f;
    private Transform tr;
   // private UnityEngine.AudioSource whale;
    //public float yVol;
    private float remapX;
    private float remapY;



    // Use this for initialization
    void Start()
    {
        // get local component
        tr = GetComponent<Transform>();
        //whale = GetComponent<UnityEngine.AudioSource>();


    }

    // Update is called once per frame
    void Update()
    {
        if (BodySourceView.bodyTracked)
        {
            // fetch joint positions
            Vector3 joint = BodySourceView.jointObjs[(int)jointId].position;

            // easing towards X
            float targetX = joint.x;
            float posX = tr.position.x;
            float dx = targetX - posX;


            if (Mathf.Abs(dx) > 1)

            {
                posX += dx * easing;
            }

            // easing towards Y
            float targetY = joint.y;
            float posY = tr.position.y;
            float dy = targetY - posY;
            if (Mathf.Abs(dy) > 1)
            {
                posY += dy * easing;
            }
            
            // update cursor position
            tr.position = new Vector3(posX, posY, tr.position.z);
            //yVol = tr.position.y / 10;
            //Debug.Log("yVol" + yVol);

            //whale.volume = yVol;

        }
    }
}
