using UnityEngine;

namespace Recording
{
    namespace Demo
    {
        public class Recorder : MonoBehaviour
        {
            [SerializeField]
            private RecordDataDemo _recordData = default;
            [Tooltip("記録対象の状態（新規に記録するか、記録を再現するか）")]
            [SerializeField]
            private RecordMode _recordMode = RecordMode.Record;

            private void Start()
            {
                if (_recordData == null)
                {
                    _recordData = new();
                    _recordMode = RecordMode.Record;
                    _recordData.RecordRun(transform);
                }
                else
                {
                    _recordData.GetTransform(transform);
                    _recordMode = RecordMode.Reproduce;
                }
            }

            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Return) && _recordMode == RecordMode.Record) { _recordData.Apply(transform); }
                else if (_recordMode == RecordMode.Reproduce) { _recordData.Reproduce(); }
            }

            private void OnDisable()
            {
                if (_recordMode == RecordMode.Reproduce) { return; }

                _recordData.Close(_recordData);
            }
        }
    }
}
