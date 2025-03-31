using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AnimationPathComparer : EditorWindow
{
    public AnimationClip clip;
    public GameObject targetModel;

    [MenuItem("Tools/Compare Animation Paths")]
    public static void ShowWindow()
    {
        GetWindow<AnimationPathComparer>("Path Compare");
    }

    private void OnGUI()
    {
        GUILayout.Label("🐾 Animation Path Comparer", EditorStyles.boldLabel);

        clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", clip, typeof(AnimationClip), false);
        targetModel = (GameObject)EditorGUILayout.ObjectField("Target Model", targetModel, typeof(GameObject), true);

        if (GUILayout.Button("Compare"))
        {
            if (clip == null || targetModel == null)
            {
                Debug.LogWarning("请设置动画文件和目标模型！");
                return;
            }

            ComparePaths(clip, targetModel);
        }
    }

    private void ComparePaths(AnimationClip anim, GameObject target)
    {
        HashSet<string> animationPaths = new HashSet<string>();
        HashSet<string> modelPaths = new HashSet<string>();

        // 从动画中提取所有路径
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(anim);
        foreach (var binding in bindings)
        {
            animationPaths.Add(binding.path);
        }

        // 从目标模型中提取所有路径
        foreach (Transform t in target.GetComponentsInChildren<Transform>())
        {
            string path = GetPath(t, target.transform);
            modelPaths.Add(path);
        }

        Debug.Log($"🎬 动画路径数: {animationPaths.Count}, 🐶 模型骨骼数: {modelPaths.Count}");

        foreach (string path in animationPaths)
        {
            if (modelPaths.Contains(path))
                Debug.Log($"✅ 匹配: {path}");
            else
                Debug.LogWarning($"❌ 不存在于模型: {path}");
        }
    }

    private string GetPath(Transform current, Transform root)
    {
        if (current == root)
            return "";
        return GetPath(current.parent, root) + (GetPath(current.parent, root) == "" ? "" : "/") + current.name;
    }
}
