using UnityEngine;
using System.Collections;

public class MyBurst : MonoBehaviour {

	ParticleSystem ps;
	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem> ();
		var em = ps.emission;
//		em. enabled = true;
//		ps.emission.rate = 0;

//		em.type = ParticleSystemEmissionType.Time;


	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("key pressed!");
//			ps.Emit (100);
			var em = ps.emission;
			em.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(.1f, 100) });
		} else {
//			ps.Emit (1);
		}
	}
}
