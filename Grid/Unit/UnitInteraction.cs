using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TNRD;
using UnityEngine;

namespace MyUtils.Grid
{
    public interface IUnitEvent
    {
        UniTask OnInteract();
    }

    public class UnitInteraction : MonoBehaviour
    {
        [SerializeField] private List<SerializableInterface<IUnitEvent>> _unitEvent;

        public UniTask OnInteract()
        {
            var tasks = Enumerable.Select(_unitEvent, unitEvent => unitEvent.Value.OnInteract()).ToList();
            return UniTask.WhenAll(tasks);
        }
    }
}