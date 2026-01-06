using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using MyUtils.Grid.Map;
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
            if (!unit.TryGetComponent<UnitMover>(out var move)) return;

            await FadeScreenManager.BeginFadeOut(_fadeSetting);

            await SceneManager.LoadSceneAsync(_nextMapData.name);

            move.SetPosition(Vector2Int.RoundToInt(_nextStartPoint));

            MapLoader.Singleton.Load(_nextMapData.GridData);

            await FadeScreenManager.BeginFadeIn(_fadeSetting);
        }
    }
}