﻿using UnityEngine;
using System.Collections;

public class StairManager : MonoBehaviour {
	public enum ON_STAIR_STATE {
		Up,
		Down,
		Not
	};

	public enum ON_STAIR_AREA {
		PrepUp,
		PrepDown,
		onStair, // should be deprecated
		Not
	};
	public ON_STAIR_AREA  onStairArea = ON_STAIR_AREA.Not;
	public ON_STAIR_STATE onStairState = ON_STAIR_STATE.Not;

	private float upDownStairAnimInterval = 0.1f;
	private PlayerController pc;
	private Animator animator;
	private float prepXcenter; // Used only for preparation state, record the place player need to go 
	private Globals.STAIR_FACING Stairfacing; // true for right, false for left

	void Start () {
		onStairState = ON_STAIR_STATE.Not;
		pc = GetComponent<PlayerController> ();
		animator = GetComponent<Animator> ();
	}

	// return whether player is involved with stair
	public bool isInStairArea () {
		return onStairArea != ON_STAIR_AREA.Not || onStairState != ON_STAIR_STATE.Not;
	}
	// return true if the object is on stair
	public bool isOnStair() {
		return onStairState != ON_STAIR_STATE.Not;
	}
	public bool isWalkingOnStair() {
		return onStairState == ON_STAIR_STATE.Up || onStairState == ON_STAIR_STATE.Down;
	}
	// change facing of the character
	public void switchToState(ON_STAIR_STATE state_in) {
		if (onStairState == ON_STAIR_STATE.Not) {
			animator.SetBool("onStair", true);
		}
		else if (state_in == ON_STAIR_STATE.Not) {
			animator.SetBool("onStair", false);
		}
		// UP to down or down to up
		else if (onStairState == ON_STAIR_STATE.Up && state_in == ON_STAIR_STATE.Down) {

			pc.Flip();
		}
		else if (onStairState == ON_STAIR_STATE.Down && state_in == ON_STAIR_STATE.Up) {
			pc.Flip();
		}
		onStairState = state_in;

	}
	// When in prep state, call this function to change the state
	public void switchToState(ON_STAIR_AREA state_in, float Xcenter_in, Globals.STAIR_FACING Stairfacing_in) {
		prepXcenter = Xcenter_in;
		Stairfacing = Stairfacing_in;
		onStairArea = state_in;

	}

	public void tryGoUpStair () {
		if (onStairState == ON_STAIR_STATE.Down || onStairState == ON_STAIR_STATE.Up) {
			if (onStairArea == ON_STAIR_AREA.PrepDown) {
				if (!animator.GetBool("UpStair"))
					StartCoroutine (GoUpStairToNormal ());
			}
			else {
				if (!animator.GetBool("UpStair"))
					StartCoroutine (GoUpStair ());
			}
			
		}
		else if (onStairState == ON_STAIR_STATE.Not && onStairArea == ON_STAIR_AREA.PrepUp) {
			transform.position = new Vector2(prepXcenter, transform.position.y);
			adjustFacingToStair();
			if (animator.GetBool("UpStair"))
				Debug.LogError("Animator in impossible state, when in prep up area, but in up animation state");
			StartCoroutine (GoUpStair ());
			// TODO
		


		}


	}
	public void tryGoDownStair () {
		// inside the area of prep_up, could resolve to normal state
		if (onStairState == ON_STAIR_STATE.Down || onStairState == ON_STAIR_STATE.Up) {
			if (onStairArea == ON_STAIR_AREA.PrepUp) {
				if (!animator.GetBool("DownStair"))
					StartCoroutine (GoDownStairToNormal ());
			}
			else {
				if (!animator.GetBool("DownStair"))
					StartCoroutine (GoDownStair ());
			}

		}
		else if (onStairState == ON_STAIR_STATE.Not && onStairArea == ON_STAIR_AREA.PrepDown) {
			// TODO
			onStairArea = ON_STAIR_AREA.onStair;
		}


	}
	
	private IEnumerator GoUpStair() {
		
		animator.SetBool ("UpStair", true);
		switchToState (StairManager.ON_STAIR_STATE.Up);
		// TODO Do Lerp here 
		int facing = pc.facingRight ? 1 : -1;
		
		transform.position = new Vector2 (
			transform.position.x + facing * Globals.StairStepLength,
			transform.position.y + Globals.StairStepLength
			);
		
		yield return new WaitForSeconds (upDownStairAnimInterval);
		
		animator.SetBool ("UpStair", false);
	}
	
	private IEnumerator GoDownStair() {
		
		animator.SetBool ("DownStair", true);
		switchToState (StairManager.ON_STAIR_STATE.Down);
		// TODO Do Lerp here 
		int facing = pc.facingRight ? 1 : -1;
		
		transform.position = new Vector2 (
			transform.position.x + facing * Globals.StairStepLength,
			transform.position.y + -1 * Globals.StairStepLength
			);
		
		yield return new WaitForSeconds (upDownStairAnimInterval);
		
		animator.SetBool ("DownStair", false);
	}
	private IEnumerator GoDownStairToNormal() {
		StartCoroutine (GoDownStair ());
		yield return new WaitForSeconds (upDownStairAnimInterval);
		switchToState(ON_STAIR_STATE.Not);
	}

	private IEnumerator GoUpStairToNormal() {
		StartCoroutine (GoUpStair ());
		yield return new WaitForSeconds (upDownStairAnimInterval);
		switchToState(ON_STAIR_STATE.Not);
	}

	private void adjustFacingToStair() {
		if (Stairfacing == Globals.STAIR_FACING.Right && !pc.facingRight) {
			pc.Flip();
		}
		else if (Stairfacing == Globals.STAIR_FACING.Left && pc.facingRight) {
			pc.Flip();
		}
	}

}