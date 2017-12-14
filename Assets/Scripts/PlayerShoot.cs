using UnityEngine;
using System;

public class PlayerShoot : MonoBehaviour {

	public GameObject projectile;
	private float playerWidth;
	private float[] shootTimes = new float[10];
	private int index = 0;
	private float compareTime;
	private float projectileSpeed = 8.0f;
	private Animator animator;

	public void Start()
	{
		animator = GetComponent <Animator>();
		playerWidth = GetComponent <SpriteRenderer> ().bounds.extents.x * 2.0f;
	}

	public void Update()
	{
		bool fire = Input.GetKeyDown (KeyCode.Space);

		if (fire) {
			AudioManager.PlayEnemyShoot ();
			animator.Play("Shoot");

			float direction = PlayerController.spriteFlipped ? -1 : 1;
			GameObject missile = Instantiate (projectile);
			missile.transform.position = transform.position + new Vector3((playerWidth * direction), 0, 0);
			missile.GetComponent <Missile>().Initialize(Vector3.right * direction, projectileSpeed);

			shootTimes [index++] = Time.time - compareTime;
			compareTime = Time.time;

			if (index == shootTimes.Length) {
				float[] temp = new float[shootTimes.Length * 2];
				shootTimes.CopyTo(temp, 0);
				shootTimes = temp;
			}
		}
	}

	public float[] GetShootTimes() {
		float[] shootTimesChopped = new float[index];
		Array.Copy (shootTimes, shootTimesChopped, index);
		shootTimes = new float[10];
		index = 0;

		compareTime = Time.time;

		return shootTimesChopped;
	}
}
