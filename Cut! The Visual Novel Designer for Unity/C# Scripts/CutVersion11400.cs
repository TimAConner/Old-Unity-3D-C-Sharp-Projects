/*
 

CUT! Visual Novel Language - Tim A. Conner
Version 1.1525
c.timmy@yahoo.com
� 2014 Tim A. Conner

 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class CutVersion11400 : MonoBehaviour {
    //Version & Name Variables
    private string CutVersion = "1.1525";
    private string GameFileName = "Game00.cut";
    //private string GameVersion = "1.00";// CHKNG-03
    //Data File File Location Variables
    private string GameFileLocation;
    private string MenuFileLocation;
    private string SettingFileLocation;

    //Data File Variables
    public TextAsset GameFile;
    public TextAsset MenuFile;
    public TextAsset SettingsFile;

    [Tooltip ("The current line that the game is reading.")]
    public int LineSearching = -1; //line currently being looked at in script
    private int SetLineSearching = 0; //Settings line
    private int MenuLineSearching = 0;
    private string CurrentLineText = "";
    [HideInInspector]
    public string[] lines;
    [HideInInspector]
    public string[] SetLines; //The lines for the settings file to be read
    [HideInInspector]
    public string[] MenuLines;

    //Decleration List Variables
    public List<AnimationClass> Animations = new List<AnimationClass> ();
    public List<Character> Characters = new List<Character> ();
    [Tooltip ("Images that are currently being displayed on the screen. This does not include the background.")]
    public List<CurrentImage> CurrentImages = new List<CurrentImage> (); //array of images that are currently being used
    [Tooltip ("This is for the actual text in the textbox.")]
    public GUIStyle DefaultTextStyle; //default text
    public List<GUIStyle> FontStyles = new List<GUIStyle> ();
    public List<TextureVariable> Images = new List<TextureVariable> ();
    public List<MovieClass> Movies = new List<MovieClass> ();
    public List<SceneClass> Scenes = new List<SceneClass> (); //Scenes are after declerations but are found when searching before game for them
    public List<AudioClass> Sounds = new List<AudioClass> ();
    [Tooltip ("This is only for the text box image & size.")]
    public GUIStyle TextBoxStyle; //text box textures
    [Tooltip ("This is for the bar that displays the name of the character that is currently speaking.")]
    public GUIStyle TitleBar; //title bar textures

    //GUI Styles

    //Reading File Variables
    private Boolean Running = true; //True means it is currently running, false means the game is done/paused.
    private Boolean DeclerationCheck = true; //This is true while getting declerations

    private bool FoundStart = false; //true if #Scene Start has been found

    //Dialog Variables
    private String DialogName = ""; //The name over the dialog box
    private String Dialog = ""; //Dialog to be shown
    private string DialogColor = ""; //Color of Dialog
    private string NewDialog = ""; //This is the dialog that is currently being read
    private int StringNumber = 0; //the current text-character number that is being transfered
    private int LineNumber = 1; //The current line number in the dialog box
    private int curWord = 0;
    private bool NWFSTF = false; //NextWordFinishgSlowTextFirst - Used for waiting till the current speech is done to go to the next line if there is speech

    private Color CharacterTextColor = Color.white; //Color of the characters text
    private bool TextBox = true; //If the text box is on or not
    private bool TextBoxWasOn = true; //Used when flipping between menus to check whether the text box was on before the menu came up
    float BackgroundAlpha = 1.0f; //The transparency of the text box
    [HideInInspector]
    public List<String> DialogHistory = new List<String> (); //An array of strings that are the previous dialog
    //Background Variables
    [HideInInspector]
    public TextureVariable Background = new TextureVariable ();
    //Background Fading Variables
    [HideInInspector]
    public TextureVariable BackgroundB = new TextureVariable (); //The background that is being faded to
    private double TimeToFade = -1.0; //The ammount of seconds to fade from the original transprency to the next.
    private double BackgroundFade = 1.0; //The current transparency of the background
    private bool ClickedThrough = false; // var if you click through it will automaticly finish animation

    private double AnimationTime = 0.0; //Used for animations

    //Choice GUI Variables
    [HideInInspector]
    public List<ButtonClass> ChoiceButtons = new List<ButtonClass> (); //array of buttons and what their command sare when you click them
    [HideInInspector]
    public bool Choices = false;

    //Sound Variables
    //Sound Fading Variables
    [HideInInspector]
    public AudioSource Channel1;
    [HideInInspector]
    public ChannelClass Channel1Options = new ChannelClass (1);
    [HideInInspector]
    public AudioSource Channel2;
    [HideInInspector]
    public ChannelClass Channel2Options = new ChannelClass (2);

    private bool MusicWasPlaying1 = false;
    private bool MusicWasPlaying2 = false;

    //Control Variables
    private bool MouseBreak = false;

    //Settings From Settings File Variables
    private int TextSpeed;
    private int WindowWidth;
    private int WindowHeight;
    private int TextBoxHeight;
    private int TextBoxWidth;
    private float TextBoxOpacity;
    private string GameName;
    private string WindowName;
    private int FullScreen;
    private string FileEndText = "File Read to End.";
    private bool CommandLine = true;
    private string TextFont;
    private int TextSize;
    private string Language;
    private float GameVersion;

    //COMMAND LINE
    private bool CommandLineOpen = false;
    private string CommandLineCommand = "";
    private string CommandLineOutput = "";

    private bool IsError = false;

    GUIStyle WindowStyle;

    string CurrentFontStyle = "";

    GUIStyle searchingStyle; //CHKNG-04

    //Prefrences CHKNG-06
    float soundLevel = 1.0f; //CHKNG-05

    int resizeWidth = Screen.width;
    int resizeHeight = Screen.height;
    int oldWidth = -1;
    int oldHeight = -1;
    bool resized = false;

    //Rectangle/Window Variables
    private Rect DialogHistoryRect = new Rect (0, 0, 0, 0);
    private Vector2 DialogHistoryScroll = Vector2.zero;
    private Rect DialogBoxRect = new Rect (0, 0, 0, 0);

    //Shaking Camera Variables
    float CameraXOffset = 0.0f; //current offset
    float SecondsToShake = 0.0f; //how many seconds to shake, counts down
    float IntervalShake = 0.0f; //how many seconds to wait
    float AmmountToShake = 0.0f; //how much to shake
    float TotalShakeTime = 0.0f; //how many seconds to shake original

    //Movie Variables
    [HideInInspector]
    public MovieTexture currentMovie;
    float movieFadein;
    float movieFadeout;
    float moviecurrentFade = 0.0f;
    float moviePlayedForTime = 0.0f;

    float Previous = 0;
    //public List<GUIStyle> Styles = new List<GUIStyle>();

    //Screen Resolution & Resizing Variables
    float lastScreenWidth = 0f;

    //Menu Variables
    private bool MenuShowing = false; //if menus are being shown
    private string CurrentMenuShowing = ""; //what menu is being shown
    private string LastMenuShowed = ""; //the previous menu for "back"
    [HideInInspector]
    public List<MenuClass> MenuList = new List<MenuClass> (); //contains all the menus
    public List<MenuControls> MenuControlsList = new List<MenuControls> (); //list of the current controls up
    private bool FadeInMainMenu = false;

    private bool IsNextActive = false; //Has NEXT been called
    private bool IsLineComplete = true; //Starts off as true and will be set to false if somthing happens

    //Regex Variables
    string SpecialKeywords = @"\s*(NEXT|MouseBreak|MouseFix|NextFade)*";
    /*Add to end of all non-declerations (Regex-End-String).  This is for things like:
     * NEXT
     * NextFade
     * MouseBreak
     * MouseFix 
     * */
    int countnum = 0; //The ammount of seconds currently passed in the game.

    private float TimeToWait = 0; //Time to wait until wait is complete
    private bool Waiting = false; //Is the progam waiting?

    IEnumerator Wait (float WaitTime = 0) {

        if (Running == true && Waiting == true) {
            TimeToWait = WaitTime == 0 ? TimeToWait : WaitTime;
            while (TimeToWait > 0) {
                TimeToWait -= 0.01f;
                yield return new WaitForSeconds (0.01f);
            }
            if (TimeToWait <= 0) {
                Waiting = false;
                NEXTKeyword (1);
            }
        }
    }

    IEnumerator ShakeScreen (float WaitTime, int numberOfTimes = 1) {

        yield return new WaitForSeconds (WaitTime);
        CameraXOffset += 25f;
        Debug.Log ("hit" + CameraXOffset);
        yield return new WaitForSeconds (WaitTime);
        CameraXOffset = 0f;
        Debug.Log ("hitB" + CameraXOffset);

        StartCoroutine (ShakeScreen (0.1f, 100));
    }
    void ShakeScreen (float timeToShake, float offset, float interval) {

    }
    IEnumerator Count () {
        yield return new WaitForSeconds (1);
        countnum++;
        //Debug.Log("TIME:" + countnum);
        StartCoroutine (Count ());
    }
    IEnumerator WriteText () //Sliowly adds text function
    {

        Dialog = EscapeCharacters (Dialog);
        if (StringNumber == 0) {
            curWord = 0;
        }
        if (MenuShowing == false) {

            if (NewDialog != "") //if theres dialog, wait then add it!
            {
                IsLineComplete = false;

                yield return new WaitForSeconds ((float) (1.0 / TextSpeed));
                string[] DialogMod = Dialog.Split ("\n" [0]);
                string[] CurWord = NewDialog.Split (" " [0]);
                if (curWord <= CurWord.Length - 1) {
                    GUIContent Dial = new GUIContent (DialogMod[LineNumber - 1] + CurWord[curWord]);

                    Vector2 width = DefaultTextStyle.CalcSize (Dial);

                    int left = DefaultTextStyle.padding.left;
                    int right = DefaultTextStyle.padding.right;
                    float curWidth = (float) ((width.x + left));
                    if ((curWidth) >= (Screen.width - right) && NewDialog[StringNumber - 1] == ' ') {

                        Previous += curWidth;
                        LineNumber++;
                        Dialog += "\r\n";
                    }

                }

                if (StringNumber < NewDialog.Length) {
                    Dialog += NewDialog[StringNumber];
                    if (NewDialog[StringNumber] == ' ') {
                        curWord++;
                    }
                    StringNumber++;
                }

            }

            if (StringNumber >= NewDialog.Length && Running == true) //reset string number if its done with the string
            {
                NewDialog = "";
                StringNumber = 0;

                if (IsNextActive == true) {
                    yield return new WaitForSeconds ((float) (1.0));
                    NewDialog = "";
                    StringNumber = 0;

                    NEXTKeyword (1);
                    StopCoroutine ("WriteText");
                }

                /*if (NWFSTF == true)//If the next line is speech and next has been set, go to the next line.
                {
                    NWFSTF = false;

                    yield return new WaitForSeconds((float)(0.5));

                    NewDialog = "";
                    StringNumber = 0;

                    NextLine();
                    ContinueGame();
                    StopCoroutine("SlowText");
                }*/

            } else if (StringNumber != 0) //if thgere is still text to go repeat the function!
            {
                StartCoroutine (WriteText ());
            }

        }
    }

    void FinishText (string SpecificDialog = "") //SpecificDialog, it will only complete specific dialog and then keep counting
    {
        LineNumber = 1;
        Dialog = "";
        int str = 0;
        curWord = 0;
        Dialog = EscapeCharacters (Dialog);
        StopAllCoroutines ();
        foreach (char a in NewDialog) {

            string[] DialogMod = Dialog.Split ("\n" [0]);
            string[] CurWord = NewDialog.Split (" " [0]);
            if (curWord <= CurWord.Length - 1) {
                GUIContent Dial = new GUIContent (DialogMod[LineNumber - 1] + CurWord[curWord]);

                Vector2 width = DefaultTextStyle.CalcSize (Dial);

                int left = DefaultTextStyle.padding.left;
                int right = DefaultTextStyle.padding.right;
                float curWidth = (float) ((width.x + left));
                if ((curWidth) >= (Screen.width - right) && NewDialog[str - 1] == ' ') {
                    Previous += curWidth;
                    LineNumber++;
                    Dialog += "\r\n";
                }

            }

            if (StringNumber < NewDialog.Length) {
                Dialog += NewDialog[str];
                if (NewDialog[str] == ' ') {
                    curWord++;
                }
                str++;
            }

        }
    }

    void Start () {
        // StartCoroutine(ShakeScreen(0.1f, 100));

        lastScreenWidth = Screen.width; //get the starting screen size
        StartCoroutine (Count ());
        DefaultTextStyle.wordWrap = false;

        Channel1 = gameObject.AddComponent<AudioSource> ();
        Channel2 = gameObject.AddComponent<AudioSource> ();

        LoadDefaults ();
        OpenSettingsFile ();

    }
    void LoadDefaults () {
        LoadImage ("Images/DefaultImages/BlackSpace", "DefaultImage-BlackSpace");
        LoadImage ("Images/DefaultImages/ImageDoesNotExist", "DefaultImage-ImageDoesNotExist");
    }
    void OpenSettingsFile () //use this to get the settings file
    {

        SetLines = SettingsFile.text.Split ("\n" [0]);
        ReadSettings ();

    }
    void OpenGameFile () { // Read the file and display it line by line.
        lines = GameFile.text.Split ("\n" [0]);
        GetDeclerations ();
    } // Use this for initialization, opens the game file/gets the text
    void OpenMenu () {
        MenuLines = MenuFile.text.Split ("\n" [0]);
        ReadMenu ();
    }
    void ReadSettings () //Readst the settings of the settings file
    {

        foreach (string line in SetLines) {
            SetLineSearching++;
            if (line.Contains ("//")) { } else if (line.Contains ("GameName")) {
                GameName = SettingsSubString ("GameName", line);
            } else if (line.Contains ("WindowName")) {
                WindowName = SettingsSubString ("WindowName", line);
            } else if (line.Contains ("FullScreen")) {
                try { FullScreen = Int32.Parse (SettingsSubString ("FullScreen", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("TextFont")) {
                TextFont = SettingsSubString ("TextFont", line);
            } else if (line.Contains ("TextSpeed")) {
                try { TextSpeed = Int32.Parse (SettingsSubString ("TextSpeed", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("TextSize")) {
                try { TextSize = Int32.Parse (SettingsSubString ("TextSize", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("Language")) {
                Language = SettingsSubString ("Language", line);
            } else if (line.Contains ("WindowHeight")) {
                try { WindowHeight = Int32.Parse (SettingsSubString ("WindowHeight", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("WindowWidth")) {
                try { WindowWidth = Int32.Parse (SettingsSubString ("WindowWidth", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("TextBoxWidth")) {
                try { TextBoxWidth = Int32.Parse (SettingsSubString ("TextBoxWidth", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("TextBoxHeight")) {
                try { TextBoxHeight = Int32.Parse (SettingsSubString ("TextBoxHeight", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("TextBoxOpacity")) {
                try { TextBoxOpacity = float.Parse (SettingsSubString ("TextBoxOpacity", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else if (line.Contains ("FileEndText")) {
                FileEndText = SettingsSubString ("FileEndText", line);
            } else if (line.Contains ("CommandLine")) {
                string
                var = SettingsSubString ("CommandLine", line);
                if (var == "1") {
                    CommandLine = true;
                } else {
                    CommandLine = false;
                }

            } else if (line.Contains ("GameVersion")) {
                try { GameVersion = float.Parse (SettingsSubString ("GameVersion", line)); } catch (Exception e) { PrintTextError ("ERROR: <b>Settings-File</b>: Parsing at line: "); }
            } else {
                PrintTextError ("ERROR: <b>Settings-File</b>: No valid setting name at line: ");
            }
        }
        OpenMenu ();
    }
    void ReadMenu () //Will parse and read the menus 
    {
        string MenuName = "";
        List<string> LinesForMenu = new List<string> ();
        for (int index = 0; index < MenuLines.Length; index++) {

            if (MenuLines[index].Contains ("[")) //if line has [ making it be the name of the menu
            {
                if (MenuName == "") //if its the first time going through
                {
                    MenuName = MenuLines[index].Substring (MenuLines[index].IndexOf ("[") + 1, MenuLines[index].IndexOf ("]") - MenuLines[index].IndexOf ("[") - 1);
                } else if (MenuName != "") {
                    List<string> LinesForMenuCopy = new List<string> (LinesForMenu);
                    MenuList.Add (new MenuClass (MenuName, LinesForMenuCopy));
                    LinesForMenu.Clear ();
                    MenuName = MenuLines[index].Substring (MenuLines[index].IndexOf ("[") + 1, MenuLines[index].IndexOf ("]") - MenuLines[index].IndexOf ("[") - 1); //Add the new name after the old menu has been added

                }

            } else if (!String.IsNullOrEmpty (MenuLines[index]) && !MenuLines[index].Contains ("StartFold") && !MenuLines[index].Contains ("EndFold")) {
                LinesForMenu.Add (MenuLines[index]);
            }
            if (index >= (MenuLines.Length - 1)) { //if its at the end, add the last one
                MenuList.Add (new MenuClass (MenuName, LinesForMenu));
            }

        }
        OpenGameFile ();
    }

    void GetDeclerations () //Searches through before START SCENE and gets declerations
    {
        RemoveSpacingAndComments ();

        foreach (string line in lines) {
            string GDCurLine = line.Trim ();
            Debug.Log (GDCurLine);
            CurrentLineText = GDCurLine;
            LineSearching++; //Increment current line number
            if (Running == true) //Check if its currently running.
            {

                if ((!line.Contains (":")) && line.Contains ("#")) //make sure the user puts spaces before and after the colon
                {

                    // PrintTextError("<b>ERROR: Game-File</b>:  ':' not found at line: " );

                }
                if (lines[LineSearching].Contains ("StartFold") || lines[LineSearching].Contains ("EndFold")) { } //Contains Start || end fold do nothing
                else if (FoundStart == false && LineSearching >= lines.Length) //if its at the end of the file and no start was found
                {
                    PrintTextError ("<b>ERROR: Game-File</b>:  No '#Scene Start' found.");
                } else if (!line.Contains (":") && line.Trim () != "" && !Regex.Match (GDCurLine, @"^#Scene(\s*)Start$", RegexOptions.None).Success && line.Contains ("#")) {
                    PrintTextError ("<b>ERROR: Game-File</b>: Decleration does not contain a ':' at line: ");
                } else if (Regex.Match (GDCurLine, @"^#Scene(\s*)Start$", RegexOptions.None).Success) {
                    FoundStart = true;
                    break;
                } else if (line.Contains ("#Speech") || line.Contains ("#SetBackground") || line.Contains ("#ShowImage") || line.Contains ("#RemoveImage") || line.Contains ("#Animate") || line.Contains ("#PlayAudio") || line.Contains ("#PauseAudio")) //Check if functions are apart of declerations
                {
                    PrintTextError ("<b>ERROR: Game-File</b>:  Function Keyword cannot be before '#Scene Start' or no '#Scene Start' was found at line: ");
                } else if (Regex.Match (GDCurLine, @"^#Character(\s)*(\w*)(\s)*:(\s)*\((.*)\).*", RegexOptions.None).Success) {
                    string CharacterVarName = ""; //Keyword Variable Name 
                    string CharacterName = null; //The written name of the character
                    string TextColor = "#ffffffff"; //Text color of the character
                    try {

                        Match thisMatch = Regex.Match (GDCurLine, @"^#Character(\s*)(\w*)(\s*):(\s*)\((.*)\).*", RegexOptions.None);

                        CharacterVarName = thisMatch.Groups[2].Value; //Get Keyword Variable Name

                        string[] parameters = thisMatch.Groups[5].Value.Split (',');
                        foreach (string thisParameter in parameters) {
                            string thisParameterTrim = thisParameter.Trim ();
                            if (thisParameterTrim.Contains ("name")) {
                                CharacterName = Regex.Match (thisParameterTrim, @"^name(\s*)=(\s*)""(\w*)""$", RegexOptions.None).Groups[3].Value;
                            } else if (thisParameterTrim.Contains ("textcolor")) {
                                TextColor = Regex.Match (thisParameterTrim, @"^textcolor(\s*)=(\s*)""(.*)""$", RegexOptions.None).Groups[3].Value; //Get text color
                            }
                        }

                        if (CharacterName == null) {
                            PrintTextError ("<b>ERROR: Game-File</b>:  'name' Attribute not found at line: ");
                        }
                        Characters.Add (new Character (CharacterName, CharacterVarName, TextColor)); //Get all these and add them to the Characters list.
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }

                } //
                else if (Regex.Match (GDCurLine, @"^#Background(\s)*(\w*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)"".*", RegexOptions.None).Success) {

                    string Filename = "";
                    string BackgroundVarName = "";
                    Match thisMatch = Regex.Match (GDCurLine, @"^#Background(\s)*(\w*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)"".*", RegexOptions.None);
                    try {

                        BackgroundVarName = thisMatch.Groups[2].Value; //Get Keyword Variable Name
                        if (GDCurLine.Contains ("filename")) {
                            Filename = thisMatch.Groups[7].Value;

                        } else {

                            PrintTextError ("<b>ERROR: Game-File</b>:  'filename' Attribute not found at line: ");
                        }

                        LoadImage (Filename.Trim (), BackgroundVarName.Trim ());
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }

                } else if (Regex.Match (GDCurLine, @"^#Movie(\s)*(\w*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)"".*", RegexOptions.None).Success) {
                    string Filename = "";
                    string MovieVarName = "";
                    Match thisMatch = Regex.Match (GDCurLine, @"^#Movie(\s)*(\w*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)"".*", RegexOptions.None);
                    //  Debug.Log("ERRCD TEMPOUT-00-00-3-(1/25/2015,) % ERRCDF 00-00-3" + thisMatch.Groups[2].Value);
                    try {
                        MovieVarName = thisMatch.Groups[2].Value; //Get Keyword Variable Name
                        if (line.Contains ("filename")) {
                            Filename = thisMatch.Groups[7].Value; //Get filename

                        } else {
                            PrintTextError ("<b>ERROR: Game-File</b>:  'filename' Attribute not found at line: ");
                        }

                        Movies.Add (new MovieClass (MovieVarName, Filename));
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }

                } else if (Regex.Match (GDCurLine, @"^#Image(\s)*(.*)(\s)*:.*", RegexOptions.None).Success) {

                    string Filename = "";
                    string BackgroundVarName = "";
                    Match thisMatch = Regex.Match (GDCurLine, @"^#Image(\s)*(.*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)"".*", RegexOptions.None);
                    try {

                        BackgroundVarName = thisMatch.Groups[2].Value; //Get Keyword Variable Name
                        if (GDCurLine.Contains ("filename")) {
                            Filename = thisMatch.Groups[7].Value;

                        } else {

                            PrintTextError ("<b>ERROR: Game-File</b>:  'filename' Attribute not found at line: ");
                        }

                        LoadImage (Filename.Trim (), BackgroundVarName.Trim ());
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }

                } else if (Regex.Match (GDCurLine, @"^#Animation(\s)*(.*)(\s)*:(\s)*\(.*\)$", RegexOptions.None).Success) //IF I REMOVE THIS IT WORKS, FOR SOME REASON THIS TRIGGERS FILE READ TO END DIALOG
                {

                    string AnimationName = "";
                    string Animation = "";
                    Match thisMatch = Regex.Match (GDCurLine, @"^#Animation(\s)*(.*)(\s)*:(\s)*(\(.*\))$", RegexOptions.None);
                    try {

                        AnimationName = thisMatch.Groups[2].Value; //Get Keyword Variable Name

                        Animation = thisMatch.Groups[5].Value; //Gets everything after ':'

                        Animations.Add (new AnimationClass (AnimationName.Trim (), Animation.Trim ()));
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }
                } else if (Regex.Match (GDCurLine, @"^#Audio(\s)*(.*)(\s)*:(\s)*(.*)$", RegexOptions.None).Success) {
                    string Filename = "";
                    string AudioName = "";
                    Match thisMatch = Regex.Match (GDCurLine, @"^#Audio(\s)*(.*)(\s)*:(\s)*filename(\s)*=(\s)*""(.*)""$", RegexOptions.None);
                    try {
                        AudioName = thisMatch.Groups[2].Value; //Get Keyword Variable Name

                        if (GDCurLine.Contains ("filename")) {
                            Filename = thisMatch.Groups[7].Value; //Get filename
                            AudioClip AudioFile = Resources.Load (Filename) as AudioClip;
                            if (AudioFile == null) {
                                PrintTextError ("<b>ERROR: Game-File</b>:  Audio file could not be found at line: ");
                            }
                        } else {
                            PrintTextError ("<b>ERROR: Game-File</b>:  'filename' Attribute not found at line: ");
                        }
                        Sounds.Add (new AudioClass (AudioName.Trim (), Filename.Trim ()));
                    } catch (Exception e) {
                        PrintTextError ("<b>ERROR: Game-File</b>: at line: ");
                    }

                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Error at line: ");
                }

            }

        }

        if (Running == true) {
            GetScenes ();
        }

    }
    void RemoveSpacingAndComments () //Removes any comments, extra spacing and blank lines.
    {
        for (int i = 0; i < lines.Length; i++) {
            if (lines[i].Trim () == "" || lines[i].Contains ("StartFold") || lines[i].Contains ("EndFold")) {
                lines[i] = "";
            }
            if (Regex.Match (lines[i], @"^\s*\/\/.*", RegexOptions.None).Success) //remove all whole line comments
            {
                lines[i] = "";
            } else {
                lines[i] = Regex.Replace (lines[i], @"\/\/(.*?)\/", ""); //remove all comments
            }
        }
        lines = lines.Where (x => !string.IsNullOrEmpty (x)).ToArray (); //Remove lines from array
    }
    void GetScenes () //Gets all scenes, cant be in declerations cause it looks before continue game.
    {
        int Line = -1;
        foreach (string line in lines) {
            Line++;
            string CurLine = line.Trim ();
            if (Regex.Match (CurLine, @"^#Scene(\s*)(.*)", RegexOptions.None).Success) {
                Match thisMatch = Regex.Match (CurLine, @"^#Scene(\s*)(.*)", RegexOptions.None);
                Scenes.Add (new SceneClass (thisMatch.Groups[2].Value.Trim (), (Line + 1)));
            }

        }
        ContinueGame (); // Automaticly triggers the game to start
    }
    void ContinueGame () //Checks current line of code for any more input!
    {
        IsLineComplete = true;
        IsNextActive = false;

        string CGCurLine = lines[LineSearching].Trim ();
        Debug.Log (CGCurLine);
        // ERORR FIXE 1  print("continuing..:" + lines[LineSearching]);

        CurrentLineText = lines[LineSearching];

        if (lines[LineSearching].Contains ("#Scene Start") || lines[LineSearching].Contains ("StartFold") || lines[LineSearching].Contains ("EndFold")) //if the line is empty or null or has start/end fold go to next line, else add one to line
        {
            NextLine ();
            return;
        } else if (lines[LineSearching].Contains ("#FileEnd")) //Gets current line of lines using LineSearching as the index
        {
            PrintTextWithoutName (FileEndText);
            Running = false; //so the game wont work after this
        } else if (lines[LineSearching].Contains ("#Character") || lines[LineSearching].Contains ("#Background") || lines[LineSearching].Contains ("#Image") || lines[LineSearching].Contains ("#Animation") || lines[LineSearching].Contains ("#Audio")) {
            PrintTextError ("<b>ERROR: Game-File</b>: Decleration Keyword cannot be after '#Start Scene' at line: ");
        } else if (Regex.Match (CGCurLine, @"^#Speech (\w*)(\s)*:(\s)*""(.*)"".*" + SpecialKeywords, RegexOptions.None).Success) //SPEECH #Speech Name : "Text"
        {
            Match thisMatch = Regex.Match (CGCurLine, @"^#Speech\s*(.*)(\s)*:(\s)*""(.*)"".*" + SpecialKeywords, RegexOptions.None);
            try {

                string CharacterVarName = thisMatch.Groups[1].Value.Trim (); //Get the Character Varible Name
                string Dialog = thisMatch.Groups[4].Value; //Get Dialog 
                Character CharacterToFind = new Character ();
                CharacterToFind = Characters.Find (SearchingCharacter => SearchingCharacter.CharacterVarName == CharacterVarName); //find the character if it exists
                PrintText (CharacterToFind, Dialog);

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Speech Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^""(.*)"".*" + SpecialKeywords, RegexOptions.None).Success) //SPEECH "TEXT"
        {
            Match thisMatch = Regex.Match (CGCurLine, @"^""(.*)"".*" + SpecialKeywords, RegexOptions.None);
            string Dialog = thisMatch.Groups[1].Value; //Get teh Character Varible Name
            PrintTextWithoutName (Dialog);
        } else if (Regex.Match (CGCurLine, @"^(\w*)(\s*)""(.*)"".*" + SpecialKeywords, RegexOptions.None).Success) //SPEECH NAME "TEXT"
        {
            Match thisMatch = Regex.Match (CGCurLine, @"^(\w*)(\s*)""(.*)"".*" + SpecialKeywords, RegexOptions.None);
            try {

                string CharacterVarName = thisMatch.Groups[1].Value.Trim (); //Get the Character Varible Name
                string Dialog = thisMatch.Groups[3].Value; //Get Dialog 
                Character CharacterToFind = new Character ();
                CharacterToFind = Characters.Find (SearchingCharacter => SearchingCharacter.CharacterVarName == CharacterVarName); //find the character if it exists
                PrintText (CharacterToFind, Dialog);

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Speech Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#SetBackground(\s*)(\w*)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#SetBackground(\s*)(\w*)" + SpecialKeywords, RegexOptions.None);
            try {
                //ERRCD 00-00-2(1/24/2015,)%ERRCDF 00-00-2 ^#SetBackground(\s*)(.*?) "

                string BackgroundVarName = thisMatch.Groups[2].Value; //Get the background image name
                BackgroundVarName = BackgroundVarName.Trim (); //remove white spaces
                //Debug.Log(BackgroundVarName);
                if (Images.Exists (SearchingBackground => SearchingBackground.Name == BackgroundVarName)) //Check if this background exits
                {
                    //Debug.Log("ERRCD TEMPOUT-00-00-2-(1/24/2015,)%ERRCDF 00-00-2" + Background);
                    Background = SelectImage (BackgroundVarName); //Background varible equals found background
                    //Debug.Log("ERRCD TEMPOUT-00-00-2-(1/24/2015,)%ERRCDF 00-00-2" + Background);
                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Background Image cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: SetBackground Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#PlayMovie(\w*)(.*)(\s*):?(\s*)\(?(.*)\)?" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#PlayMovie(\s*)(\w*)(\s*):?(\s*)\(?(((\s*)fadein(\s*)=(\s*)([0-9]*\.?[0-9]*)\,?(\s*)|(\s*)fadeout(\s*)=(\s*)([0-9]*\.?[0-9]*)\,?(\s*))*)*\)?" + SpecialKeywords, RegexOptions.None);
            try {
                // Debug.Log("ERRCD TEMPOUT-00-00-4-(1/25/2015,) % ERRCDF 00-00-4" + thisMatch.Groups[2].Value);
                string MovieVarName = thisMatch.Groups[2].Value; //Get the movie name
                MovieVarName = MovieVarName.Trim (); //remove white spaces
                if (Movies.Exists (SearchingBackground => SearchingBackground.Name == MovieVarName)) //Check if this movie exits
                {
                    MovieClass thisMovie = Movies.Find (SearchingBackground => SearchingBackground.Name == MovieVarName); //Background varible equals found background

                    movieFadein = CGCurLine.Contains ("fadein") ? (float) Convert.ToDouble (thisMatch.Groups[10].Value) : 0;
                    movieFadeout = CGCurLine.Contains ("fadeout") ? (float) Convert.ToDouble (thisMatch.Groups[15].Value) : 0;

                    moviePlayedForTime = 0.0f;
                    currentMovie = thisMovie.MovieFile;
                    currentMovie.Play ();

                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Movie cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Movie Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#ShowImage\s*(.*)\s*:\s*(.*)" + SpecialKeywords, RegexOptions.None).Success) {

            Match thisMatch = Regex.Match (CGCurLine, @"^#ShowImage\s*(.*)\s*:\s*((\s*pos\s*=\s*\(\s*(\-?[0-9]*\.?[0-9]*\s*%?\s*)\s*\,?\s*(\-?[0-9]*\.?[0-9]*\s*%?\s*)(\s*\s*%?\s*)\,?\s*(\-?[0-9]*\.?[0-9]*\s*%?\s*)\s*\,?\s*(\-?[0-9]*\.?[0-9]*\s*%?\s*)\s*\,?\))\,?|\s*alpha\s*=\s*([0-9]*\.?[0-9]*)\,?|\s*animation\s*=\s*""(.*)""\,?)*" + SpecialKeywords, RegexOptions.None);
            try {
                float xpos = 0;
                float ypos = 0;
                float height = 0;
                float width = 0;
                double fade = 1.0;
                string animation = "";
                string ImageVarName = thisMatch.Groups[1].Value.Trim (); //Get teh Character Varible Name
                string PosVariable = "  "; //Position variable all info between ( and ) of pos()

                //  xpos = ScreenSize(Convert.ToDouble(FloatString), true);
                if (Images.Exists (SearchingImage => SearchingImage.Name == ImageVarName)) //Check if this image exits
                {

                    xpos = thisMatch.Groups[4].Value.Contains ("%") ? ScreenSize (Convert.ToDouble (thisMatch.Groups[4].Value.Replace ('%', ' ')), true) : (float) (Convert.ToDouble ((thisMatch.Groups[4].Value)));
                    ypos = thisMatch.Groups[5].Value.Contains ("%") ? ScreenSize (Convert.ToDouble (thisMatch.Groups[4].Value.Replace ('%', ' ')), false) : (float) (Convert.ToDouble ((thisMatch.Groups[5].Value)));
                    width = thisMatch.Groups[7].Value.Trim () == "" ? -1f : (float) (Convert.ToDouble ((thisMatch.Groups[7].Value)));
                    height = thisMatch.Groups[8].Value.Trim () == "" ? -1f : (float) (Convert.ToDouble ((thisMatch.Groups[8].Value)));
                    animation = thisMatch.Groups[2].Value.Contains ("animation") ? thisMatch.Groups[10].Value.Trim () : "";
                    fade = CGCurLine.Contains ("alpha") ? Convert.ToDouble (thisMatch.Groups[9].Value) : 1.0;

                    CurrentImages.Add (new CurrentImage (ImageVarName, xpos, ypos, height, width, fade, animation)); //Add the image to the current images list
                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Image cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Image Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#RemoveImage\s*(.*?)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#RemoveImage\s*(.*?)" + SpecialKeywords, RegexOptions.None);
            try {

                string ImageVarName = thisMatch.Groups[1].Value.Trim (); //Get the image name
                if (CurrentImages.Exists (SearchingImages => SearchingImages.Name == ImageVarName)) //Check if this image exits
                {
                    CurrentImage ImageToRemove = SelectCurrentImage (ImageVarName);; //find image
                    CurrentImages.Remove (ImageToRemove); //remove image
                } else {

                    PrintTextError ("<b>ERROR: Game-File</b>: Image to remove: " + ImageVarName + " cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: RemoveImage Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#Animate\s*(.*)\s*:\s*\((.*)\)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#Animate\s*(.*)\s*:\s*\((.*)\)" + SpecialKeywords, RegexOptions.None);
            try {

                string ImageVarName = thisMatch.Groups[1].Value.TrimEnd (); //Get the Character Varible Name
                string animation = "";

                animation = thisMatch.Groups[2].Value.TrimEnd (); //Get the animation string

                if (animation.Contains (",")) //if there is more than one animation
                {

                    string[] values = animation.Split (',');
                    foreach (string V in values) //for each parsed string seperated by ','
                    {
                        if (V.Contains (" ")) //Check if it has white spaces
                        {
                            PrintTextError ("<b>ERROR: Game-File</b>: Animate 'animation' attribute contains white space(s) at line: ");
                        } else {
                            AddAnimation (V, ImageVarName);
                        }
                    }
                } else {
                    AddAnimation (animation, ImageVarName);
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Animate Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#PlayAudio\s*(.*)\s*:\s*\((\s*volume\s*=\s*([0-9]*\.?[0-9]*)\,?\s*channel\s*=\s*([0-9]*\.?[0-9]*)\,?|\s*loop\s*=\s*((0|1|true|false))\,?|\s*fadein\s*=\s*([0-9]*\.?[0-9]*)\,?|\s*fadeout\s*=\s*([0-9]*\.?[0-9]*)\,?)*\s*\)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#PlayAudio\s*(.*)\s*:\s*\((\s*volume\s*=\s*([0-9]*\.?[0-9]*)\,?\s*channel\s*=\s*([0-9]*\.?[0-9]*)\,?|\s*loop\s*=\s*((0|1|true|false))\,?|\s*fadein\s*=\s*([0-9]*\.?[0-9]*)\,?|\s*fadeout\s*=\s*([0-9]*\.?[0-9]*)\,?)*\s*\)" + SpecialKeywords, RegexOptions.None);

            int Channel = -1;
            double Volume = -1.0;
            bool Loop = false;
            double FadeIn = -1.0;
            double FadeOut = -1.0;
            try {

                string AudioVarName = thisMatch.Groups[1].Value.TrimEnd (); //Get the image name

                if (Sounds.Exists (SearchingImages => SearchingImages.Name == AudioVarName)) //Check if this audio exits
                {
                    AudioClass AudioToFind = Sounds.Find (SearchingSound => SearchingSound.Name == AudioVarName); //find audio

                    if (AudioToFind != null) {
                        if (thisMatch.Groups[1].Length > 0) {
                            Volume = Convert.ToDouble (thisMatch.Groups[3].Value);

                        } else {
                            PrintTextError ("<b>ERROR: Game-File</b>: Audio 'volume' attribute not found at line: ");
                        }

                        if (thisMatch.Groups[7].Length > 0) { //Check if volume is there
                            FadeIn = Convert.ToDouble (thisMatch.Groups[7].Value);
                        }

                        if (thisMatch.Groups[8].Length > 0) { //Check if volume is there
                            FadeOut = Convert.ToDouble (thisMatch.Groups[8].Value);
                        }
                        if (thisMatch.Groups[4].Length > 0) { //Check if volume is there
                            Channel = Convert.ToInt32 (thisMatch.Groups[4].Value);
                        } else {
                            PrintTextError ("<b>ERROR: Game-File</b>: Audio 'channel' attribute not found at line: ");
                        }
                        if (thisMatch.Groups[5].Length > 0) { //Check if volume is there

                            if (thisMatch.Groups[5].Value.ToLower () == "true" || thisMatch.Groups[5].Value.ToLower () == "1") {
                                Loop = true;
                            } else if (thisMatch.Groups[5].Value.ToLower () == "false" || thisMatch.Groups[5].Value.ToLower () == "0") {
                                Loop = false;
                            } else {
                                PrintTextError ("<b>ERROR: Game-File</b>: Audio 'loop' attribute cannot be parsed at line: ");
                            }
                        }
                        ControlSound ("Play", Channel, Volume, Loop, FadeIn, AudioToFind, FadeOut);
                    } else {
                        PrintTextError ("<b>ERROR: Game-File</b>: Audio: " + AudioVarName + " cannot be found at line: ");
                    }

                }
            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: PlayAudio Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#PauseAudio\s*(C1|Channel1|C2|Channel2)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#PauseAudio\s*(C1|Channel1|C2|Channel2)" + SpecialKeywords, RegexOptions.None);

            try {

                string ChannelName = thisMatch.Groups[1].Value; //Get the background image name
                ChannelName = ChannelName.Trim (); //remove white spaces
                if (ChannelName == "Channel1" || ChannelName == "C1") {
                    Channel1.Pause ();
                } else if (ChannelName == "Channel2" || ChannelName == "C2") {
                    Channel2.Pause ();
                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Channel cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Channel Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#ResumeAudio\s*(C1|Channel1|C2|Channel2)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#ResumeAudio\s*(C1|Channel1|C2|Channel2)" + SpecialKeywords, RegexOptions.None);

            try {

                string ChannelName = thisMatch.Groups[1].Value; //Get the background image name
                ChannelName = ChannelName.Trim (); //remove white spaces
                if (ChannelName == "Channel1" || ChannelName == "C1") {
                    Channel1.Play ();
                } else if (ChannelName == "Channel2" || ChannelName == "C2") {
                    Channel2.Play ();
                } else {
                    PrintTextError ("<b>ERROR: Game-File</b>: Channel cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Channel Error at line: ");
            }

        }
        /* else if (lines[LineSearching].Contains("#StopAudio"))           //NOT USED, DEPRICATED!!!
         {
             try
             {

                 string ChannelName = lines[LineSearching].Substring(lines[LineSearching].IndexOf("#StopAudio ") + 10);//Get the background image name
                 ChannelName = ChannelName.Trim();//remove white spaces
                 if (ChannelName == "Channel1")
                 {
                     Channel1.Stop();
                 }
                 else if (ChannelName == "Channel2")
                 {
                     Channel1.Stop();
                 }
                 else
                 {
                     PrintTextError("<b>ERROR: Game-File</b>: Channel cannot be found at line: " );
                 }


             }
             catch (Exception e)
             {
                 PrintTextError("<b>ERROR: Game-File</b>: Channel Error at line: " );
             }

         }*/
        else if (lines[LineSearching].Contains ("#WaitForAnimation ")) {
            try {

                /*string WaitAmmount = lines[LineSearching].Substring(lines[LineSearching].IndexOf("#WaitForAnimation") + 18);
                print(WaitAmmount);
                WaitAmmount = WaitAmmount.Trim();//remove white spaces
                float waitammountfloat = (float)(Convert.ToDouble(WaitAmmount));
                WaitedTime = waitammountfloat;
                WaitingForAnimations = true;
                Running = false;*/
            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: WaitForAnimation Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#Wait\s*([0-9]*\.?[0-9]*)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#Wait\s*([0-9]*\.?[0-9]*)" + SpecialKeywords, RegexOptions.None);

            try {
                IsLineComplete = false;
                Waiting = true;
                TimeToWait = (float) (Math.Round (Convert.ToDouble (thisMatch.Groups[1].Value.TrimEnd ()), 2));
                StartCoroutine (Wait (0));
            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Wait Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#Jump\s*(.*)" + SpecialKeywords, RegexOptions.None).Success) {
            Match thisMatch = Regex.Match (CGCurLine, @"^#Jump\s*(.*)" + SpecialKeywords, RegexOptions.None);
            try {

                string SceneToJump = thisMatch.Groups[1].Value.Trim ();

                if (Scenes.Exists (SearchingScenesB => SearchingScenesB.Name == SceneToJump)) {

                    SceneClass SceneToFind = Scenes.Find (SearchingScenesA => SearchingScenesA.Name == SceneToJump);
                    Jump (SceneToFind.LineNumber);
                } else {

                    PrintTextError ("<b>ERROR: Game-File</b>: Jump Scene cannot be found at line: ");
                }

            } catch (Exception e) {
                PrintTextError ("<b>ERROR: Game-File</b>: Jump Error at line: ");
            }

        } else if (Regex.Match (CGCurLine, @"^#Choice(\s)*:(\s)*(.*)", RegexOptions.None).Success) {

            try {
                MouseBreak = true;
                Match thisMatch = Regex.Match (CGCurLine, @"^#Choice(\s)*:(\s)*(.*)", RegexOptions.None);
                Match secondMatch = Regex.Match (thisMatch.Groups[3].Value, @"(\((.*?)\),?)", RegexOptions.None);
                IList<string> ChoiceButtonsList = new List<string> ();

                while (secondMatch.Success) {
                    ChoiceButtonsList.Add (secondMatch.Groups[1].Value);
                    secondMatch = secondMatch.NextMatch ();
                }

                foreach (string Choice in ChoiceButtonsList) {
                    Match thirdMatch = Regex.Match (Choice, @"\(\"" (.* ? )\
                        "", ([0 - 9] * \. ? [0 - 9] * ), ([0 - 9] * \. ? [0 - 9] * ), \"" (.*)\
                        "", ? (.*) ? \), ? ", RegexOptions.None);
                    ChoiceButtons.Add(new ButtonClass(thirdMatch.Groups[1].Value.Trim(), thirdMatch.Groups[4].Value.Trim(), (float)Convert.ToDouble(thirdMatch.Groups[2].Value), (float)Convert.ToDouble(thirdMatch.Groups[3].Value), thirdMatch.Groups[5].Value.Trim()));
                }
                Choices = true;

            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : Choice Error at line: " );
            }

        }
        else if (Regex.Match(CGCurLine, @" ^ #FadeBackground\ s * (.*)\ s *: \s * (([0 - 9] * \. ? [0 - 9] * ))
                    " + SpecialKeywords, RegexOptions.None).Success)
        {
           
            Match thisMatch = Regex.Match(CGCurLine, @" ^ #FadeBackground\ s * (.*)\ s *: \s * (([0 - 9] * \. ? [0 - 9] * ))
                    " + SpecialKeywords, RegexOptions.None);
            try
            {
            
                string NextBackground = "
                    ";

                if (thisMatch.Groups[1].Length > 0)
                {
                    NextBackground = thisMatch.Groups[1].Value.Trim();
                }
                else
                {
                    PrintTextError(" < b > ERROR : Game - File</ b> : FadeBackground 'background'
                    attribute cannot be found at line: " );
                }

                if (thisMatch.Groups[2].Length > 0)
                {
                    TimeToFade = Convert.ToDouble(thisMatch.Groups[2].Value);
                }
                else
                {
                    PrintTextError(" < b > ERROR : Game - File</ b> : FadeBackground 'time'
                    attribute cannot be found at line: " );
                }

                if (Images.Exists(SearchingBackground => SearchingBackground.Name == NextBackground))
                {
                
                    IsLineComplete = false;
                    BackgroundB = SelectImage(NextBackground);
                }
                else
                {
                    PrintTextError(" < b > ERROR : Game - File</ b> : Cannot find FadeBackground background image at line: " );
                }

            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : FadeBackground Error at line: " );
            }

        }
        else if (!CGCurLine.Contains("
                    Scene ") && !CGCurLine.Contains("#
                    TextBoxOn ") && !CGCurLine.Contains("#
                    TBON ") && !CGCurLine.Contains("#
                    TextBoxOff ") && !CGCurLine.Contains("#
                    TBOF "))
        {
            PrintTextError(" < b > ERROR : Game - File</ b> : Command not recognized at line: ");
        }
       
        if (Regex.Match(CGCurLine, @" ^ #(TextBoxOn | TBON)
                    " + SpecialKeywords, RegexOptions.None).Success)
        {
            try
            {
                TextBox = true;
                TextBoxWasOn = true;
            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : TextBoxOn Error at line: " );
            }

        }
        if (Regex.Match(CGCurLine, @" ^ #(TextBoxOff | TBOF)
                    " + SpecialKeywords, RegexOptions.None).Success)
        {

            try
            {
                TextBoxWasOn = false;
                TextBox = false;
            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : TextBoxOff Error at line: " );
            }

        }
        if (Regex.Match(CGCurLine, @" ^ #(TextBoxTransparent | TBT)\ s *: \s * ([0 - 9] * \. ? [0 - 9] * )
                    " + SpecialKeywords, RegexOptions.None).Success)
        {

            Match thisMatch = Regex.Match(CGCurLine, @" ^ #(TextBoxTransparent | TBT)\ s *: \s * ([0 - 9] * \. ? [0 - 9] * )
                    " + SpecialKeywords, RegexOptions.None);
            try
            {

                BackgroundAlpha = (float)Convert.ToDouble(thisMatch.Groups[2].Value.TrimEnd());

            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : TextBoxTransparent Error at line: " );
            }

        }
        if (Regex.Match(CGCurLine, @" ^ #FontStyle\ s *: \s * (.*)
                    " + SpecialKeywords, RegexOptions.None).Success)
        {

            Match thisMatch = Regex.Match(CGCurLine, @" ^ #FontStyle\ s *: \s * (.*)
                    " + SpecialKeywords, RegexOptions.None);
            try
            {
                Dialog = "
                    ";
                CurrentFontStyle = thisMatch.Groups[1].Value.Trim();//set alpha value from code
            }
            catch (Exception e)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : FontStyle Error at line: " );
            }

        }
        else if (Regex.Match(CGCurLine, @" ^ #Scene MainMenu.*", RegexOptions.None).Success)
        {
            MenuShowing = true;
            TextBox = false;

            ClearScreen();
            NextMenu("
                    MainMenu ");
        }
       




        if (lines[LineSearching].Contains("
                    MouseBreak "))//The mouse will not work till the line is complete.
        {
            MouseBreak = true;
        }
        if (lines[LineSearching].Contains("
                    MouseFix "))//The mouse will not work till the line is complete.
        {
            MouseBreak = false;
        }


        NEXTKeyword();

    }
    void NEXTKeyword(int Input = 0)
    {

        if (Regex.Match(lines[LineSearching], ".*(NEXT).*", RegexOptions.None).Success)//Go to next line automaticly if contains next
        {
            IsNextActive = true;

            if (IsLineComplete == true || Input == 1)//1 is automatic from WriteText() function
            {
                NextLine();
            }
        }
    }
    void PrintText(Character ThisCharacter, string DialogGiven)
    {
        StopCoroutine("
                    SlowText ");
        LineNumber = 1;
        DialogColor = ThisCharacter.TextColor;//Set chararcters text color here because you can only set it in GUI function
        DialogName = ThisCharacter.CharacterName;
        DialogGiven = SetDialogColor(DialogGiven);


        if (TextSpeed == 0)
        {//if text speed is 0 then immediatly change it
            AddToDialogHistory(DialogGiven);
            Dialog = DialogGiven;
        }
        else
        {
            Dialog = "
                    ";
            NewDialog = DialogGiven;
            AddToDialogHistory(" < b > " + DialogName + " < / b > " + " - " + DialogGiven);
            StartCoroutine(WriteText());
        }


    }//prints text into dialog box
    void PrintTextWithoutName(string DialogGiven)
    {

        LineNumber = 1;
       // CharacterTextColor = DefaultTextStyle.normal.textColor;
        DialogName = "
                    ";

        DialogGiven = SetDialogColor(DialogGiven);



        if (TextSpeed == 0)
        {//if text speed is 0 then immediatly change it
            AddToDialogHistory(DialogGiven);
            Dialog = DialogGiven;
        }
        else
        {
            Dialog = "
                    ";
            NewDialog = DialogGiven;
            AddToDialogHistory(DialogGiven);
            StartCoroutine(WriteText());

        }

    }//prints text into dialog box, puts name as empty  
    string SetDialogColor(string DialogGiven)
    {

        Match thisMatch = Regex.Match(DialogGiven, @" ( < (#.* ? ) >) (.*)
                    ", RegexOptions.None);
        if (thisMatch.Groups[2].Value.Trim() != "
                    ")
        {
            DialogColor = thisMatch.Groups[2].Value;
            DialogGiven = thisMatch.Groups[3].Value;
        }
        return DialogGiven;
    }
    void PrintTextError(string Error)//Print text specifically for error so it stops the game
    {
        IsError = true;
        /* = 1;
        Dialog = DialogGiven;
        Running = false;*/
        Debug.Log(" < color = #ff0000ff > " + Error + " < b > " + CurrentLineText + " < / b > " + " < / color > ");
    }
    void OnGUI()
    {
        if (IsError == true)
        {
            GUI.Label(new Rect(0,0,75,25), "
                    Error Found ");
        }
        if (Background != null && Background.Texture == null)
        {
            Background = Images[0];
        }
        if (BackgroundB != null && BackgroundB.Texture == null)
        {
            BackgroundB = Images[0];
        }


        float verticalRatio = Screen.width / 1600f;
        float horizontalRatio = Screen.height / 900f;


        verticalRatio = verticalRatio > 1f ? 1f : verticalRatio;
        horizontalRatio = horizontalRatio > 1f ? 1f : horizontalRatio;

        float height = (Screen.height * (1 - verticalRatio)) / 2;
        float width = ((Screen.width * (1 - horizontalRatio)) / 2) + CameraXOffset;

        GUI.matrix = Matrix4x4.TRS(new Vector3(width, height, 0), Quaternion.identity, new Vector3(horizontalRatio, verticalRatio, 1));



        DialogBoxRect = new Rect(0, (Screen.height) - TextBoxHeight, Screen.width, TextBoxHeight);//The Dialog Box
        DialogHistoryRect = new Rect(Screen.width / 3f, Screen.height / 3f, Screen.width / 2.75f, Screen.height / 2.75f);//The Dialog Box

        if (currentMovie != null)
        {

            if (movieFadein != 0.0 || movieFadeout != 0.0)
            {
                //Debug.Log(movieFadein + "
                    " + movieFadeout + "
                    " + moviePlayedForTime);

                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(moviecurrentFade));    //Fade the image
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), currentMovie, ScaleMode.ScaleToFit, false, 0.0f);
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(1.0));   //So it does not fade the rest of the GUI only the image

            }
            else
            {

                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), currentMovie, ScaleMode.ScaleToFit, false, 0.0f);

            }

        }

        if (Background != null && Background.Texture)//Set background if one is there
        {
            
            if (TimeToFade != -1.0 && Running == true)//Fade background if fade is not -1.0
            {

                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak)//If you click through it
                {
                    Background = BackgroundB;
                    TimeToFade = -1.0;
                    BackgroundFade = 1.0;
                    ClickedThrough = false;
                    NEXTKeyword(1);
                }
                else if (ClickedThrough == false)//itrs not getting through this, when you click you automaticly do the code above
                {

                    BackgroundFade -= ((Time.smoothDeltaTime / (TimeToFade * 2)));//fade it
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundB.Texture, ScaleMode.StretchToFill, true, 10.0F);//next background
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(BackgroundFade));    //apply fade
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background.Texture, ScaleMode.StretchToFill, true, 10.0F);//old background
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(1.0));   //So it does not fade the rest of the GUI only the background
                    if (BackgroundFade <= 0.0)//Set variables back to whyere they were in the begining1
                    {
                        Background = BackgroundB;
                        TimeToFade = -1.0;
                        BackgroundFade = 1.0;
                        NEXTKeyword(1);
                    }
                }
            }
            else
            {
                if (BackgroundB != null && BackgroundB.Texture != null) { GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundB.Texture, ScaleMode.StretchToFill, true, 10.0F); }//next background}
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(BackgroundFade));    //apply fade
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background.Texture, ScaleMode.StretchToFill, true, 10.0F);//old background
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(1.0));   //So it does not fade the rest of the GUI only the background
                
                   
            }
        }
        if (CurrentImages.Count > 0 && MenuShowing == false)
        {

            foreach (CurrentImage img in CurrentImages)
            {//Draw all current images in teh current image list

                TextureVariable ImageToFind = SelectImage(img.Name);//find the character if it exists

                Texture2D Image = ImageToFind.Texture; //sets the texture to the image that it just found
                if (Image == null)
                {
                    PrintTextError(" < b > ERROR : Game - File</ b> : Image cannot be found at line: " );
                }
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(img.Alpha));    //Fade the image
                if (img.Width != -1f || img.Height != -1f)
                {
                    GUI.DrawTexture(new Rect((float)((Screen.width * img.Xpos) - Screen.width), (float)((Screen.height * img.Ypos) - Screen.height), (float)(Screen.width * img.Width), (float)(Screen.height * img.Height)), Image, ScaleMode.StretchToFill, true, 10.0F);
                }
                else
                {
                    GUI.DrawTexture(new Rect((float)((Screen.width * img.Xpos) - Screen.width), (float)((Screen.height * img.Ypos) - Screen.height), Image.width/4,  Image.height/4), Image, ScaleMode.StretchToFill, true, 10.0F);
                }
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(1.0));   //So it does not fade the rest of the GUI only the image
                if (Running == true)//Only do animations there is no errors
                {
                    if (img.Animation != null || img.Animation != "
                    ")
                    {

                        if (img.Animation.Contains(", "))//if there is more than one animation
                        {

                            string[] values = img.Animation.Split(',');
                            foreach (string V in values)//for each parsed string seperated by ','
                            {
                                if (V.Contains("
                    "))//Check if it has white spaces
                                {
                                    PrintTextError(" < b > ERROR : Game - File</ b>: 'animation'
                    attribute contains white space (s) at line: ");
                                }
                                else
                                {
                                    ReadAnimation(V, img.Name);
                                }
                            }
                        }
                        else
                        {

                            ReadAnimation(img.Animation, img.Name);
                        }


                    }
                }
            }
        }
        if (TextBox == true)//If the textbox is on show it
        {

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, BackgroundAlpha);    //Fade the text box
            DialogBoxRect = GUI.Window(0, DialogBoxRect, DialogBoxFunction, "
                    ", TextBoxStyle);//Window Style here
            if (DialogName.Trim() != "
                    ")
            {
                GUI.Label(new Rect(0, Screen.height - TextBoxHeight - 25, 100, 25), DialogName, TitleBar);
            }
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1.0f);    //Fade the rest of GUI back

        }
        if (CurrentMenuShowing == "
                    DialogHistory ")
        {
            DialogHistoryRect = GUI.Window(0, DialogHistoryRect, DialogHistoryFunction, "
                    Dialog History ");//Window Style here
        }
        //Choices
        if (Choices == true && MenuShowing == false)
        {
            int ButtonAmmount = 0;

            for (int x = 0; x < ChoiceButtons.Count; x++)
            {

                ChoiceButtons[x].Name = ChoiceButtons[x].Name.Trim();//remove extra spaces on end and beingin of name
                ChoiceButtons[x].Name = EscapeCharacters(ChoiceButtons[x].Name);//escape string

                if (ChoiceButtons[x].Texture != "
                    ")
                {

                    Texture2D ButtonTexture = FindImage(ChoiceButtons[x].Texture);
                    GUIStyle ButtonStyle = new GUIStyle();
                    ButtonStyle.normal.background = ButtonTexture;
                    if (GUI.Button(new Rect((Screen.width * ChoiceButtons[x].XPos), (Screen.height * ChoiceButtons[x].YPos) + (25 * ButtonAmmount), (10 * ChoiceButtons[x].Name.Length) + 7, 25), ChoiceButtons[x].Name, ButtonStyle))
                    {
                        Choices = false;

                        ChoiceButtons[x].Command = ChoiceButtons[x].Command.TrimEnd();
                        // ERORR FIXE 1   print("
                    The command: " + Buttons.Command + ": The Name: " + Buttons.Name);


                        if (Scenes.Exists(SearchingScenesA => SearchingScenesA.Name.Contains(ChoiceButtons[x].Command)))
                        {

                            SceneClass SceneToFind = Scenes.Find(SearchingScenesB => SearchingScenesB.Name.Contains(ChoiceButtons[x].Command));//get scene
                            ChoiceButtons.Clear();
                            MouseBreak = false;
                            Jump(SceneToFind.LineNumber);//jump to scene line

                        }
                        else
                        {
                            // ERORR FIXE 1    print("
                    didint worki...Scene: " + Buttons.Command + ": Name " + Buttons.Name);

                        }

                    }
                }
                else
                {
                    if (GUI.Button(new Rect((Screen.width * ChoiceButtons[x].XPos), (Screen.height * ChoiceButtons[x].YPos) + (25 * ButtonAmmount), (10 * ChoiceButtons[x].Name.Length) + 5, 25), ChoiceButtons[x].Name))
                    {
                        Choices = false;

                        ChoiceButtons[x].Command = ChoiceButtons[x].Command.TrimEnd();
                        // ERORR FIXE 1   print("
                    The command: " + Buttons.Command + ": The Name: " + Buttons.Name);


                        if (Scenes.Exists(SearchingScenesA => SearchingScenesA.Name.Contains(ChoiceButtons[x].Command)))
                        {

                            SceneClass SceneToFind = Scenes.Find(SearchingScenesB => SearchingScenesB.Name.Contains(ChoiceButtons[x].Command));//get scene
                            ChoiceButtons.Clear();
                            MouseBreak = false;
                            Jump(SceneToFind.LineNumber);//jump to scene line

                        }
                        else
                        {
                            // ERORR FIXE 1    print("
                    didint worki...Scene: " + Buttons.Command + ": Name " + Buttons.Name);

                        }

                    }
                }



                ButtonAmmount++;//For each button space them out
            }
        }

        if (CommandLineOpen == true)            //Command LIne!
        {

            GUI.SetNextControlName("
                    CommandField ");
            CommandLineCommand = GUI.TextField(new Rect(25, 25, 200, 20), CommandLineCommand);//INput
            GUI.TextField(new Rect(25, 50, 300, 150), CommandLineOutput);        //Output

            if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ current\ s * line ", RegexOptions.None).Success)
            {
                CommandLineOutput += "
                    Current Line: " + LineSearching + "\
                    n ";
                CommandLineCommand = "
                    ";
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ current\ s * images ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                int CurrentImageNumber = 0;
                foreach (CurrentImage ImageA in CurrentImages)
                {
                    CurrentImageNumber++;
                    CommandLineOutput += CurrentImageNumber + ": " + ImageA.Name + "\
                    n ";
                }
                if (CurrentImageNumber == 0)
                {
                    CommandLineOutput += "
                    No non - background images are currently being displayed.\n ";
                }
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ channel\ s * 1\ s * clip ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Channel 1: " + Channel1.clip + "\
                    n ";

            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ channel\ s * 2\ s * clip ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Channel 2: " + Channel2.clip + "\
                    n ";

            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ background ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Background: " + Background.Name + "\
                    n ";
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ engine\ s * version ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Engine Version: " + CutVersion + "\
                    n ";
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ game\ s * file ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Game File: " + GameFile.name + "\
                    n ";
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^
                        continue\ s * game ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    Running.\n ";
            }
            else if (Regex.Match(CommandLineCommand.Trim().ToLower(), @" ^ mouse\ s *
                        break ", RegexOptions.None).Success)
            {
                CommandLineCommand = "
                    ";
                CommandLineOutput += "
                    MouseBreak: " + MouseBreak + "\
                    n ";
            }
        }




        if (MenuShowing == true)
        {

            for (int i = 0; i <= MenuControlsList.Count - 1; i++)
            {
                GUIStyle ButtonStyle = new GUIStyle();

                if (FadeInMainMenu == false)
                {
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(MenuControlsList[i].CurrentFade));    //Fade the image
                }

                if (MenuControlsList[i].Type == "
                    Button " && !MenuControlsList[i].Command.Contains("
                    Pref "))
                {
                    // (Screen.width / 1600f)
                    // ((Screen.height * (1 - verticalRatio)) / 2)
                    /*
                    if (GUI.Button(new Rect(MenuControlsList[i].XLocation * ((Screen.width * (1 - (Screen.height/900f))) ), MenuControlsList[i].YLocation * ((Screen.height * (1 - (Screen.width / 1600f))) / 2), MenuControlsList[i].Width*Screen.width, MenuControlsList[i].Height * Screen.height), MenuControlsList[i].Text))
                    {*/
                    if (MenuControlsList[i].ButtonTexture != null)
                    {
                        ButtonStyle.normal.background = MenuControlsList[i].ButtonTexture;
                    }
                    else
                    {
                        ButtonStyle = new GUIStyle(GUI.skin.button);
                    }
                    if (MenuControlsList[i].OnHoverTexture != null)
                    {
                        ButtonStyle.hover.background = MenuControlsList[i].OnHoverTexture;
                    }
                    if (GUI.Button(new Rect(MenuControlsList[i].XLocation * Screen.width, MenuControlsList[i].YLocation * Screen.height, MenuControlsList[i].Width * Screen.width, MenuControlsList[i].Height * Screen.height), MenuControlsList[i].Text, ButtonStyle))
                    {
                        FadeInMainMenu = true;
                        if (MenuControlsList[i].Command.Contains("
                    Play "))
                        {
                            if (Scenes.Exists(SearchingScenesB => SearchingScenesB.Name.Contains("
                    GameStart "))) //Check if the gamestart exists and get line
                            {

                                CurrentImages.Clear();//Clear out old data
                                ChoiceButtons.Clear();

                                Channel1.clip = null;
                                Channel2.clip = null;
                                currentMovie = null;

                                MouseBreak = false;
                                TextBox = true;
                                TextBoxWasOn = true;
                                Running = true;

                                ResumeGame();

                                SceneClass SceneToFind = Scenes.Find(SearchingScenesA => SearchingScenesA.Name.Contains("
                    GameStart "));
                                Jump(SceneToFind.LineNumber);
                            }
                            else
                            {

                                PrintTextError("
                    ERROR: < b > Menu - File</ b> : Play command Start scene cannot be found at line: " );
                            }
                        }
                        else if (MenuControlsList[i].Command.Contains("
                    Load ") && !MenuControlsList[i].Command.Contains("
                    Menu - "))
                        {
                            ResumeGame();
                            resized = false;
                            Load(Convert.ToInt32(MenuControlsList[i].Command.IndexOf("
                    Saves / Load ") + 4));


                        }
                        else if (MenuControlsList[i].Command.Contains("
                    Credits "))
                        {
                            NextMenu("
                    Credits ");

                        }
                        else if (MenuControlsList[i].Command.Contains("
                    Save ") && !MenuControlsList[i].Command.Contains("
                    Menu - "))
                        {
                            Save(Convert.ToInt32(MenuControlsList[i].Command.IndexOf("
                    Saves / Save ") + 4));
                            ResumeGame();
                        }
                        else if (MenuControlsList[i].Command.Contains("
                    IGMBack "))
                        {
                            ResumeGame();

                        }
                        else if (MenuControlsList[i].Command.Contains("
                    Back "))
                        {
                            if(CurrentMenuShowing == "
                    InGameMenu "){
                                ResumeGame();
                            }
                            NextMenu(LastMenuShowed);
                            
                        }
                        else if (MenuControlsList[i].Command.Contains("
                    Menu - "))
                        {
                            if (CurrentMenuShowing == "
                    InGameMenu " && MenuControlsList[i].Command.Substring(MenuControlsList[i].Command.IndexOf("
                    Menu - ") + 5) == "
                    MainMenu ")//going form in game menu to main menu
                            {
                                Channel1.Stop();
                                Channel2.Stop();
                            }

                            NextMenu(MenuControlsList[i].Command.Substring(MenuControlsList[i].Command.IndexOf("
                    Menu - ") + 5));
                        }

                        else if (MenuControlsList[i].Command.Contains("
                    Exit "))
                        {
                            Application.Quit();
                        }



                    }

                }
                else if (MenuControlsList[i].Type == "
                    Button " && MenuControlsList[i].Command.Contains("
                    Pref "))
                {
                    if (MenuControlsList[i].Command == "
                    Pref - Sound ")
                    {
                        GUI.Label(new Rect(MenuControlsList[i].XLocation - 125, MenuControlsList[i].YLocation - 5, 125, 20), "
                    Sound Level: (" + soundLevel.ToString("
                        F2 ") + ")
                    ");
                        soundLevel = GUI.HorizontalSlider(new Rect(MenuControlsList[i].XLocation, MenuControlsList[i].YLocation, 150, 20), soundLevel, 0.0F, 1.0F);
                    }
                }
                else if (MenuControlsList[i].Type == "
                    Text ")
                {
                    GUI.Label(new Rect(MenuControlsList[i].XLocation * Screen.width, MenuControlsList[i].YLocation * Screen.height, Screen.width, Screen.height), MenuControlsList[i].Text);
                }
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, (float)(1.0));   //So it does not fade the rest of the GUI only the image


            }

        }
    }
    void DialogBoxFunction(int windowID)
    {

           /* searchingStyle = FontStyles.Find(x => x.name == CurrentFontStyle);
            if (searchingStyle == null)
            {
                PrintTextError(" < b > ERROR : Game - File</ b> : Font Style cannot be found at line: " );
            }*/

            GUI.Label(new Rect(5, 15, Screen.width, Screen.height / 5), " < color = " + DialogColor + " > " + Dialog + " < / color > ", DefaultTextStyle);//This outputs the text
        

    }//Is the output for the GUI window

    void DialogHistoryFunction(int windowID)
    {
        DialogHistoryScroll = GUILayout.BeginScrollView(DialogHistoryScroll);

        // DialogHistoryScroll = GUI.BeginScrollView(new Rect(8,22, Screen.width/2.85f, Screen.height/3.1f), DialogHistoryScroll, new Rect(0, 0, Screen.width/3f, DialogHistory.Count * 50));
        foreach (string x in DialogHistory.Reverse<string>())
        {
            GUILayout.Label(x);
            GUILayout.Space(25);
        }
        // GUI.EndScrollView();

        GUILayout.EndScrollView();


    }//Is the output for the GUI window




    Color StringToColor(string ColorString)
    {
        Color ReturnColor = Color.white;
        ColorString = ColorString.ToLower();
        switch (ColorString)
        {
            case "
                    red ":
                ReturnColor = Color.red;
                break;
            case "
                    blue ":
                ReturnColor = Color.blue;
                break;
            case "
                    yellow ":
                ReturnColor = Color.yellow;
                break;
            case "
                    green ":
                ReturnColor = Color.green;
                break;
            case "
                    white ":
                ReturnColor = Color.white;
                break;
            case "
                    black ":
                ReturnColor = Color.black;
                break;
            case "
                    clear ":
                ReturnColor = Color.clear;
                break;
            case "
                    cyan ":
                ReturnColor = Color.cyan;
                break;
            case "
                    gray ":
                ReturnColor = Color.gray;
                break;
            case "
                    magenta ":
                ReturnColor = Color.magenta;
                break;

            default:
                PrintTextError(" < b > Game - File</ b> Color does not exist in database at line: " );
                break;
        }

        return ReturnColor;

    }//Changes a string to a color
 


    void Update()
    {



        if (lastScreenWidth != Screen.width)//adjust text on resize
        {
            lastScreenWidth = Screen.width;
            FinishText();
        }


        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (MenuShowing == false)
            {
                Save();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            if (MenuShowing == false)
            {
                Load();
            }
        }

        if (currentMovie != null && currentMovie.isPlaying == true)//movie fadein
        {

            if ((moviePlayedForTime + movieFadeout) >= currentMovie.duration)
            {

                //Debug.Log("
                    hereb " + currentMovie.duration);
                if (movieFadeout != 0.0)
                {
                    moviecurrentFade -= (float)(Time.smoothDeltaTime / movieFadeout);
                }
            }
            else
            {
                //Debug.Log("
                    here " + currentMovie.duration);
                if (movieFadein != 0.0)
                {
                    moviecurrentFade += (float)(Time.smoothDeltaTime / movieFadein);
                }
            }
        }
        // if (Dialog.Contains(FileEndText))//if the dialog contains the end text then remove any extra text because of the slow text function
        // {
        //   Dialog = FileEndText;
        //}

        //PREFRENCES
        Channel1.volume = Channel1.volume > soundLevel ? soundLevel : Channel1.volume;
        Channel2.volume = Channel2.volume > soundLevel ? soundLevel : Channel2.volume;




        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuShowing == false)
            {
                MenuShowing = true;
                CurrentMenuShowing = "
                    InGameMenu ";

                TextBoxWasOn = TextBox;
                TextBox = false;

                PauseGame();
                LoadMenu();
            }
            else if (MenuShowing == true && CurrentMenuShowing == "
                    InGameMenu ")
            {

                ResumeGame();
            }

        }
        if (Running == true)//Make sure the game is running so you cannot go forward if there is an error
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak && StringNumber >= NewDialog.Length && MenuShowing == false)//if you click, the mouse break is false and the dialog is done go to the next line
            {
                // ERORR FIXE 1   print("
                    clicked and text done ");
                NextLine();

            }
            else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak && StringNumber <= NewDialog.Length && MenuShowing == false) //if there new dialog not written and you click output it all
            {
                // ERORR FIXE 1   print("
                    clicked and text not done!");
                FinishText();
                NewDialog = "
                    ";
                StringNumber = 0;
            }

           /* else if (NextFade == true && TimeToFade == -1.0)//if the line has next fade
            {
                NextFade = false;
                NextLine();
                ContinueGame();
            }*/

        }




        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (CommandLineOpen == false && CommandLine == true)
            {
                CommandLineOpen = true;
                // Running = false;
                GUI.FocusControl("
                    CommandField ");
            }
            else
            {
                CommandLineOpen = false;
                // Running = true;
            }


        }

        if (Channel2.clip != null)
        {
            if (Channel2Options.FadeOut != -1.0 && (Channel2Options.FadeOut + Channel2Options.PlayedFor) >= Channel2.clip.length)// if fadein is set for channel 1
            {
                Channel2.volume -= (float)(Time.smoothDeltaTime / Channel2Options.FadeIn);
            }
            else if (Channel2Options.FadeIn != -1.0)// if fadein is set for channel 1
            {

                if (Channel2.volume <= 1.0 && Channel2.volume < soundLevel)//make sure its not fully playing and not over sound limit
                {
                    if (Channel2.volume < Channel2Options.MaxVolume)
                    {
                        Channel2.volume += (float)(Time.smoothDeltaTime / Channel2Options.FadeIn);
                    }
                }
            }
        }

        if (Channel1.clip != null)
        {
            if (Channel1Options.FadeOut != -1.0 && (Channel1Options.FadeOut + Channel1Options.PlayedFor) >= Channel1.clip.length)// if fadein is set for channel 1
            {

                Channel1.volume -= (float)(Time.smoothDeltaTime / Channel1Options.FadeIn);
            }
            else if (Channel1Options.FadeIn != -1.0)// if fadein is set for channel 1
            {

                if (Channel1.volume <= 1.0 && Channel1.volume < soundLevel)//make sure its not fully playing and not over sound limit
                {
                    if (Channel1.volume < Channel1Options.MaxVolume)
                    {
                        Channel1.volume += (float)(Time.smoothDeltaTime / Channel1Options.FadeIn);
                    }
                }
            }
        }

        if (MenuShowing == true)
        {
            foreach (MenuControls m in MenuControlsList)
            {
                if (m.Fadein != 0.0f)
                {
                    m.CurrentFade += (float)(Time.smoothDeltaTime / m.Fadein);
                }
            }
        }
    }
    int ScreenSize(double Value, bool width)
    {//input value is a % and return the screen size
        int ValueToReturn;
        if (width == true)//width value
        {
            ValueToReturn = (int)(Screen.width * Value);
            return ValueToReturn;
        }
        else//height value
        {
            ValueToReturn = (int)(Screen.height * Value);
            return ValueToReturn;
        }

    }//Function to get screen size from a percent
    void ReadAnimation(string Animation, string ImageName)//Reads the animnations and executes them         DOESNT QUITE MOVE RIGHT, MUST PUT IN FIXED UPDATE
    {

        double time = -1.0;//how long this should take
        float xpos = -1;//XPOS that it will move to if there is a xpos
        float ypos = -1;//YPOS that it will mvoe to if there is a ypos
        double alpha = -1.0;
        float speed = -1;
        int direction = -1;//1=left, 2=right, 3=up, 4=down
        AnimationClass AnimationA = null;
        CurrentImage Image = null;
        try
        {


            if (Animation != "
                    ")
            {

                AnimationA = Animations.Find(x => x.Name == Animation);//Animation data
                Image = SelectCurrentImage(ImageName);//Find image within list of animations

                string thisRegex = @"\ ((\s * xpos\ s *= \s * (\- ? [0 - 9] * \. ? [0 - 9] * )\ s * \, ? | \s * ypos\ s *= \s * (\- ? [0 - 9] * \. ? [0 - 9] * )\ s * \, ? | \s * alpha\ s *= \s * (\- ? [0 - 9] * \. ? [0 - 9] * )\ s * \, ? | \s * direction\ s *= \s * (\- ? [0 - 9] * \. ? [0 - 9] * )\ s * \, ? | \s * speed\ s *= \s * (\- ? [0 - 9] * \. ? [0 - 9] * )\ s * \, ? ) * \s * \)
                    ";
                if (Regex.Match(AnimationA.Animation, thisRegex, RegexOptions.None).Success)
                {

                    Match thisMatch = Regex.Match(AnimationA.Animation, thisRegex, RegexOptions.None);

                    if (thisMatch.Groups[6].Length > 0)
                    {
                        time = Convert.ToDouble(thisMatch.Groups[6].Value);
                    }

                    if (thisMatch.Groups[2].Length > 0)
                    {
                        xpos = thisMatch.Groups[2].Value.Contains(" % ") ? ScreenSize(Convert.ToDouble(thisMatch.Groups[2].Value.Replace('%', ' ')), true) : (float)(Convert.ToDouble((thisMatch.Groups[3].Value)));
                    }
                    if (thisMatch.Groups[3].Length > 0)
                    {
                        ypos = thisMatch.Groups[3].Value.Contains(" % ") ? ScreenSize(Convert.ToDouble(thisMatch.Groups[3].Value.Replace('%', ' ')), false) : (float)(Convert.ToDouble((thisMatch.Groups[4].Value)));
                    }

                    alpha = thisMatch.Groups[4].Length > 0 ? Convert.ToDouble(thisMatch.Groups[4].Value) : -1.0;

                    if (thisMatch.Groups[5].Length > 0)
                    {
                        direction = Convert.ToInt32(thisMatch.Groups[5].Value);
                    }
                    if (thisMatch.Groups[6].Length > 0)
                    {
                        speed = (float)Convert.ToDouble(thisMatch.Groups[6].Value);
                    }
                }
            }
        }
        catch (Exception e)
        {
            PrintTextError(" < b > ERROR : Game - File</ b>: " + Animation + "
                    animation cannot be read at line: " );
        }

        try
        {

            if (time != -1.0)//check if time is set         SKIP TO TO LOCATION IF CLICK AGAIN
            {
                if (xpos != -1)//check if x has a value set
                {
                    if (xpos == 0)//If its zero the program wont divide correctly, so if it is make it 1.
                    {
                        xpos = 1;
                    }
                    if (((Image.Xpos * Screen.width) < ((xpos * Screen.width) - 5) || Image.Xpos * Screen.width > ((xpos * Screen.width) + 5)) && (AnimationTime % ((xpos * Screen.width) / time) > -0.20 || AnimationTime % ((xpos * Screen.width) / time) < 0.20))//check if its in the location yet
                    {

                        Image.Xpos = Mathf.Lerp((float)(Image.Xpos), (float)(xpos), (float)(Time.deltaTime / (time * 1.1)));

                    }
                    else
                    {
                        Image.Animation = Image.Animation.Replace(AnimationA.Name, "
                    ");//remove animatoin when done
                    }
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak)
                    {
                        Image.Animation = Image.Animation.Replace(AnimationA.Name, "
                    ");//remove animatoin when done
                        Image.Xpos = xpos;
                    }
                }
                if (ypos != -1)//check if x has a value set
                {
                    if (ypos == 0)//If its zero the program wont divide correctly, so if it is make it 1.
                    {
                        ypos = 1;
                    }
                    if ((Image.Ypos * Screen.height) < ((ypos * Screen.height) - 5) || (Image.Ypos * Screen.height) > ((ypos * Screen.height) + 5))//check if its in the location yet
                    {

                        if (AnimationTime % ((ypos * Screen.height) / time) > -0.20 || AnimationTime % ((ypos * Screen.height) / time) < 0.20)//check animation time
                        {

                            Image.Ypos = Mathf.Lerp((float)(Image.Ypos), (float)(ypos), (float)(Time.deltaTime / (time * 1.1)));
                        }


                    }
                    else
                    {
                        Image.Animation = Image.Animation.Replace(AnimationA.Name, "
                    ");//remove animatoin when done
                    }
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak)
                    {
                        Image.Animation = Image.Animation.Replace(AnimationA.Name, "
                    ");//remove animatoin when done
                        Image.Ypos = ypos;
                    }
                }

                if (alpha != -1.0)
                {
                    if (Image.Alpha != alpha)
                    {
                        Image.Alpha = Mathf.MoveTowards((float)(Image.Alpha), (float)(alpha), (float)(Time.deltaTime / (time * 2.5)));
                    }
                    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !MouseBreak)
                    {
                        Image.Alpha = alpha;
                    }
                }
            }
            else if (speed != -1 && direction != -1)
            {

                if (direction == 1)//left
                {
                    Image.Xpos -= speed * 0.001;
                }
                else if (direction == 2)//right
                {
                    Image.Xpos += speed * 0.001;
                }
                else if (direction == 3)//up
                {
                    Image.Ypos += speed * 0.001;
                }
                else if (direction == 4)//down
                {
                    Image.Ypos -= speed * 0.001;
                }
            }

        }
        catch (Exception e)
        {
            PrintTextError(" < b > ERROR : Game - File</ b>: " + Animation + "
                    animation run error with line: " );
        }
    }//Reads the animation
    void FixedUpdate()
    {

        // CommandLineOutput += Time.fixedTime;
        if (CurrentMenuShowing != "
                    InGameMenu ")
        {
            AnimationTime += Time.fixedDeltaTime;
            Channel2Options.PlayedFor += Time.fixedDeltaTime;
            Channel1Options.PlayedFor += Time.fixedDeltaTime;


            moviePlayedForTime += Time.fixedDeltaTime;
        }
    }//FixedUpdate is used so animation time works correctly
    void IncrementLineNumber()//Goes to next line in the program.
    {
        LineSearching++;
    }
    void NextLine()
    {
        LineSearching++;
        ContinueGame();
    }

    void AddAnimation(string Animation, string ImageName)//For Animate to use to add an animation to an image
    {
        CurrentImage Image = null;
        try
        {

            Image = SelectCurrentImage(ImageName);//Find animation within list of animations
            if (Image.Animation != "
                    ")
            {
                Image.Animation += ", " + Animation;
            }
            else
            {
                Image.Animation += Animation;
            }

        }
        catch (Exception e)
        {
            PrintTextError(" < b > ERROR : Game - File</ b> : at line: " );
        }
    }
    void ControlSound(string Command, int channel, double volume, bool Loop, double FadeIn, AudioClass Audio, double FadeOut)//Executes play sound
    {
        if (Command == "
                    Play ")
        {
            switch (channel)
            {
                case 2:
                    
                    Channel2Options.MaxVolume = volume;
                    Channel2.clip = (AudioClip)Resources.Load(Audio.SoundFile);
                    
                    if (FadeIn != -1.0)
                    {
                        Channel2.volume = 0.0F;
                        Channel2Options.FadeIn = FadeIn;
                    }
                    else
                    {

                        Channel2.volume = (float)(volume);
                    }
                    Channel2Options.FadeOut = FadeOut;
                    Channel2Options.PlayedFor = 0.0f;
                    Channel2.loop = Loop;
                    Channel2.Play();

                    break;
                case 1:
                    Channel1Options.MaxVolume = volume;
                    Channel1.clip = (AudioClip)Resources.Load(Audio.SoundFile);
                    Debug.Log(Channel1.clip + "
                    " +  Audio.SoundFile + "
                    " + Audio.Name);
                    if (FadeIn != -1.0)
                    {
                        Channel1.volume = 0.0F;
                        Channel1Options.FadeIn = FadeIn;
                    }
                    else
                    {

                        Channel1.volume = (float)(volume);
                    }
                    Channel1Options.FadeOut = FadeOut;
                    Channel1Options.PlayedFor = 0.0f;
                    Channel1.loop = Loop;
                    Channel1.Play();

                    break;
                default:
                    PrintTextError(" < b > ERROR : Game - File</ b>: 'channel'
                    attribute value is incorrect at line: " );
                    break;
            }
        }
        else if (Command == "
                    Pause ")
        {

        }
        else if (Command == "
                    Resume ")
        {

        }
    }
    void Jump(int Line)//Jumps to a specific line
    {
        LineSearching = Line;
        ContinueGame();
    }
    string SettingsSubString(string VarName, string Line)
    {
        try
        {
            string ReturnValue = "
                    ";
            Line = Line.Substring((Line.IndexOf(VarName + " = ") - 1) + (VarName.Length + 3));

            Line = Line.Trim();
            Line = Line.Remove(Line.Length - 1);
            ReturnValue = Line;
            return ReturnValue;
        }
        catch (Exception e)
        {
            PrintTextError("
                    ERROR: < b > Settings - File</ b> : Error at line: " + " < b > " + Line + " < / b > ");
            return null;
        }

    }
    void LoadMenu()
    {
        MenuControlsList.Clear();//Clear the main menu menus

        if (MenuShowing == true)
        {

            if (MenuList.Exists(SearchingMenu => SearchingMenu.Name == CurrentMenuShowing))//see if menu exits
            {

                MenuClass CurrentMenu = MenuList.Find(SearchingMenu => SearchingMenu.Name == CurrentMenuShowing);//searches if the menu exists
                foreach (string line in CurrentMenu.Lines)
                {
                    
                    if (Regex.Match(line, @"#
                    Button\ s * \(([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , ? \s * ([0 - 9] * \. ? [0 - 9] * ) ? \)\ s *: \s * \(Text\ s *= \s * "" (.*)
                        ""\
                        s * , \s * Command\ s *= \s * "" (.* ? )
                        ""\
                        s * (, \s * Texture\ s *= \s * "" (.* ? )
                            ""\
                            s * ) ? (, \s * OnHover\ s *= \s * "" (.* ? )
                            ""\
                            s * ) ? (, \s * AlphaFadeIn\ s *= \s * ([0 - 9] * \. ? [0 - 9] * )) ? \)
                    ", RegexOptions.None).Success)
                    {
                        Match thisMatch = Regex.Match(line, @"#
                    Button\ s * \(([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , \s * ([0 - 9] * \. ? [0 - 9] * )\ s * , ? \s * ([0 - 9] * \. ? [0 - 9] * ) ? \)\ s *: \s * \(Text\ s *= \s * "" (.*)
                        ""\
                        s * , \s * Command\ s *= \s * "" (.* ? )
                        ""\
                        s * (, \s * Texture\ s *= \s * "" (.* ? )
                            ""\
                            s * ) ? (, \s * OnHover\ s *= \s * "" (.* ? )
                            ""\
                            s * ) ? (, \s * AlphaFadeIn\ s *= \s * ([0 - 9] * \. ? [0 - 9] * )) ? \)
                    ", RegexOptions.None);
                    
                        
                        float XLocation = (float)Convert.ToDouble(thisMatch.Groups[1].Value);
                        float YLocation = (float)Convert.ToDouble(thisMatch.Groups[2].Value);
                        float Width = (float)Convert.ToDouble(thisMatch.Groups[3].Value);
                        float Height = (float)Convert.ToDouble(thisMatch.Groups[4].Value);
                        string Text = thisMatch.Groups[6].Value;
                        string Command = thisMatch.Groups[7].Value;
                        float Fadein = 0.0f;
                        if (thisMatch.Groups[13].Value.Trim() != "
                    ")
                        {
                        Fadein = (float)Convert.ToDouble(thisMatch.Groups[13].Value);
                        }
                        Texture2D ButtonTexture = null;
                      
                        if(thisMatch.Groups[9].Value.Trim() != "
                    ")
                        {
                         ButtonTexture = FindImage(thisMatch.Groups[9].Value);
                        }

                        Texture2D OnHoverButtonTexture = null;
                          
                        if (thisMatch.Groups[11].Value.Trim() != "
                    ")
                        {
                            OnHoverButtonTexture = FindImage(thisMatch.Groups[11].Value);
                        }

                        Text = Text.Replace("\
                    "", "");

                MenuControlsList.Add (new MenuControls (XLocation, YLocation, Width, Height, "<b>" + Text + "</b>", Command, "Button", Fadein, ButtonTexture, OnHoverButtonTexture));
            } else if (Regex.Match (line, @"^#SetBackground(\s*)(.*)" + SpecialKeywords, RegexOptions.None).Success) {
                Match thisMatch = Regex.Match (line, @"^#SetBackground(\s*)(.*)" + SpecialKeywords, RegexOptions.None);
                try {

                    string BackgroundVarName = thisMatch.Groups[2].Value; //Get the background image name
                    BackgroundVarName = BackgroundVarName.Trim (); //remove white spaces

                    if (Images.Exists (SearchingBackground => SearchingBackground.Name == BackgroundVarName)) //Check if this background exits
                    {
                        Background = SelectImage (BackgroundVarName);; //Background varible equals found background
                    } else {
                        PrintTextError ("ERROR: <b>Menu-File</b>: Background Image cannot be found at line: ");
                    }

                } catch (Exception e) {
                    PrintTextError ("ERROR: <b>Menu-File</b>: SetBackground Error at line: ");
                }

            } else if (line.Contains ("#Text")) {

                float x = 0;
                float y = 0;
                string Text = line.Substring (line.IndexOf (":") + 1);
                string position = line.Substring (line.IndexOf ("#Text") + 6, line.IndexOf (":") - line.IndexOf ("#Text") - 6); //Get the position string between the ( & ) 
                char[] charsToTrim = { '(', ' ', ')' };
                position = position.Trim (charsToTrim);
                string[] positions = position.Split (',');
                float Fadein = 0.0f;
                foreach (string posLine in positions) {

                    if (posLine.Contains ("x")) {
                        if (posLine.Contains ("%")) {

                            string XLocationPercent = posLine.Replace ("%", "");
                            double XLocationPercentDouble = Convert.ToDouble (XLocationPercent.Trim ('x'));
                            x = ScreenSize (XLocationPercentDouble, true);
                        } else {

                            x = (float) Convert.ToDouble (posLine.Trim ('x'));
                        }

                    } else if (posLine.Contains ("y")) {

                        if (posLine.Contains ("%")) {

                            string YLocationPercent = posLine.Replace ("%", "");
                            double YLocationPercentDouble = Convert.ToDouble (YLocationPercent.Trim ('y'));
                            y = ScreenSize (YLocationPercentDouble, false);
                        } else {

                            y = (float) Convert.ToDouble (posLine.Trim ('y'));
                        }

                    } else if (posLine.Contains ("f")) {

                        Fadein = (float) Convert.ToDouble (posLine.Trim ('f'));

                    }

                }
                Text = Text.Replace ("\"", ""); //remove quotes
                MenuControlsList.Add (new MenuControls (x, y, 0, 0, Text, "", "Text", Fadein));
            } else if (line.Contains ("#PlayAudio")) {
                int Channel = -1;
                double Volume = -1.0;
                bool Loop = false;
                double FadeIn = -1.0;
                double FadeOut = -1.0;
                try {

                    string AudioVarName = line.Substring (line.IndexOf ("#PlayAudio") + 11, line.IndexOf (":") - line.IndexOf ("#PlayAudio") - 12); //Get the image name

                    if (Sounds.Exists (SearchingImages => SearchingImages.Name == AudioVarName)) //Check if this audio exits
                    {
                        AudioClass AudioToFind = Sounds.Find (SearchingSound => SearchingSound.Name == AudioVarName); //find audio

                        if (AudioToFind != null) {
                            if (line.Contains ("volume")) { //Check if volume is there
                                string VolumeString = line.Substring (line.IndexOf ("volume=") + 8, line.IndexOf (")v") - line.IndexOf ("volume=") - 8); //Get the volume string
                                if (Double.TryParse (VolumeString, out Volume)) { } else { PrintTextError ("<b>ERROR: Game-File</b>: Audio 'volume' attribute cannot be parsed at line: "); }
                            } else {
                                PrintTextError ("<b>ERROR: Game-File</b>: Audio 'volume' attribute not found at line: ");
                            }
                            if (line.Contains ("fadein")) { //Check if volume is there
                                string ChannelString = line.Substring (line.IndexOf ("fadein=") + 8, line.IndexOf (")f") - line.IndexOf ("fadein=") - 8); //Get the volume string

                                if (Double.TryParse (ChannelString, out FadeIn)) { } else { PrintTextError ("<b>ERROR: Game-File</b>: Audio 'fadein' attribute cannot be parsed at line: "); }
                            }
                            if (line.Contains ("channel")) { //Check if volume is there
                                string ChannelString = line.Substring (line.IndexOf ("channel=") + 9, line.IndexOf (")c") - line.IndexOf ("channel=") - 9); //Get the volume string

                                if (Int32.TryParse (ChannelString, out Channel)) { } else { PrintTextError ("<b>ERROR: Game-File</b>: Audio 'channel' attribute cannot be parsed at line: "); }
                            } else {
                                PrintTextError ("<b>ERROR: Game-File</b>: Audio 'channel' attribute not found at line: ");
                            }
                            if (line.Contains ("loop")) { //Check if volume is there
                                string LoopString = line.Substring (line.IndexOf ("loop=") + 6, line.IndexOf (")l") - line.IndexOf ("loop") - 6); //Get the volume string
                                LoopString = LoopString.ToLower ();
                                if (LoopString == "true") {
                                    Loop = true;
                                } else if (LoopString == "false") {
                                    Loop = false;
                                } else {
                                    PrintTextError ("<b>ERROR: Game-File</b>: Audio 'loop' attribute cannot be parsed at line: ");
                                }
                            }
                            if ((Channel == 1 && Channel1.clip == null) || (Channel == 2 && Channel2.clip == null)) {
                                ControlSound ("Play", Channel, Volume, Loop, FadeIn, AudioToFind, FadeOut);
                            }

                        } else {
                            PrintTextError ("<b>ERROR: Game-File</b>: Audio: " + AudioVarName + " cannot be found at line: ");
                        }

                    }
                } catch (Exception e) {
                    PrintTextError ("<b>ERROR: Game-File</b>: PlayAudio Error at line: ");
                }

            } else if (line.Contains ("#DialogHistory")) {
                MenuShowing = true;
            }
        }
    } else {
        PrintTextError ("ERROR: <b>Menu-File</b>: Menu cannot be found at line: " + "<b>" + (MenuLineSearching + 1) + "</b>");

    }

}

}

public void Save (int SaveNumber) {

    // SaveData data = new SaveData();

    //Save Variables
    //string SaveFileName = "Save" + SaveNumber + ".cut";

    /* data.CurrentLine = (LineSearching);




     data.Background = Background.Name;
     data.CurrentDialog = Dialog;
     data.Images = CurrentImages;
     data.NewDialog = NewDialog;
     data.StringNumber = StringNumber;
     data.OldHeight = Screen.height;
     data.OldWidth = Screen.width;
     if (currentMovie != null)
     {
         data.CurrentMovie = currentMovie.name;
     }
     if (Channel1.clip != null)
     {
         data.Channel1 = Channel1.clip.name;
     }
     if (Channel2.clip != null)
     {
         data.Channel2 = Channel2.clip.name;
     }*/
    //data.AllSaveData = this;
    /*Stream stream = File.Open(SaveFileName, FileMode.Create);
    BinaryFormatter bformatter = new BinaryFormatter();
    bformatter.Binder = new VersionDeserializationBinder();



    //Debug.Log("Saving Information...");
    CommandLineOutput += "Saving Information! \n";
    CommandLineOutput += "Game File: " + GameFileLocation + "\n";
    bformatter.Serialize(stream, data);
    stream.Close();
    CommandLineOutput += "Information Saved! \n";*/

    //  Debug.Log("Information Saved!");
}
public void Load (int SaveNumber) {
    StopCoroutine ("SlowText");
    SaveData data = new SaveData ();
    string SaveFileName = "Save" + SaveNumber + ".cut";
    Stream stream = File.Open (SaveFileName, FileMode.Open);
    StopCoroutine ("SlowText");
    //  Debug.Log("Finding save data...");
    CommandLineOutput += "Finding save data... \n";
    BinaryFormatter bformatter = new BinaryFormatter ();
    // Debug.Log("Formating data...");
    bformatter.Binder = new VersionDeserializationBinder ();
    // Debug.Log("Loading Data Packs...");
    data = (SaveData) bformatter.Deserialize (stream);
    /*
    //Load actual variables!
    CommandLineOutput += "Loading Data Packet 1... \n";

    LineSearching = (data.AllSaveData.LineSearching - 1);
    Dialog = data.AllSaveData.Dialog;
    NewDialog = data.AllSaveData.NewDialog;
    StringNumber = 0;
    ChoiceButtons.Clear();
    // Debug.Log("-Loading Data Packet 2...");
    CommandLineOutput += "Loading Data Packet 2... \n";
    Background = Backgrounds.Find(SearchingBackground => SearchingBackground.Name == data.AllSaveData.Background.Name);//Background varible equals found background
    // Debug.Log("-Loading Data Packet 3...");
    CommandLineOutput += "Loading Data Packet 3... \n";
    if (data.Channel1 != "")
    {
        Channel1.clip = (AudioClip)Resources.Load("Sounds/" + data.Channel1);
        Channel1.Play();
        Channel1.volume = 1.0f;
        Channel1Options.FadeOut = -1.0;
        Channel1Options.FadeIn = 5.0;
    }
    if (data.Channel2 != "")
    {
        Channel2.clip = (AudioClip)Resources.Load("Sounds/" + data.Channel2);
        Channel2.Play();
        Channel2.volume = 1.0f;
        Channel2Options.FadeOut = -1.0;
        Channel2Options.FadeIn = 5.0;
    }




    // Debug.Log("-Loading Data Packet 4...");//resets dialog
    CommandLineOutput += "Loading Data Packet 4... \n";

    CurrentImages = data.Images;
    CommandLineOutput += "Loading Data Packet 5... \n";
    //  Debug.Log("-Loading Data Packet 5...");//resets dialog
    //  Debug.Log("Continuing Game...");
    oldHeight = data.OldHeight;
    oldWidth = data.OldWidth;

    CommandLineOutput += "Continuing Game... \n";

    if (data.CurrentMovie != null)
    {
        currentMovie = (MovieTexture)Resources.Load("Movies/" + data.CurrentMovie);
        currentMovie.Play();
    }
     */
    // this.
    stream.Close ();
    NextLine ();

}
public void Save () {

    SaveData data = new SaveData ();

    //Save Variables
    string SaveFileName = "QuickSave" + ".cut";

    data.CurrentLine = (LineSearching);

    data.Background = Background.Name;
    data.CurrentDialog = Dialog;
    data.Images = CurrentImages;
    data.NewDialog = NewDialog;
    data.StringNumber = StringNumber;
    data.OldHeight = Screen.height;
    data.OldWidth = Screen.width;
    if (currentMovie != null) {
        data.CurrentMovie = currentMovie.name;
    }
    if (Channel1.clip != null) {
        data.Channel1 = Channel1.clip.name;
    }
    if (Channel2.clip != null) {
        data.Channel2 = Channel2.clip.name;
    }
    Stream stream = File.Open (SaveFileName, FileMode.Create);
    BinaryFormatter bformatter = new BinaryFormatter ();
    bformatter.Binder = new VersionDeserializationBinder ();

    //Debug.Log("Saving Information...");
    CommandLineOutput += "Saving Information! \n";
    CommandLineOutput += "Game File: " + GameFileLocation + "\n";
    bformatter.Serialize (stream, data);
    stream.Close ();
    CommandLineOutput += "Information Saved! \n";
    //  Debug.Log("Information Saved!");
} //QUICK SAVE
public void Load () {

    SaveData data = new SaveData ();
    string SaveFileName = "QuickSave" + ".cut";
    Stream stream = File.Open (SaveFileName, FileMode.Open);

    //   CommandLineOutput += "Finding save data... \n";
    BinaryFormatter bformatter = new BinaryFormatter ();

    bformatter.Binder = new VersionDeserializationBinder ();

    data = (SaveData) bformatter.Deserialize (stream);

    //  CommandLineOutput += "Loading Data Packet 1... \n";
    Dialog = "";
    NewDialog = data.NewDialog;
    StringNumber = 0;
    LineNumber = 0;
    LineSearching = (data.CurrentLine - 1);

    ChoiceButtons.Clear ();

    // CommandLineOutput += "Loading Data Packet 2... \n";
    Background = SelectImage (data.Background);; //Background varible equals found background

    //  CommandLineOutput += "Loading Data Packet 3... \n";
    if (data.Channel1 != "") {
        Channel1.clip = (AudioClip) Resources.Load ("Sounds/" + data.Channel1);
        Channel1.Play ();
        Channel1.volume = 1.0f;
        Channel1Options.FadeOut = -1.0;
        Channel1Options.FadeIn = 5.0;
    }
    if (data.Channel2 != "") {
        Channel2.clip = (AudioClip) Resources.Load ("Sounds/" + data.Channel2);
        Channel2.Play ();
        Channel2.volume = 1.0f;
        Channel2Options.FadeOut = -1.0;
        Channel2Options.FadeIn = 5.0;
    }

    if (data.CurrentMovie != null) {
        currentMovie = (MovieTexture) Resources.Load ("Movies/" + data.CurrentMovie);
        currentMovie.Play ();
    }

    // CommandLineOutput += "Loading Data Packet 4... \n";

    CurrentImages = data.Images;
    //CommandLineOutput += "Loading Data Packet 5... \n";

    oldHeight = data.OldHeight;
    oldWidth = data.OldWidth;

    // CommandLineOutput += "Continuing Game... \n";

    StopAllCoroutines ();
    stream.Close ();
    NextLine ();

} //QUICK LOAD

void ResetBasics () {
    LineSearching = 0;

    //Dialog
    NewDialog = "";
    Dialog = "";
    StringNumber = 0;
    LineNumber = 1;

    //Sound
    Channel1.clip = null;
    Channel2.clip = null;

    //Movies
    currentMovie.Stop ();

    //Images
    CurrentImages.Clear ();
    Background = null;
    BackgroundB = null;

    ChoiceButtons.Clear ();

    TextBox = true;
    TextBoxWasOn = true;

}

public void ClearScreen () //Cleares the screen of old images etc
{

    CurrentImages.Clear ();
    Background = null;
    BackgroundB = null;
}
public void ResumeGame () {

    Running = true;
    MenuShowing = false;
    StartCoroutine (Wait (0));
    if (TextBoxWasOn == true) {
        TextBox = true;
    }
    if (Dialog != NewDialog) { StartCoroutine (WriteText ()); } //restart text appearing
    if (MusicWasPlaying1 == true) {
        Channel1.Play ();
        MusicWasPlaying1 = false;
    }
    if (MusicWasPlaying2 == true) {
        Channel2.Play ();
        MusicWasPlaying2 = false;
    }
    if (currentMovie != null) {
        currentMovie.Play ();
    }
} //back to game from looking at a menu
public void PauseGame () {
    StopAllCoroutines ();
    Running = false;
    if (Channel1.isPlaying == true) {
        Channel1.Pause ();
        MusicWasPlaying1 = true;
    }
    if (Channel2.isPlaying == true) {
        Channel2.Pause ();
        MusicWasPlaying2 = true;
    }
    if (currentMovie != null) {
        currentMovie.Pause ();
    }
}
public void NextMenu (string Menu) {
    LastMenuShowed = CurrentMenuShowing;
    CurrentMenuShowing = Menu;
    LoadMenu ();
} //set the previous menu and goes to the current

/*    public void AutoResize(int screenWidth, int screenHeight)
    {
         print(screenWidth + "-" + screenHeight + " " + oldWidth + "-" + oldHeight);
        if (oldHeight != -1 && oldWidth != -1)
        {
            Vector2 resizeRatio = new Vector2((float)oldWidth / screenWidth, (float)oldHeight / screenHeight);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
            if (resized == false)
            {
                foreach (CurrentImage x in CurrentImages)//go through and resets
                {
                    print(resizeRatio.x + " " + resizeRatio.y);
                    x.Xpos = (x.Xpos / resizeRatio.x);
                    x.Ypos = (x.Ypos / resizeRatio.y);
                    //x.width = (int)(x.width/resizeRatio.x);
                    //x.height = (int)(x.height/resizeRatio.y);


                    x.Animation = "";

                }

                resized = true;
            }

        }
        else
        {
            Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
        }
        oldHeight = screenHeight;
        oldWidth = screenWidth;
    }*/

string EscapeCharacters (string Text) { //escape characters!
    Text = Text.Replace ("/,", ","); //escaping the string for commas
    Text = Text.Replace ("/:", ":"); //escaping the string for :
    Text = Text.Replace ("/'", "\""); //escaping the string for "

    return Text;
}

void LoadImage (string Filename, string Name) //Load a background/image into the background array
{
    TextureVariable x = new TextureVariable (Name, Filename); //if its anythign else add it to background
    if (x != null && x.Texture != null) {
        Images.Add (x);
    } else {
        PrintTextError ("<b>ERROR: Game-File</b>: Cannot find image at line: ");
    }
}

Texture2D FindImage (string Name) //Find Texture 2d
{ //finds an image
    TextureVariable ImageToFind = Images.Find (SearchingImage => SearchingImage.Name == Name); //find the character if it exists
    if (ImageToFind != null && ImageToFind.Texture != null) {
        return ImageToFind.Texture;
    } else {
        ImageToFind = Images[1];
        PrintTextError ("<b>ERROR: Game-File</b>: Cannot find image at line: ");
        return ImageToFind.Texture;
    }

}

TextureVariable SelectImage (string Name) //Finds texture variable from images
{
    TextureVariable ImageToFind = Images.Find (SearchingImage => SearchingImage.Name == Name);
    if (ImageToFind != null && ImageToFind.Texture != null) {
        return ImageToFind;
    } else {
        ImageToFind = Images[1];
        PrintTextError ("<b>ERROR: Game-File</b>: Cannot find image at line: ");
        return ImageToFind;
    }

}

CurrentImage SelectCurrentImage (string Name) //Finds current image from current images
{
    CurrentImage ImageToFind = CurrentImages.Find (SearchingImage => SearchingImage.Name == Name);
    if (ImageToFind.Name != "") {
        return ImageToFind;
    } else {
        PrintTextError ("<b>ERROR: Game-File</b>: Cannot find image at line: ");
        return ImageToFind;
    }

}
void AddToDialogHistory (string Text) {
    DialogHistory.Add (Text);
}

}