#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ScriptableObjectの作成とデータ取得処理
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Slack Sender/Create ScriptableObject", order = 1)]
public class SendWebhookScriptableObject : ScriptableObject
{
    public string sendName;
    public string url;
}

public class ScriptableObjectControl
{
    public static IEnumerable<SendWebhookScriptableObject> GetScriptableObject()
    {
        var data = AssetDatabase.FindAssets("t:ScriptableObject") // プロジェクトに存在する全ScriptableObjectのGUIDを取得
         .Select(guid => AssetDatabase.GUIDToAssetPath(guid)) // GUIDをパスに変換
         .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SendWebhookScriptableObject))) // パスからPermanentDataの取得を試みる
         .Where(obj => obj != null).Cast<SendWebhookScriptableObject>(); // null要素は取り除く
        return data;
    }
}
#endif
