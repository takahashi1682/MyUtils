// ReSharper disable once RedundantUsingDirective
using UnityEngine;
using R3;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.ApplicationUtils
{
    public class QuitGameButton : AbstractTargetBehaviour<Button>
    {
        protected override void Start()
        {
            base.Start();
            if (TryGetComponent(out Target))
            {
                Target.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                    })
                    .AddTo(this);
            }
        }
    }
}