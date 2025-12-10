using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.Grid.Unit
{
    public class UnitTransition : MonoBehaviour, IUnitEvent
    {
        [SerializeField] private SceneAsset _nextScene;
        [SerializeField] private Vector2Int _nextStartPoint;
        [SerializeField] private FadeSetting _fadeSetting;

        public async UniTask OnInteract(UnitIdentity unit)
        {
            if (!unit.TryGetComponent<UnitMover>(out var move)) return;

            await FadeScreenManager.BeginFadeOut(_fadeSetting);

            await SceneManager.LoadSceneAsync(_nextScene.name);

            move.SetPosition(Vector2Int.RoundToInt(_nextStartPoint));

            await FadeScreenManager.BeginFadeIn(_fadeSetting);

            // if (unit.TryGetComponent<UnitAnimation>(out var anim))
            // {
            //     anim.SetDirection(_startDirection);
            // }
        }
    }
}