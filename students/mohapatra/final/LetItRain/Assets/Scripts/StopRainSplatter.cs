using UnityEngine;
using System.Collections;

public class StopRainSplatter : MonoBehaviour {

    void OnCollisionExit(Collision collision)
    {
        if(collision.contacts.Length == 0)
        {
            Debug.Log("No contact w/body");
        }
    }
}
