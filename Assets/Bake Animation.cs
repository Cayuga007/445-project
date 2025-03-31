using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;
using System.IO;

public class AnimationBaker : MonoBehaviour
{
    public GameObject sourceDog;
    public GameObject targetDog;
    public AnimationClip sourceClip;
    public float sampleRate = 30f;

    private HashSet<string> printedPaths = new HashSet<string>();

    [ContextMenu("Bake Animation")]
    public void Bake()
    {
        if (sourceDog == null || targetDog == null || sourceClip == null)
        {
            Debug.LogError("❌ 请设置 Source Dog、Target Dog 和 Source Clip！");
            return;
        }

        sourceDog.SetActive(true);
        targetDog.SetActive(true);
        EditorUtility.SetDirty(sourceDog);
        EditorUtility.SetDirty(targetDog);

        string safeName = SanitizeFileName(sourceClip.name);
        string savePath = "Assets/BakedAnimations";
        string saveFile = $"{savePath}/Baked_{safeName}_ForShiba.anim";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        var bakedClip = new AnimationClip { frameRate = sampleRate };
        float length = sourceClip.length;
        int totalFrames = Mathf.CeilToInt(length * sampleRate);

        var curveDict = new Dictionary<string, AnimationCurve[]>();
        Transform[] bones = targetDog.GetComponentsInChildren<Transform>();

        foreach (var bone in bones)
        {
            string path = GetRelativePath(bone, targetDog.transform);
            curveDict[path] = new AnimationCurve[10];
            for (int i = 0; i < 10; i++) curveDict[path][i] = new AnimationCurve();

            if (!printedPaths.Contains(path))
            {
                Debug.Log("🎯 绑定路径: " + path);
                printedPaths.Add(path);
            }
        }

        for (int frame = 0; frame <= totalFrames; frame++)
        {
            float time = frame / sampleRate;

            // 使用 PlayableGraph 强制播放
            ApplyClipByPlayable(sourceDog, sourceClip, time);

            foreach (var bone in bones)
            {
                string path = GetRelativePath(bone, targetDog.transform);
                if (!curveDict.ContainsKey(path)) continue;

                Vector3 pos = bone.localPosition;
                Quaternion rot = bone.localRotation;
                Vector3 scale = bone.localScale;

                curveDict[path][0].AddKey(time, pos.x);
                curveDict[path][1].AddKey(time, pos.y);
                curveDict[path][2].AddKey(time, pos.z);
                curveDict[path][3].AddKey(time, rot.x);
                curveDict[path][4].AddKey(time, rot.y);
                curveDict[path][5].AddKey(time, rot.z);
                curveDict[path][6].AddKey(time, rot.w);
                curveDict[path][7].AddKey(time, scale.x);
                curveDict[path][8].AddKey(time, scale.y);
                curveDict[path][9].AddKey(time, scale.z);
            }

            if (frame % 10 == 0)
                EditorUtility.DisplayProgressBar("Baking Animation", $"处理帧 {frame}/{totalFrames}", (float)frame / totalFrames);
        }

        EditorUtility.ClearProgressBar();

        foreach (var kvp in curveDict)
        {
            string path = kvp.Key;
            var curves = kvp.Value;

            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.x"), curves[0]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.y"), curves[1]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.z"), curves[2]);

            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.x"), curves[3]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.y"), curves[4]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.z"), curves[5]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.w"), curves[6]);

            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalScale.x"), curves[7]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalScale.y"), curves[8]);
            AnimationUtility.SetEditorCurve(bakedClip, EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalScale.z"), curves[9]);
        }

        AssetDatabase.CreateAsset(bakedClip, saveFile);
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ 烘焙成功！文件保存于：{saveFile}");
    }

    void ApplyClipByPlayable(GameObject target, AnimationClip clip, float time)
    {
        var graph = PlayableGraph.Create("TempAnimGraph");
        var output = AnimationPlayableOutput.Create(graph, "AnimOutput", target.GetComponent<Animator>());
        var playable = AnimationClipPlayable.Create(graph, clip);

        output.SetSourcePlayable(playable);
        playable.SetTime(time);
        playable.SetTime(time); // Unity bug workaround
        playable.SetApplyFootIK(false);
        playable.SetApplyPlayableIK(false);
        graph.Evaluate();
        graph.Destroy();
    }

    string GetRelativePath(Transform current, Transform root)
    {
        if (current == root) return "";
        return GetRelativePath(current.parent, root) + (GetRelativePath(current.parent, root) == "" ? "" : "/") + current.name;
    }

    string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}

