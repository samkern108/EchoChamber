using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Die", 1.0f);
	}
	
	private void Die() {
		Destroy (gameObject);
	}
}
