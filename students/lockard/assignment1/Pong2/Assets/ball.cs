using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ball : MonoBehaviour {

	public float speed = 30;
	private AudioSource snd;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		GetComponent<Rigidbody2D> ().velocity = Vector2.right * speed;
		snd = GetComponent<AudioSource> ();

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.name == "leftpaddle") {
			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (1, y).normalized;
			rb.velocity = direction * speed;
			snd.Play ();
		}

		if (col.gameObject.name == "rightpaddle") {
			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (-1, y).normalized;
			rb.velocity = direction * speed;
			snd.Play ();
		}

		if (col.gameObject.name == "leftwall") {
			rb.velocity = Vector2.zero;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

		if (col.gameObject.name == "rightwall") {
			rb.velocity = Vector2.zero;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);

		}

	}

	float calculatePosition(Vector2 ballPosition, Vector2 racketPosition, float racketHeight){
		float value = (ballPosition.y - racketPosition.y) / racketHeight;
		return(value);
	}


}
