
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "UpgradeData")]
public class UpgradeData : ScriptableObject {
    public string id;
    public UpgradeType type;
    public bool isUnlocked;
    public int cost;
}
