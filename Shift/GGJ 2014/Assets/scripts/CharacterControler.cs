using UnityEngine;
using System.Collections;

public class CharacterControler : MonoBehaviour {

	public float maxSpeed = 10f;
	bool facingRight = true;

	public Animator anim;

	//falling
	public bool grounded = false;
	public Transform groundCheck;
	float groundRadius = 0.50f;
	public LayerMask ground;

	//spinner on off
	private bool spinner =false;
	private int spinnernum =0;

	//jumping
	public float jumpForce = 0f;
	private bool jumpable = true;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		//checks if grounded
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, ground);
		if(!grounded)
		{
			jumpable = true;
		}
		anim.SetBool ("Ground", grounded);

		anim.SetFloat ("vSpeed",rigidbody2D.velocity.y);
		
		//movement
		float move = Input.GetAxis ("Horizontal");

		anim.SetFloat ("Speed", Mathf.Abs(move));

		rigidbody2D.velocity = new Vector2(move * maxSpeed, rigidbody2D.velocity.y);

		if(move < 0 && !facingRight)
			Flip ();
		else if(move > 0 && facingRight)
			Flip ();
	}

	void Update()
	{
		if(jumpable && grounded && (Input.GetKeyDown (KeyCode.W)|| Input.GetKeyDown (KeyCode.UpArrow) ||Input.GetKeyDown(KeyCode.Space)))//Input.GetAxis ("Jump") > 0)
		{
			jumpable = false;
			anim.SetBool("Ground",false);
			rigidbody2D.AddForce(new Vector2(0, jumpForce));
		}

		if(transform.position.y < -185)
		{
			transform.position = GameObject.Find("start").transform.position;
		}
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *=-1;
		transform.localScale = theScale;
	}

	public void Detected()
	{
		GameObject.Find ("Character/Mark").SetActive(true);
	}

	public void Removed()
	{
		GameObject.Find ("Character/Mark").SetActive(false);
	}

	void OnTriggerStay2D(Collider2D col) // need this for it to work, on start only checks at initial entry, onexit is only for when you leave, this is for while its in : DJ
	{
		GameObject s = col.gameObject;
		if(s.name == "box")
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				//Debug.Log("Change");
				//cycle the box properties (type)
				s.gameObject.GetComponent<BoxClass>().cycle();
				//works but the key feels tempermental sometimes it works sometimes you have to hit it twice or so : DJ
			}
		}
		if(s.name == "Moving")
		{
			if(Input.GetKeyDown(KeyCode.E)&&spinner)
			{
				s.gameObject.GetComponent<MovingPlatform>().cycle (spinnernum);
				GameObject.Find ("Character/spinner1").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner2").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner3").GetComponent<SpriteRenderer>().enabled = false;
				spinner = false;
			}
			else if(Input.GetKeyDown (KeyCode.E)&&!spinner)
			{

				GameObject.Find ("Character/spinner1").GetComponent<SpriteRenderer>().enabled = true;
				spinner = true;
			}
			else if(spinner&&(Input.GetKeyDown (KeyCode.Keypad1)||Input.GetKeyDown (KeyCode.Alpha1)))
	        {
				//rotate spinner to 0
				GameObject.Find ("Character/spinner1").GetComponent<SpriteRenderer>().enabled = true;
				GameObject.Find ("Character/spinner2").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner3").GetComponent<SpriteRenderer>().enabled = false;
				spinnernum = 0;
			}
			else if(spinner&&(Input.GetKeyDown (KeyCode.Keypad2)||Input.GetKeyDown (KeyCode.Alpha2)))
			{
				//rotate spinner to 1
				GameObject.Find ("Character/spinner2").GetComponent<SpriteRenderer>().enabled = true;
				GameObject.Find ("Character/spinner1").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner3").GetComponent<SpriteRenderer>().enabled = false;
				spinnernum = 1;
			}
			else if(spinner&&(Input.GetKeyDown (KeyCode.Keypad3)||Input.GetKeyDown (KeyCode.Alpha3)))
			{
				//rotate spinner to 2
				GameObject.Find ("Character/spinner1").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner2").GetComponent<SpriteRenderer>().enabled = false;
				GameObject.Find ("Character/spinner3").GetComponent<SpriteRenderer>().enabled = true;
				spinnernum = 2;
			}
		}
	}

	void OnCollisionStay2D(Collision2D col)
	{
		if(col.gameObject.name == "Moving")
		{
			if(col.gameObject.GetComponent<MovingPlatform>().type == "Horizontal")
			{
					
				transform.Translate(col.gameObject.GetComponent<MovingPlatform>().useSpeed*Time.deltaTime,0,0);
			}
			
			else if(col.gameObject.GetComponent<MovingPlatform>().type == "Vertical")
			{

				transform.Translate(0,col.gameObject.GetComponent<MovingPlatform>().useSpeed * Time.deltaTime,0);
				
			}
		}
	}

	/// <summary>
	/// Trigger collider
	/// </summary>
	void OnTriggerEnter2D(Collider2D col)
	{
		GameObject s = col.gameObject;
		Debug.Log ("Trigger Collision with " + s.name);
		if(s.name == "box" || s.name == "Moving")
		{
			GameObject.Find ("Character/Mark").GetComponent<SpriteRenderer>().enabled = true;
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("Change");
			}
		}

	}
	
	void OnTriggerExit2D(Collider2D col)
	{
		Debug.Log ("Collision Exit");
		string s = col.gameObject.name;
		if(s == "box" || s == "Moving")
		{
			GameObject.Find ("Character/Mark").GetComponent<SpriteRenderer>().enabled = false;
		}
		GameObject.Find ("Character/spinner").GetComponent<SpriteRenderer>().enabled = false;
	}

}
