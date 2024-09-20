using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.persistentDataPath + "/Saves/";
    public const string FILE_NAME = "SaveFile";
    private const string SAVE_EXTENTION = ".dir";
    public static string FileName { get; private set; }
    public static string FilePath { get; private set; }

    public static void Initialize()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);

        }

        FileName = FILE_NAME + SAVE_EXTENTION;
        FilePath = SAVE_FOLDER + FILE_NAME + SAVE_EXTENTION;
    }

    public static void Save(SaveData saveData)
    {
        var settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        string saveString = JsonConvert.SerializeObject(saveData, settings);
        Debug.Log("Saved string to " + FilePath);
        File.WriteAllText(FilePath, saveString);
    }

    public static SaveData Load()
    {
        if (File.Exists(FilePath))
        {
            string saveString = File.ReadAllText(FilePath);
            SaveData loaded = JsonConvert.DeserializeObject<SaveData>(saveString);
            if (loaded == null)
            {
                return new SaveData();
            }

            return loaded;
        }
        else
        {
            return new SaveData();
        }
    }
}
