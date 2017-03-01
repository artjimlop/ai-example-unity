using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float HealthPoints {
		get { 
			return healthPoints;
		}

		set { 
			healthPoints = value;
			if (healthPoints <= 0) {
				Destroy (gameObject);
			}
		}
	}

	[SerializeField]
	private float healthPoints = 100.0f;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
