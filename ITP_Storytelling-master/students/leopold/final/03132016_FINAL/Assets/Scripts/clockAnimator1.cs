using UnityEngine;
using System;
using System.Collections;

public class clockAnimator1 : MonoBehaviour {

	public Transform hours, minutes, seconds;
	public AudioSource clockTick;

	private const float
	hoursToDegrees = 360f / 12f,
	minutesToDegrees = 360f / 60f,
	secondsToDegrees = 360f / 60f;



	public bool analog;

	void Start () {

//		clockTick = GetComponent<AudioSource> ();
	}

	private void Update() {
		if (analog) {
			TimeSpan timespan = DateTime.Now.TimeOfDay;
			hours.localRotation = Quaternion.Euler (
				0f, 0f, (float)timespan.TotalHours * -hoursToDegrees);
			minutes.localRotation = Quaternion.Euler (
				0f, 0f, (float)timespan.TotalMinutes * -minutesToDegrees);
			seconds.localRotation = Quaternion.Euler (
				0f, 0f, (float)timespan.TotalSeconds * -secondsToDegrees);
		} else {
			DateTime time = DateTime.Now;
			hours.localRotation =
			Quaternion.Euler (0f, 0f, time.Hour * -hoursToDegrees);
			minutes.localRotation =
			Quaternion.Euler (0f, 0f, time.Minute * -minutesToDegrees);
			seconds.localRotation = Quaternion.Euler (0f, 0f, time.Second * -secondsToDegrees);
//
//			clockTick.Play ();
//			clockTick.Stop ();
		}
		Debug.Log (clockTick.time);

	}




//	void Update () {
//	
//	}
}
