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
        saveData.upgrades = MenuUpgrades.instance.upgrades;
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
                LoadUpgrades(saveData);
            }
        }
    }

    #region Load methods

    private static void LoadStatistics(SaveData saveData) {
        statistics = saveData.statistics;
    }

    private static void LoadUpgrades(SaveData saveData) {

        for (int i = 0; i < saveData.upgrades.Count; i++) {
            for (int j = 0; j < MenuUpgrades.instance.upgrades.Count; j++) {
                if (saveData.upgrades[i] == null) {
                    Debug.LogError("Some data got corrupted => index of upgrade:  " + i);

                } else {
                    if (saveData.upgrades[i].id == MenuUpgrades.instance.upgrades[j].id) {
                        MenuUpgrades.instance.upgrades[j].isUpgraded = saveData.upgrades[i].isUpgraded;
                        MenuUpgrades.instance.ApplyUpgradeEffect(saveData.upgrades[i]);
                    }
                }
            }
        }
    }

    #endregion
}

[System.Serializable]
public class SaveData {
    public int dataVersion = 1; //NEVER delete this variable, even if it's not used, you will thank me 6 months later

    public long money;
    public PlayerStatistics statistics;
    public List<UpgradeDataSO> upgrades = new();
}

[System.Serializable]
public enum UpgradeType {
    HitDamage = 0,
    DrillSpeed = 1,
    HitArea = 2,
    HandDrill = 3,
    HandDrillSpeed = 4,
    BetterGeodes = 5,
    ExtraInventorySlots = 6,
    BiggerStacks = 7,
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
    public long moneyGained;
    public long moneySpent;

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