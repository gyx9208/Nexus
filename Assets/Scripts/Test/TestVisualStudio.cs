using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using GamePlayProto;

public class TestVisualStudio : MonoBehaviour {
	public Text _Text;
	// Use this for initialization
	void Start () {
		HeartBeat inbeat = new HeartBeat();
		inbeat.TimeStamp = 1;

		//Act
		MemoryStream stream = new MemoryStream();
		ProtoBuf.Serializer.Serialize(stream, inbeat);

		stream.Seek(0, SeekOrigin.Begin);

		HeartBeat outbeat = ProtoBuf.Serializer.Deserialize<HeartBeat>(stream);

		_Text.text = outbeat.TimeStamp.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
