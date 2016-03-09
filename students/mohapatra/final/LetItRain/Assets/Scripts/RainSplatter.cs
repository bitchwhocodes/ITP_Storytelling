using UnityEngine;
using System.Collections;

public class RainSplatter : MonoBehaviour {

    //modified Camera.backgroundColor code - http://docs.unity3d.com/ScriptReference/Camera-backgroundColor.html
    //hex value for rain: 3C444705
    //hex value for clear sky: 05D3E405
    public Color color1 = new Color(5.0F,211.0F,229.0F,5.0F);
    public Color color2 = new Color(60.0F,68.0F,71.0F,5.0F);
    public float duration = 1.0F;

    Camera camera;


    // Use this for initialization
    void Start () {
        camera = Camera.main;
        camera.clearFlags = CameraClearFlags.SolidColor;
        Debug.Log("Starting BG color: " + camera.backgroundColor.ToString());

    }

    // Update is called once per frame
    void Update () {
	
	}

    //skybox color change modified from http://stackoverflow.com/questions/12551768/unity3d-change-skybox-color-via-script
    //on colliosion enter - http://docs.unity3d.com/ScriptReference/Rigidbody.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision)
    {
       // foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log("collision made. BG color: " + camera.backgroundColor.ToString());

            float t = Mathf.PingPong(Time.time, duration) / duration;
            camera.backgroundColor = Color.Lerp(color1, color2, t);
         
        }
    }

   

   

}
