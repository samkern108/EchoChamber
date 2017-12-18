using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovementEffects;

public class Animate : MonoBehaviour {

	public void AnimateToColorAndBack(Color start, Color finish, float t) {
		Timing.RunCoroutine (C_AnimateToColorAndBack(start, finish, t, true));
	}

	private static IEnumerator<float> C_AnimateToColorAndBack (Color start, Color finish, float duration, bool goBack) {
		float startTime = Time.time;
		float timer = 0;
		while(timer <= duration) {
			timer = Time.time - startTime;
			Color.Lerp (start, finish, timer/duration);
			yield return 0;
		}
		if(goBack) {
			Timing.RunCoroutine (C_AnimateToColorAndBack(finish, start, duration, false));
		}
	}
}
