using System.Collections.Generic;
using UnityEngine;

namespace Recording
{
    namespace Demo
    {
        public enum RecordMode
        {
            /// <summary> 記録 </summary>
            Record,
            /// <summary> 再現 </summary>
            Reproduce
        }

        public class RecordDataDemo : ScriptableObject
        {
            /// <summary> このデータが何番目のものか </summary>
            public int ID = -1;
            /// <summary> 最終的な位置 </summary>
            public Vector3 Position;
            /// <summary> 最終的な角度 </summary>
            public Quaternion Rotation;
            /// <summary> キャラクターがたどった経路 </summary>
            public List<Vector3> WayPoints;

            private int _targetIndex = 0;
            private Vector3 _targetPos = default;
            private Transform _target = default;

            protected bool IsTargetPosChange => (_target.position - _targetPos).sqrMagnitude <= 1f;

            public RecordDataDemo()
            {
                WayPoints = new();
            }

            #region Record
            /// <summary> 記録開始 </summary>
            public void RecordRun(Transform transform)
            {
                ID = 0;
                Position = transform.position;
                Rotation = transform.rotation;
                WayPoints.Add(Position);

                Debug.Log("Record Run");
            }

            /// <summary> 現在のデータを取得する </summary>
            public RecordDataDemo GetCurrentRecord() => this;

            /// <summary> データの更新 </summary>
            public void Apply(Transform transform)
            {
                Position = transform.position;
                Rotation = transform.rotation;
                WayPoints.Add(Position);

                Debug.Log("apply");
            }

            /// <summary> 最新データの保存 </summary>
            public void Close(RecordDataDemo recordData)
            {
                FileCreator.CreateFile("Assets/Resources/RecordDataDemo.asset", recordData);
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
        }

        /// <summary> Animationパラメータの保存情報 </summary>
        public class AnimationRecordDataDemo : ScriptableObject
        {
            public float Speed { get; private set; }
            public Dictionary<string, bool> AnimationFlags { get; private set; }

            /// <summary> 記録開始 </summary>
            public void RecordRun()
            {
                Speed = 0f;

                AnimationFlags = new()
                {
                    { "Collision", false },
                    { "Crouch", false },
                    { "Change", false },
                    { "SelectAvility", false },
                    { "UseAvility", false },
                    { "GetItem", false },
                    { "UseItem", false },
                    { "UseKey", false },
                    { "UseAED", false },
                    { "Talk", false },
                };
                Debug.Log("Record Run");
            }

            /// <summary> 現在のデータを取得する </summary>
            public AnimationRecordDataDemo GetCurrentRecord() => this;

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
            public void Close(AnimationRecordDataDemo recordData)
            {
                FileCreator.CreateFile("Assets/Resources/AnimationRecordDataDemo.asset", recordData);
                Debug.Log("closed");
            }
        }
    }
}
