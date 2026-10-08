using System;
using System.Collections.Generic;
using R3;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    /// <summary>
    /// 毎フレームの処理の段階。上から順番に実行される
    /// (入力→Action(AIの意思決定含む)→CameraPrepare→CameraApply→Animation→UI)。
    /// CameraPrepareは各UnitがCameraTarget等を書き込む段階、CameraApplyはカメラが
    /// それを読んで実際のカメラTransformを確定させる段階。この2つを分けているのは、
    /// 「書き込みが先、確定が後」という順序を暗黙のSubscribe順に頼らず保証するため。
    /// </summary>
    public enum EUpdatePhase
    {
        Input, // 入力
        Default, // デフォルトの更新処理
        CameraPrepare, // カメラの操作
        CameraApply, // カメラの更新
        Animation, // アニメーションの更新
        UI // UIの更新
    }

    /// <summary>
    /// Update()の代わりにこれを使う。実行順が保証されるので、
    /// 「どのスクリプトが先に動くか」を気にしなくてよくなる。
    /// </summary>
    public interface IUpdateObservable
    {
        Observable<Unit> OnUpdate(EUpdatePhase phase);
    }

    /// <summary>
    /// 毎フレーム、フェーズの順番に更新を通知する。
    /// VContainerのITickableとして、MonoBehaviourのUpdateより前に実行される。
    /// </summary>
    public class UpdateScheduler : ITickable, IUpdateObservable, IDisposable
    {
        private readonly SortedDictionary<EUpdatePhase, Subject<Unit>> _updateStreams = new();

        public void Tick()
        {
            foreach (var pair in _updateStreams)
            {
                pair.Value.OnNext(Unit.Default);
            }
        }

        public Observable<Unit> OnUpdate(EUpdatePhase phase)
        {
            if (_updateStreams.TryGetValue(phase, out var stream))
            {
                return stream;
            }

            stream = new Subject<Unit>();
            _updateStreams[phase] = stream;
            return stream;
        }

        public void Dispose()
        {
            foreach (var stream in _updateStreams.Values)
            {
                stream.Dispose();
            }

            _updateStreams.Clear();
        }
    }
}