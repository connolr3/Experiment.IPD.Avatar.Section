using System;
using UnityEngine;
using System.IO;
public class MyTransformRecorder : MonoBehaviour
{
    private int frameCount;
    private float currentTime;
    private string filePath;

public string path;
public Transform spawnA;
public Transform spawnB;
    private void Start()
    {
 

        string fileName = "CameraRecordedActions_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        filePath = Path.Combine(path, fileName);
        Debug.Log("File will be saved at: " + filePath);
        string[] header = { "Frame", "Time", "PositionX", "PositionY", "PositionZ" };
        WriteToCSV(header);

  string[] data1 = {"spawnA", "spawnA", spawnA.position.x.ToString(), spawnA.position.y.ToString(), spawnA.position.z.ToString() };
  string[] data2 = {"spawnB", "spawnB", spawnB.position.x.ToString(), spawnB.position.y.ToString(), spawnB.position.z.ToString() };
        WriteToCSV(data1);
         WriteToCSV(data2);
    }

    private void Update()
    {
        // Record the position every 5 frames
        if (frameCount % 5 == 0)
        {
            RecordPosition();
        }

        frameCount++;
    }

    private void RecordPosition()
    {
        // Update class fields with the current time and frame values
        currentTime = Time.time;

        // Record position information
        Vector3 position = transform.position;
        string[] data = { frameCount.ToString(), currentTime.ToString(), position.x.ToString(), position.y.ToString(), position.z.ToString() };
        WriteToCSV(data);
    }

    private void WriteToCSV(string[] data)
    {
        // Check if the file exists, create it if not
        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                // Write the header row
                sw.WriteLine(string.Join(",", data));
            }
        }
        else
        {
            // Append data to the existing file
            using (StreamWriter sw = File.AppendText(filePath))
            {
                // Write the data row
                sw.WriteLine(string.Join(",", data));
            }
        }
    }
}
