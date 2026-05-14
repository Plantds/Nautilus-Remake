using System.Collections.Generic;
using UnityEngine;

public class CS_BakeSwitch : CS_ElctricalBaseScript
{
    [SerializeField] private Texture2D[] offLightMapDir, offLightMapColor; // Dark
    [SerializeField] private Texture2D[] onLightMapDir, onLightMapColor;   // Bright

    private LightmapData[] offLightMap, onLightMap; // Dark, Bright

    private void Start()
    {
        List<LightmapData> fLightMap = new List<LightmapData>();

        for(int i = 0; i < offLightMapDir.Length; i++) // Dark
        {
            LightmapData lmData = new LightmapData();

            lmData.lightmapDir = offLightMapDir[i];
            lmData.lightmapColor = offLightMapColor[i];

            fLightMap.Add(lmData);
        }

        offLightMap = fLightMap.ToArray();

        List<LightmapData> nLightMap = new List<LightmapData>();

        for(int i = 0; i < onLightMapDir.Length; i++) // Bright
        {
            LightmapData lmData = new LightmapData();

            lmData.lightmapDir = onLightMapDir[i];
            lmData.lightmapColor = onLightMapColor[i];

            nLightMap[i] = lmData;
        }

        onLightMap = nLightMap.ToArray();
    }

    public override void TurnOn(bool flash)
    {
        LightmapSettings.lightmaps = onLightMap;
    }

    public override void TurnOff(bool flash)
    {
        LightmapSettings.lightmaps = offLightMap;
    }
}
