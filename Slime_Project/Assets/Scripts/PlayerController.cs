using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public float maxSpeed = 10f;
	public Transform groundCheck;
	public Transform wallCheck;
	public LayerMask whatIsGround;
	public LayerMask whatIsGround_1;
	public float jumpForce = 200f;
	public int HP =6;
	public float invincible_time  = 1f;
	public SpriteRenderer renderer;
	//public int eatingCD = 0;
	public static bool facingRight = true;
	public GameObject hurtParticle;
	public GameObject bubble;
	public static int energy = 0;

	private bool grounded = false;
	private bool wallTouch = false;
	private Rigidbody2D rb2d;
	private float groundRadius = 0.2f;
	private float groundRadius_1 = 0.2f;
	private int invincible = 1;
	private int direction = 1;
	private bool eating = false;
	private Animator animator;
	private Animator bubble_animator;
	private bool isJumping = false;
	private bool isAte = false;
	private string[] Level_list = {"Level_0","Level_1","Level_2","Level_3","Level_4","Level_5"};
	private bool ifdead = false;
	private Vector2 enemyForce;

	public AudioClip jumpSound;
	public AudioClip getHpSound;
	public AudioClip eatingModeSound;
	public AudioClip eatEnemySound;
	public AudioClip hitSound;

	void Start () 
	{
		facingRight = true;
		animator = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D> ();	
		ifdead = false;
	}

	void FixedUpdate () 
	{	if (!ifdead) {
			grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);
			wallTouch = Physics2D.OverlapCircle (wallCheck.position, groundRadius_1, whatIsGround_1);
			float move = Input.GetAxis ("Horizontal");
			Vector2 movement = new Vector2 (move * maxSpeed, rb2d.velocity.y);
			if (!wallTouch) {
				rb2d.velocity = movement;
			}
			if (move > 0 && !facingRight) {
				direction = -1;
				Flip ();
			} 
			else if (move < 0 && facingRight) {
				direction = 1;
				Flip ();
			}
		}
	}

	void Update () 
	{   
		if (energy > 6)
			energy = 6;

		if (grounded) {
			isJumping = false;
		}

		if (grounded && Input.GetKeyDown (KeyCode.Space) && !ifdead) 
		{   
			if (isJumping == false) {
				rb2d.AddForce (new Vector2 (0, jumpForce));
				animator.SetTrigger ("Jump");
				isJumping = true;

				SoundManager.instance.PlaySingle (jumpSound);

				if (GameManager.level == 3) {
					bubble.transform.position = GetComponent<Transform>().position;
					bubble_animator = bubble.GetComponent<Animator> ();
					bubble_animator.SetTrigger ("Release");
				}
			}

		}
		if (Input.GetKeyDown (KeyCode.C)) 
		{	
			if (eating == false && energy == 6) {
				EatingMode (true);
				Invoke ("CancelEatingMode", 3);
				SoundManager.instance.PlaySingle (eatingModeSound);

			}
		}
		if (Input.GetKeyDown (KeyCode.X)) 
		{	
			GameManager.Ability_Index = (GameManager.Ability_Index + 1) % GameManager.Ability_num;
		}

		Weapon.fireMode = GameManager.Ability_List[GameManager.Ability_Index];

	}
	void loseHP()
	{
		if (HP > 0 && invincible != 0) {
			GameObject hurt = Instantiate (hurtParticle, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
			HP--;
			invincible = 1;
			EatingMode (false);
			Destroy (hurt, 1);
			SoundManager.instance.PlaySingle (hitSound);
		}
	}

	void EatingMode(bool status)
	{
		if (status) {
			animator.SetTrigger ("Eating");
			energy = 0;
			renderer.color = Color.red;
			eating = true;

		} else {
			renderer.color = Color.white;
			eating = false;
		}
	}

	void CancelEatingMode(){
		renderer.color = Color.white;
		eating = false;
	}


	void Flip () 
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	void Add_ability(int num)
	{
		if (GameManager.Ability_num >= 3) {
			GameManager.Ability_List [0] = num;
		}
		else
			GameManager.Ability_List [GameManager.Ability_num] = num;
		GameManager.Ability_num++;
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		enemyForce = new Vector2 (direction * 5000f, 0f);

		switch (other.tag) {

		case "Dragon":
			if (eating) {
				Destroy (other.gameObject);
				Add_ability (1);
				EatingMode (false);
				SoundManager.instance.PlaySingle (eatEnemySound);
			} else {
				rb2d.AddForce (enemyForce);
				isJumping = true;
				loseHP ();
			}
			break;
		
		
		case "Eye_Monster":
			if (eating) {
				Destroy (other.gameObject);
				Add_ability (2);
				EatingMode (false);
				SoundManager.instance.PlaySingle (eatEnemySound);
			} else {
				rb2d.AddForce (enemyForce);
				isJumping = true;
				loseHP ();
			}
			break;

		case "Enemy_Bolt":
			Destroy (other.gameObject);
			rb2d.AddForce (enemyForce);
			isJumping = true;
			loseHP ();
			break;

		case "Jellyfish":
			if (eating) {
				Destroy (other.gameObject);
				Add_ability (3);
				EatingMode (false);
				SoundManager.instance.PlaySingle (eatEnemySound);
			} else {
				rb2d.AddForce (enemyForce);
				isJumping = true;
				loseHP ();
			}
			break;

		case "Enemy_Eball":
			rb2d.AddForce (enemyForce);
			isJumping = true;
			loseHP ();
			break;

		case "Trap":
			//rb2d.AddForce (new Vector2 (0f, 1000f));
			loseHP ();
			break;

		case "Health_pickup":
			if (HP != 6) {
				
				if (HP + 2 > 6)
					HP = 6;
				else
					HP += 2;
				other.gameObject.SetActive (false);
				SoundManager.instance.PlaySingle (getHpSound);
			}
			break;

		case "Exit":
			Restart ();
			break;

		case "Lava": 
			Renderer render = GetComponent<Renderer> ();
			render.enabled = false;
			ifdead = true;
			break;

		default:
			break;
		}

	}


	private void Restart()
	{	
		GameManager.level++;
		if (!facingRight) {
			Flip ();
			facingRight = true;
		}
		Application.LoadLevel (Level_list[GameManager.level]);
		SoundManager.instance.PlayNextBGM (GameManager.level);
	}

	IEnumerator Hurt() {
		GameObject hurt = Instantiate(hurtParticle, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		yield return new WaitForSeconds(1.0f);
		Destroy (hurt);
	}
}