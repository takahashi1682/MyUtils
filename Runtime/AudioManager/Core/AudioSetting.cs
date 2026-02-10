using System;
using UnityEngine;

namespace MyUtils.AudioManager.Core
{
    public enum SpatialBlendType
    {
        _2D = 0,
        _3D = 1,
    }

    [Serializable]
    public class AudioSetting : ISerializationCallbackReceiver
    {
        public AudioClip Clip;
        [Tooltip("ミュートするかどうか")]
        [SerializeField] public bool Mute;
        [Tooltip("エフェクトをバイパスするかどうか")]
        [SerializeField] public bool BypassEffects;
        [Tooltip("リスナーエフェクトをバイパスするかどうか")]
        [SerializeField] public bool BypassListenerEffects;
        [Tooltip("リバーブゾーンをバイパスするかどうか")]
        [SerializeField] public bool BypassReverbZones;
        [Tooltip("ループ再生するかどうか")]
        [SerializeField] public bool IsLoop;
        [Tooltip("優先度 0=最高, 256=最低")]
        [Range(0, 256)]
        [SerializeField] public int Priority = 128;
        [Tooltip("音量 0=無音, 1=最大")]
        [Range(0, 1)]
        [SerializeField] public float Volume = 1f;
        [Tooltip("ピッチ調整")]
        [Range(-3f, 3f)]
        [SerializeField] public float Pitch = 1f;
        [Tooltip("左右のパンニング -1=左, 0=中央, 1=右")]
        [Range(-1, 1)]
        [SerializeField] public float PanStereo;
        [Tooltip("空間化の度合い")]
        [SerializeField] public SpatialBlendType SpatialBlendType;
        [Tooltip("リバーブゾーンミックス 0=リバーブなし, 1=フルリバーブ")]
        [Range(0f, 1.1f)]
        [SerializeField] public float ReverbZoneMix = 1f;
        [Tooltip("ドップラー効果の強さ")]
        [Range(0f, 5f)]
        [SerializeField] public float DopplerLevel = 1f;
        [Tooltip("音の広がり具合 (単位: 度)")]
        [Range(0f, 360f)]
        [SerializeField] public float Spread;
        [Tooltip("音の減衰方法")]
        [SerializeField] public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
        [Tooltip("最小距離")]
        [SerializeField] public float MinDistance = 1f;
        [Tooltip("最大距離")]
        [SerializeField] public float MaxDistance = 50f;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            // デシリアライズ後にデフォルト値を適用(配列の要素追加時などに対応)
            ApplyDefaultValues();
        }

        private void ApplyDefaultValues()
        {
            Priority = 128;
            Volume = 1f;
            Pitch = 1f;
            PanStereo = 0f;
            SpatialBlendType = SpatialBlendType._2D;
            ReverbZoneMix = 1f;
            DopplerLevel = 1f;
            Spread = 0f;
            RolloffMode = AudioRolloffMode.Logarithmic;
            MinDistance = 1f;
            MaxDistance = 50f;
        }
    }
}