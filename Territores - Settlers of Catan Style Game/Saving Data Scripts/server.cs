using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text;
using UnityEngine;

public class server : MonoBehaviour {
    public List<HexGridClass> Hexes = new List<HexGridClass> ();
    public string HexInfoString;
    bool done = false;
    int locationx = 0;
    int locationy = 0;
    string Type = "";
    int Width = 50;
    string width = "5";
    public Material Farmland;
    public Material Forest;
    public Material Water;
    public Material Moutain;
    public Material Hill;
    string GameName = "MapA";
    public List<PlayerClass> Players = new List<PlayerClass> ();
    public GameObject HexPrefab; //the prefab for the hex model
    string Set = "Settings";
    public HexGridClass currentlySelectedGrid;
    string NewPlayer = "Player Name";
    Rect windowRect = new Rect (20, 20, 200, Screen.height);
    public List<SettingsClass> Settings = new List<SettingsClass> ();

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown (0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, 100) && GUIUtility.hotControl == 0) {
                string hexName = hit.transform.gameObject.name;
                string hexCordinates = hexName.Substring (hexName.IndexOf ("("), hexName.IndexOf (")") - hexName.IndexOf ("(") + 1);

                int y = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("-") + 1, hexName.IndexOf (")") - hexName.IndexOf ("-") - 1));

                int x = System.Convert.ToInt32 (hexCordinates.Substring (hexCordinates.IndexOf ("(") + 1, hexName.IndexOf ("-") - hexName.IndexOf ("(") - 1));

                // List<GridPoint> hexes = GetHexesAround(x, y, 1);

                currentlySelectedGrid = PointData (x, y);
            }
        }
        if (Type == "Farmland" || Type == "Forest" || Type == "Water" || Type == "Hill" || Type == "Mountain" || Type == "Plain") {

            if (locationx == Width) {
                locationy++;
            }

            if (locationx < Width) {
                locationx++;
            } else {
                locationx = 0;
            }

            UpdateLand ();
            Type = "";

        }

    }
    HexGridClass PointData (int x, int y) {
        HexGridClass theHex = Hexes.Find (xVar => xVar.x == x && xVar.y == y);

        return theHex;
    }
    void OnGUI () {
        windowRect = GUI.Window (0, windowRect, DoMyWindow, Set);

    }
    void ChangeHex () {

        GameObject change;
        change = GameObject.Find ("Hex (" + currentlySelectedGrid.x + "-" + currentlySelectedGrid.y + ")");

        if (currentlySelectedGrid.Type == "Farmland") {
            currentlySelectedGrid.Rotation = UnityEngine.Random.Range (-18f, 18f) * 10;
            currentlySelectedGrid.RotationB = UnityEngine.Random.Range (-18f, 18f) * 10;
            change.GetComponent<Renderer> ().material = Farmland;
        } else if (currentlySelectedGrid.Type == "Forest") {
            currentlySelectedGrid.Rotation = UnityEngine.Random.Range (-18f, 18f) * 10;
            change.GetComponent<Renderer> ().material = Forest;
        } else if (currentlySelectedGrid.Type == "Water") {
            change.GetComponent<Renderer> ().material = Water;
        } else if (currentlySelectedGrid.Type == "Mountain") {
            currentlySelectedGrid.Rotation = UnityEngine.Random.Range (-18f, 18f) * 10;
            change.GetComponent<Renderer> ().material = Moutain;
        } else if (currentlySelectedGrid.Type == "Hill") {
            currentlySelectedGrid.Rotation = UnityEngine.Random.Range (-18f, 18f) * 10;
            change.GetComponent<Renderer> ().material = Hill;
        } else if (currentlySelectedGrid.Type == "Plain") {
            change.GetComponent<Renderer> ().material = null;
        }
    }
    void UpdateLand () {

        Hexes.Add (new HexGridClass (locationx, locationy, Type));

        GameObject instance;
        if (locationy % 2 == 0) {
            instance = (GameObject) Instantiate (HexPrefab, new Vector3 ((float) (locationx + (2 * 0.5)), 0, (float) (locationy * 0.85)), HexPrefab.transform.rotation);
        } else {
            instance = (GameObject) Instantiate (HexPrefab, new Vector3 ((float) (locationx + (1 * 0.5)), 0, (float) (locationy * 0.85)), HexPrefab.transform.rotation);

        }
        foreach (Transform child in instance.transform) {
            child.name = "Hex (" + locationx + "-" + locationy + ")";
            if (Type == "Farmland") {
                child.GetComponent<Renderer> ().material = Farmland;

            } else if (Type == "Forest") {
                child.GetComponent<Renderer> ().material = Forest;
            } else if (Type == "Water") {
                child.GetComponent<Renderer> ().material = Water;
            } else if (Type == "Mountain") {
                child.GetComponent<Renderer> ().material = Moutain;
            } else if (Type == "Hill") {
                child.GetComponent<Renderer> ().material = Hill;
            } else if (Type == "Plain") {
                child.GetComponent<Renderer> ().material = Forest;
            }
        }

    }
    IEnumerator AddHexes () {
        string Data;
        string url = "http://timaconner.com/DOTPHPFiles/newHex.php";
        WWWForm form = new WWWForm ();

        form.AddField ("auth", "12345");
        form.AddField ("command", "Add");
        form.AddField ("hexes", HexInfoString);
        form.AddField ("gamename", GameName);

        WWW download = new WWW (url, form);
        yield return download;
        if ((!string.IsNullOrEmpty (download.error))) {
            print ("Error downloading: " + download.error);
        } else {
            Data = download.text;
            print (Data);
            Set = "<b>Done</b>";

        }

    }
    void DoMyWindow (int windowID) {
        GUI.DragWindow (new Rect (0, 0, 10000, 20));
        GUILayout.Label ("<b>Add</b>" + " X: " + locationx + " Y: " + locationy);
        GUILayout.BeginHorizontal ();

        // Type = GUILayout.TextField(Type, 25);
        /*  if (GUILayout.Button("Water"))
        {
            Type = "Water";

           // Add();
    
        }
        if (GUILayout.Button("Forest"))
        {
            Type = "Forest";

           // Add();

        }
        if (GUILayout.Button("Farm"))
        {
            Type = "Farmland";

           // Add();

        }*/
        GUILayout.EndHorizontal ();
        GUILayout.BeginHorizontal ();
        /* if (GUILayout.Button("Hill"))
         {
             Type = "Hill";

             //Add();

         }
         if (GUILayout.Button("Mt"))
         {
             Type = "Mountain";

             //Add();

         }*/
        if (GUILayout.Button ("WaterRow")) {
            for (int i = 0; i < Width; i++) {
                Type = "Water";

                Add ();
            }

            locationy++;
            locationx = 0;

        }
        GUILayout.EndHorizontal ();

        GUILayout.Space (10);
        GUILayout.Label ("<b>Edit</b>");
        GUILayout.TextField (currentlySelectedGrid.Type, 25);
        GUILayout.BeginHorizontal ();

        if (GUILayout.Button ("Water") || Input.GetKeyDown (KeyCode.A)) {
            currentlySelectedGrid.Type = "Water";
            ChangeHex ();
        }
        if (GUILayout.Button ("Forest") || Input.GetKeyDown (KeyCode.S)) {
            currentlySelectedGrid.Type = "Forest";
            ChangeHex ();
        }
        if (GUILayout.Button ("Farm") || Input.GetKeyDown (KeyCode.D)) {
            currentlySelectedGrid.Type = "Farmland";
            ChangeHex ();
        }
        GUILayout.EndHorizontal ();
        GUILayout.BeginHorizontal ();
        if (GUILayout.Button ("Hill") || Input.GetKeyDown (KeyCode.F)) {
            currentlySelectedGrid.Type = "Hill";
            ChangeHex ();
        }
        if (GUILayout.Button ("Mt") || Input.GetKeyDown (KeyCode.G)) {
            currentlySelectedGrid.Type = "Mountain";
            ChangeHex ();
        }
        if (GUILayout.Button ("Plain") || Input.GetKeyDown (KeyCode.H)) {
            currentlySelectedGrid.Type = "Plain";
            ChangeHex ();
        }
        if (GUILayout.Button ("Rnd") || Input.GetKeyDown (KeyCode.R)) {
            int random = UnityEngine.Random.Range (0, 5);
            if (random == 0) {
                currentlySelectedGrid.Type = "Forest";
            } else if (random == 1) {
                currentlySelectedGrid.Type = "Mountain";
            } else if (random == 2) {
                currentlySelectedGrid.Type = "Farmland";
            } else if (random == 3) {
                currentlySelectedGrid.Type = "Hill";
            } else if (random == 4) {
                currentlySelectedGrid.Type = "Plain";
            }

            ChangeHex ();
        }

        GUILayout.EndHorizontal ();

        GUILayout.Space (10);

        GUILayout.BeginHorizontal ();
        GUILayout.Label ("Map Name");
        GameName = GUILayout.TextField (GameName, 25);
        GUILayout.EndHorizontal ();
        GUILayout.Space (10);

        width = GUILayout.TextField (width, 2);
        Width = Width > 0 ? System.Convert.ToInt32 (width) : 1;

        GUILayout.Space (10);
        NewPlayer = GUILayout.TextField (NewPlayer, 25);
        if (done == false && Hexes.Count > 0) {
            if (GUILayout.Button ("Save")) {
                Settings.Add (new SettingsClass ("TurnNumber", "0"));
                Save ();
            }
        }
        if (GUILayout.Button ("Add Player")) {
            PlayerClass x = new PlayerClass (NewPlayer);
            Players.Add (x);
            currentlySelectedGrid.Owner = NewPlayer;
            NewPlayer = "";
            switch (Players.Count) {
                case 1:
                    x.Color = "red";
                    break;
                case 2:
                    x.Color = "blue";
                    break;
                case 3:
                    x.Color = "yellow";
                    break;
                case 4:
                    x.Color = "green";
                    break;
                case 5:
                    x.Color = "magenta";
                    break;
                case 6:
                    x.Color = "cyan";
                    break;
                case 7:
                    x.Color = "gray";
                    break;
                default:
                    x.Color = "red";
                    break;
            }
        }

        foreach (PlayerClass x in Players) {
            GUILayout.Label (x.Name);
        }

    }
    void Add () {
        if (Type == "Farmland" || Type == "Forest" || Type == "Water" || Type == "Hill" || Type == "Mountain" || Type == "Plain") {

            UpdateLand ();

            if (locationx == Width) {
                locationy++;
            }

            if (locationx < Width) {
                locationx++;
            } else {
                locationx = 0;
            }
            Type = "";
        }

    }
    void Save () {

        SaveData data = new SaveData ();
        data.HexGrid = Hexes;
        data.PlayerList = Players;
        data.Settings = Settings;
        if (Players.Count > 0) {
            data.CurrentPlayerName = Players[0].Name;
        }
        Stream stream = File.Open (GameName, FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter ();
        bformatter.Binder = new VersionDeserializationBinder ();
        bformatter.Serialize (stream, data);
        stream.Close ();
    }
}