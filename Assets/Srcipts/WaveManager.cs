using System.Collections.Generic;
//using UnityEditor.Overlays;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Manager")]
public class WaveManager : ScriptableObject
{
    public List<WaveData> waves = new();
}

