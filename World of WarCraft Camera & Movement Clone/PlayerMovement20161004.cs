/*
Script written by Tim A. Conner 

© 2016 Tim A. Conner.  All rights reserved. 

This script should be placed on the player's character's model.

V1: 6 Sep 2016 - WASD Movement
V2: 7 Sep 2016 - Jumping
20160908: 8 Sep 2016 - New Animation & Running Controls

Minor Updates:
2016910: 10 Sep 2016 - Both mouse buttons down to move forward.
20161004: Colliders change when going through jumping animations and normal movement.
*/

using System.Collections;
using UnityEngine;

public class PlayerMovement20161004 : MonoBehaviour {

	[Header ("--Movement")]

	[Tooltip ("Speed at which the character walks & strafes while walking.  Default is 0.03 ")]
	public float WalkSpeed = 0.03f;

	[Tooltip ("Speed at which the character Runs & strafes while running.  Default is 0.06")]
	public float RunSpeed = 0.075f;

	[Tooltip ("Speed at which the character rotates. Default is 3")]
	public float RotationSpeed = 3.0f;

	[Tooltip ("Rate at which the character turns when orbiting.")]
	public float OrbitSpeed = 5f;

	[Header ("--Jumping")]

	[Tooltip ("Speed at which the character jumps.")]
	public float JumpSpeed = 2.5f;
	//You want the jump speed to be high enough, so it is a jolting up jump, but the gravity to be high enough to bring you down quickly.
	[Tooltip ("Rate at which the character loses its velocity.")]
	public float JumpGravity = 0.5f;

	[Header ("--Colliders")]
	[Tooltip ("Colliders for normal movment vs. falling movement.  0 is female character prefab.  The rest are the colliders lower on her hierarchy.  Should be 6 size.")]
	public Collider[] Colliders;

	private float JumpingModifier = 0.0f; //When the player jumps, JumpGravity is added to this, nullifying the velocity added by jump speed, bringing the character to the ground.
	private bool IsJumping = false; //True if the player hit space bar--jumping.

	private bool IsWalking = false; //True if character is walking, if false, character is running.

	private Animator animator; //This is what we talk to to control animations.

	/*
		Animation State
		0 = Idle
		1 = Walking Forward
		2 = Walking Backwards
		3 = Left Turn
		4 = Right Turn
		5 = Strafe Running Left
		6 = Strafe Running Right
		7 = Jumping
		8 = Running
		9 = Falling
		10 = Landing
		11 = Strafe Walking Right 
		12 = Strafe Walking Left
		13 = Idle 2
	*/

	void Awake () {
		animator = GetComponent<Animator> (); //Get the animator component so we can set its animation states later
	}

	void FixedUpdate () {

		//Get WASD key input, or some other user set keys/input method.
		float HorizontalInput = Input.GetAxis ("Horizontal"); //Rotation
		float VerticalInput = Input.GetAxis ("Vertical"); //Transform
		float StrafeInput = Input.GetAxis ("Strafe"); //Strafe
		float JumpInput = Input.GetAxis ("Jump"); //Jump

		VerticalInput = (Input.GetMouseButton (0) && Input.GetMouseButton (1)) ? 1 : VerticalInput; //If mouse buttons are down, set the veritcal input to 1.

		if (IsGrounded (1f) && animator.GetCurrentAnimatorStateInfo (0).IsName ("Falling") && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Hit Ground")) { //Check if the character is falling and there is something underneath, then play "Hit Ground"
			animator.SetInteger ("Animation State", 10);
		} else if (!IsGrounded (0.5f) && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Hit Ground") && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) { //Check if anything is beneath character, if not, alert the character it is falling, then play "Falling"
			animator.SetInteger ("Animation State", 9);
		} else if (Input.GetAxis ("Jump") != 0 && IsGrounded (0.35f) && !animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) { //Character Jumps
			animator.SetInteger ("Animation State", 7);
			IsJumping = true;
			JumpingModifier = 0.0f; //reset the jumping modifier
		} else if (Input.GetAxis ("Strafe") < 0 && !IsJumping) { //Character is strafing left
			if (!IsWalking) { //Running
				animator.SetInteger ("Animation State", 5);
			} else { //Walking
				animator.SetInteger ("Animation State", 12);
			}
		} else if (Input.GetAxis ("Strafe") > 0 && !IsJumping) { //Character is strafing right
			if (!IsWalking) { //Running
				animator.SetInteger ("Animation State", 6);
			} else { //Walking
				animator.SetInteger ("Animation State", 11);
			}
		} else if (((Input.GetAxis ("Vertical") > 0) || (Input.GetMouseButton (0) && Input.GetMouseButton (1))) && !IsWalking && !IsJumping) { //Character is running
			animator.SetInteger ("Animation State", 8);
		} else if (((Input.GetAxis ("Vertical") > 0) || (Input.GetMouseButton (0) && Input.GetMouseButton (1))) && IsWalking && !IsJumping) { //Character is walking forward
			animator.SetInteger ("Animation State", 1);
		} else if (Input.GetAxis ("Vertical") < 0 && !IsJumping) { //Character is walking backwards
			animator.SetInteger ("Animation State", 2);
		} else if (Input.GetAxis ("Horizontal") > 0 && !IsJumping) { //Character is turning right
			animator.SetInteger ("Animation State", 3);
		} else if (Input.GetAxis ("Horizontal") < 0 && !IsJumping) { //Character is turning left
			animator.SetInteger ("Animation State", 4);
		} else { //Character is idling
			int RandomIdle = Random.Range (0, 3);
			if (RandomIdle >= 2) {
				animator.SetInteger ("Animation State", 13);
			} else {
				animator.SetInteger ("Animation State", 0);
			}
		}

		if (Input.GetKeyDown (KeyCode.KeypadDivide)) { //Toggle running and walking
			IsWalking = !IsWalking;
		}

		transform.Translate (0f, 0f, (!IsWalking) ? VerticalInput * RunSpeed : VerticalInput * WalkSpeed); // Translates the character.  By default, they are running.

		if (GUIUtility.hotControl == 0) {
			if (Input.GetMouseButton (0) || Input.GetMouseButton (1)) {
				Cursor.visible = false;
			} else {
				Cursor.visible = true;
			}
		}

		if (Input.GetMouseButton (0) && Input.GetMouseButton (1)) {

			if (Input.GetAxis ("Mouse X") < 0) {
				transform.Rotate (0f, Input.GetAxis ("Mouse X") * OrbitSpeed, 0f);
			}
			if (Input.GetAxis ("Mouse X") > 0) {
				transform.Rotate (0f, Input.GetAxis ("Mouse X") * OrbitSpeed, 0f);
			}
		} else {
			transform.Rotate (0f, HorizontalInput * RotationSpeed, 0f); //Rotates Character
		}

		transform.Translate ((!IsWalking) ? StrafeInput * RunSpeed : StrafeInput * WalkSpeed, 0f, 0f); // Strafes  Character

		if (IsJumping == true) {

			float AnimationTime = animator.GetCurrentAnimatorClipInfo (0) [0].clip.length * animator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			if ((AnimationTime >= 0.3f && animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump")) || animator.GetCurrentAnimatorStateInfo (0).IsName ("Falling")) {
				JumpingModifier += JumpGravity;
				GetComponent<Rigidbody> ().velocity += (JumpSpeed - JumpingModifier) * Vector3.up; //Jumps character
				if (JumpingModifier >= JumpSpeed) { //Check if the character is falling, then stop changing velocity.
					IsJumping = false;
				}
			}

		} else {

		}

		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Jump") || animator.GetCurrentAnimatorStateInfo (0).IsName ("Falling") || animator.GetCurrentAnimatorStateInfo (0).IsName ("Hit Ground")) {
			int ColliderLength = 0;
			foreach (Collider theCollider in Colliders) { //If we currently are in the midst of jumping, switch to feet, thigh & spine colliders.
				if (ColliderLength == 0) {
					theCollider.enabled = false;
				} else {
					theCollider.enabled = true;
				}
				ColliderLength++;
			}
		} else {
			int ColliderLength = 0;
			foreach (Collider theCollider in Colliders) { //If we currently are not in the midst of jumping, switch to capsule collider.
				if (ColliderLength == 0) {
					theCollider.enabled = true;
				} else {
					theCollider.enabled = false;
				}
				ColliderLength++;
			}
		}
	}

	bool IsGrounded (float dist = 0.25f) { //dist is distance below it checks.  Returns true if there is ground beneath the character
		return Physics.Raycast (new Vector3 (transform.position.x, transform.position.y + 0.25f, transform.position.z), -Vector3.up, dist);
	}

}