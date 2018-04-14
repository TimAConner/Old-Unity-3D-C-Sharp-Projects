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
[System.Serializable]
public class VariablesClass {

    //Version & Name Variables

    public string CutVersion = "1.1520";
    public string GameFileName = "Game00.cut";
    //public string GameVersion = "1.00";// CHKNG-03
    //Data File File Location Variables
    public string GameFileLocation;
    public string MenuFileLocation;
    public string SettingFileLocation;

    //Data File Variables
    public TextAsset GameFile;
    public TextAsset SettingsFile;
    public TextAsset MenuFile;

    [Tooltip ("The current line that the game is reading.")]
    public int LineSearching = -1; //line currently being looked at in script
    [HideInInspector]
    public int SetLineSearching = 0; //Settings line
    [HideInInspector]
    public int MenuLineSearching = 0;
    [HideInInspector]
    public string[] lines;
    [HideInInspector]
    public string[] SetLines; //The lines for the settings file to be read
    [HideInInspector]
    public string[] MenuLines;

    //Decleration List Variables
    public List<Character> Characters = new List<Character> ();
    public List<TextureVariable> Backgrounds = new List<TextureVariable> ();
    public List<TextureVariable> Images = new List<TextureVariable> ();
    public List<AnimationClass> Animations = new List<AnimationClass> ();
    public List<AudioClass> Sounds = new List<AudioClass> ();
    public List<MovieClass> Movies = new List<MovieClass> ();

    public List<SceneClass> Scenes = new List<SceneClass> (); //Scenes are after declerations but are found when searching before game for them

    //Reading File Variables
    public Boolean Running = true; //True means it is currently running, false means the game is done/paused.
    public Boolean DeclerationCheck = true; //This is true while getting declerations

    public bool FoundStart = false; //true if #Scene Start has been found

    //Dialog Variables
    public String DialogName = ""; //The name over the dialog box
    public String Dialog = ""; //Dialog to be shown
    public string NewDialog = ""; //This is the dialog that is currently being read
    public int StringNumber = 0; //the current text-character number that is being transfered
    public int LineNumber = 1; //The current line number in the dialog box
    public int curWord = 0;
    public bool NWFSTF = false; //NextWordFinishgSlowTextFirst - Used for waiting till the current speech is done to go to the next line if there is speech

    public Color CharacterTextColor = Color.white; //Color of the characters text
    public bool TextBox = true; //If the text box is on or not
    public bool TextBoxWasOn = true; //Used when flipping between menus to check whether the text box was on before the menu came up
    public float BackgroundAlpha = 1.0f; //The transparency of the text box
    public List<String> DialogHistory = new List<String> (); //An array of strings that are the previous dialog

    //Background Variables
    [HideInInspector]
    public TextureVariable Background;
    //Background Fading Variables
    [HideInInspector]
    public TextureVariable BackgroundB; //The background that is being faded to
    public double TimeToFade = -1.0; //The ammount of seconds to fade from the original transprency to the next.
    public double BackgroundFade = 1.0; //The current transparency of the background
    public bool ClickedThrough = false; // var if you click through it will automaticly finish animation
    public bool NextFade = false; //If the next line will also fade. CHKNG-01

    public List<CurrentImage> CurrentImages = new List<CurrentImage> (); //array of images that are currently being used

    public double AnimationTime = 0.0; //Used for animations
    public bool WaitingForAnimations = false; //True if you are waiting fr animations to complete.
    public float WaitedTime; //Time until animation is done? CHKNG-02

    //Choice GUI Variables
    [HideInInspector]
    public List<ButtonClass> ChoiceButtons = new List<ButtonClass> (); //array of buttons and what their command sare when you click them
    [HideInInspector]
    public bool Choices = false;

    //Sound Variables
    //Sound Fading Variables
    [HideInInspector]
    public AudioSource Channel1;
    public double Channel1FadeIn = -1.0;
    public double Channel1FadeOut = -1.0;
    public float Channel1PlayedFor = 0.0f;
    [HideInInspector]
    public AudioSource Channel2;
    public double Channel2FadeIn = -1.0;
    public double Channel2FadeOut = -1.0;
    public float Channel2PlayedFor = 0.0f;

    public bool MusicWasPlaying1 = false;
    public bool MusicWasPlaying2 = false;

    //Control Variables
    public bool MouseBreak = false;

    //Settings From Settings File Variables
    public bool Waiting = false;
    public bool nextAfterWait = false;
    public int TextSpeed;
    public int WindowWidth;
    public int WindowHeight;
    public int TextBoxHeight;
    public int TextBoxWidth;
    public float TextBoxOpacity;
    public string GameName;
    public string WindowName;
    public int FullScreen;
    public string FileEndText = "File Read to End.";
    public bool CommandLine = true;
    public string TextFont;
    public int TextSize;
    public string Language;
    public float GameVersion;

    //COMMAND LINE
    public bool CommandLineOpen = false;
    public string CommandLineCommand = "";
    public string CommandLineOutput = "";

    //GUI Styles
    [Tooltip ("This is for the bar displaying character names as they speak.")]
    public GUIStyle TitleBar; //title bar textures
    [Tooltip ("This is only for the text box image & size.")]
    public GUIStyle TextBoxStyle; //text box textures
    [Tooltip ("This is for the actual text in the textbox.")]
    public GUIStyle DefaultTextStyle; //default text
    public GUIStyle WindowStyle;
    public List<GUIStyle> FontStyles = new List<GUIStyle> ();
    public string CurrentFontStyle = "";

    public GUIStyle searchingStyle; //CHKNG-04

    //Prefrences CHKNG-06
    public float soundLevel = 1.0f; //CHKNG-05

    public int resizeWidth = Screen.width;
    public int resizeHeight = Screen.height;
    public int oldWidth = -1;
    public int oldHeight = -1;
    public bool resized = false;

    //Rectangle/Window Variables
    public Rect DialogHistoryRect = new Rect (0, 0, 0, 0);
    public Vector2 DialogHistoryScroll = Vector2.zero;
    public Rect DialogBoxRect = new Rect (0, 0, 0, 0);

    //Shaking Camera Variables
    public float CameraXOffset = 0.0f; //current offset
    public float SecondsToShake = 0.0f; //how many seconds to shake, counts down
    public float IntervalShake = 0.0f; //how many seconds to wait
    public float AmmountToShake = 0.0f; //how much to shake
    public float TotalShakeTime = 0.0f; //how many seconds to shake original

    //Movie Variables
    [HideInInspector]
    public MovieTexture currentMovie;
    public float movieFadein;
    public float movieFadeout;
    public float moviecurrentFade = 0.0f;
    public float moviePlayedForTime = 0.0f;

    public float Previous = 0;
    //public List<GUIStyle> Styles = new List<GUIStyle>();

    //Screen Resolution & Resizing Variables
    public float lastScreenWidth = 0f;

    //Menu Variables
    public bool MenuShowing = false; //if menus are being shown
    public string CurrentMenuShowing = ""; //what menu is being shown
    public string LastMenuShowed = ""; //the previous menu for "back"
    [HideInInspector]
    public List<MenuClass> MenuList = new List<MenuClass> (); //contains all the menus
    public List<MenuControls> MenuControlsList = new List<MenuControls> (); //list of the current controls up
    public bool FadeInMainMenu = false;

    //Regex Variables
    public string RES = @"\s*(NEXT|MouseBreak|MouseFix|NextFade)*";
    /*Add to end of all non-declerations (Regex-End-String).  This is for things like:
     * NEXT
     * NextFade
     * MouseBreak
     * MouseFix 
     * */
    public int countnum = 0; //The ammount of seconds currently passed in the game.
}