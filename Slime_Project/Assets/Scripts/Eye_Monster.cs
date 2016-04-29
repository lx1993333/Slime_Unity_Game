﻿using UnityEngine;
using System.Collections;

public class Eye_Monster : Enemy {

	private Transform target;
	private int Hp = 2;


	// Use this for initialization
	protected override void Start () {
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
	}
		
	void FixedUpdate () {
		int x = 0;
		int y = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			y = target.position.y > transform.position.y ? 1 : -1;
		else
			x = target.position.x > transform.position.x ? 1 : -1;
		Move (x, y);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag ("Bolt")) 
		{
			Hp--;
			Destroy (other.gameObject);
			SoundManager.instance.PlaySingle (enemyHitSound);
			if (Hp == 0) {
				GameObject deadcopy = Instantiate (dead, transform.position, transform.rotation) as GameObject;
				Destroy (deadcopy, 1);
				Destroy (gameObject);
			} 
			else 
			{
				GameObject hitcopy = Instantiate (hit, transform.position, transform.rotation) as GameObject;
				Destroy (hitcopy, 0.5f);
			}
		}
	}
}
