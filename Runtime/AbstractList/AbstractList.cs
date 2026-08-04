using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.AbstractList
{
    public abstract class AbstractList<TItem, T> : MonoBehaviour where TItem : AbstractListItem<T>
    {
        [SerializeField] protected TItem _listItemPrefab;
        [SerializeField] private Transform _container;

        protected readonly List<TItem> _list = new();

        protected void RefreshList(IEnumerable<T> listData)
        {
            ClearItems();

            if (_listItemPrefab == null)
            {
                Debug.LogError($"{nameof(AbstractList<TItem, T>)}: {nameof(_listItemPrefab)} is not assigned.", this);
                return;
            }

            if (listData == null) return;

            var dataList = listData as IList<T> ?? listData.ToList();
            if (dataList.Count == 0) return;

            var parent = _container != null ? _container : _listItemPrefab.transform.parent;

            for (int i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                var listItem = Instantiate(_listItemPrefab, parent);
                listItem.gameObject.SetActive(true);
                listItem.Initialize(i, data);

                if (listItem.TryGetComponent(out Button button))
                {
                    button.onClick.AddListener(() => OnItemClicked(listItem));
                }

                _list.Add(listItem);
            }
        }

        public void ClearItems()
        {
            foreach (var item in _list)
            {
                if (item == null) continue;

                if (item.TryGetComponent(out Button button))
                {
                    button.onClick.RemoveAllListeners();
                }

                Destroy(item.gameObject);
            }

            _list.Clear();
        }

        protected abstract void OnItemClicked(TItem item);
    }
}