using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils.TweenUtils
{
    /// <summary>
    /// DOTweenに依存しない軽量Tweenエンジン。UniTaskの毎フレームループで駆動する。
    /// </summary>
    public sealed class Tween
    {
        private readonly float _duration;
        private readonly Action<float, float> _onProgress; // (easedT, rawT)

        private float _delay;
        private Ease _ease = Ease.Linear;
        private int _loops = 1;
        private LoopType _loopType = LoopType.Restart;
        private bool _inverted;
        private bool _autoKill = true;
        private bool _isRelative;

        private Action _onComplete;
        private Action _onKill;
        private Action _onUpdate;

        private float _elapsed; // 現在のループパス内での経過時間。delay中は負値。
        private int _direction = 1; // 1:順再生 -1:逆再生
        private bool _isPlaying;
        private bool _isKilled;
        private bool _loopRunning;
        private CancellationTokenSource _cts;

        public bool IsRelative => _isRelative;

        private Tween(float duration, Action<float, float> onProgress)
        {
            _duration = Mathf.Max(0f, duration);
            _onProgress = onProgress;
            _elapsed = -_delay;
        }

        public static Tween Create(float duration, Action<float, float> onProgress) => new(duration, onProgress);

        public static Tween Create(float duration, Action<float> onEasedProgress)
            => Create(duration, (eased, _) => onEasedProgress?.Invoke(eased));

        /// <summary>DOVirtual.Float 相当。start から end へ duration 秒かけて補間する。</summary>
        public static Tween Value(float from, float to, float duration, Action<float> onUpdate)
            => Create(duration, eased => onUpdate?.Invoke(Mathf.LerpUnclamped(from, to, eased)));

        public static Tween To(Func<float> getter, Action<float> setter, float endValue, float duration)
        {
            float start = getter != null ? getter() : 0f;
            return Value(start, endValue, duration, setter);
        }

        public static Tween To(Func<int> getter, Action<int> setter, int endValue, float duration)
        {
            float start = getter != null ? getter() : 0;
            return Create(duration,
                eased => setter?.Invoke(Mathf.RoundToInt(Mathf.LerpUnclamped(start, endValue, eased))));
        }

        public static Tween To(Func<long> getter, Action<long> setter, long endValue, float duration)
        {
            double start = getter != null ? getter() : 0L;
            double end = endValue;
            return Create(duration, eased => setter?.Invoke((long)Math.Round(start + (end - start) * eased)));
        }

        public static Tween To(Func<double> getter, Action<double> setter, double endValue, float duration)
        {
            double start = getter != null ? getter() : 0d;
            return Create(duration, eased => setter?.Invoke(start + (endValue - start) * eased));
        }

        // --- フルーエント設定 ---

        public Tween SetDelay(float delay)
        {
            _delay = Mathf.Max(0f, delay);
            _elapsed = -_delay;
            return this;
        }

        public Tween SetEase(Ease ease) { _ease = ease; return this; }

        public Tween SetLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            _loops = loops;
            _loopType = loopType;
            return this;
        }

        public Tween SetInverted(bool inverted) { _inverted = inverted; return this; }
        public Tween SetAutoKill(bool autoKill) { _autoKill = autoKill; return this; }
        public Tween SetRelative(bool relative = true) { _isRelative = relative; return this; }

        public Tween OnComplete(Action callback) { _onComplete += callback; return this; }
        public Tween OnKill(Action callback) { _onKill += callback; return this; }
        public Tween OnUpdate(Action callback) { _onUpdate += callback; return this; }

        public Tween SetLink(GameObject target) => SetLink(target, LinkBehaviour.KillOnDestroy);

        public Tween SetLink(GameObject target, LinkBehaviour behaviour)
        {
            if (target != null && behaviour != LinkBehaviour.None)
                TweenGameObjectLink.Attach(target, this, behaviour);
            return this;
        }

        // --- 再生制御 ---

        public void Play()
        {
            if (_isKilled) return;
            if (_direction == 0) _direction = 1;
            _isPlaying = true;
            EnsureLoopRunning();
        }

        public void PlayBackwards()
        {
            if (_isKilled) return;
            _direction = -1;
            _isPlaying = true;
            EnsureLoopRunning();
        }

        public void Pause() => _isPlaying = false;

        public void Restart()
        {
            if (_isKilled) return;
            _elapsed = -_delay;
            _direction = 1;
            Apply(0f);
            _isPlaying = true;
            EnsureLoopRunning();
        }

        public void Rewind()
        {
            if (_isKilled) return;
            _elapsed = -_delay;
            _direction = 1;
            _isPlaying = false;
            Apply(0f);
        }

        public void Complete()
        {
            if (_isKilled) return;
            _elapsed = _duration;
            Apply(1f);
            _isPlaying = false;
            _onComplete?.Invoke();
            if (_autoKill) Kill();
        }

        /// <summary>現在の再生方向を反転させる（順再生中なら逆再生に、逆再生中なら順再生に）。</summary>
        public void Flip() => _direction *= -1;

        public void Kill(bool complete = false)
        {
            if (_isKilled) return;
            if (complete)
            {
                _elapsed = _duration;
                Apply(1f);
                _onComplete?.Invoke();
            }

            _isKilled = true;
            _isPlaying = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _onKill?.Invoke();
        }

        private void EnsureLoopRunning()
        {
            if (_loopRunning || _isKilled) return;
            _loopRunning = true;
            _cts ??= new CancellationTokenSource();
            RunLoopAsync(_cts.Token);
        }

        private async UniTaskVoid RunLoopAsync(CancellationToken token)
        {
            try
            {
                while (!_isKilled)
                {
                    if (!_isPlaying)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                        continue;
                    }

                    bool finished = StepFrame(Time.deltaTime);

                    if (finished)
                    {
                        _isPlaying = false;
                        _onComplete?.Invoke();
                        if (_autoKill)
                        {
                            Kill();
                            break;
                        }
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Kill() によるキャンセル
            }
            finally
            {
                _loopRunning = false;
            }
        }

        /// <summary>経過時間を1フレーム分進め、ループ境界処理を行う。全ループ完了時に true を返す。</summary>
        private bool StepFrame(float dt)
        {
            if (_elapsed < 0f)
            {
                // Delay消化中。方向に関わらず時間経過のみを消費し、ループ境界処理は行わない。
                _elapsed += dt;
                if (_elapsed < 0f) return false;
                // Delayを消化しきった。オーバーフロー分(_elapsed >= 0)はそのまま本編の経過として扱い、
                // このフレームでは重複してdt*directionを加算しない。
            }
            else
            {
                _elapsed += dt * _direction;
            }

            if (_duration <= 0f)
            {
                Apply(_direction >= 0 ? 1f : 0f);
                return _loops >= 0;
            }

            while (true)
            {
                if (_elapsed > _duration)
                {
                    float overflow = _elapsed - _duration;
                    if (IsLoopsExhausted())
                    {
                        Apply(1f);
                        return true;
                    }

                    if (_loopType == LoopType.Yoyo)
                    {
                        _elapsed = _duration - overflow;
                        _direction = -1;
                    }
                    else
                    {
                        _elapsed = overflow;
                    }

                    continue;
                }

                if (_elapsed < 0f)
                {
                    float overflow = -_elapsed;
                    if (IsLoopsExhausted())
                    {
                        Apply(0f);
                        return true;
                    }

                    if (_loopType == LoopType.Yoyo)
                    {
                        _elapsed = overflow;
                        _direction = 1;
                    }
                    else
                    {
                        _elapsed = _duration - overflow;
                    }

                    continue;
                }

                break;
            }

            Apply(_elapsed / _duration);
            return false;
        }

        private int _completedLoops;

        private bool IsLoopsExhausted()
        {
            if (_loops < 0) return false; // 無限ループ
            _completedLoops++;
            return _completedLoops >= _loops;
        }

        private void Apply(float rawT)
        {
            rawT = Mathf.Clamp01(rawT);
            float effectiveRaw = _inverted ? 1f - rawT : rawT;
            float eased = Easing.Evaluate(_ease, effectiveRaw);
            _onProgress?.Invoke(eased, effectiveRaw);
            _onUpdate?.Invoke();
        }
    }
}
