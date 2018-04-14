using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class MainGame : MonoBehaviour {

	//Grid & Players
	public List<HexGridClass> HexGrid = new List<HexGridClass> (); //The list hexs
	public List<SettingsClass> Settings = new List<SettingsClass> ();

	public PlayerClass CurrentPlayer;
	private string[] HexInfo; //all the raw hex data before its turned into the hex grid
	public string CurrentPlayerName = "";
	public List<PlayerClass> PlayerList = new List<PlayerClass> ();
	//Material Batching
	public Material Farmland;
	public Material Forest;
	public Material Water;
	public Material Mountain;
	public Material Hill;

	//Buildings
	public GameObject HallBuilding;
	public GameObject OneHall;
	public GameObject TwoHall;
	public GameObject ForestModel;
	public GameObject LargerForestModel;
	public GameObject HillModel;
	public GameObject FarmlandModel;
	public GameObject HexPrefab; //the prefab for the hex model
	public GameObject MountainModel;
	public GameObject SmokeModel;

	public Texture woodTexture;
	public Texture actionTexture;
	public Texture coinTexture;
	public Texture foodTexture;
	public Texture stoneTexture;

	public Texture resourcesBackground;

	public Texture2D moveCursorTexture;
	public Texture2D attackCursorTexture;

	//UNITS
	public GameObject Unit;
	public GameObject Boat;
	UnitClass currentUnit = new UnitClass ();
	UnitClass otherUnit = new UnitClass ();
	string UnitAction = "";

	//GUI
	private Rect loginRect = new Rect ((Screen.width / 2) - 100, (Screen.height / 2) - 65, 200, 130);
	private Rect playerRect = new Rect (0, 25, 175, Screen.height);
	private Rect errorScreenRect = new Rect (Screen.width / 2 - 100, Screen.height / 2 - 125, 200, 250);
	private Rect upgradeRect = new Rect (25, 25, Screen.width - 50, Screen.height - 50);
	private Rect armyRect = new Rect (25, 25, Screen.width - 50, Screen.height - 50);
	private Rect armywaveRect = new Rect (25, 25, Screen.width - 50, Screen.height - 50);

	public bool upgradeShowing = false;
	public bool alliesShowing = false;
	public bool tradingShowing = false;
	public bool selectingHex = false;
	public bool armyShowing = false;
	public bool armywaveShowing = false;

	bool selectingA = false;

	private UpgradeClass currentlySelectedUpgrade = null;

	bool test = false;
	// public string Changes = "";

	//PHP Form handling
	private string LoginData;
	private string Login = "Username";
	private string Password = "Password";
	private string GameInfo = "Map";
	private string GameName;
	private string PlayerRefrence;

	public bool firstMake = true;

	//Eror Screen
	private string ErrorText;
	private bool ErrorScreenShowing = false;
	private HexGridClass currentlySelectedGrid;
	bool OwnerView = false;

	int unitId = 0;

	string Message = "Write a message...";
	ResourcesClass TradingGive = new ResourcesClass ();
	ResourcesClass TradingWant = new ResourcesClass ();
	string PlayerTradingWith = "";
	GridPoint WantHex = new GridPoint ();
	GridPoint GiveHex = new GridPoint ();

	UnitClass unitToBeMade = new UnitClass ();
	int Horsemen = 0; //0
	int Archer = 0; //1
	int Pikemen = 0; //2

	void Start () {
		Settings.Add (new SettingsClass ("TurnNumber", "0"));
	}
	string FindSettings (string SettingName) {
		SettingsClass value = Settings.Find (Setting => Setting.Name == SettingName);
		return value.Value;
	}
	bool hasUpgrade (string UpgradeName, PlayerClass thePlayer) {
		SettingsClass value = thePlayer.Upgrades.Find (Upgrade => Upgrade.Name == UpgradeName);
		if (value != null) {
			return true;
		} else {
			return false;
		}
	}
	void Update () {

		if (currentlySelectedGrid != null) {
			if (selectingHex == true) {
				if (selectingA == true) {
					if (currentlySelectedGrid.Owner == CurrentPlayer.Name) {
						TradingGive.Hex = new GridPoint (currentlySelectedGrid);
						selectingHex = false;
					}
				} else {
					if (currentlySelectedGrid.Owner == PlayerTradingWith) {
						TradingWant.Hex = new GridPoint (currentlySelectedGrid);
						selectingHex = false;
					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			foreach (HexGridClass theHex in HexGrid) //Draw the hexes
			{
				if (CanTheySee (theHex, CurrentPlayer.Name, 1)) {
					GameObject theHexB = GameObject.Find ("Hex: " + theHex.x + "-" + theHex.y);
					foreach (Transform child in theHexB.transform) {
						if (!String.IsNullOrEmpty (theHex.Owner)) {
							child.transform.GetComponent<Renderer> ().material.color = StringToColor (GetPlayer (theHex.Owner).Color);
							HexHoverScript hover = child.transform.GetComponent<HexHoverScript> ();
							hover.StartingColor = StringToColor (GetPlayer (theHex.Owner).Color);
						}
					}
				}
			}

			OwnerView = true;
		}

		if (Input.GetKeyDown (KeyCode.P)) {
			foreach (HexGridClass theHex in HexGrid) //Draw the hexes
			{
				if (CanTheySee (theHex, CurrentPlayer.Name, 1)) {
					GameObject theHexB = GameObject.Find ("Hex: " + theHex.x + "-" + theHex.y);
					foreach (Transform child in theHexB.transform) {
						if (!String.IsNullOrEmpty (theHex.Owner)) {
							child.transform.GetComponent<Renderer> ().material.color = Color.white;
							HexHoverScript hover = child.transform.GetComponent<HexHoverScript> ();
							hover.StartingColor = Color.white;
						}
					}
				}
			}
			OwnerView = false;
		}

		/*if(Input.GetKeyDown(KeyCode.T)){
		if(currentlySelectedGrid != null && HasResources(2.5f, 2.0f, 0.0f,0.0f,2.0f)  && currentlySelectedGrid.Building == "Hall" && currentlySelectedGrid.Owner == CurrentPlayer.Name){    
		/*AddSoldier();
		armyShowing = true;
			Horsemen = 0;
								Archer = 0;
								Pikemen = 0;
		}
		}*/

		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl == 0) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				UnitAction = "";
				currentUnit.Selected = false;
				string hexName = hit.transform.gameObject.name;

				string hexCordinates = hexName.Substring (hexName.IndexOf ("("), hexName.IndexOf (")") - hexName.IndexOf ("(") + 1);

				int y = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("-") + 1, hexName.IndexOf (")") - hexName.IndexOf ("-") - 1));

				int x = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("(") + 1, hexName.IndexOf ("-") - hexName.IndexOf ("(") - 1));

				currentlySelectedGrid = PointData (x, y);

				//int x = System.Convert.ToInt32(hexCordinates.Substring(hexName.IndexOf("(") +1, hexName.IndexOf("-") - hexName.IndexOf("(")-1));
			}
		}
		if (Input.GetMouseButtonDown (1) && GUIUtility.hotControl == 0) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				currentUnit.Selected = false;
				string hexName = hit.transform.gameObject.name;
				string hexCordinates = hexName.Substring (hexName.IndexOf ("("), hexName.IndexOf (")") - hexName.IndexOf ("(") + 1);

				int y = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("-") + 1, hexName.IndexOf (")") - hexName.IndexOf ("-") - 1));

				int x = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("(") + 1, hexName.IndexOf ("-") - hexName.IndexOf ("(") - 1));

				// List<GridPoint> hexes = GetHexesAround(x, y, 1);
				if (UnitAction == "Move") {
					float Distance = GetDistance (currentlySelectedGrid, PointData (x, y));
					if (CurrentPlayer.TurnPoints >= Distance && Distance <= 1 && Distance > 0.5 && PointData (x, y).Units.Count == 0) {
						if ((PointData (x, y).Type == "Water" && hasUpgrade ("Ship Making", CurrentPlayer)) || PointData (x, y).Type != "Water" && PointData (x, y).Type != "Mountain") {
							if ((currentlySelectedGrid.Owner != CurrentPlayer.Name) || !currentlySelectedGrid.Viewers.Contains (CurrentPlayer.Name)) {
								currentlySelectedGrid.Viewed.Add (CurrentPlayer.Name);
							}
							CurrentPlayer.TurnPoints -= Distance;
							currentlySelectedGrid.Changed = true;
							PointData (x, y).Changed = true;
							currentlySelectedGrid.Units.Remove (currentUnit);
							PointData (x, y).Units.Add (currentUnit);
							currentlySelectedGrid = PointData (x, y);
							CreateGrid (HexGrid);
						} else {
							PointData (x, y).Viewed.Add (CurrentPlayer.Name);
							PointData (x, y).Changed = true;

							ShowErrorScreen ("You do not have the Ship Making Upgrade or You can't move over mountains.");
							CreateGrid (HexGrid);
						}
					} else {
						ShowErrorScreen ("You do not have enough turn points to move this far or it is greater than 1 or another unit currently occupies the hex.");
					}

				} else if (UnitAction == "Attack") {
					Debug.Log ("Attack");
					float Distance = GetDistance (currentlySelectedGrid, PointData (x, y));
					if (CurrentPlayer.TurnPoints >= Distance) {

						//if(PointData(x, y).Units.Count > 0){
						//PointData(x, y).Units[0].Count -= (int)(UnityEngine.Random.Range(0, currentUnit.Count));
						Debug.Log ("Attack 2");
						//currentUnit.Count -= (int)(UnityEngine.Random.Range(0f, PointData(x, y).Units[0].Count ));

						Battle (currentUnit, PointData (x, y));

						/*if(PointData(x, y).Units[0].Count <= 0){
						PointData(x, y).Units.Remove(PointData(x, y).Units[0]);
						UnitAction = "";
						currentUnit = null;
						PointData(x, y).Changed = true;
						currentlySelectedGrid.Changed = true;
						CreateGrid(HexGrid);
						} else {
						UnitAction = "";
						currentUnit.Selected = false;
						}*/
						//}

					}
				} else {
					currentlySelectedGrid = PointData (x, y);
				}

				//int x = System.Convert.ToInt32(hexCordinates.Substring(hexName.IndexOf("(") +1, hexName.IndexOf("-") - hexName.IndexOf("(")-1));
			}
		}
	}

	bool CanTheySee (HexGridClass theHex, string Name, int options = 0) { //the hex is hex to see. Name is current player name
		if (options == 0) {
			if ((theHex.Owner == CurrentPlayer.Name) || theHex.Viewers.Contains (CurrentPlayer.Name) || CurrentPlayer.Name == "ServerView" || CheckUnit ("Soldier", theHex, 1)) {
				return true;
			} else {
				return false;
			}
		} else {
			if ((theHex.Owner == CurrentPlayer.Name) || theHex.Viewers.Contains (CurrentPlayer.Name) || CurrentPlayer.Name == "ServerView" || CheckUnit ("Soldier", theHex, 1) || theHex.Viewed.Contains (CurrentPlayer.Name)) {
				return true;
			} else {
				return false;
			}
		}
	}

	void CreateGrid (List<HexGridClass> Hexes, int options = 0) {

		GameObject[] gameObjects;
		gameObjects = GameObject.FindGameObjectsWithTag ("Map");
		/*foreach(GameObject i in gameObjects){
		         Destroy(i);
		 
		 }*/

		Hexes = Hexes.OrderBy (xHexVar => xHexVar.x).ThenBy (xHexVar => xHexVar.y).ToList (); //re order list

		int ShiftRows = 0;
		foreach (HexGridClass theHex in Hexes) //Draw the hexes
		{

			/*HexGridClass CheckForDeadUnits = PointData(theHex.x, theHex.y);
			if(CheckForDeadUnits.Units != null && CheckForDeadUnits.Units.Count >0){
			if(CheckForDeadUnits.Units[0].Wave1.Count <= 0 && CheckForDeadUnits.Units[0].Wave2.Count <= 0 && CheckForDeadUnits.Units[0].Wave3.Count <= 0 && CheckForDeadUnits.Units[0].Soldiers.Count <= 0){
			Debug.Log("Clear");
			CheckForDeadUnits.Units.Remove(CheckForDeadUnits.Units[0]);
			}
			}*/

			if (theHex.Viewed.Contains (CurrentPlayer.Name) && (theHex.Owner == CurrentPlayer.Name || theHex.Viewers.Contains (CurrentPlayer.Name))) {
				theHex.Viewed.Remove (CurrentPlayer.Name);
			}
			if (theHex.Owner == CurrentPlayer.Name && theHex.Viewers.Contains (CurrentPlayer.Name)) {
				theHex.Viewers.Remove (CurrentPlayer.Name);
			}
			ShiftRows++;
			bool canFind = HexGrid.Exists (Hex => Hex == theHex);
			if (theHex.Changed == true || firstMake == true) {

				Destroy (GameObject.Find ("Hex: " + theHex.x + "-" + theHex.y));

				GameObject hexInstance = null;
				GameObject buildingInstance = null;
				GameObject naturalInstance = null;
				GameObject unitInstance = null;

				if (theHex.y % 2 == 0) {
					hexInstance = (GameObject) Instantiate (HexPrefab, new Vector3 ((float) (theHex.x + (2 * 0.5)), 0, (float) (theHex.y * 0.85)), HexPrefab.transform.rotation);
					hexInstance.name = "Hex: " + theHex.x + "-" + theHex.y;
				} else {
					hexInstance = (GameObject) Instantiate (HexPrefab, new Vector3 ((float) (theHex.x + (1 * 0.5)), 0, (float) (theHex.y * 0.85)), HexPrefab.transform.rotation);
					hexInstance.name = "Hex: " + theHex.x + "-" + theHex.y;
				}

				foreach (Transform child in hexInstance.transform) { //go through chiildren to chagne their color

					child.name = "Hex (" + theHex.x + "-" + theHex.y + ")"; //give it the name with its cordinates
					if (CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name)) //if it is the users land, 1=OWNER 2=Viewers 3=VIEWED
					{
						if (theHex.Type == "Farmland") {
							child.GetComponent<Renderer> ().material = Farmland;
						} else if (theHex.Type == "Forest") {
							child.GetComponent<Renderer> ().material = Forest;
						} else if (theHex.Type == "Plain") {
							child.GetComponent<Renderer> ().material = Forest;
						} else if (theHex.Type == "Water") {
							child.GetComponent<Renderer> ().material = Water;
							child.position = new Vector3 (hexInstance.transform.position.x, hexInstance.transform.position.y - 0.05f, hexInstance.transform.position.z);
							hexInstance.transform.position = new Vector3 (hexInstance.transform.position.x, hexInstance.transform.position.y - 0.05f, hexInstance.transform.position.z);
						} else if (theHex.Type == "Mountain") {
							child.transform.GetComponent<Renderer> ().material = Mountain;
						} else if (theHex.Type == "Hill") {
							child.GetComponent<Renderer> ().material = Hill;
						}
					} else //if you dont own/view it
					{

						/* if (theHex.Type == "Water")
                        {
                            child.renderer.material = Water;
							child.position = new Vector3(hexInstance.transform.position.x,hexInstance.transform.position.y-0.05f,hexInstance.transform.position.z);
							hexInstance.transform.position = new Vector3(hexInstance.transform.position.x,hexInstance.transform.position.y-0.05f,hexInstance.transform.position.z);
                        }
                        else{*/
						child.GetComponent<Renderer> ().material.color = Color.black;
						//}

					}

					if (OwnerView == true) {
						if (!String.IsNullOrEmpty (theHex.Owner)) {
							child.transform.GetComponent<Renderer> ().material.color = StringToColor (GetPlayer (theHex.Owner).Color);
							HexHoverScript hover = child.transform.GetComponent<HexHoverScript> ();
							hover.StartingColor = StringToColor (GetPlayer (theHex.Owner).Color);

						}
					}
					if (theHex.Viewed.Contains (CurrentPlayer.Name) && !theHex.Viewers.Contains (CurrentPlayer.Name) && theHex.Owner != CurrentPlayer.Name && !CheckUnit ("Soldier", theHex, 1)) {
						buildingInstance = (GameObject) Instantiate (SmokeModel, new Vector3 ((float) (child.transform.position.x), 0.5f, (float) child.transform.position.z), SmokeModel.transform.rotation);
						//child.GetComponent<Renderer>().material.color = Color.gray;
					}
					//BUILDINGS / MODELS

					if (theHex.Building == "Hall" && theHex.Type != "Farmland" && theHex.Type != "Hill" && (CanTheySee (theHex, CurrentPlayer.Name))) {
						buildingInstance = (GameObject) Instantiate (HallBuilding, new Vector3 ((float) (child.transform.position.x), 0.2f, (float) child.transform.position.z), HallBuilding.transform.rotation);

						foreach (Transform childB in buildingInstance.transform) {
							if (childB.name == "Small") {

								foreach (Transform childC in childB) {
									if (childC.name == "RoofPart") {
										childC.GetComponent<Renderer> ().material.color = StringToColor (GetPlayer (theHex.Owner).Color);
									}
								}
							}
						}
					}
					if (theHex.Building == "Hall" && theHex.Type == "Farmland" && (CanTheySee (theHex, CurrentPlayer.Name))) {
						buildingInstance = (GameObject) Instantiate (OneHall, new Vector3 ((float) (child.transform.position.x + (theHex.Rotation * 0.001f)), 0.2f, (float) child.transform.position.z + (theHex.RotationB * 0.001f)), OneHall.transform.rotation);
						foreach (Transform childB in buildingInstance.transform) {
							if (childB.name == "Small") {

								foreach (Transform childC in childB) {
									if (childC.name == "RoofPart") {
										childC.GetComponent<Renderer> ().material.color = StringToColor (GetPlayer (theHex.Owner).Color);
									}
								}
							}
						}
					}
					if (theHex.Type == "Hill" && ((CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name)))) {
						naturalInstance = (GameObject) Instantiate (HillModel, new Vector3 ((float) (child.transform.position.x), 0.2f, (float) child.transform.position.z), HillModel.transform.rotation);
						naturalInstance.transform.localEulerAngles = new Vector3 (LargerForestModel.transform.rotation.x, theHex.Rotation, LargerForestModel.transform.rotation.x);
					}
					if (theHex.Building == "Hall" && theHex.Type == "Hill" && (CanTheySee (theHex, CurrentPlayer.Name))) {
						buildingInstance = (GameObject) Instantiate (TwoHall, new Vector3 ((float) (child.transform.position.x), 0.2f, (float) child.transform.position.z), TwoHall.transform.rotation);
						foreach (Transform childB in buildingInstance.transform) {
							if (childB.name == "Small") {

								foreach (Transform childC in childB) {
									if (childC.name == "RoofPart") {
										childC.GetComponent<Renderer> ().material.color = StringToColor (GetPlayer (theHex.Owner).Color);
									}
								}
							}
						}
					}
					if (theHex.Type == "Forest" && theHex.Building == "Hall" && ((CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name)))) {
						naturalInstance = (GameObject) Instantiate (ForestModel, new Vector3 ((float) (child.transform.position.x - 0.15f), 0.4f, (float) child.transform.position.z - 0.1f), ForestModel.transform.rotation);
					} else if (theHex.Type == "Forest" && theHex.Building == "None" && (CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name))) {
						naturalInstance = (GameObject) Instantiate (LargerForestModel, new Vector3 ((float) (child.transform.position.x), 0.4f, (float) child.transform.position.z), LargerForestModel.transform.rotation);
						naturalInstance.transform.localEulerAngles = new Vector3 (LargerForestModel.transform.rotation.x, theHex.Rotation, LargerForestModel.transform.rotation.x);
					}
					if (theHex.Type == "Farmland" && (CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name))) {
						naturalInstance = (GameObject) Instantiate (FarmlandModel, new Vector3 ((float) (child.transform.position.x), 0.1f, (float) child.transform.position.z), FarmlandModel.transform.rotation);
					}
					if (theHex.Type == "Mountain" && (CanTheySee (theHex, CurrentPlayer.Name) || theHex.Viewed.Contains (CurrentPlayer.Name))) {
						naturalInstance = (GameObject) Instantiate (MountainModel, new Vector3 ((float) (child.transform.position.x), 0.19f, (float) child.transform.position.z), MountainModel.transform.rotation);
						naturalInstance.transform.localEulerAngles = new Vector3 (MountainModel.transform.rotation.x, theHex.Rotation, MountainModel.transform.rotation.x);
					}
					if (CanTheySee (theHex, CurrentPlayer.Name)) {
						if (theHex.Type != "Water") {
							foreach (UnitClass x in theHex.Units) {
								if (theHex.Building == "Hall") {
									unitInstance = (GameObject) Instantiate (Unit, new Vector3 ((float) (child.transform.position.x + UnityEngine.Random.Range (-0.05f, 0.05f)), 0.1f, (float) child.transform.position.z + 0.2f), Unit.transform.rotation);
								} else if (theHex.Type == "Mountain") {
									unitInstance = (GameObject) Instantiate (Unit, new Vector3 ((float) (child.transform.position.x + UnityEngine.Random.Range (-0.05f, 0.05f)), 0.8f, (float) child.transform.position.z), Unit.transform.rotation);
								} else {
									unitInstance = (GameObject) Instantiate (Unit, new Vector3 ((float) (child.transform.position.x + UnityEngine.Random.Range (-0.05f, 0.05f)), 0.1f, (float) child.transform.position.z), Unit.transform.rotation);
								}

								PlayerClass owner = PlayerList.Find (Player => Player.Name == x.Owner);
								if (owner != null) {
									foreach (Transform childD in unitInstance.transform) {
										if (childD.name == "Shield") {
											childD.GetComponent<Renderer> ().material.color = StringToColor (owner.Color);
										}
									}

								}
								unitInstance.transform.name = "Soldiers";
								unitInstance.transform.parent = child;
							}
						} else {
							foreach (UnitClass x in theHex.Units) {

								unitInstance = (GameObject) Instantiate (Boat, new Vector3 ((float) (child.transform.position.x), -.05f, (float) child.transform.position.z), Boat.transform.rotation);

								PlayerClass owner = PlayerList.Find (Player => Player.Name == x.Owner);
								if (owner != null) {
									foreach (Transform childD in unitInstance.transform) {
										if (childD.name == "Mast") {
											childD.GetComponent<Renderer> ().material.color = StringToColor (owner.Color);
										}
									}

								}
								unitInstance.transform.name = "Soldiers";
								unitInstance.transform.parent = child;
							}
						}
					}

					if (buildingInstance != null) {
						buildingInstance.transform.parent = child;
					}
					if (naturalInstance != null) {
						naturalInstance.transform.parent = child;
					}

				}

			}

		}

		foreach (HexGridClass x in HexGrid) {
			x.Changed = false;
		}
		firstMake = false;

		Hexes = Hexes.OrderBy (xHexVar => xHexVar.x).ThenBy (xHexVar => xHexVar.y).ToList (); //re order list

	}

	float GetDistance (HexGridClass a, HexGridClass b) {
		float distance = 1;
		GameObject aObject = GameObject.Find ("Hex: " + a.x + "-" + a.y);
		GameObject bObject = GameObject.Find ("Hex: " + b.x + "-" + b.y);
		distance = Vector3.Distance (aObject.transform.position, bObject.transform.position);
		return Mathf.Round (distance);
	}

	List<GridPoint> GetHexesAround (int x, int y, int Range) //range is how many around, 1 equals 1 hex around the hex
	{
		List<GridPoint> Hexes = new List<GridPoint> ();

		for (int xHex = 0; xHex <= Range; xHex++) //loop through it to get possibilities within x.
		{
			for (int yHex = 0; yHex <= Range; yHex++) //all y possibilites
			{
				if (xHex != 0 && yHex != 0) {

					Hexes.Add (new GridPoint ((x - xHex), (y)));
					Hexes.Add (new GridPoint ((x + xHex), (y)));

					Hexes.Add (new GridPoint ((x), (y + yHex)));
					Hexes.Add (new GridPoint ((x), (y - yHex)));

					Hexes.Add (new GridPoint ((x - xHex), (y - yHex)));
					Hexes.Add (new GridPoint ((x + xHex), (y - yHex)));
					Hexes.Add (new GridPoint ((x - xHex), (y + yHex)));
					Hexes.Add (new GridPoint ((x + xHex), (y + yHex)));
				}

			}
		}

		Hexes.RemoveAll (i => i.y < 0); //remove negatives
		Hexes.RemoveAll (i => i.x < 0);

		for (int gridpointA = 0; gridpointA < Hexes.Count; gridpointA++) //remove duplicates
		{
			for (int gridpointB = 0; gridpointB < Hexes.Count; gridpointB++) //remove duplicates
			{
				if ((Hexes[gridpointB].x == Hexes[gridpointA].x) && (Hexes[gridpointB].y == Hexes[gridpointA].y) && (gridpointA != gridpointB)) {
					Hexes.RemoveAt (gridpointA);
				}
			}
		}

		return Hexes;
	}

	bool OwnedHexAround (List<GridPoint> x) {
		foreach (GridPoint y in x) {
			HexGridClass theHex = PointData (y.x, y.y);
			if (theHex != null && theHex.Owner == CurrentPlayer.Name) {
				return true;
			}
		}
		return false;
	}
	void SetView (List<GridPoint> gridPoints) {

		foreach (GridPoint point in gridPoints) {

			HexGridClass hexPoint = getArrayPoint (point.x, point.y);
			if (hexPoint != null) {
				if (!hexPoint.Viewers.Contains (CurrentPlayer.Name) && hexPoint.Owner != CurrentPlayer.Name) {
					hexPoint.Viewers.Add (CurrentPlayer.Name);
					if (hexPoint.Viewed.Contains (CurrentPlayer.Name)) {
						hexPoint.Viewed.Remove (CurrentPlayer.Name);
					}
					hexPoint.Changed = true;
				}

			}
		}
	}
	bool CheckUnit (string Name, HexGridClass hex, int options = 0) {
		foreach (UnitClass x in hex.Units) {
			if (x.Owner == CurrentPlayer.Name && options == 1) {
				return true;
			}
		}
		return false;
	}
	void OnGUI () {
		GUIStyle resStyle = new GUIStyle (GUI.skin.label);
		resStyle.normal.textColor = Color.black;

		if (ErrorScreenShowing == false && CurrentPlayer.Name != "None") {
			GUI.DrawTexture (new Rect (0, 0, 300, 25), resourcesBackground, ScaleMode.StretchToFill, true, 10.0F);

			GUI.DrawTexture (new Rect (10, 0, 25, 25), actionTexture, ScaleMode.StretchToFill, true, 10.0F);
			GUI.Label (new Rect (30, 3, 50, 25), CurrentPlayer.TurnPoints + " ", resStyle);

			GUI.DrawTexture (new Rect (57, 6, 15, 15), woodTexture, ScaleMode.StretchToFill, true, 10.0F);
			GUI.Label (new Rect (77, 3, 53, 25), CurrentPlayer.Wood + " ", resStyle);

			GUI.DrawTexture (new Rect (100, 5, 15, 15), foodTexture, ScaleMode.StretchToFill, true, 10.0F);
			GUI.Label (new Rect (120, 3, 50, 25), CurrentPlayer.Food + " ", resStyle);

			GUI.DrawTexture (new Rect (145, 5, 15, 15), stoneTexture, ScaleMode.StretchToFill, true, 10.0F);
			GUI.Label (new Rect (165, 3, 50, 25), CurrentPlayer.Stone + " ", resStyle);

			GUI.DrawTexture (new Rect (190, 4, 15, 15), coinTexture, ScaleMode.StretchToFill, true, 10.0F);
			GUI.Label (new Rect (210, 3, 50, 25), CurrentPlayer.Coin + " ", resStyle);

			if (Settings.Count > 0) {
				GUI.Label (new Rect (240, 2, 50, 25), "Turn: " + FindSettings ("TurnNumber"), resStyle);
			}
		}

		if (UnitAction == "Move") {
			Cursor.SetCursor (moveCursorTexture, new Vector2 (42.5f, 44f), CursorMode.Auto);
		} else if (UnitAction == "Attack") {
			Cursor.SetCursor (attackCursorTexture, new Vector2 (42.5f, 44f), CursorMode.Auto);
		} else {
			Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
		}

		if (Screen.height > 450) {
			playerRect.height = 450;
		}
		if (CurrentPlayer.Name == "None") {
			loginRect = GUI.Window (0, loginRect, LoginWindow, "Login - Version 0.020"); //login window

			if (GUI.Button (new Rect (Screen.width - 100, 0, 100, 50), "Exit")) //quit game
			{
				Application.Quit ();
			}
		} else {
			if (alliesShowing == false && upgradeShowing == false && selectingHex == false) {
				if (CurrentPlayerName == CurrentPlayer.Name) {
					if (GUI.Button (new Rect (Screen.width - 100, 0, 100, 50), "End Turn")) //Log out Button
					{
						NextPlayer ();
						Save ();
						LogOut ();
					}
				}
				/*if (GUI.Button(new Rect(Screen.width - 100, 51, 100, 50), "Update")) //Log out Button
            {
             LoadAndSetData();
            }*/
			}

			if ((upgradeShowing == false && alliesShowing == false && tradingShowing == false && armyShowing == false && armywaveShowing == false) || selectingHex == true) {
				playerRect = GUI.Window (0, playerRect, PlayerWindow, CurrentPlayer.Name);
			} else if (armywaveShowing == true) {
				armywaveRect = GUI.Window (0, armywaveRect, ArmyWaveWindow, "Army Strategy");
			} else if (armyShowing == true) {
				armyRect = GUI.Window (0, armyRect, ArmyWindow, "Military");
			} else {
				upgradeRect = GUI.Window (0, upgradeRect, UpgradeWindow, "");
			}
		}

		if (ErrorScreenShowing == true) {
			errorScreenRect = GUI.Window (0, errorScreenRect, ErrorWindow, "Alert");

		}
	}

	string WhatUnit (int x) {
		if (x == 0) {
			return "Horseman";
		} else if (x == 1) {
			return "Archer";
		} else if (x == 2) {
			return "Pikeman";
		} else {
			return "Error";
		}
	}
	void LoginWindow (int windowID) {
		Login = GUI.TextField (new Rect (50, 25, 100, 20), Login, 25);
		Password = GUI.PasswordField (new Rect (50, 50, 100, 20), Password, "*" [0], 25);
		GameInfo = GUI.TextField (new Rect (50, 75, 100, 20), GameInfo, 25);
		if (GUI.Button (new Rect (50, 100, 100, 25), "Login") || Input.GetKeyDown (KeyCode.Return)) {
			//PlayerRefrence = GameInfo.Substring(GameInfo.IndexOf(":") + 1);
			if (Password == "") {
				//Camera.main.transform.position = new Vector3(
				LoadAndSetData ();
			} else {
				StartCoroutine (TryLogin ());
				StopCoroutine ("TryLogin");
			}
		}
	}
	public bool hasUpgrades (string x) {
		SettingsClass theUpgrade = CurrentPlayer.Upgrades.Find (var =>
			var.Name == x);
		if (theUpgrade == null) {
			return false;
		} else {
			return true;
		}
	}
	void ArmyWindow (int windowID) {

		float TotalSoldiers = Horsemen + Archer + Pikemen;

		float FoodCost = 5;
		float CoinCost = 2;
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Total Cost: " + TotalSoldiers * FoodCost + " Food and " + TotalSoldiers * CoinCost + " Coin");
		GUILayout.Label ("Knights: " + Horsemen);
		GUILayout.Label ("Pikemen: " + Pikemen);
		GUILayout.Label ("Archers: " + Archer);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+ Horsemen")) {
			Horsemen++;
		}
		if (GUILayout.Button ("- Horsemen")) {
			if (Horsemen > 0) {
				Horsemen--;
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+ Archer")) {
			Archer++;
		}
		if (GUILayout.Button ("- Archer")) {
			if (Archer > 0) {
				Archer--;
			}
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+ Pikemen")) {
			Pikemen++;
		}
		if (GUILayout.Button ("- Pikemen")) {
			if (Pikemen > 0) {
				Pikemen--;
			}
		}
		GUILayout.EndHorizontal ();

		if (HasResources (2f, TotalSoldiers * FoodCost, 0, 0, TotalSoldiers * CoinCost) && TotalSoldiers > 0) {
			if (GUILayout.Button ("Train Soldiers")) {
				CurrentPlayer.Food -= TotalSoldiers * FoodCost;
				CurrentPlayer.Coin -= TotalSoldiers * CoinCost;
				for (int x = 0; x < Horsemen; x++) {
					unitToBeMade.Soldiers.Add (new SoldierClass (0));
				}
				for (int x = 0; x < Archer; x++) {
					unitToBeMade.Soldiers.Add (new SoldierClass (1));
				}
				for (int x = 0; x < Pikemen; x++) {
					unitToBeMade.Soldiers.Add (new SoldierClass (2));
				}
				Pikemen = 0;
				Horsemen = 0;
				Archer = 0;
				armyShowing = false;
				AddSoldier ();
			}

		}
		if ((GUILayout.Button ("Back (B)") || Input.GetKeyDown (KeyCode.B))) {
			armyShowing = false;
		}
	}
	void ArmyWaveWindow (int windowID) {

		List<StringIntClass> soldierTypes = new List<StringIntClass> ();
		soldierTypes.Add (new StringIntClass (0, WhatUnit (0)));
		soldierTypes.Add (new StringIntClass (1, WhatUnit (1)));
		soldierTypes.Add (new StringIntClass (2, WhatUnit (2)));

		GUILayout.BeginHorizontal (); //Wave 1
		GUILayout.Label ("<color=red>First Wave:</color>");
		foreach (SoldierClass x in currentUnit.Wave1) {
			GUILayout.Label (WhatUnit (x.Type) + " (" + x.Wounds + "/" + "3" + ")");
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal (); //Wave 2
		GUILayout.Label ("<color=lightblue>Second Wave:</color>");
		foreach (SoldierClass x in currentUnit.Wave2) {
			GUILayout.Label (WhatUnit (x.Type) + " (" + x.Wounds + "/" + "3" + ")");
		}
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal (); //Wave 3
		GUILayout.Label ("<color=green>Third Wave:</color>");
		foreach (SoldierClass x in currentUnit.Wave3) {
			GUILayout.Label (WhatUnit (x.Type) + " (" + x.Wounds + "/" + "3" + ")");
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("<color=red>1st</color>");
		foreach (StringIntClass z in soldierTypes) {
			if (GUILayout.Button ("+ " + z.s)) {
				if (currentUnit.Soldiers.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Soldiers.FirstOrDefault (x => x.Type == z.i); //find unit
					currentUnit.Wave1.Add (soldierToFind); //add to wave
					currentUnit.Soldiers.Remove (soldierToFind); //remove from main pool
				}
			}
			if (GUILayout.Button ("- " + z.s)) {
				if (currentUnit.Wave1.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Wave1.FirstOrDefault (x => x.Type == z.i);
					currentUnit.Soldiers.Add (soldierToFind); // add to wave
					currentUnit.Wave1.Remove (soldierToFind); //remove from main pool
				}
			}
			GUILayout.Space (20);
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("<color=lightblue>2nd</color>");
		foreach (StringIntClass z in soldierTypes) {

			if (GUILayout.Button ("+ " + z.s)) {
				if (currentUnit.Soldiers.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Soldiers.FirstOrDefault (x => x.Type == z.i); //find unit
					currentUnit.Wave2.Add (soldierToFind); //add to wave
					currentUnit.Soldiers.Remove (soldierToFind); //remove from main pool
				}
			}
			if (GUILayout.Button ("- " + z.s)) {
				if (currentUnit.Wave2.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Wave1.FirstOrDefault (x => x.Type == z.i);
					currentUnit.Soldiers.Add (soldierToFind); // add to wave
					currentUnit.Wave2.Remove (soldierToFind); //remove from main pool
				}
			}
			GUILayout.Space (20);
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("<color=green>3rd</color>");
		foreach (StringIntClass z in soldierTypes) {
			if (GUILayout.Button ("+ " + z.s)) {
				if (currentUnit.Soldiers.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Soldiers.FirstOrDefault (x => x.Type == z.i); //find unit
					currentUnit.Wave3.Add (soldierToFind); //add to wave
					currentUnit.Soldiers.Remove (soldierToFind); //remove from main pool
				}
			}
			if (GUILayout.Button ("- " + z.s)) {
				if (currentUnit.Wave3.Exists (x => x.Type == z.i)) {
					SoldierClass soldierToFind = currentUnit.Wave1.FirstOrDefault (x => x.Type == z.i);
					currentUnit.Soldiers.Add (soldierToFind); // add to wave
					currentUnit.Wave3.Remove (soldierToFind); //remove from main pool
				}
			}
			GUILayout.Space (20);
		}
		GUILayout.EndHorizontal ();

		GUILayout.Space (50);

		GUILayout.BeginHorizontal (); // total forces
		GUILayout.Label ("Unorganized Forces:");
		foreach (SoldierClass x in currentUnit.Soldiers) {
			if (x.Type == 0) {
				GUILayout.Label (WhatUnit (0) + " (" + x.Wounds + "/" + "3" + ")");
			} else if (x.Type == 1) {
				GUILayout.Label (WhatUnit (1) + " (" + x.Wounds + "/" + "3" + ")");
			} else if (x.Type == 2) {
				GUILayout.Label (WhatUnit (2) + " (" + x.Wounds + "/" + "3" + ")");
			}
		}
		GUILayout.EndHorizontal ();
		if (currentlySelectedGrid.Owner == CurrentPlayer.Name && currentlySelectedGrid.Building == "Hall" && currentlySelectedGrid.Units[0].Owner == CurrentPlayer.Name) {
			GUILayout.Label ("Reinforce This Army");

			float TotalSoldiers = Horsemen + Archer + Pikemen;

			float FoodCost = 5;
			float CoinCost = 2;
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Total Cost: " + TotalSoldiers * FoodCost + " Food and " + TotalSoldiers * CoinCost + " Coin");
			GUILayout.Label ("Knights: " + Horsemen);
			GUILayout.Label ("Pikemen: " + Pikemen);
			GUILayout.Label ("Archers: " + Archer);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+ Horsemen")) {
				Horsemen++;
			}
			if (GUILayout.Button ("- Horsemen")) {
				if (Horsemen > 0) {
					Horsemen--;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+ Archer")) {
				Archer++;
			}
			if (GUILayout.Button ("- Archer")) {
				if (Archer > 0) {
					Archer--;
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("+ Pikemen")) {
				Pikemen++;
			}
			if (GUILayout.Button ("- Pikemen")) {
				if (Pikemen > 0) {
					Pikemen--;
				}
			}
			GUILayout.EndHorizontal ();

			if (HasResources (2.5f, TotalSoldiers * FoodCost, 0, 0, TotalSoldiers * CoinCost) && TotalSoldiers > 0) {
				if (GUILayout.Button ("Train Soldiers")) {
					CurrentPlayer.Food -= TotalSoldiers * FoodCost;
					CurrentPlayer.Coin -= TotalSoldiers * CoinCost;
					for (int x = 0; x < Horsemen; x++) {
						unitToBeMade.Soldiers.Add (new SoldierClass (0));
					}
					for (int x = 0; x < Archer; x++) {
						unitToBeMade.Soldiers.Add (new SoldierClass (1));
					}
					for (int x = 0; x < Pikemen; x++) {
						unitToBeMade.Soldiers.Add (new SoldierClass (2));
					}
					Pikemen = 0;
					Horsemen = 0;
					Archer = 0;
					armyShowing = false;
					ReinforceSoldier ();
				}

			}
		}

		if (GUILayout.Button ("Back")) {
			armywaveShowing = false;
		}

	}
	void UpgradeWindow (int windowID) {
		currentlySelectedGrid = null;
		if (upgradeShowing == true) {
			if (!hasUpgrades ("Wood Working")) {
				if (GUILayout.Button ("Wood Working")) {
					currentlySelectedUpgrade = new UpgradeClass ("Wood Working", "Wood Working allows you to start getting more .", 2, 2, 2, 2);
				}
			}
			if (!hasUpgrades ("Sail Making") && hasUpgrades ("Wood Working")) {
				if (GUILayout.Button ("Sail Making")) {
					currentlySelectedUpgrade = new UpgradeClass ("Sail Making", "You can construct basic sails for ships.", 2, 2, 2, 2);
				}
			}
			if (!hasUpgrades ("Ship Making") && hasUpgrades ("Sail Making")) {
				if (GUILayout.Button ("Ship Making")) {
					currentlySelectedUpgrade = new UpgradeClass ("Ship Making", "You can now create ships and your unit can travel across water.", 2, 2, 2, 2);
				}
			}

			GUILayout.BeginHorizontal ();
			if (currentlySelectedUpgrade != null) {
				GUILayout.Label ("<b>" + currentlySelectedUpgrade.Name + "</b>: " + currentlySelectedUpgrade.Value);
				if (HasResources (currentlySelectedUpgrade)) {
					if (GUILayout.Button ("Buy")) {
						CurrentPlayer.Food -= currentlySelectedUpgrade.Food;
						CurrentPlayer.Wood -= currentlySelectedUpgrade.Wood;
						CurrentPlayer.Stone -= currentlySelectedUpgrade.Stone;
						CurrentPlayer.Coin -= currentlySelectedUpgrade.Coin;
						CurrentPlayer.Upgrades.Add (new SettingsClass (currentlySelectedUpgrade.Name, currentlySelectedUpgrade.Value));
						currentlySelectedUpgrade = null;
					}
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (20);
			if (CurrentPlayer.Upgrades.Count > 0) {
				GUILayout.Label ("<b>Your Upgrades</b>");
			}
			foreach (SettingsClass x in CurrentPlayer.Upgrades) {
				GUILayout.Label (x.Name);
			}

			if ((GUILayout.Button ("Back (B)") || Input.GetKeyDown (KeyCode.B))) {
				upgradeShowing = false;
			}
		} else if (tradingShowing == true) {
			GUILayout.Label ("Trading with " + PlayerTradingWith);
			GUILayout.BeginHorizontal ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("<b>Give</b>");
			GUILayout.Label ("Food: " + TradingGive.Food);
			TradingGive.Food = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingGive.Food, 0.0F, GreaterThanZero (CurrentPlayer.Food)));
			GUILayout.Label ("Wood: " + TradingGive.Wood);
			TradingGive.Wood = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingGive.Wood, 0.0F, GreaterThanZero (CurrentPlayer.Wood)));
			GUILayout.Label ("Stone: " + TradingGive.Stone);
			TradingGive.Stone = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingGive.Stone, 0.0F, GreaterThanZero (CurrentPlayer.Stone)));
			GUILayout.Label ("Coin: " + TradingGive.Coin);
			TradingGive.Coin = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingGive.Coin, 0.0F, GreaterThanZero (CurrentPlayer.Coin)));
			if (GUILayout.Button ("Select Hex")) {
				selectingHex = true;
				selectingA = true;
			}
			if (TradingGive.Hex != null) {
				GUILayout.Label ("X: " + TradingGive.Hex.x + " Y:" + TradingGive.Hex.y);
			}
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			GUILayout.Label ("<b>Want</b>");
			GUILayout.Label ("Food: " + TradingWant.Food);
			TradingWant.Food = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingWant.Food, 0.0F, 10)); //leaves room for "better barganing" and stealing
			GUILayout.Label ("Wood: " + TradingWant.Wood);
			TradingWant.Wood = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingWant.Wood, 0.0F, 10));
			GUILayout.Label ("Stone: " + TradingWant.Stone);
			TradingWant.Stone = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingWant.Stone, 0.0F, 10));
			GUILayout.Label ("Coin: " + TradingWant.Coin);
			TradingWant.Coin = Mathf.RoundToInt (GUILayout.HorizontalSlider (TradingWant.Coin, 0.0F, 10));
			if (GUILayout.Button ("Select Hex")) {
				selectingHex = true;
				selectingA = false;
			}
			if (TradingWant.Hex != null) {
				GUILayout.Label ("X: " + TradingWant.Hex.x + " Y:" + TradingWant.Hex.y);
			}

			GUILayout.EndVertical ();

			GUILayout.EndHorizontal ();

			Message = GUILayout.TextField (Message, GUILayout.Height (50));

			GUILayout.BeginHorizontal ();
			/*if(GUILayout.Button("Give")){

			}
			if(GUILayout.Button("Request")){

			}*/
			if (GUILayout.Button ("Offer")) {
				GetPlayer (PlayerTradingWith).Messages.Add (new MessageClass (CurrentPlayer.Name, 2, isthereaMessage (), TradingGive, TradingWant));
				TradingGive = new ResourcesClass ();
				TradingWant = new ResourcesClass ();
				Message = "Write a message...";
			}
			GUILayout.EndHorizontal ();

			if ((GUILayout.Button ("Back (B)") || Input.GetKeyDown (KeyCode.B))) {
				tradingShowing = false;
			}
		} else if (alliesShowing == true) {
			foreach (PlayerClass player in PlayerList) {
				if (player.Name != CurrentPlayer.Name) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label (player.Name);

					if (HasAlly (player.Name) && GUILayout.Button ("Trade", GUILayout.Width (150))) {
						tradingShowing = true;
						PlayerTradingWith = player.Name;
					}

					if (HasAlly (player.Name, 1)) {
						if (GUILayout.Button ("<b> Ally Request Sent </b>", GUILayout.Width (150))) { }
					} else if (HasAlly (player.Name)) {
						if (GUILayout.Button ("<b> Remove Ally </b>", GUILayout.Width (150))) {
							player.Messages.Add (new MessageClass (CurrentPlayer.Name, 0, CurrentPlayer.Name + " is not your ally now.  " + isthereaMessage ()));
							AlliesClass ally = CurrentPlayer.Allies.Find (var =>
								var.OtherPlayer == player.Name);
							CurrentPlayer.Allies.Remove (ally);
							AlliesClass allyB = player.Allies.Find (var =>
								var.OtherPlayer == CurrentPlayer.Name);
							player.Allies.Remove (allyB);
							break;
						}

					} else {
						if (GUILayout.Button ("Request Ally", GUILayout.Width (150))) {
							if (player.Messages == null) {
								player.Messages = new List<MessageClass> ();
							}
							player.Messages.Add (new MessageClass (CurrentPlayer.Name, 1, Message));
							Message = "Write a message...";
						}
					}

					if (GUILayout.Button ("Message", GUILayout.Width (150))) {

						if (player.Messages == null) {
							player.Messages = new List<MessageClass> ();
						}

						if (Message != "Write a message...") {
							player.Messages.Add (new MessageClass (CurrentPlayer.Name, 0, Message));
							GUI.FocusControl ("");
						} else {
							GUI.FocusControl ("Message");
						}
						Message = "Write a message...";

					}
					GUILayout.EndHorizontal ();
				}
			}
			GUI.SetNextControlName ("Message");
			Message = GUILayout.TextField (Message, GUILayout.Height (50));

			GUILayout.Space (20);
			if (CurrentPlayer.Messages != null) {
				foreach (MessageClass aMessage in CurrentPlayer.Messages) {
					GUILayout.BeginHorizontal ();

					if (aMessage.Type == 0) {
						GUILayout.Label ("<b>" + aMessage.Sender + "</b>" + "-" + aMessage.Text);

						if (GUILayout.Button ("Reply")) {
							if (CurrentPlayer.Messages == null) {
								CurrentPlayer.Messages = new List<MessageClass> ();
							}
							if (Message != "Write a message...") {
								GetPlayer (aMessage.Sender).Messages.Add (new MessageClass (CurrentPlayer.Name, 0, Message));
								GetPlayer (aMessage.Sender).Messages.Remove (aMessage);
								CurrentPlayer.Messages.Remove (aMessage);
								Message = "Write a message...";
								GUI.FocusControl ("");
								break;
							} else {
								GUI.FocusControl ("Message");
							}
						}
						if (GUILayout.Button ("Delete")) {
							CurrentPlayer.Messages.Remove (aMessage);
							break;
						}
					} else if (aMessage.Type == 1) {
						GUILayout.Label (aMessage.Sender + " requests to be allied with you. " + isthereaMessage ());
						if (GUILayout.Button ("Accept")) {
							GetPlayer (aMessage.Sender).Messages.Add (new MessageClass (CurrentPlayer.Name, 0, "Accepted your request to be allied.  " + isthereaMessage ()));
							GetPlayer (aMessage.Sender).Allies.Add (new AlliesClass (CurrentPlayer.Name));
							CurrentPlayer.Allies.Add (new AlliesClass (aMessage.Sender));
							GetPlayer (aMessage.Sender).Messages.Remove (aMessage);
							CurrentPlayer.Messages.Remove (aMessage);
							break;
						}
						if (GUILayout.Button ("Deny")) {
							GetPlayer (aMessage.Sender).Messages.Add (new MessageClass (CurrentPlayer.Name, 0, "Denied your request to be allied.  " + isthereaMessage ()));
							GetPlayer (aMessage.Sender).Messages.Remove (aMessage);
							CurrentPlayer.Messages.Remove (aMessage);
							break;
						}
					} else if (aMessage.Type == 2) {
						GUILayout.Label ("<b>" + aMessage.Sender + "</b>" + " has requested a trade and will give " + ResourcesTradeToString (aMessage.Trade[0]) + aMessage.Sender + " wants " + ResourcesTradeToString (aMessage.Trade[1]) + " " + aMessage.Text);
						if (HasResources (0, aMessage.Trade[1].Food, aMessage.Trade[1].Wood, aMessage.Trade[1].Stone, aMessage.Trade[1].Coin)) {
							if (GUILayout.Button ("Accept")) {
								GetPlayer (aMessage.Sender).Messages.Add (new MessageClass (CurrentPlayer.Name, 0, "Accepted your trade offer of " + ResourcesTradeToString (aMessage.Trade[0]) + " for " + ResourcesTradeToString (aMessage.Trade[1]) + ". " + isthereaMessage ()));

								CurrentPlayer.Food -= aMessage.Trade[1].Food;
								CurrentPlayer.Wood -= aMessage.Trade[1].Wood;
								CurrentPlayer.Stone -= aMessage.Trade[1].Stone;
								CurrentPlayer.Coin -= aMessage.Trade[1].Coin;

								GetPlayer (aMessage.Sender).Food += aMessage.Trade[1].Food;
								GetPlayer (aMessage.Sender).Wood += aMessage.Trade[1].Wood;
								GetPlayer (aMessage.Sender).Stone += aMessage.Trade[1].Stone;
								GetPlayer (aMessage.Sender).Coin += aMessage.Trade[1].Coin;

								CurrentPlayer.Food += aMessage.Trade[0].Food;
								CurrentPlayer.Wood += aMessage.Trade[0].Wood;
								CurrentPlayer.Stone += aMessage.Trade[0].Stone;
								CurrentPlayer.Coin += aMessage.Trade[0].Coin;

								GetPlayer (aMessage.Sender).Food -= aMessage.Trade[0].Food;
								GetPlayer (aMessage.Sender).Wood -= aMessage.Trade[0].Wood;
								GetPlayer (aMessage.Sender).Stone -= aMessage.Trade[0].Stone;
								GetPlayer (aMessage.Sender).Coin -= aMessage.Trade[0].Coin;

								if (aMessage.Trade[0].Hex.x != -1) {
									PointData (aMessage.Trade[0].Hex.x, aMessage.Trade[0].Hex.y).Owner = CurrentPlayer.Name;
									PointData (aMessage.Trade[0].Hex.x, aMessage.Trade[0].Hex.y).Changed = true;
								}
								if (aMessage.Trade[1].Hex.x != -1) {
									PointData (aMessage.Trade[1].Hex.x, aMessage.Trade[1].Hex.y).Owner = aMessage.Sender;
									PointData (aMessage.Trade[1].Hex.x, aMessage.Trade[1].Hex.y).Changed = true;
								}
								CreateGrid (HexGrid);
								Message = "Write a message...";
								CurrentPlayer.Messages.Remove (aMessage);
								GetPlayer (aMessage.Sender).Messages.Remove (aMessage);
								break;
							}
						} else {
							if (GUILayout.Button ("Not Enough Resources")) { }
						}
						if (GUILayout.Button ("Deny")) {
							GetPlayer (aMessage.Sender).Messages.Add (new MessageClass (CurrentPlayer.Name, 0, "Denied your trade offer of " + ResourcesTradeToString (aMessage.Trade[0]) + " for " + ResourcesTradeToString (aMessage.Trade[1]) + ". " + isthereaMessage ()));

							CurrentPlayer.Messages.Remove (aMessage);
							GetPlayer (aMessage.Sender).Messages.Remove (aMessage);
							Message = "Write a message...";
							break;
						}

					}

					GUILayout.EndHorizontal ();
				}
			}

			if ((GUILayout.Button ("Back (B)") || Input.GetKeyDown (KeyCode.B))) {
				alliesShowing = false;
			}
		}
	}

	string ResourcesTradeToString (ResourcesClass x) {
		string returnValue = "";

		if (x.Food > 0) {
			returnValue += x.Food + " Food";
		}
		if (x.Wood > 0) {
			if (returnValue != "") {
				returnValue += ", ";
			}
			returnValue += x.Wood + " Wood";
		}
		if (x.Stone > 0) {
			if (returnValue != "") {
				returnValue += ", ";
			}
			returnValue += x.Stone + " Stone";
		}
		if (x.Coin > 0) {
			if (returnValue != "") {
				returnValue += ", ";
			}
			returnValue += x.Coin + " Coin";
		}
		if (x.Hex.x != -1) {
			if (returnValue != "") {
				returnValue += ", ";
			}
			returnValue += "hex X: " + x.Hex.x + " Y: " + x.Hex.y;
		}
		returnValue += ". ";
		return returnValue;
	}

	void AddSoldier () {
		CurrentPlayer.TurnPoints -= 2f;
		//CurrentPlayer.Food -= 2;
		//CurrentPlayer.Coin -= 2;
		currentlySelectedGrid.Units.Add (new UnitClass (CurrentPlayer.Name + "'s " + "Soldiers", unitId, CurrentPlayer.Name, 0, unitToBeMade.Soldiers));
		currentlySelectedGrid.Changed = true;
		CreateGrid (HexGrid);
	}

	void ReinforceSoldier () {
		CurrentPlayer.TurnPoints -= 2.5f;
		currentlySelectedGrid.Units[0].Soldiers.AddRange (unitToBeMade.Soldiers);
	}

	float GreaterThanZero (float x) {
		if (x > 0) {
			return x;
		} else {
			return 0;
		}
	}
	void PlayerWindow (int windowID) {

		//GUILayout.Label("<b>Actions: </b>" + CurrentPlayer.TurnPoints);

		/*GUILayout.Label("<b>Food: </b>" + CurrentPlayer.Food);
		GUILayout.Label("<b>Stone: </b>" + CurrentPlayer.Stone);
		GUILayout.Label("<b>Coin: </b>" + CurrentPlayer.Coin);*/

		//if(GUILayout.Button("Save"))
		{

		}
		if (GUILayout.Button ("Upgrades (G)") || Input.GetKeyDown (KeyCode.G)) {
			upgradeShowing = true;
			currentlySelectedUpgrade = null;
		}
		if (GUILayout.Button ("Allies & Trading (R)") || Input.GetKeyDown (KeyCode.R)) {
			alliesShowing = true;
			currentlySelectedUpgrade = null;
			TradingGive.Hex = new GridPoint (-1, -1);
			TradingWant.Hex = new GridPoint (-1, -1);
			GUI.FocusControl ("");
			Message = "Write a message...";
		}
		if (currentUnit != null && currentUnit.Selected == true) {
			GUILayout.Label ("--------------------------------");
			GUILayout.Label (currentUnit.Count + " of " + currentUnit.Name);
			if (HasResources (1f) && currentUnit.Owner == CurrentPlayer.Name && CurrentPlayerName == CurrentPlayer.Name) {
				if (GUILayout.Button ("Move (S)") || Input.GetKeyDown (KeyCode.S)) { // move to a location
					UnitAction = "Move";
				}
				if (GUILayout.Button ("Attack (A)") || Input.GetKeyDown (KeyCode.A)) { // Attack a thing
					UnitAction = "Attack";
				}
			}
			if (currentUnit.Owner == CurrentPlayer.Name) {
				if (GUILayout.Button ("Reorganize (V)") || Input.GetKeyDown (KeyCode.V)) {
					armywaveShowing = true;
				}
			} else {
				if (GUILayout.Button ("View Army (V)") || Input.GetKeyDown (KeyCode.V)) {
					armywaveShowing = true;
				}
			}
			if (GUILayout.Button ("Back (B)") || Input.GetKeyDown (KeyCode.B)) { // go back
				currentUnit.Selected = false;
				UnitAction = "";
			}
			if (currentUnit.Owner == CurrentPlayer.Name || HasAlly (currentlySelectedGrid.Owner)) { //show unit information
				foreach (SoldierClass theSoldier in currentUnit.Wave1) { //Show soldiers
					GUILayout.Label ("Type:" + theSoldier.Type);
				}
				foreach (SoldierClass theSoldier in currentUnit.Wave2) { //Show soldiers
					GUILayout.Label ("Type:" + theSoldier.Type);
				}
				foreach (SoldierClass theSoldier in currentUnit.Wave3) { //Show soldiers
					GUILayout.Label ("Type:" + theSoldier.Type);
				}
			}
		} else if (currentlySelectedGrid != null) {
			if (CanTheySee (currentlySelectedGrid, CurrentPlayer.Name)) {

				GUILayout.Label ("--------------------------------");
				if (currentlySelectedGrid.Owner == CurrentPlayer.Name) {
					GUILayout.Label ("<size=15><i>Owned Territory</i></size>");
				} else {
					if (String.IsNullOrEmpty (currentlySelectedGrid.Owner)) {
						GUILayout.Label ("<size=15><i>Discoverd Territory</i></size>");
					} else {
						GUILayout.Label ("<size=15><i>" + currentlySelectedGrid.Owner + "'s Territory</i></size>");

					}
				}

				GUILayout.Label ("<b>(" + currentlySelectedGrid.x + "-" + currentlySelectedGrid.y + ")</b>" + " " + currentlySelectedGrid.Type);
				if (!currentlySelectedGrid.Viewed.Contains (CurrentPlayer.Name)) {
					GUILayout.Label (currentlySelectedGrid.Building);
				}

				if (CurrentPlayer.Name == CurrentPlayerName) {

					if (currentlySelectedGrid.Building == "None" && currentlySelectedGrid.Owner == CurrentPlayer.Name && currentlySelectedGrid.Type != "Mountain") {
						float Wood = 0;
						float Stone = 0;
						float Food = 0;
						if (currentlySelectedGrid.Type == "Farmland") {
							Wood = 2;
							Stone = 2;
						} else if (currentlySelectedGrid.Type == "Forest") {
							Food = 2;
							Stone = 2;
						} else if (currentlySelectedGrid.Type == "Hill") {
							Wood = 2;
							Food = 2;
						}
						if (HasResources (2.0f, Food, Wood, Stone)) {
							if (GUILayout.Button ("Build Hall (H)") || Input.GetKeyDown (KeyCode.H)) {
								CurrentPlayer.TurnPoints -= 2;
								CurrentPlayer.Food -= Food;
								CurrentPlayer.Wood -= Wood;
								CurrentPlayer.Stone -= Stone;
								AddBuilding (currentlySelectedGrid, "Hall");
							}
						}
					} else if (String.IsNullOrEmpty (currentlySelectedGrid.Owner) && CanTheySee (currentlySelectedGrid, CurrentPlayer.Name) && currentlySelectedGrid.Type != "Mountain" && currentlySelectedGrid.Type != "Water") {
						float Wood = 0;
						float Stone = 0;
						float Food = 0;
						if (currentlySelectedGrid.Type == "Farmland") {
							Wood = 2;
							Stone = 2;
						} else if (currentlySelectedGrid.Type == "Forest") {
							Food = 2;
							Stone = 2;
						} else if (currentlySelectedGrid.Type == "Hill") {
							Wood = 2;
							Food = 2;
						}
						if (HasResources (1.0f, Food, Wood, Stone, 2.0f)) {
							if (OwnedHexAround (GetHexesAround (currentlySelectedGrid.x, currentlySelectedGrid.y, 2))) {
								if (GUILayout.Button ("Buy Hex (B)") || Input.GetKeyDown (KeyCode.B)) {
									CurrentPlayer.TurnPoints -= 1;
									CurrentPlayer.Food -= Food;
									CurrentPlayer.Wood -= Wood;
									CurrentPlayer.Stone -= Stone;
									CurrentPlayer.Coin -= 2.0f;
									BuyHex (currentlySelectedGrid);
								}
							}
						}
					} else if ( /*HasResources(2.5f, 2.0f, 0.0f,0.0f,2.0f)  &&*/ currentlySelectedGrid.Building == "Hall" && currentlySelectedGrid.Owner == CurrentPlayer.Name) {
						if (GUILayout.Button ("Train Soldier (T)") || Input.GetKeyDown (KeyCode.T)) {
							/*AddSoldier();*/
							Horsemen = 0;
							Archer = 0;
							Pikemen = 0;
							armyShowing = true;
						}
					}

				} else {
					GUILayout.Label ("It is currently " + CurrentPlayerName + "'s turn.");
				}
				if (currentlySelectedGrid.Units.Count > 0) {
					int unitNumber = 0;
					foreach (UnitClass x in currentlySelectedGrid.Units) {
						unitNumber++;
						if (unitNumber == 1) {
							if (GUILayout.Button ("(1) " + x.Name) || Input.GetKeyDown (KeyCode.Alpha1)) {
								currentUnit = x;
								currentUnit.Selected = true;
							}

						} else if (unitNumber == 2) {
							if (GUILayout.Button ("(2) " + x.Name) || Input.GetKeyDown (KeyCode.Alpha2)) {
								currentUnit = x;
								currentUnit.Selected = true;
							}
						} else {
							if (GUILayout.Button (x.Name)) {
								currentUnit = x;
								currentUnit.Selected = true;
							}
						}
					}
				}
			} else {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("<i>Undiscoverd Territory</i>");
				GUILayout.EndHorizontal ();
			}

			if (CurrentPlayer.Name == CurrentPlayerName) {
				/*if (GUILayout.Button(EndTurnText))
                {
                    if (CurrentPlayer.TurnPoints != 5)
                    {
					NextPlayer();
                    }
                    else
                    {
                        ShowErrorScreen("You have not taken any actions!");
                    }
                }*/
			}

		}
		GUILayout.Space (25);
		//if (GUILayout.Button("End Turn")){

		//	}		
	}

	string isthereaMessage () {
		if (Message.Trim () == "Write a message...") {
			return "";
		} else {
			return Message;
		}
	}
	bool HasResources (float turnpoints = 0.0f, float food = 0.0f, float wood = 0.0f, float stone = 0.0f, float coin = 0.0f) {
		if (CurrentPlayer.Food >= food && CurrentPlayer.TurnPoints >= turnpoints && CurrentPlayer.Wood >= wood && CurrentPlayer.Stone >= stone && CurrentPlayer.Coin >= coin) {
			return true;
		} else {
			return false;
		}
	}
	bool HasResources (UpgradeClass theUpgrade) {
		if (CurrentPlayer.Food >= theUpgrade.Food && CurrentPlayer.Wood >= theUpgrade.Wood && CurrentPlayer.Stone >= theUpgrade.Stone && CurrentPlayer.Coin >= theUpgrade.Coin) {
			return true;
		} else {
			return false;
		}
	}
	void ErrorWindow (int windowID) {
		GUILayout.Label ("<i>" + ErrorText + "</i>");
		if (ErrorText == "You have not taken any actions!") {
			if (GUILayout.Button ("Finish Turn!")) {
				NextPlayer ();
				ErrorScreenShowing = false;
			}
			if (GUILayout.Button ("Don't finish turn!")) {
				ErrorScreenShowing = false;
			}
		} else {
			if (GUILayout.Button ("Ok (Enter)") || Input.GetKeyDown (KeyCode.Return)) {
				ErrorScreenShowing = false;
			}
		}
	}

	bool HasAlly (string name, int options = 0) {
		bool hasAlly = false;
		if (options == 0) {
			AlliesClass theAlly = new AlliesClass (name);
			foreach (AlliesClass x in CurrentPlayer.Allies) {
				if (x.OtherPlayer == name) {
					hasAlly = true;
					break;
				}
			}
		} else {
			PlayerClass thePlayer = GetPlayer (name);
			foreach (MessageClass x in thePlayer.Messages) {
				if (x.Sender == CurrentPlayer.Name && x.Type == 1) {
					hasAlly = true;
					break;
				}
			}
		}

		return hasAlly;
	}
	IEnumerator TryLogin () {

		string url = "http://timaconner.com/DOTPHPFiles/login.php";
		WWWForm form = new WWWForm ();

		form.AddField ("login", Login); //Add fields to url
		form.AddField ("password", Password);

		WWW download = new WWW (url, form);
		yield return download;
		if ((!string.IsNullOrEmpty (download.error))) {
			print ("Error downloading: " + download.error);
		} else {
			LoginData = download.text;
			if (LoginData == "Correct Login") {
				CurrentPlayer.Name = Login;
				LoadAndSetData ();
			} else {
				Password = "";
				ShowErrorScreen ("Wrong Username or Password");
			}
		}
	}

	HexGridClass PointData (int x, int y) {
		HexGridClass theHex = HexGrid.Find (xVar => xVar.x == x && xVar.y == y);

		return theHex;
	}

	void AddBuilding (HexGridClass Grid, string BuildingType) {
		HexGridClass hexToFind = getArrayPoint (Grid.x, Grid.y);
		hexToFind.Building = BuildingType;
		hexToFind.Changed = true;
		if (BuildingType == "Hall") {
			SetView (GetHexesAround (Grid.x, Grid.y, 1));
			CreateGrid (HexGrid);
		}

	}
	void BuyHex (HexGridClass Grid, int option = 0) {

		HexGridClass hexToFind = getArrayPoint (Grid.x, Grid.y);
		if (option == 1) { //upgrade to always see it
			hexToFind.Viewers.Remove (hexToFind.Owner);
			hexToFind.Viewed.Add (hexToFind.Owner);
		}
		hexToFind.Owner = CurrentPlayer.Name;
		hexToFind.Viewers.Remove (CurrentPlayer.Name);
		hexToFind.Changed = true;
		SetView (GetHexesAround (Grid.x, Grid.y, 1));

		CreateGrid (HexGrid);
	}

	HexGridClass getArrayPoint (int x, int y) {
		HexGridClass hexToFind = HexGrid.Find (theHex => theHex.x == x && theHex.y == y);

		return hexToFind;
	}

	void LogOut () //reloads and logs out
	{
		Application.LoadLevel (Application.loadedLevelName);
	}

	void ShowErrorScreen (string text) {
		ErrorText = text;
		ErrorScreenShowing = true;

	}

	void NextPlayer () {
		int index = 0;
		for (int x = 0; x < PlayerList.Count; x++) {
			if (PlayerList[x].Name == CurrentPlayerName) {
				index = x;
			}
		}
		if ((index + 1) >= PlayerList.Count) {
			CollectResources ();
			NaturalDisasters ();
			SetCurrentPlayer (0);
		} else {
			SetCurrentPlayer (index + 1);
		}
		Save ();

	}
	PlayerClass GetPlayer (string Name) {
		return PlayerList.Find (Players => Players.Name == Name);
	}
	void CollectResources () {

		int CurTurnNumber = Int32.Parse (FindSettings ("TurnNumber"));
		CurTurnNumber++;
		SettingsClass value = Settings.Find (Setting => Setting.Name == "TurnNumber");
		value.Value = "" + CurTurnNumber + "";

		foreach (PlayerClass x in PlayerList) {
			x.TurnPoints += 3;
		}
		foreach (HexGridClass x in HexGrid) {
			PlayerClass theOwner = new PlayerClass ();
			if (!String.IsNullOrEmpty (x.Owner)) {
				theOwner = PlayerList.Find (Players => Players.Name == x.Owner);
			}

			if (x.Building == "None" && !String.IsNullOrEmpty (x.Owner)) {
				switch (x.Type) {
					case "Farmland":
						theOwner.Food += UnityEngine.Random.Range (1, 2);
						break;
					case "Forest":
						theOwner.Wood += UnityEngine.Random.Range (1, 2);
						break;
					case "Hill":
						theOwner.Stone += UnityEngine.Random.Range (1, 2);
						break;
				}
			} else if (x.Building == "Hall" && !String.IsNullOrEmpty (x.Owner)) {
				switch (x.Type) {
					case "Farmland":
						theOwner.Food += UnityEngine.Random.Range (2, 4);
						theOwner.Coin += 1.0f;
						break;
					case "Forest":
						theOwner.Wood += UnityEngine.Random.Range (2, 4);
						theOwner.Coin += 1.0f;
						break;
					case "Hill":
						theOwner.Stone += UnityEngine.Random.Range (2, 4);
						theOwner.Coin += 1.0f;
						break;
					case "Plain":
						theOwner.Coin += 1.0f;
						break;
				}
			}
		}

	}
	void NaturalDisasters () {

	}
	void SetCurrentPlayer (int index) {
		CurrentPlayerName = PlayerList[index].Name;
	}

	Color StringToColor (string ColorString) {
		Color ReturnColor = Color.red;
		ColorString = ColorString.ToLower ();
		switch (ColorString) {
			case "red":
				ReturnColor = Color.red;
				break;
			case "blue":
				ReturnColor = Color.blue;
				break;
			case "yellow":
				ReturnColor = Color.yellow;
				break;
			case "green":
				ReturnColor = Color.green;
				break;
			case "white":
				ReturnColor = Color.white;
				break;
			case "black":
				ReturnColor = Color.black;
				break;
			case "clear":
				ReturnColor = Color.clear;
				break;
			case "cyan":
				ReturnColor = Color.cyan;
				break;
			case "gray":
				ReturnColor = Color.gray;
				break;
			case "magenta":
				ReturnColor = Color.magenta;
				break;

			default:
				ReturnColor = Color.red;
				break;
		}

		return ReturnColor;

	}

	void Save () {
		SaveData data = new SaveData ();
		data.HexGrid = HexGrid;
		data.CurrentPlayerName = CurrentPlayerName;
		data.PlayerList = PlayerList;
		data.Settings = Settings;
		Stream stream = File.Open (GameInfo, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter ();
		bformatter.Binder = new VersionDeserializationBinder ();
		bformatter.Serialize (stream, data);
		stream.Close ();
	}

	void Load () {
		HexGrid = null;
		SaveData data = new SaveData ();
		Stream stream = File.Open (GameInfo, FileMode.Open);

		BinaryFormatter bformatter = new BinaryFormatter ();
		bformatter.Binder = new VersionDeserializationBinder ();
		data = (SaveData) bformatter.Deserialize (stream);
		HexGrid = (data.HexGrid);
		CurrentPlayerName = (data.CurrentPlayerName);
		PlayerList = (data.PlayerList);
		if (data.Settings != null) {
			Settings = (data.Settings);
		}
		stream.Close ();
	}
	void LoadAndSetData () {
		gameObject.transform.position = new Vector3 (4.5f, 16f, -7f);
		CurrentPlayer.Name = Login;
		Load ();
		if (CurrentPlayer.Name != "ServerView") {
			CurrentPlayer = PlayerList.Find (Players => Players.Name == CurrentPlayer.Name);
		}
		//FixPlayers();
		CreateGrid (HexGrid);
		if (CurrentPlayer.Name != "ServerView") {
			foreach (HexGridClass x in HexGrid) {
				if (x.Owner == CurrentPlayer.Name) {
					GameObject theHex = GameObject.Find ("Hex: " + x.x + "-" + x.y);
					Camera.main.transform.position = new Vector3 (theHex.transform.position.x, Camera.main.transform.position.y, theHex.transform.position.z - 10);
					break;
				}
			}
		}

	}

	/*IEnumerator NextPlayer()
    {
           string Data;  // the data that will be recieved
        string url = "http://timaconner.com/DOTPHPFiles/GlobalInfo.php";
        WWWForm form = new WWWForm();

        form.AddField("authentication", "12345");//Add fields to url
        form.AddField("command", "NextPlayer");
        form.AddField("current", CurrentPlayer.Name);
        form.AddField("gamename", GameName);
        form.AddField("globalid", PlayerRefrence);
        WWW download = new WWW(url, form);
        yield return download;
        if ((!string.IsNullOrEmpty(download.error)))
        {
            print("Error downloading: " + download.error);
        }
        else
        {
            print(download.text);
        }

    }
*/
	/*IEnumerator UpdatePlayer()
    {
        string Data;  // the data that will be recieved
        string url = "http://timaconner.com/DOTPHPFiles/updateplayer.php";
        WWWForm form = new WWWForm();

        form.AddField("authentication", "12345");//Add fields to url
        form.AddField("command", "UpdatePlayer");
        form.AddField("wood", CurrentPlayer.Wood);
        form.AddField("food", CurrentPlayer.Food);
        form.AddField("coin", CurrentPlayer.Coin);
        form.AddField("stone", CurrentPlayer.Stone);
        form.AddField("name", CurrentPlayer.Name);
        form.AddField("gamename", GameName);


        WWW download = new WWW(url, form);
        yield return download;
        if ((!string.IsNullOrEmpty(download.error)))
        {
            print("Error downloading: " + download.error);
        }
        else
        {
            print(download.text);
        }
    }
*/
	void Battle (UnitClass x, HexGridClass y) {
		if (y.Units.Count > 0 && y.Units[0].Owner != CurrentPlayer.Name && y.Units[0] != x && !HasAlly (y.Units[0].Owner)) { //fight units
			Debug.Log ("Unit");
			/*
			y.Units[0].Wave1
			y.Units[0].Wave2
			y.Units[0].Wave3
			x.Units[0]
			int AttackingPoints = 0;
			int DefendingPoints = 0;

			foreach*/

			SoldierClass FighterA;
			SoldierClass FighterB;

			Debug.Log (x.Wave1.Count + " " + y.Units[0].Wave1.Count);
			if (x.Wave1.Count > 0 && y.Units[0].Wave1.Count > 0) {
				bool Wave1Dead = false;
				FighterA = x.Wave1[0];
				FighterB = y.Units[0].Wave1[0];
				while (Wave1Dead == false) {
					//while(armydead == false){
					Debug.Log ("A " + WhatUnit (FighterA.Type) + ":" + FighterA.Wounds + " - B " + WhatUnit (FighterB.Type) + ":" + FighterB.Wounds);
					Fight (FighterA, FighterB);
					Debug.Log ("A " + WhatUnit (FighterA.Type) + ":" + FighterA.Wounds + " - B " + WhatUnit (FighterB.Type) + ":" + FighterB.Wounds);
					Fight (FighterB, FighterA);
					Debug.Log ("A " + WhatUnit (FighterA.Type) + ":" + FighterA.Wounds + " - B " + WhatUnit (FighterB.Type) + ":" + FighterB.Wounds);
					if (FighterA.Wounds <= 0) {
						x.Wave1.Remove (FighterA);
						Debug.Log ("x" + x.Wave1.Count);
						if (x.Wave1.Count > 0) {
							FighterA = x.Wave1[0];
						}
					}
					if (FighterB.Wounds <= 0) {
						y.Units[0].Wave1.Remove (FighterB);
						Debug.Log ("y" + y.Units[0].Wave1.Count);
						if (y.Units[0].Wave1.Count > 0) {
							FighterB = y.Units[0].Wave1[0];
						}
					}

					if (y.Units[0].Wave1.Count <= 0 || x.Wave1.Count <= 0) {
						Wave1Dead = true;
					}
				}
			} else {
				ShowErrorScreen ("You have no army in your first wave!");
			}

			Debug.Log (x.Wave1.Count + " -" + y.Units[0].Wave1.Count);
			//if(y.Units[0].Wave1.Count 

			if (x.Wave1.Count <= 0 && x.Soldiers.Count <= 0) {
				currentlySelectedGrid.Units.Remove (x);
				Destroy (GameObject.Find ("Hex: " + currentlySelectedGrid.x + "-" + currentlySelectedGrid.y).transform.FindChild ("Hex (" + currentlySelectedGrid.x + "-" + currentlySelectedGrid.y + ")").transform.FindChild ("Soldiers").gameObject);

			}

			if (y.Units[0].Wave1.Count <= 0 && y.Units[0].Soldiers.Count <= 0) {
				y.Units.Remove (y.Units[0]);

				Destroy (GameObject.Find ("Hex: " + y.x + "-" + y.y).transform.FindChild ("Hex (" + y.x + "-" + y.y + ")").transform.FindChild ("Soldiers").gameObject);

			}
			CreateGrid (HexGrid);

		}

		/*
		foreach(SoldierClass s1 in x.Wave1){//Defender
		Debug.Log(" " + i + " " + y.Units[0].Wave1.Count);
		if(s1.Type == 0 && y.Units[0].Wave1[i].Type == 1){//0 horse 1 archer 2 pike soldier
		y.Units[0].Wave1[i].Wounds -= 2;
		}else if(s1.Type == 1 && y.Units[0].Wave1[i].Type == 2){//0 horse 1 archer 2 pike soldier
		y.Units[0].Wave1[i].Wounds -= 2;
		}else if(s1.Type == 2 && y.Units[0].Wave1[i].Type == 0){//0 horse 1 archer 2 pike soldier
		y.Units[0].Wave1[i].Wounds -= 2;
		} else{
		y.Units[0].Wave1[i].Wounds -= 1;
		}
		i++;
		}

		foreach(SoldierClass died1 in y.Units[0].Wave1){
		if(died1.Wounds <= 0){
		y.Units[0].Wave1.Remove(died1);
		}
		}

		i = 0;
		if(y.Units[0].Wave1.Count > 0 && x.Wave1.Count > 0){
		Debug.Log(" " + i + " " + x.Wave1.Count);
		foreach(SoldierClass s1 in y.Units[0].Wave1){//Attacker=
		if(s1.Type == 0 && x.Wave1[i].Type == 1){//0 horse 1 archer 2 pike soldier
		x.Wave1[i].Wounds -= 2;
		}else if(s1.Type == 1 && x.Wave1[i].Type == 2){//0 horse 1 archer 2 pike soldier
		x.Wave1[i].Wounds -= 2;
		}else if(s1.Type == 2 && x.Wave1[i].Type == 0){//0 horse 1 archer 2 pike soldier
		x.Wave1[i].Wounds -= 2;
		}else{
		x.Wave1[i].Wounds -= 1;
		} 
		i++;
		}

		}
		*/
		else if (y.Owner != CurrentPlayer.Name && !HasAlly (y.Owner) && y.Owner != "" && y.Building != "None") {
			Debug.Log ("Building");
			BuyHex (y, 1);
		} else {
			Debug.Log ("Neither!");
		}
	}
	void Fight (SoldierClass x, SoldierClass y) {
		if (x.Type == 0 && y.Type == 1) { //0 horse 1 archer 2 pike soldier
			y.Wounds -= 2;
		} else if (x.Type == 1 && y.Type == 2) { //0 horse 1 archer 2 pike soldier
			y.Wounds -= 2;
		} else if (x.Type == 2 && y.Type == 0) { //0 horse 1 archer 2 pike soldier
			y.Wounds -= 2;
		} else {
			y.Wounds -= 1;
		}
	}
}