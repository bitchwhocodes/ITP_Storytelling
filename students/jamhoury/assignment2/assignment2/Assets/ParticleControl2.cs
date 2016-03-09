using UnityEngine;
using System.Collections;

public class ParticleControl2: MonoBehaviour {

	ParticleSystem ps;
	ParticleSystem.EmissionModule em;

	// 
	void Start () {
		ps = GetComponent<ParticleSystem>();
		em = ps.emission;

	}

	// reference: http://forum.unity3d.com/threads/what-is-the-unity-5-3-equivalent-of-the-old-particlesystem-emissionrate.373106/
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)){

			var rate = new ParticleSystem.MinMaxCurve();
			rate.constantMax = 500f;
			em.rate = rate;

			Invoke("ResetParticleRate", 50f);
		}

	}

	void ResetParticleRate(){

		var rate = new ParticleSystem.MinMaxCurve();
		rate.constantMax = 50f;
		em.rate = rate;
	}
}
