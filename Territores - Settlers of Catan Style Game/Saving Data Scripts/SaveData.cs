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

    public List<HexGridClass> HexGrid;
    public string CurrentPlayerName = "";
    public List<PlayerClass> PlayerList = new List<PlayerClass> ();
    public List<SettingsClass> Settings = new List<SettingsClass> ();

    public SaveData (SerializationInfo info, StreamingContext ctxt) {
        HexGrid = (List<HexGridClass>) info.GetValue ("HexGrid", typeof (List<HexGridClass>));
        CurrentPlayerName = (string) info.GetValue ("CurrentPlayerName", typeof (string));
        PlayerList = (List<PlayerClass>) info.GetValue ("PlayerList", typeof (List<PlayerClass>));
        Settings = (List<SettingsClass>) info.GetValue ("Settings", typeof (List<SettingsClass>));
    }

    public void GetObjectData (SerializationInfo info, StreamingContext ctxt) {
        info.AddValue ("HexGrid", (HexGrid));
        info.AddValue ("CurrentPlayerName", (CurrentPlayerName));
        info.AddValue ("PlayerList", (PlayerList));
        info.AddValue ("Settings", Settings);
    }

    public SaveData () { }
}