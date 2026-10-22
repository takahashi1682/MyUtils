// ReSharper disable once RedundantUsingDirective
using UnityEngine;
using R3;
using UnityEngine.UI;
using MyUtils.Abstract;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyUtils.ApplicationUtils
{
    [RequireComponent(typeof(Button))]
    public class QuitGameButton : AbstractTargetBehaviour<Button>
    {
        protected override void Start()
        {
            base.Start();
            if (Target != null)
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