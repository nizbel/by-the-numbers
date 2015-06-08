using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class GhostBlockDataGenerator : MonoBehaviour
{
	float timeLastDataGathered = 0;

	string fileName = "testfile.txt";

	StreamWriter streamWriter;

	// Use this for initialization
	void Start ()
	{
		if (File.Exists (fileName)) {
			Debug.Log (fileName + " already exists.");
			return;
		}
		streamWriter = File.CreateText(fileName);
	}

	// Update is called once per frame
	void Update ()
	{
		if (Time.timeSinceLevelLoad - timeLastDataGathered > 0.5f) {
			// TODO gather data
			float playerPositionY = transform.position.y;

			timeLastDataGathered = Time.timeSinceLevelLoad;

			// TODO write file
			streamWriter.WriteLine ("{0};{1}", 
			              timeLastDataGathered, playerPositionY);
			streamWriter.Close();
		}
	}
}
