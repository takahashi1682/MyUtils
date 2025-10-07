using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneChangeDetector
{
    /// <summary>
    /// シーン切替に応じてボタンなどのオブジェクトを選択する機能
    /// </summary>
    public class CurrentSceneTransitionUISelecter : AbstractSceneChangeDetector
    {
        [Header("シーンアクティブ時に選択するオブジェクト")]
        [SerializeField] private GameObject _firstSelected;

        [Header("最後に選択したオブジェクトを再度アクティブ時に選択するか")]
        [SerializeField] private bool _isLastSelected = true;

        private GameObject _lastSelected;

        protected override void OnCurrentSceneExit(Scene scene)
        {
            if (_isLastSelected)
            {
                _lastSelected = EventSystem.current.currentSelectedGameObject;
            }
        }

        protected override void OnCurrentSceneEnter(Scene scene, LoadSceneMode mode)
        {
            var selectObj = _isLastSelected && _lastSelected != null ? _lastSelected : _firstSelected;
            EventSystem.current.SetSelectedGameObject(selectObj);
        }
    }
}