using System;
using System.IO;
using UnityEngine;

public class FileManager
{
    private string fileUrl;
    private const string FILE = "SaveFiles/Save.json";

    public FileManager()
    {
        fileUrl = Path.Combine(Application.persistentDataPath, FILE);
    }
    public void SaveFile(SaveData data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileUrl));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(fileUrl, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SAVING FAILED: " + e);
        }
    }
    public SaveData LoadFile()
    {
        SaveData saveData = null;

        if (File.Exists(fileUrl))
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fileUrl, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                saveData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("LOADING FAILED: " + e);
            }
        }

        return saveData;
    }
}
