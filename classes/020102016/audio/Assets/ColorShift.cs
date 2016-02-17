//-----------------------------------------
//   Jason Walters
//   http://glitchbeam.com
//   @jasonrwalters
//
//   last edit on 8/23/2015
//-----------------------------------------

using UnityEngine;
using System.Collections;

public class ColorShift : MonoBehaviour
{
	public float hue = 0.6f;
	public float saturation = 1f;
	public float brightness = 1f;

	private Light lt;
	private HSBColor col;

	// Use this for initialization
	void Start()
	{
		// setup components
		lt = GetComponent<Light>();
		// setup hsb color
		col = new HSBColor(hue, saturation, brightness);
	}

	// Update is called once per frame
	void Update()
	{
		// move through the hue range
		col.h += 0.05f * Time.deltaTime;

		// if at hue max, reset to zero
		if (col.h > 1.0f)
		{
			col.h = hue;
		}

		// update light color with converted HSB color
		lt.color = col.ToColor();
	}
}