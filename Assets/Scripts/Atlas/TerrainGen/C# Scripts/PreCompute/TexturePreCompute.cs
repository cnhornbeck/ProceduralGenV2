using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class TexturePreCompute : MonoBehaviour
{
    [SerializeField] List<TerrainLevelColor> colorList = new();
    public static readonly int numberOfColors = 200;
    public static NativeArray<Color> lookupTable = new();

    // Preprocess the colors
    void Awake()
    {
        PreprocessColors(colorList);
    }


    public void PreprocessColors(List<TerrainLevelColor> terrainLevels)
    {
        List<Color> lookupTableList = new();
        for (int i = 0; i <= numberOfColors; i++)
        {
            float previousMaxHeight = 0f;
            foreach (TerrainLevelColor level in terrainLevels)
            {
                if (level.MaxHeight >= i / (float)numberOfColors)
                {
                    float lerpFactor = (i / (float)numberOfColors - previousMaxHeight) / (level.MaxHeight - previousMaxHeight);
                    lookupTableList.Add(Color.Lerp(level.ColorStart, level.ColorEnd, level.Gradient.Evaluate(lerpFactor)));
                    break;
                }
                previousMaxHeight = level.MaxHeight;
            }
        }
        lookupTable = new NativeArray<Color>(lookupTableList.ToArray(), Allocator.Persistent);
    }

    // On Destroy, dispose of the lookup table
    void OnDestroy()
    {
        lookupTable.Dispose();
    }
}
