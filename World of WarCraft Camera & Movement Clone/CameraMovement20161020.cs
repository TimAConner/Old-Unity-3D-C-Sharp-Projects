/*
Script written by Tim A. Conner

Ãƒâ€šÃ‚Â© 2016 Tim A. Conner.  All rights reserved.

This script should be placed on the main camera.

2016090902: Basic Smooth Movement
20160910: Zoom in when object is between
20160911: Rotate when 2 mb held down.
20160912: Rotate when 2 mb held down && CameraZoom improvement
20160913: Camera Zoom fix
20160919: Camera Zoom fix
*/

using System.Collections;
using UnityEngine;

public class CameraMovement20161020 : MonoBehaviour {

	public Transform Character; //The main character to follow
	public Transform CharacterMaterialModel;

	private Vector3 CharactersHead;

	float CameraHeightAdd = 1;

	public float OriginalCameraHeightAdd = 1; //How high above the ground the camera is focused on
	public float CameraAngle = 0f;

	public float HeightSmoothness = 2;
	public float MovementSmoothness = 2;
	public float TurnSmoothness = 2;
	public float ResetCameraZoomSmoothness = 2; //Speed at which you go back to original positon after holding one mouse button to rotate
	public float ZoomInSmoothness = 15;
	public float ZoomOutSmoothness = 15;

	public float TwoMBTurnSmoothness = 50f;
	public float TwoMBTHeightSmoothness = 50f;

	public float OneMBCameraOrbitSmoothness = 25f;

	public float ScrollWheelZoomMin = 0.2f;
	public float ScrollWheelZoomMax = 10f;

	public int yMinLimit = -40;
	public int yMaxLimit = 80;

	public float Distance = 3;
	public float MaxDistance = 3;
	public float DistanceWhileZoomed = 3; //This is always being changed, so while you are zoomed in, the distance is being check against this to see if you can now zoom with scroll wheel if this is zoomed in past what the scroll wheel was.

	private float CameraPitch = 0.0f;

	public LayerMask CharacterLayer; //Layers to ignore

	private GameObject OriginalCameraPosition; //Position before camera is pushed in because of object
	private float OCPDistance = 3;
	private GameObject Checker;

	private bool ObjectInTheWay = false;

	private bool TwoMB = false; //True if both mouse buttons are held down.
	private bool OneMB = false; //true if only one mouse buttons is held down.

	private bool ZoomingIn = false;

	public RaycastHit ObjectInWay;
	public RaycastHit ObjectBehind;

	private float CameraXAngle = 0f;
	private float CameraYAngle = 0f;

	private Vector3 CameraAngles = new Vector3 ();

	public float PreviousCameraX = 0f;

	private bool SetAngles = false;

	private RaycastHit hit;

	public float DistanceToFadeInAt = 0.5f;

	/*
	To have better recognition if an object is in the way, think of the point that the camera is ray casting too.  Check out where that is.
	Bugs:
	Two MB Shake
	TWO MB Jump
	ONE MB is clicked going to TWO MB
	Zooming in from ground, goes to body.

	All rotations need to start right where the character currently is.
	*/

	private bool ResetCameraByRotate = false; //If true, the Camera will go back to its original position by rotation around the character.

	//rotate back to first position zoom in and out FIXED
	//rotate fix end  FIXED <<<<< AFter we zoom around with rotate around, then position it starts to rotate from jumps to right behind the character and then has to rotate the other direction.   I think problem is wehre it is jumping too, not the rotation.
	//hold x rotation FIXED
	//smooth zoom along ground

	//SLIGHT stutter of location when click.  It needs to starte xactly from where you are.

	[Header ("--	One Mouse Button Rotate")]
	[Header ("--	Two Mouse Button Movement")]
	[Header ("--	Movement Without Mouse")]

	[Header ("--	Rotation Max")]

	[Header ("--	Scrollwheel")]

	[Header ("--	Character")]

	[Header ("--	Fadein")]

	[Header ("--	Camera Position")]

	private int CurrentCharacterState = 0; //0 == normal, 1 = wasd not pressed and holding camera angle

	//How does each variable affect the character?

	/* 
	KNown Bugs

	When clicking, slight change, because it is not changing form current positon
	jagged zoom in along ground
	slight constant zoom out when infront  of object blocking path
	Walking  backwards from left click

	One MB to two MB movement screw up rotation.  Whenver one mb is triggered, it should take from the current position.
	*/

	void Start () {

		CameraHeightAdd = OriginalCameraHeightAdd;
		Vector3 angles = transform.eulerAngles;
		CameraXAngle = angles.x;
		//	Debug.Log("start" + CameraXAngle);

		CameraXAngle = angles.x;
		OriginalCameraPosition = new GameObject ();
		Checker = new GameObject ();
		OriginalCameraPosition.transform.name = "Camera-OriginalCameraPosition";
		Checker.transform.name = "Camera-Checker";
		OCPDistance = Distance;

	}

	void Update () {

		if (CurrentCharacterState == 1 && (Input.GetAxis ("Jump") != 0 || Input.GetAxis ("Strafe") != 0 || Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0)) { //If we were idling then movement was inputted, snap the camera back
			CurrentCharacterState = 0;
		}

		CharactersHead = Character.position;
		CharactersHead.y += CameraHeightAdd;

		if (Input.GetMouseButtonUp (0)) {

			PreviousCameraX = CameraXAngle;
			Debug.Log (CameraXAngle);
			Debug.Log ("here");
		}
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) && (!Input.GetMouseButtonUp (0) && Input.GetMouseButtonUp (1))) {

			//Debug.Log(transform.localEulerAngles.x);
			//CameraXAngle = PreviousCameraX;
			CameraXAngle = 15f;
			CameraYAngle = 0f;
			//Debug.Log(CameraXAngle);
			//Debug.Log("set");
			//	CameraYAngle = transform.eulerAngles.y;
			ResetCameraByRotate = false;
			SetAngles = true;
		}
		if (Input.GetMouseButton (0) && Input.GetMouseButton (1)) {
			CurrentCharacterState = 0; //If both buttons are held down set this to 0 so rotation from 1 mb is not held.

			CameraXAngle -= Input.GetAxis ("Mouse Y"); //Add mouse input to the angle
			CameraYAngle += Input.GetAxis ("Mouse X"); //Add mouse input to the angle
			//Debug.Log("-----------------" +transform.eulerAngles.x + " ---" +  CameraXAngle);
			TwoMB = true;
			OneMB = false;
			ResetCameraByRotate = false;
		} else {
			TwoMB = false;
		}
		if ((Input.GetMouseButton (0) && !Input.GetMouseButton (1)) || (!Input.GetMouseButton (0) && Input.GetMouseButton (1)) && (!Input.GetMouseButtonUp (0) && Input.GetMouseButtonUp (1))) {
			CameraXAngle -= Input.GetAxis ("Mouse Y"); //Add mouse input to the angle
			CameraYAngle += Input.GetAxis ("Mouse X"); //Add mouse input to the angle
			OneMB = true;
			TwoMB = false;
			ResetCameraByRotate = false;
		} else {
			OneMB = false;
		}

		if (Input.GetMouseButtonUp (0)) {
			ResetCameraByRotate = true;
		}

	}

	//Check a space behing if there is something between places.

	void FixedUpdate () {

		//Debug.Log((Vector3.Distance(CharactersHead, transform.position)-OriginalCameraHeightAdd));
		if (((Vector3.Distance (CharactersHead, transform.position) - OriginalCameraHeightAdd) <= DistanceToFadeInAt)) {

			Color color = CharacterMaterialModel.GetComponent<Renderer> ().material.color;
			float SmoothChange = (Vector3.Distance (CharactersHead, transform.position) - OriginalCameraHeightAdd);
			if (SmoothChange < -0.95) {
				SmoothChange = -5f;
			}
			color.a = SmoothChange;
			CharacterMaterialModel.GetComponent<Renderer> ().material.color = color;
			//CameraHeightAdd = ZoomInCameraHeightAdd;
			//CharacterMaterialModel.GetComponent<SkinnedMeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		} else {
			//Character.GetComponent<Renderer>().material.color = new Color(1.0f,1.0f.1.0f,1.0f); // 50 % transparent
			//	CameraHeightAdd = OriginalCameraHeightAdd;
			Color color = CharacterMaterialModel.GetComponent<Renderer> ().material.color;
			color.a = 1f;
			CharacterMaterialModel.GetComponent<Renderer> ().material.color = color;
		}

		if (SetAngles == true) {
			//Debug.Log("1 " + CameraXAngle + "-" + transform.eulerAngles.x);
			SetAngles = false;
			CameraXAngle = PreviousCameraX;
			CameraYAngle = transform.eulerAngles.y;
			//Debug.Log("1 " + CameraXAngle + "-" + transform.eulerAngles.x);
		}

		if (Round (Distance) != Round (MaxDistance)) {
			if (Physics.Linecast (CharactersHead, OriginalCameraPosition.transform.position, out ObjectBehind, CharacterLayer)) { //if checker can see character
				if (ObjectBehind.distance > 0.5f) {
					Distance = Mathf.Lerp (Distance, (ObjectBehind.distance), 1 * Time.fixedDeltaTime); //If not, pull out.
					ObjectInTheWay = true;
				}
			} else if (!Physics.Linecast (CharactersHead, OriginalCameraPosition.transform.position, out ObjectBehind, CharacterLayer)) {
				Distance = Mathf.Lerp (Distance, MaxDistance, ZoomOutSmoothness * Time.fixedDeltaTime); //If not, pull out.
			}
		}

		if (Round (Distance) != Round (ObjectInWay.distance - 0.5f) && Physics.Linecast (CharactersHead, transform.position, out ObjectInWay, CharacterLayer)) { //Object between character and camera
			Distance = Mathf.Lerp (Distance, (ObjectInWay.distance - 0.5f), ZoomInSmoothness * Time.fixedDeltaTime); //If not, pull out.

		}
		if (DistanceWhileZoomed < Distance) {
			Distance = DistanceWhileZoomed;
			MaxDistance = DistanceWhileZoomed;
		}
		if (ObjectInTheWay == true && Mathf.RoundToInt (Distance) == Mathf.RoundToInt (MaxDistance)) {
			ObjectInTheWay = false;
		}

		if (ObjectInTheWay == false && Input.GetAxis ("Mouse ScrollWheel") < 0) { // Zoom out if there is no object in the way
			Distance += (Distance < ScrollWheelZoomMax) ? ScrollWheelZoomMin : 0f;
			OCPDistance += (OCPDistance < ScrollWheelZoomMax) ? ScrollWheelZoomMin : 0f;
			MaxDistance += (MaxDistance < ScrollWheelZoomMax) ? ScrollWheelZoomMin : 0f;
			DistanceWhileZoomed += (DistanceWhileZoomed < ScrollWheelZoomMax) ? ScrollWheelZoomMin : 0f;
		} else if (ObjectInTheWay == false && Input.GetAxis ("Mouse ScrollWheel") > 0) { // Zoom in if there is no object in the way
			Distance -= (Distance > ScrollWheelZoomMin) ? 0.2f : 0f;
			OCPDistance -= (OCPDistance > ScrollWheelZoomMin) ? ScrollWheelZoomMin : 0f;
			MaxDistance -= (MaxDistance > ScrollWheelZoomMin) ? ScrollWheelZoomMin : 0f;
			DistanceWhileZoomed -= (DistanceWhileZoomed > ScrollWheelZoomMin) ? ScrollWheelZoomMin : 0f;
		}

		if (Input.GetAxis ("Mouse ScrollWheel") < 0) { // Zoom out no matter what.
			DistanceWhileZoomed += (DistanceWhileZoomed < ScrollWheelZoomMax) ? ScrollWheelZoomMin : 0f;
		} else if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // Zoom in
			DistanceWhileZoomed -= (DistanceWhileZoomed > 1) ? ScrollWheelZoomMin : 0f;
		}

		SetPosition (transform, Distance);
		SetPosition (OriginalCameraPosition.transform, OCPDistance);
	}

	void SetPosition (Transform Camera, float dist) {
		//height
		float CameraHeight = Camera.position.y; //The cameras height
		float CharactersHeight = 0.1f; //The height of the Character
		//transform
		Vector3 CameraTransform = Camera.position;
		Vector3 CharacterTransform = (Character.position - (Character.forward * dist)); //Put camera behind the character
		//rotate
		Quaternion CharacterRotation = new Quaternion ();

		//Debug.Log(PreviousCameraX + " " + CameraXAngle);

		//Debug.Log(( Vector3.Angle(Character.forward, Camera.position - Character.position)  + " " + Vector3.Angle(Character.forward, Camera.position - CharactersHead) ));

		//Debug.Log(Vector3.Angle(Character.forward, Camera.position - Character.position) + " " + transform.rotation.eulerAngles.x);

		//Make it somehow so that if behind the character and one condiiton does not cover it, it snaps back.

		if (!OneMB & !TwoMB && CurrentCharacterState == 0) { //NORMAL

			CameraHeight = Mathf.Lerp (CameraHeight, CharactersHeight + CameraHeightAdd, HeightSmoothness * Time.fixedDeltaTime); //Slowly change the height towards the character's height

			CharacterRotation = Character.rotation;
			CharacterRotation.eulerAngles = new Vector3 (Character.rotation.eulerAngles.x, CharacterRotation.eulerAngles.y, Character.rotation.eulerAngles.z);

			Vector3 toTarget = (Camera.position - Character.position).normalized;
			float relativePointA = Vector3.Dot (toTarget, Character.forward); //find direction to rotate back

			//We move to this location in a line.  We want to move to this location while skirting around the character.
			if (ResetCameraByRotate == true) {
				//	

				Vector3 relativePoint = Character.InverseTransformPoint (Camera.position); //find direction to rotate back

				if (relativePoint.x < 0.0) { //Left
					Camera.position = CharactersHead + (Camera.position - CharactersHead).normalized * dist; //set the distance we want to be away from the cahracter
					Camera.RotateAround (CharactersHead, Vector3.down, ResetCameraZoomSmoothness * Time.fixedDeltaTime);
				} else if (relativePoint.x > 0.0) { //right
					Camera.position = CharactersHead + (Camera.position - CharactersHead).normalized * dist; //set the distance we want to be away from the cahracter
					Camera.RotateAround (CharactersHead, Vector3.up, ResetCameraZoomSmoothness * Time.fixedDeltaTime);
				}
				if (Vector3.Angle (Character.forward, Camera.position - Character.position) > 130 || (((transform.rotation.eulerAngles.x > 300 && transform.rotation.eulerAngles.x<360) ? 0 : transform.rotation.eulerAngles.x)> 35 && Vector3.Angle (Character.forward, Camera.position - Character.position) > 90 && Vector3.Angle (Character.forward, Camera.position - Character.position) < 140)) { //if we are  behind character, stop rotating around to get back to position.
					ResetCameraByRotate = false;

				}

			} else {
				/*
				CharacterTransform = (Character.position-(rotation * Character.forward*dist));
				Camera.rotation = Quaternion.Lerp(Camera.rotation, CharacterRotation, TurnSmoothness * Time.fixedDeltaTime);
				CameraTransform = Vector3.Lerp(CameraTransform, CharacterTransform, MovementSmoothness * Time.fixedDeltaTime);
				Camera.position  = new Vector3(CameraTransform.x, CameraHeight, CameraTransform.z);*/

				CameraHeight = CharactersHeight + CameraHeightAdd;

				PreviousCameraX = ClampAngle (PreviousCameraX, yMinLimit, yMaxLimit);
				Quaternion rotation = Quaternion.AngleAxis (PreviousCameraX, Character.right); //Translate mouse input into an angel

				CharacterTransform = Character.position - (rotation * Character.forward * dist); // PROBLEM IS THE POSITION THAT IS JUMPED TO
				//CharacterTransform.y += CameraHeight;

				CharacterRotation = Character.rotation;
				CharacterRotation.eulerAngles = new Vector3 (PreviousCameraX, CharacterRotation.eulerAngles.y, Character.rotation.eulerAngles.z);
				Camera.rotation = Quaternion.Lerp (Camera.rotation, CharacterRotation, TurnSmoothness * Time.fixedDeltaTime);

				CameraTransform.x = Mathf.Lerp (CameraTransform.x, CharacterTransform.x, MovementSmoothness * Time.fixedDeltaTime);
				CameraTransform.z = Mathf.Lerp (CameraTransform.z, CharacterTransform.z, MovementSmoothness * Time.fixedDeltaTime);
				CameraTransform.y = Mathf.Lerp (CameraTransform.y, CharacterTransform.y + CameraHeight, HeightSmoothness * Time.fixedDeltaTime);

				Camera.position = new Vector3 (CameraTransform.x, CameraTransform.y, CameraTransform.z);

			}

		} else if (TwoMB && !OneMB && CurrentCharacterState == 0) { //TWO MB

			//CameraYAngle = 0; //make sure the Y Angle does not carry over from 1 mb rotate

			CameraHeight = CharactersHeight + CameraHeightAdd;

			CameraXAngle = ClampAngle (CameraXAngle, yMinLimit, yMaxLimit);
			Quaternion rotation = Quaternion.AngleAxis (CameraXAngle, Character.right); //Translate mouse input into an angel
			Quaternion rotationB = Quaternion.AngleAxis (CameraYAngle, Character.up); //Translate mouse input into an angel
			CharacterTransform = Character.position - (rotation * Character.forward * dist);
			CharacterTransform.y += CameraHeight;
			CameraTransform = CharacterTransform; //Set the camera location to the location we have calculated

			CharacterRotation = Character.rotation;
			CharacterRotation.eulerAngles = new Vector3 (CameraXAngle, CharacterRotation.eulerAngles.y, Character.rotation.eulerAngles.z);
			Camera.rotation = Quaternion.Lerp (Camera.rotation, CharacterRotation, TwoMBTurnSmoothness * Time.fixedDeltaTime);

			Camera.position = new Vector3 (CameraTransform.x, CameraTransform.y, CameraTransform.z);

		} else if (CurrentCharacterState == 1 || (OneMB && !TwoMB)) { //ONE MB

			//Debug.Log( Vector3.Angle(Character.forward, Camera.position));

			//Debug.Log(CameraYAngle);

			CurrentCharacterState = 1; //Set character state to looking around

			CameraHeight = CharactersHeight + CameraHeightAdd;

			CameraXAngle = ClampAngle (CameraXAngle, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler (CameraXAngle, CameraYAngle, 0);
			CharacterTransform = Character.position - (rotation * Vector3.forward * dist);
			CharacterTransform.y += CameraHeight;
			CameraTransform = CharacterTransform; //Set the camera location to the location we have calculated

			//	Camera.LookAt(new Vector3(Character.position.x, CameraHeight, Character.position.z));
			Quaternion lookRotation = Quaternion.LookRotation ((new Vector3 (Character.position.x, Character.position.y + CameraHeight, Character.position.z) - transform.position).normalized);

			//over time
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.fixedDeltaTime * 1);

			//instant
			transform.rotation = lookRotation;

			Camera.position = CameraTransform; // Vector3.Lerp(Camera.position, new Vector3(CameraTransform.x, CameraTransform.y, CameraTransform.z), Time.fixedDeltaTime*OneMBCameraOrbitSmoothness);
		}

	}

	float Round (float number) {
		return Mathf.Round (number * 1000000f) / 1000000f;
	}

	private static float ClampAngle (float angle, float min, float max) {
		if (angle < -360) {
			angle += 360;
		}
		if (angle > 360) {
			angle -= 360;
		}
		return Mathf.Clamp (angle, min, max);
	}

	/*void OnCollisionStay(Collision collision) {
	Distance = Mathf.Lerp(Distance, Distance-0.1f, ZoomInSmoothness*Time.deltaTime);//If not, pull out.
	}*/

}