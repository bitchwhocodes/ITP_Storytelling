using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class KinectCursor : MonoBehaviour
{
    public JointType jointId = JointType.HandLeft;
    public float easing = 0.03f;
    public GameObject Wall;
    private Transform tr;
    private UnityEngine.AudioSource whale;
    public float yVol;
    private float remapX;
    private float remapY;



    // Use this for initialization
    void Start ()
    {
        // get local component
        tr = GetComponent<Transform>();
        Wall = GameObject.FindGameObjectWithTag("Wall");
        whale = GetComponent<UnityEngine.AudioSource>();


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

        //  Wall.GetComponent<MeshRenderer>().material.color = new Color(0.01f, (tr.position.x) / 10, (tr.position.y) / 10, 0.1f);
            Wall.GetComponent<MeshRenderer>().material.color = new Color(0.01f, (tr.position.x) /-10, (tr.position.y) /10, 0.1f);


            // update cursor position
            tr.position = new Vector3(posX, posY, tr.position.z);
            yVol = tr.position.y /10 ;
            Debug.Log("yVol" + yVol);

            whale.volume = yVol;

        }
    }
}
