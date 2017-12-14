using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	public GameObject projectile;
	private float playerWidth;

	private bool active = false;
	private Vector3 startPosition;
	private Vector2 moveInput;

	private float[] positions;
	private float[] shootTimes;
	private int positionIndex = 0;
	private int shootIndex = 0;

	private float shootTelemarkTime = .5f;

	private bool spriteFlipped = false;

	private Animator animator;

	public void Initialize (float[] positions, float[] shootTimes) {
		this.positions = positions;
		this.shootTimes = shootTimes;

		spriteFlipped = false;
		playerWidth = GetComponent <SpriteRenderer> ().bounds.extents.x * 2.0f;

		animator = GetComponent <Animator>();
	}

	public void EnactRoutine () {
		animator.Play ("Appear");

		active = true;
		positionIndex = 0;
		shootIndex = 0;
		if (shootIndex < shootTimes.Length) {
			Invoke ("Shoot", shootTimes [shootIndex]);// - shootTelemarkTime);
		}
	}

	public void Update() {
		if (positionIndex < positions.Length - 2) {
			float oldx = transform.position.x;
			transform.position = new Vector3 (positions [positionIndex++], positions [positionIndex++], 0);

			if (Mathf.Abs(transform.position.x - oldx) >= .1f) {
				float sign = Mathf.Sign (transform.position.x - oldx);

				if ((sign == -1f && !spriteFlipped) || (sign == 1f && spriteFlipped)) {
					spriteFlipped = !spriteFlipped;
					GetComponent<SpriteRenderer> ().flipX = spriteFlipped;
				}
			}
		} else if(active) {
			StopRoutine ();
		}
	}

	private void TelemarkShoot() {
		animator.speed = .8f;
		animator.Play ("Shoot");
		//Invoke ("Shoot", shootTelemarkTime);
	}

	// Lot of duplicate shoot code
	private void Shoot() {
		AudioManager.PlayEnemyShoot ();
		animator.speed = 1.0f;
		float direction = spriteFlipped ? -1 : 1;
		GameObject missile = Instantiate (projectile);
		missile.transform.position = transform.position + new Vector3((playerWidth * direction), 0, 0);
		missile.GetComponent <Missile>().Initialize(Vector3.right * direction, 8.0f);

		shootIndex++;
		if (shootIndex < shootTimes.Length) {
			Invoke ("Shoot", shootTimes [shootIndex]);
		}
	}

	public void StopRoutine() {
		active = false;
		animator.Play ("Disappear");
	}
}
