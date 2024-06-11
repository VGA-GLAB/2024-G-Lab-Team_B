using System.IO;
using UnityEngine;

/// <summary>JSON形式のデータをセーブ、ロードします</summary>
public class RecordsDataSaver : MonoBehaviour
{
    /// <summary>データをロードします</summary>
    public RecordsDataList RoadData()
    {
        string filePath = Application.dataPath + "/PlayerRecordsSaveData.json"; // ファイルパス

        try
        {
            //jsonDataを読み込む
            var dataString = File.ReadAllText(filePath);

            var reader = new StreamReader(filePath);
            dataString = reader.ReadToEnd();
            reader.Close();
            return JsonUtility.FromJson<RecordsDataList>(dataString);
        }
        catch (FileNotFoundException)
        {
            Debug.Log("new SaveData instance created");
            return new RecordsDataList();
        }
    }

    /// <summary>データをセーブします</summary>
    public void SaveData()
    {
        string filePath = Application.dataPath + "/PlayerRecordsSaveData.json"; // ファイルパス

        // PlayerMoveRecorderにあるRecordsDataListを取得します
        PlayerMoveRecorder recorder = FindFirstObjectByType<PlayerMoveRecorder>();
        recorder.AddRecord();
        RecordsDataList data = recorder.GetRecordsDataList;
        data.ProfessorDeadFlag = FindFirstObjectByType<ProfessorDeadOrAlive>().IsDead;
        data.AssociateProfessorDeadFlag = FindFirstObjectByType<AssociateProfessorDeadOrAlive>().IsDead;

        // JSON形式に変換して保存
        string jsonString = JsonUtility.ToJson(data);

        var writer = new StreamWriter(filePath, false);
        writer.WriteLine(jsonString);//JSONデータを書き込み
        writer.Flush();//バッファをクリアする
        writer.Close();//ファイルをクローズする
        Debug.Log("Game saved!"); // 保存されたことをログに出力
    }
}
