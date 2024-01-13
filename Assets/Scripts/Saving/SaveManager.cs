using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    public static SaveData Data { get; private set; }

    private static FileManager _fileManager;

    private static bool _loaded = false;

    public static void LoadData(Action<SaveData> callback)
    {
        if (_fileManager == null)
            _fileManager = new FileManager();

        if (!_loaded)
        {
            Data = _fileManager.LoadFile();
            _loaded = true;
        }

        callback(Data);
    }

    public static void SaveData(int level, int currentHealth)
    {
        if (_fileManager == null)
            _fileManager = new FileManager();

        if (Data == null)
            Data = new SaveData();

        Data.Level = level;
        Data.CurrentHealth = currentHealth;
        Data.PartitionSlices = null; // if we don't do this, the next level will have the same layout

        //save to file
        _fileManager.SaveFile(Data);
    }

    public static void SaveData(List<PartitionSlice> partitionSlices)
    {
        if (Data == null)
            Data = new SaveData();

        Data.PartitionSlices = partitionSlices;

        //save to file
        _fileManager.SaveFile(Data);
    }
}
