using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class GhostBlockDataGenerator : MonoBehaviour
{
	float timeLastDataGathered = 0;

	string fileName = "pdataw.txt";

	// Writer for the data
	StreamWriter streamWriter;

	// Array to keep position data
	float[] dataPositions;

	// Points to the current index of position
	int curPositionIndex;

	// Use this for initialization
	void Start ()
	{
		if (File.Exists (fileName)) {
			File.Delete(fileName);
		}
		streamWriter = File.CreateText(fileName);
		streamWriter.Close();

		// Initialize data positions array
		dataPositions = new float[100];
		curPositionIndex = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Time.timeSinceLevelLoad - timeLastDataGathered > StageController.GHOST_DATA_GATHER_INTERVAL) {
			// TODO gather data
			dataPositions[curPositionIndex] = transform.position.y;
			curPositionIndex++;

			timeLastDataGathered = Time.timeSinceLevelLoad;

			if (curPositionIndex == 100) {
				writeToFile();
			}
		}
	}

	// Method for writing data on the file
	public void writeToFile() {
		// TODO write file
		streamWriter = File.AppendText(fileName);
		for (int i = 0; i < dataPositions.Length; i++) {
			streamWriter.WriteLine ("{0}", dataPositions[i]);
		}
		streamWriter.Close();

		Debug.Log("Wrote to file");

		// Resets current position index
		curPositionIndex = 0;
	}

	public void endFile() {
		streamWriter = File.AppendText(fileName);
		streamWriter.WriteLine("END");
		streamWriter.Close();
	}
}
