using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
        IFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(FilePath, FileMode.OpenOrCreate);
        
        formatter.Serialize(stream, saveData);

        stream.Close();
    }

    public static SaveData Load()
    {
        if (File.Exists(FilePath))
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(FilePath, FileMode.OpenOrCreate);
            SaveData loaded = (SaveData)formatter.Deserialize(stream);

            stream.Close();
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
