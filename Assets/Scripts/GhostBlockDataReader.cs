using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class GhostBlockDataReader : MonoBehaviour {
	float timeLastDataGathered = 0;
	
	string fileName = "pdata.txt";
	
	// Writer for the data
	StreamReader streamReader;
	
	// Array to keep position data
	float[] dataPositions;
	
	// Points to the current index of position
	int curPositionIndex;

	// Points if there is data to be read
	bool noDataToRead = false;

	// Shows if the file was read to its end
	int endFileReached = -1;
	
	// Use this for initialization
	void Start ()
	{
		if (!File.Exists (fileName)) {
			noDataToRead = true;
		}
		else {	
			streamReader = File.OpenText(fileName);
		}
		
		// Initialize data positions array
		dataPositions = new float[100];
		curPositionIndex = 0;

		// Read from file
		readFile();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!noDataToRead) {
			if (Time.timeSinceLevelLoad - timeLastDataGathered > StageController.GHOST_DATA_GATHER_INTERVAL) {
				// Test if there is a valid position ahead
				if (curPositionIndex == endFileReached) {
					gameObject.renderer.enabled = false;
					noDataToRead = true;
					return;
				}

				// Increment index
				curPositionIndex++;
				
				timeLastDataGathered = Time.timeSinceLevelLoad;

				if (curPositionIndex == 100) {
					readFile();
				}
			}

			// Move ghost
			transform.position = Vector3.Lerp(transform.position, new Vector3(
				transform.position.x, dataPositions[curPositionIndex], transform.position.z), 1.5f*Time.deltaTime);
		}
	}
	
	// Method for writing data on the file
	void readFile() {
		if (!noDataToRead) {
			// Reads to the last position gathered
			for (int i = 0; i < 100 && endFileReached == -1; i++) {
				string newLine = streamReader.ReadLine();
				if (newLine.Equals("END")) {
					endFileReached = i;
				} else {
					dataPositions[i] = float.Parse(newLine);
				}
			}
			
			// Resets current position index
			curPositionIndex = 0;
		}
	}

	public void closeReader() {
		streamReader.Close();
	}
}
