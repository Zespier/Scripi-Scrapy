
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "UpgradeData")]
public class UpgradeDataSO : ScriptableObject {
    public string id;
    public new string name; // Visual Studio recomended me hiding the base "name" value with new, but I honestly don't fucking know what it does
    public string details;
    public long cost;
    public UpgradeType type;
    public bool isUpgraded;
    public Sprite sprite;
    public float mpsMultiplierEstimated = 1;
    public float mpsAddedEstimated;

    [Header("EconomyEditorVariables")]
    public Vector2 savedPosition;
}
