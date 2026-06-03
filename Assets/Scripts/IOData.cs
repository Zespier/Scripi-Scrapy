using UnityEngine;
using System.IO;

public static class IOData {

    public static void Delete(string path) {
        path = Application.persistentDataPath + "/" + path;

        File.Delete(path);
    }

    public static SaveData LoadSaveSystem(string path) {
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }

        return default;
    }

    public static void Save(string path, string json) {
        File.WriteAllText(path, json);
    }
}
