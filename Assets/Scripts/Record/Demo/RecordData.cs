using System.Collections.Generic;
using UnityEngine;

namespace Recording
{
    namespace Master
    {
        public enum RecordMode
        {
            /// <summary> 記録 </summary>
            Record,
            /// <summary> 再現 </summary>
            Reproduce
        }

        public class RecordData : ScriptableObject
        {
            [HideInInspector]
            /// <summary> このデータが何番目のものか </summary>
            public int ID = -1;
            [HideInInspector]
            /// <summary> 最終的な位置 </summary>
            public Vector3 Position;
            [HideInInspector]
            /// <summary> 最終的な角度 </summary>
            public Quaternion Rotation;
            [HideInInspector]
            /// <summary> キャラクターがたどった経路 </summary>
            public List<Vector3> WayPoints;

            private int _targetIndex = 0;
            private Vector3 _targetPos = default;
            private Transform _target = default;

            protected bool IsTargetPosChange => (_target.position - _targetPos).sqrMagnitude <= 1f;

            public RecordData()
            {
                WayPoints = new();
            }

            #region Record
            /// <summary> 記録開始 </summary>
            public void RecordRun(int id, Transform transform)
            {
                ID = id;
                Position = transform.position;
                Rotation = transform.rotation;
                WayPoints.Add(Position);

                Debug.Log("Record Run");
            }

            /// <summary> 現在のデータを取得する </summary>
            public RecordData GetCurrentRecord() => this;

            /// <summary> データの更新 </summary>
            public void Apply(Transform transform)
            {
                Position = transform.position;
                Rotation = transform.rotation;
                WayPoints.Add(Position);

                Debug.Log("apply");
            }

            /// <summary> 最新データの保存 </summary>
            public void Close(RecordData recordData)
            {
                FileCreator.CreateFile($"Assets/Resources/RecordDataDemo {ID}.asset", recordData);
                Debug.Log("closed");
            }
            #endregion

            #region Reproduce
            /// <summary> 再現対象の設定 </summary>
            public void GetTransform(Transform target)
            {
                _target = target;
                _targetPos = WayPoints[0];
                _targetIndex = 0;
            }

            /// <summary> 保存されたデータの再現 </summary>
            public void Reproduce()
            {
                if (IsTargetPosChange)
                {
                    if (_targetIndex + 1 >= WayPoints.Count) { return; }

                    _targetIndex++;
                    var nextPos = WayPoints[_targetIndex];
                    _targetPos = nextPos;
                }
                _target.position = Vector3.Lerp(_target.position, _targetPos, Time.deltaTime);
            }
            #endregion

            /// <summary> IDから保存したデータを取得する </summary>
            public static bool TryGetRecordData(int id, out RecordData getData)
            {
                try
                {
                    var data = Resources.Load<RecordData>($"RecordDataDemo {id}.asset");
                    getData = data;
                    return true;
                }
                catch
                {
                    getData = null;
                    return false;
                }
            }
        }

        /// <summary> Animationパラメータの保存情報 </summary>
        public class AnimationRecordData : ScriptableObject
        {
            [HideInInspector]
            /// <summary> このデータが何番目のものか </summary>
            public int ID = -1;
            [HideInInspector]
            public float Speed;
            [HideInInspector]
            public Dictionary<string, bool> AnimationFlags;

            /// <summary> 記録開始 </summary>
            public void RecordRun(int id)
            {
                ID = id;
                Speed = 0f;

                AnimationFlags = new()
                {
                    { "Collision", false },
                    { "Crouch", false },
                    { "Change", false },
                    { "Select", false },
                    { "Decide", false },
                    { "GetItem", false },
                    { "UseItem", false },
                    { "UseKey", false },
                    { "UseAED", false },
                    { "Talk", false },
                };
                Debug.Log("Record Run");
            }

            /// <summary> 現在のデータを取得する </summary>
            public AnimationRecordData GetCurrentRecord() => this;

            public void Apply(float speed)
            {
                Speed = speed;
                ApplyLog();
            }

            /// <summary> データの更新 </summary>
            public void Apply(string param, bool flag)
            {
                try
                {
                    AnimationFlags[param] = flag;
                    ApplyLog();
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogError(
                        $"KeyNotFound : 指定された名称のフラグ「{param}」が見つかりませんでした。" +
                        $"名称が正しいか確認してください。");
                }
            }

            private void ApplyLog() => Debug.Log("apply");

            /// <summary> 最新データの保存 </summary>
            public void Close(AnimationRecordData recordData)
            {
                FileCreator.CreateFile($"Assets/Resources/AnimationRecordDataDemo {ID}.asset", recordData);
                Debug.Log("closed");
            }

            /// <summary> IDから保存したデータを取得する </summary>
            public static AnimationRecordData GetRecordData(int id)
            {
                try
                {
                    return Resources.Load<AnimationRecordData>($"AnimationRecordDataDemo {id}.asset");
                }
                catch { return null; }
            }
        }
    }
}
