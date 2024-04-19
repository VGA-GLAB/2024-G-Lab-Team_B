using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

/// <summary>JSON形式のデータをセーブ、ロードします</summary>
public class RecordsDataSaver : MonoBehaviour
{
    /// <summary>データをロードします</summary>
    public RecordsDataList RoadData()
    {
        string filePath = Application.dataPath + "/PlayerRecordsSaveData.json"; // ファイルパス
        string dataString = "";
        
        try
        {
            //jsonDataを読み込む
            dataString = File.ReadAllText(filePath);

            StreamReader reader = new StreamReader(filePath);
            dataString = reader.ReadToEnd();
            reader.Close();
            return JsonUtility.FromJson<RecordsDataList>(dataString);
        }
        catch (FileNotFoundException)
        {
            // 無ければ生成する
            var data = new RecordsDataList();
            data = default;
            return data;
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

        StreamWriter writer = new StreamWriter(filePath, false);
        writer.WriteLine(jsonString);//JSONデータを書き込み
        writer.Flush();//バッファをクリアする
        writer.Close();//ファイルをクローズする
        Debug.Log("Game saved!"); // 保存されたことをログに出力
    }
}
