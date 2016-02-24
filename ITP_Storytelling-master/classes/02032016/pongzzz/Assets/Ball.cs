using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Ball : MonoBehaviour {

	public float speed = 30;
	private AudioSource snd;
	private Rigidbody2D rb;

	private Transform trans;




	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = Vector2.right * speed;
		snd = GetComponent<AudioSource> ();
		trans = this.transform;


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){
		// to do - refactor 
		if (col.gameObject.name == "leftRacket") {
			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (1, y).normalized;
			rb.velocity = direction * speed;
			snd.Play ();
		
		

		}

		if (col.gameObject.name == "rightRacket") {
			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (-1, 0).normalized;
			rb.velocity = direction * speed; 
			snd.Play ();

		}

		if (col.gameObject.name == "leftWall") {
			rb.velocity = Vector2.zero;

			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

		if (col.gameObject.name == "rightWall") {
			rb.velocity = Vector2.zero;

			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

	
			
	
	}



	float calculatePosition(Vector2 ballPosition, Vector2 racketPosition, float racketHeight){
		float value = (ballPosition.y - racketPosition.y) / racketHeight;
		return(value);
	}


}
