using UnityEngine;

[CreateAssetMenu(fileName = "Research", menuName = "Data/Research", order = 1)]
class ResearchItem : ScriptableObject
{
    public int ID;
    public string Group = "default";
    public ResearchItem ResearchRequired;
    public Hologram DisplayHologram;
    public float TotalSeconds = 600;
}