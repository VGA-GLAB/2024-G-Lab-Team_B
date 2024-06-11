using System.IO;
using UnityEditor;
using UnityEngine;

public static class FileCreator
{
    /// <summary> 指定パスにファイルの生成を行う </summary>
    /// <typeparam name="T"> 保存するデータの型 </typeparam>
    /// <param name="path"> データテーブルのパス </param>
    public static void CreateFile<T>(string path, T saveData) where T : ScriptableObject
    {
        //既にファイルが存在したら何もしない
        if (File.Exists(path)) { Debug.Log("already exists"); return; }

        T newAsset = ScriptableObject.CreateInstance<T>();

        //初期値のコピー
        foreach (var field in typeof(T).GetFields())
        {
            field.SetValue(newAsset, field.GetValue(saveData));
        }
        AssetDatabase.CreateAsset(newAsset, path);

        AssetDatabase.SaveAssets();
    }
}
