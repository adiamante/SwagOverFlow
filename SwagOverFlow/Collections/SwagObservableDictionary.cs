using SwagOverflow.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SwagOverFlow.Collections
{
    public class SwagObservableDictionary<TKey, TValue> : ViewModelBaseExtended, ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        #region Private/Protected Members
        Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        #endregion Private/Protected Members

        #region ICollection
        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dict).IsReadOnly;

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).GetEnumerator();
        }
        #endregion ICollection

        #region INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs changes)
        {
            CollectionChanged?.Invoke(this, changes);
        }
        #endregion INotifyCollectionChanged

        #region IDictionary
        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dict).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dict).Values;

        public TValue this[TKey key]
        {
            get { return _dict[key]; }
            set
            {
                if (!_dict.ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    _dict[key] = value;
                }
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>)_dict).ContainsKey(key);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>)_dict).TryGetValue(key, out value);
        }
        #endregion IDictionary

        #region Methods
        public int Add(TKey key, TValue value)
        {
            if (_dict.ContainsKey(key))
            {
                _dict[key] = value;
            }
            else
            {
                _dict.Add(key, value);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), Count - 1));
            return Count - 1;
        }

        public bool Remove(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            if (_dict.ContainsKey(key))
            {
                TValue val = _dict[key];
                if (_dict.Remove(key))
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, val), 0));
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _dict.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, -1));
        }
        #endregion Methods

    }
}
