using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class MessageClass {
	public string Sender = "";
	public int Type = 0;
	public string Text = "";
	public List<ResourcesClass> Trade = new List<ResourcesClass> ();

	public MessageClass (string sender, int type, string text) {
		Sender = sender;
		Type = type;
		Text = text;
	}
	public MessageClass (string sender, int type, string text, ResourcesClass Give, ResourcesClass Want) {
		Sender = sender;
		Type = type;
		Text = text;
		Trade.Add (Give);
		Trade.Add (Want);
	}
	public MessageClass () {

	}

}