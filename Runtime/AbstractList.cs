using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils
{
    public abstract class AbstractList<T> : MonoBehaviour
    {
        [SerializeField] protected AbstractListItem<T> _listItemPrefab;
        [SerializeField] private Transform _container;

        protected void CreateListItems(List<T> listData)
        {
            var container = _container != null ? _container : _listItemPrefab.transform.parent;

            foreach (var data in listData)
            {
                var listItem = Instantiate(_listItemPrefab, container);
                listItem.Initialize(data);
                listItem.GetComponent<Button>().onClick.AddListener(() => OnClickItem(data));
            }

            _listItemPrefab.gameObject.SetActive(false);
        }

        protected abstract void OnClickItem(T data);
    }
}