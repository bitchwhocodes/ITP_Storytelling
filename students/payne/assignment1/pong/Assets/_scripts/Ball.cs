using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
	

public class Ball : MonoBehaviour {

	public float speed = 3;
	private Rigidbody2D rb;
	private AudioSource snd;

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody2D> ();
		rb.velocity = Vector2.right * speed;
		snd = GetComponent<AudioSource> ();

		//when game starts apply velocity so ball moves
		//vector2 is shorthand for moving it 1 on the x and 0 on the y
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D col){
			
		//info about the collision
		//Debug.Log("collide");

		if (col.gameObject.name == "Lracket") {

			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (1, y).normalized;
			rb.velocity = direction * speed;
			snd.Play ();
		}

		if (col.gameObject.name == "Rracket") {

			float y = calculatePosition (transform.position, col.transform.position, col.collider.bounds.size.y);
			Vector2 direction = new Vector2 (-1, 0).normalized;
			rb.velocity = direction * speed;
			snd.Play ();

			}

		if (col.gameObject.name == "leftwall") {
			rb.velocity = Vector2.zero;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

		if (col.gameObject.name == "rightwall"){
			rb.velocity = Vector2.zero;
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	float calculatePosition(Vector2 ballPosition, Vector2 racketPosition, float rackerHeight){
		float value = (ballPosition.y - racketPosition.y) / rackerHeight;
		return(value);
	}
}
