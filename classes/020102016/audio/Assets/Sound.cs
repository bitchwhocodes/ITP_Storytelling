using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {


	AudioSource audio;
	float[] spectrum = new float[256];
	GameObject [] bars = new GameObject[256];

	void Start() {
		audio = GetComponent<AudioSource>();
		int i = 1;

		while (i < 40) {
			
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

			//Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(i/256, 0, 0));
			//cube.transform.position = worldPoint;
			cube.transform.position = new Vector3(i-20, 0, 0);
			cube.transform.Rotate (new Vector3 (0f, 20f, 0f));
			cube.transform.localScale = new Vector3 (0.1f, 2.0f,0.1f);

			cube.transform.parent = gameObject.transform;
			bars [i] = cube;
			i++;
		}



	}

	void Update() {
		audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		int i = 1;
		while (i < 40) {

			float height = Mathf.Clamp (0.2f + spectrum [i] * 500f, 0.2f, 500.0f);
			bars [i].transform.localScale = new Vector3 (0.2f, height, 1.0f);

			i++;
		}
	}


}