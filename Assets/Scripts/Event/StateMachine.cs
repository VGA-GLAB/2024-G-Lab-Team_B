using System.Collections.Generic;

namespace GameAutomaton
{// 日本語対応
    public class StateMachine<TOwner>
    {
        public abstract class StateBase
        {
            public StateMachine<TOwner> StateMachine { get; set; }
            protected TOwner Owner => StateMachine.Owner;
            /// <summary>ステートの遷移情報</summary>
            public readonly Dictionary<int, StateBase> Transitions = new Dictionary<int, StateBase>();

            /// <summary>このステートに入った時に行う処理</summary>
            public abstract void OnEnter();
            /// <summary>ステートが回っている間に行う処理</summary>
            public abstract void OnUpdate();
            /// <summary>このステートから出る時に行う処理</summary>
            public abstract void OnExit();
        }

        private readonly TOwner Owner = default;
        private StateBase _currentState = null;  // 現在のステート
        private readonly LinkedList<StateBase> _states = new LinkedList<StateBase>();  // ステートの定義

        public StateMachine(TOwner owner)
        {
            Owner = owner;
        }

        /// <summary>ステートを追加する</summary>
        /// <typeparam name="T">追加したいステート</typeparam>
        /// <returns>追加したステート</returns>
        private T Add<T>() where T : StateBase, new()
        {
            var newState = new T { StateMachine = this };
            _states.AddLast(newState);
            return newState;
        }

        /// <summary>ステートを取得する。取得できなかったら追加する</summary>
        /// <typeparam name="T">取得したいステート</typeparam>
        /// <returns>取得したステート</returns>
        private T GetOrAdd<T>() where T : StateBase, new()
        {
            foreach (var state in _states)  // 追加済みのステートを検索し、そのステートを返す
            {
                if (state is T result) return result;
            }
            return Add<T>();  // 追加したステートを返す
        }

        /// <summary>ステートの遷移情報を登録する</summary>
        /// <typeparam name="TFrom">遷移元のステート</typeparam>
        /// <typeparam name="TTo">遷移先のステート</typeparam>
        /// <param name="eventId">遷移する条件となるイベントのID</param>
        public void RegisterTransition<TFrom, TTo>(int eventId)
            where TFrom : StateBase, new()
            where TTo : StateBase, new()
        {
            var from = GetOrAdd<TFrom>();

            if (from.Transitions.ContainsKey(eventId))
            {
                throw new System.InvalidOperationException($"{eventId}は登録済みです");
            }
            var to = GetOrAdd<TTo>();
            from.Transitions.Add(eventId, to);
        }

        /// <summary>ステートの開始処理</summary>
        /// <typeparam name="T">最初に実行するステート</typeparam>
        public void Start<T>() where T : StateBase, new()
        {
            _currentState = GetOrAdd<T>();
            _currentState.OnEnter();
        }

        /// <summary>ステートの更新処理</summary>
        public void Update()
        {
            _currentState.OnUpdate();
        }

        /// <summary>指定されたイベントのステートに遷移する</summary>
        /// <param name="eventId">遷移させる条件となるイベントID</param>
        public void Transition(int eventId)
        {
            if (!_currentState.Transitions.TryGetValue(eventId, out StateBase nextState))
            {
                throw new System.InvalidOperationException($"{eventId}が見つかりませんでした");
            }
            _currentState.OnExit();
            _currentState = nextState;
            _currentState.OnEnter();
        }
    }
}
