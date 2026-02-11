using System;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// 複数のGameObjectを一括制御するクラス
    /// </summary>
    public class ObjectGroup : MonoBehaviour
    {
        public GameObject[] Objects;

        /// <summary>
        /// 全てのオブジェクトの活性状態を一括設定
        /// </summary>
        public void SetActiveAll(bool isActive)
        {
            if (Objects == null) return;

            foreach (var obj in Objects)
            {
                if (obj != null && obj.activeSelf != isActive)
                {
                    obj.SetActive(isActive);
                }
            }
        }

        /// <summary>
        /// 全てのオブジェクトの状態を反転
        /// </summary>
        public void InvertActiveAll()
        {
            if (Objects == null) return;

            foreach (var obj in Objects)
            {
                if (obj != null)
                {
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }

        /// <summary>
        /// 特定のインデックスのオブジェクトだけを表示し、他を非表示にする（よくあるユースケース）
        /// </summary>
        public void ShowOnly(int index)
        {
            if (Objects == null) return;

            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] != null)
                {
                    Objects[i].SetActive(i == index);
                }
            }
        }
    }
}