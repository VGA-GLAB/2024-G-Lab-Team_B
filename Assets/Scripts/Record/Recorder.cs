using UnityEngine;

namespace Recording
{
    namespace Master
    {
        public class Recorder : MonoBehaviour
        {
            [Tooltip("何フレームごとに記録を行うか")]
            [Range(2, 10)]
            [SerializeField]
            private int _recordFrame = 5;
            [SerializeField]
            private RecordData _recordData = default;

            private int _frameCounter = 0;
            /// <summary> 記録対象の状態（新規に記録するか、記録を再現するか） </summary>
            private RecordMode _recordMode = RecordMode.Record;

            public void Initialize(int id)
            {
                if (!RecordData.TryGetRecordData(id, out _recordData))
                {
                    _recordData = new();
                    _recordMode = RecordMode.Record;
                    _recordData.RecordRun(id, transform);
                }
                else
                {
                    _recordData.GetTransform(transform);
                    _recordMode = RecordMode.Reproduce;
                }
            }

            private void Update()
            {
                if ( _recordMode == RecordMode.Record) { return; }
                _recordData.Reproduce();
            }

            private void FixedUpdate()
            {
                _frameCounter++;
                if (_frameCounter % _recordFrame == 0) { _recordData.Apply(transform); }
            }

            private void OnDisable()
            {
                if (_recordMode == RecordMode.Reproduce) { return; }

                _recordData.Close(_recordData);
            }
        }
    }
}
