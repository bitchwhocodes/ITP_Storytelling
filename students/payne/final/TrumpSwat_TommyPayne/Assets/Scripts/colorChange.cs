using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class colorChange : MonoBehaviour {

    public JointType jointId = JointType.HandLeft;
   // public Color colorEnd = Color.green;
   // public float duration = 1.0F;
   // public Renderer rend;
   // public GameObject Wall;
    private Renderer wallRenderer;
    private Transform tr;
    public float yCol;
    public float xCol;

    void Start () {

     // Wall = GameObject.Find("Wall");
        wallRenderer = GetComponent<Renderer>();
    }

    void Update () {
        //if (BodySourceView.bodyTracked)
        //{
        //    // fetch joint positions
        //    Vector3 joint = BodySourceView.jointObjs[(int)jointId].position;

        //    // easing towards X
        //    float targetX = joint.x;
        //    float posX = transform.position.x;
        //    float dx = targetX - posX;


        //    if (Mathf.Abs(dx) > 1)

        //    {
        //        wallRenderer.material.color = new Color(0.0f, 0.8f, 0.4f, 1.0f);
        //    }

        //    // easing towards Y
        //    float targetY = joint.y;
        //    float posY = transform.position.y;
        //    float dy = targetY - posY;

        //    if (Mathf.Abs(dy) > 1)
        //    {
        //        wallRenderer.material.color = new Color(0.5f, 0.1f, 0.1f, 1.0f);
        //    }

        //    //float posX = tr.position.x;
        //    //float posY = tr.position.y;
        //    //  update color from cursor position
        //    tr.position = new Vector3(posX, posY, tr.position.z);
        //    yCol = tr.position.y;
        //    xCol = tr.position.x;

        //    Debug.Log("yCol" + yCol);
        //    Debug.Log("xCol" + xCol);
        //}
    }
}

