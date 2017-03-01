using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour {

	public enum ENEMY_STATE {
		PATROL,
		CHASE,
		ATTACK
	}

	[SerializeField]
	private ENEMY_STATE currentState = ENEMY_STATE.PATROL;

	private LineSight theLightSight = null;
	private UnityEngine.AI.NavMeshAgent theAgent = null;
	private Transform playerTransform = null;
	private Health playerHealth = null;
	public float maxDamage = 10.0f;

	public ENEMY_STATE CurrentState {
		get {
			return currentState;
		}

		set {
			CurrentState = value;
			StopAllCoroutines ();

			switch (currentState) {
			case ENEMY_STATE.PATROL:
				StartCoroutine (AIPatrol());
				break;
			case ENEMY_STATE.CHASE:
				StartCoroutine (AIChase());
				break;
			case ENEMY_STATE.ATTACK:
				StartCoroutine (AIAttack());
				break;
			}
		}
	}

	void Awake() {
		theLightSight = GetComponent<LineSight> ();
		theAgent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		playerTransform = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
	}

	private Transform destination = null;
	void Start() {
		GameObject[] randomDestinations = GameObject.FindGameObjectsWithTag ("Destination");
		destination = randomDestinations[Random.Range(0, randomDestinations.Length)].GetComponent<Transform>();

		CurrentState = ENEMY_STATE.PATROL;
	}

	public IEnumerator AIPatrol() {
		while (CurrentState == ENEMY_STATE.PATROL) {
			theLightSight.sensitivity = LineSight.SightSensitivity.STRICT;
			theAgent.Resume ();
			theAgent.SetDestination (destination.position);

			while (theAgent.pathPending) {
				yield return null;
			}

			if (theLightSight.canSeeTarget) {
				theAgent.Stop ();
				CurrentState = ENEMY_STATE.CHASE;
				yield break;
			}

			yield return null;
		}
	}

	public IEnumerator AIChase() {
		while (CurrentState == ENEMY_STATE.CHASE) {
			theLightSight.sensitivity = LineSight.SightSensitivity.LOOSE;

			theAgent.Resume ();
			theAgent.SetDestination (theLightSight.lastKnownSight);

			while (theAgent.pathPending) {
				yield return null;
			}

			if (theAgent.remainingDistance <= theAgent.stoppingDistance) {
				theAgent.Stop ();
				if (!theLightSight.canSeeTarget) {
					CurrentState = ENEMY_STATE.PATROL;
				} else {
					CurrentState = ENEMY_STATE.ATTACK;
				}
				yield break;
			}

			yield return null;
		}
	}

	public IEnumerator AIAttack() {
		while (CurrentState == ENEMY_STATE.CHASE) {
			theAgent.Resume ();
			theAgent.SetDestination (playerTransform.position);
			while (theAgent.pathPending) {
				yield return null;
			}
			if (theAgent.remainingDistance > theAgent.stoppingDistance) {
				CurrentState = ENEMY_STATE.CHASE;
				yield break;
			} else {
				playerHealth.HealthPoints -= maxDamage * Time.deltaTime;
			}

			yield return null;
		}
	}
}
