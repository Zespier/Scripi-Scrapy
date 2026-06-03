using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem {

    public static PlayerStatistics statistics;

    private static bool _ignorePlayerSaves = false;

    public static void Save() {
        SaveData saveData = IOData.LoadSaveSystem(Path.Combine(Application.persistentDataPath, "SavePlayerData.json"));
        if (saveData == default) { saveData = new SaveData(); }

        if (!_ignorePlayerSaves) {
            SaveMoney(saveData);
            SaveStatistics(saveData);
            SaveUpgrades(saveData);
        }

        string json = JsonUtility.ToJson(saveData, true);
        IOData.Save(Path.Combine(Application.persistentDataPath, $"SaveData.json"), json);
    }

    #region Saving Methods

    private static void SaveMoney(SaveData saveData) {
        saveData.money = Money.instance.currentMoney;
    }

    private static void SaveStatistics(SaveData saveData) {
        saveData.statistics = statistics;
    }

    private static void SaveUpgrades(SaveData saveData) {
        saveData.upgrades = new List<UpgradeData> {
        new UpgradeData(UpgradeType.HitDamage, MenuUpgrades.instance.hitLevel > 1),
        new UpgradeData(UpgradeType.DrillSpeed, DrillStats.drillSpeed > 0.5f),
        new UpgradeData(UpgradeType.HitArea, MenuUpgrades.instance.hitAreaLevel > 1),
        new UpgradeData(UpgradeType.HandDrill, HandDrillStats.isUnlocked),
        new UpgradeData(UpgradeType.HandDrillSpeed, HandDrillStats.handDrillSpeed > 5),
    };
    }

    #endregion

    public static void DeleteSaveFile() {
        string path = Path.Combine(Application.persistentDataPath, "SaveData.json");

        if (File.Exists(path)) {
            File.Delete(path);
            Debug.Log("Datos eliminados");
        } else {
            Debug.Log("Archivo Save no encontrado. No se pudo borrar");
        }
    }

    public static void Load() {
        SaveData saveData = IOData.LoadSaveSystem(Path.Combine(Application.persistentDataPath, "SaveData.json"));

        if (saveData != null) {
            if (!_ignorePlayerSaves) {
                Money.instance.LoadMoney(saveData);
                LoadStatistics(saveData);
            }
        }
    }

    #region Load methods

    private static void LoadStatistics(SaveData saveData) {
        statistics = saveData.statistics;
    }

    private static void LoadUpgrades(SaveData saveData) {
        saveData.upgrades = new List<UpgradeData> {
        new UpgradeData(UpgradeType.HitDamage, MenuUpgrades.instance.hitLevel > 1),
        new UpgradeData(UpgradeType.DrillSpeed, DrillStats.drillSpeed > 0.5f),
        new UpgradeData(UpgradeType.HitArea, MenuUpgrades.instance.hitAreaLevel > 1),
        new UpgradeData(UpgradeType.HandDrill, HandDrillStats.isUnlocked),
        new UpgradeData(UpgradeType.HandDrillSpeed, HandDrillStats.handDrillSpeed > 5),
    };

        MenuUpgrades.instance.hitLevel
    }

    #endregion
}

[System.Serializable]
public class SaveData {
    public int dataVersion = 1; //NEVER delete this variable, even if it's not used, you will thank me 6 months later

    public float money;
    public PlayerStatistics statistics;
    public List<UpgradeData> upgrades = new List<UpgradeData> {
        new UpgradeData("HitDamage_1",UpgradeType.HitDamage, false),
        new UpgradeData("DrillSpeed_1",UpgradeType.DrillSpeed, false),
        new UpgradeData("HitArea_1",UpgradeType.HitArea, false),
        new UpgradeData("HandDrill",UpgradeType.HandDrill, false),
        new UpgradeData("HandDrillSpeed_1",UpgradeType.HandDrillSpeed, false),
    };
}

[System.Serializable]
public enum UpgradeType {
    HitDamage = 0,
    DrillSpeed = 1,
    HitArea = 2,
    HandDrill = 3,
    HandDrillSpeed = 4,
}

[System.Serializable]
public class UpgradeData {
    public string id;
    public UpgradeType type;
    //public int value;
    public bool isUnlocked;

    public UpgradeData(string id, UpgradeType type, bool isUnlocked) {
        this.id = id;
        this.type = type;
        this.isUnlocked = isUnlocked;
    }
}

[System.Serializable]
public class PlayerStatistics {
    public int statisticsVersion = 1; //NEVER delete this variable, even if it's not used, you will thank me 6 months later

    public int manualHits;
    public int automaticHits;
    public int collateralHits;
    public int totalHits;
    public int geodesGenerated;
    public int geodesBroken;
    public int geodesBrokenByCollateralDamage;
    public int geodesBrokenWithAutomaticGear;
    public int itemsSelled;
    public float moneyGained;
    public float moneySpent;

    public void Hit(bool manualHit) {
        totalHits++;

        if (manualHit) {
            manualHits++;
        } else {
            automaticHits++;
        }
    }

    public void CollateralHit() {
        collateralHits++;
        totalHits++;
    }
}