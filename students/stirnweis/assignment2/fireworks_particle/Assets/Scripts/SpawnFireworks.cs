using UnityEngine;
using System.Collections;

public class SpawnFireworks : MonoBehaviour
{
    public Transform firework;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        createFirework();

    }

    void createFirework() { 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(firework, new Vector3(72, 34, Random.Range(-15, 30)), Quaternion.identity);
        }
    }
}
