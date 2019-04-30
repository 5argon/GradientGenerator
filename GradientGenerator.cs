// GradientGenerator.cs
// 5argon / Sirawat Pitaksarit

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[CreateAssetMenu]
public class GradientGenerator : ScriptableObject
{
    [Serializable]
    public struct GradientSpecs
    {
        public AnimationCurve hProgression;
        public AnimationCurve sProgression;
        public AnimationCurve vProgression;
        public AnimationCurve aProgression;
        public AnimationCurve timeRemap;
    }

    public GradientSpecs[] gradientSpecs;
    public Gradient[] generatedGradients;

    [ContextMenu("Generate")]
    public void Generate()
    {
        var gradients = new List<Gradient>();
        for (int k = 0; k < gradientSpecs.Length; k++)
        {
            var spec = gradientSpecs[k];

            var eightRainbow = Enumerable.Range(0, 9).Select(x => Mathf.InverseLerp(0, 9, x)).Take(8).Select(x =>
            Color.HSVToRGB(spec.hProgression.Evaluate(x), spec.sProgression.Evaluate(x), spec.vProgression.Evaluate(x))).ToArray();

            for (int mode = 0; mode < 2; mode++)
            {
                GradientMode gm = (GradientMode)mode;
                var modePlus = gm == GradientMode.Fixed ? 1 : 0;
                for (int i = 1; i < 8; i++)
                {
                    var timeRange = Enumerable.Range(0, i + 1).Select(x => Mathf.InverseLerp(0, i + modePlus, x + modePlus)).ToArray();
                    var g = new Gradient();
                    g.mode = gm;
                    var colorKeys = new GradientColorKey[i + 1];
                    var alphaKeys = new GradientAlphaKey[i + 1];
                    for (int j = 0; j < i + 1; j++)
                    {
                        colorKeys[j].color = eightRainbow[j];
                        colorKeys[j].time = spec.timeRemap.Evaluate(timeRange[j]);
                        alphaKeys[j].alpha = spec.aProgression.Evaluate(timeRange[j]);
                        alphaKeys[j].time = spec.timeRemap.Evaluate(timeRange[j]);
                    }
                    g.SetKeys(colorKeys, alphaKeys);
                    gradients.Add(g);
                }
            }

        }
        generatedGradients = gradients.ToArray();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
