using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using MyUtils.Grid.Map;
using MyUtils.Grid.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.Grid
{
    public class UnitTransition : MonoBehaviour, IUnitEvent
    {
        [SerializeField] private MapData _nextMapData;
        [SerializeField] private Vector2Int _nextStartPoint;
        [SerializeField] private FadeSetting _fadeSetting;

        public async UniTask OnInteract(UnitIdentity unit)
        {
            if (!unit.transform.root.TryGetComponent<PlayerCore>(out var player)) return;

            await FadeScreenManager.BeginFadeOut(_fadeSetting);
            await SceneManager.LoadSceneAsync(_nextMapData.Scene.SceneName);

            player.UnitMover.SetPosition(Vector2Int.RoundToInt(_nextStartPoint));

            await FadeScreenManager.BeginFadeIn(_fadeSetting);
        }
    }
}