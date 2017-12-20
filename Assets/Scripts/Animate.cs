﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Animate : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	public void Awake() {
		spriteRenderer = GetComponent <SpriteRenderer>();
	}

	public void AnimateToColor(Color start, Color finish, float t) {
		Timing.RunCoroutine (C_AnimateToColorAndBack(start, finish, t, false));
	}

	public void AnimateToColorAndBack(Color start, Color finish, float t) {
		Timing.RunCoroutine (C_AnimateToColorAndBack(start, finish, t, true));
	}

	private IEnumerator<float> C_AnimateToColorAndBack (Color start, Color finish, float duration, bool goBack) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= duration) {
			timer = Time.time - startTime;
			Debug.Log (spriteRenderer);
			spriteRenderer.color = Color.Lerp (start, finish, timer/duration);
			yield return 0;
		}
		if(goBack) {
			Timing.RunCoroutine (C_AnimateToColorAndBack(finish, start, duration, false));
		}
	}
}
