using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Animate : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private static int tagCounter = 0;
	private string tag;

	public void Awake() {
		spriteRenderer = GetComponent <SpriteRenderer>();
		tag = tagCounter + "";
		tagCounter++;
	}

	// SIZE

	public void AnimateToSize(Vector2 start, Vector2 finish, float t) {
		Timing.RunCoroutine (C_AnimateToSizeAndBack(start, finish, t, false), tag);
	}

	public void AnimateToSizeAndBack(Vector2 start, Vector2 finish, float t) {
		Timing.RunCoroutine (C_AnimateToSizeAndBack(start, finish, t, true), tag);
	}

	private IEnumerator<float> C_AnimateToSizeAndBack (Vector2 start, Vector2 finish, float duration, bool goBack) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= duration) {
			timer = Time.time - startTime;
			transform.localScale = Vector2.Lerp (start, finish, timer/duration);
			yield return 0;
		}
		if(goBack) {
			Timing.RunCoroutine (C_AnimateToSizeAndBack(finish, start, duration, false), tag);
		}
	}

	// COLOR

	public void AnimateToColor(Color start, Color finish, float t) {
		Timing.RunCoroutine (C_AnimateToColorAndBack(start, finish, t, false), tag);
	}

	public void AnimateToColorAndBack(Color start, Color finish, float t) {
		Timing.RunCoroutine (C_AnimateToColorAndBack(start, finish, t, true), tag);
	}

	private IEnumerator<float> C_AnimateToColorAndBack (Color start, Color finish, float duration, bool goBack) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= duration) {
			timer = Time.time - startTime;
			spriteRenderer.color = Color.Lerp (start, finish, timer/duration);
			yield return 0;
		}
		if(goBack) {
			Timing.RunCoroutine (C_AnimateToColorAndBack(finish, start, duration, false), tag);
		}
	}

	void OnDestroy() {
		Timing.KillCoroutines (tag);
	}
}
