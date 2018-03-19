using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

[Serializable ()]
public class SaveData : ISerializable {

    //DIALOG
    public int CurrentLine;
    public string CurrentDialog;
    public int StringNumber;
    public string NewDialog;

    //DIALOG STYLES
    public string CurrentTextStyle;

    //FADES
    public float CurrentDialogBoxFade;

    //IMAGES
    public string Background;
    public string CurrentMovie;

    //SOUND
    public string Channel1;
    public string Channel2;

    //resizing
    public int OldHeight;
    public int OldWidth;

    public List<CurrentImage> Images = new List<CurrentImage> ();

    public SaveData (SerializationInfo info, StreamingContext ctxt) {
        //Get the values from info and assign them to the appropriate properties
        /* CurrentLine = (int)info.GetValue("CurrentLine", typeof(int));
         Background = (string)info.GetValue("Background", typeof(string));
         CurrentDialog = (string)info.GetValue("CurrentDialog", typeof(string));
         Images = (List<CurrentImage>)info.GetValue("Images", typeof(List<CurrentImage>));
         NewDialog = (string)info.GetValue("NewDialog", typeof(string));
         StringNumber = (int)info.GetValue("StringNumber", typeof(int));
         Channel1 = (string)info.GetValue("Channel1", typeof(string));
         Channel2 = (string)info.GetValue("Channel2", typeof(string));
         OldHeight = (int)info.GetValue("OldHeight", typeof(int));
         OldWidth = (int)info.GetValue("OldWidth", typeof(int));
        CurrentMovie = (string)info.GetValue("CurrentMovie", typeof(string));*/

    }

    //Serialization function.

    public void GetObjectData (SerializationInfo info, StreamingContext ctxt) {
        //load current line
        /*info.AddValue("CurrentLine", (CurrentLine));
        info.AddValue("Background", (Background));
        info.AddValue("CurrentDialog", (CurrentDialog));
        info.AddValue("Images", (Images));
        info.AddValue("NewDialog", (NewDialog));
        info.AddValue("StringNumber", (StringNumber));
        info.AddValue("Channel1", (Channel1));
        info.AddValue("Channel2", (Channel2));
        info.AddValue("OldHeight", (OldHeight));
        info.AddValue("OldWidth", (OldWidth));
        info.AddValue("CurrentMovie", (CurrentMovie));
        */

    }

    public SaveData () { }
}