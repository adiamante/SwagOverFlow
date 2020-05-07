using SwagOverflow.ViewModels;
using SwagOverflow.WPF.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SwagOverflow.WPF.Collections
{
    public class SwagObservableOrderedDictionary<TKey, TValue> : ViewModelBaseExtended, ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        #region Private/Protected Members
        Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        List<KeyValuePair<TKey, TValue>> _list = new List<KeyValuePair<TKey, TValue>>();
        #endregion Private/Protected Members

        #region ICollection
        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_list).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_list).IsReadOnly;

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_list).Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_list).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_list).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_list).GetEnumerator();
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

        public KeyValuePair<TKey, TValue> this[int index] { get => ((IList<KeyValuePair<TKey, TValue>>)_list)[index]; set => ((IList<KeyValuePair<TKey, TValue>>)_list)[index] = value; }
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

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
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
                _list[IndexOfKey(key)] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                _dict.Add(key, value);
                _list.Add(new KeyValuePair<TKey, TValue>(key, value));
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), Count - 1));
            return Count - 1;
        }

        public bool Remove(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            int index = IndexOfKey(key);
            if (index >= 0)
            {
                if (_dict.Remove(key))
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, _list[index].Value), index));
                    _list.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            _dict.Clear();
            _list.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, -1));
        }

        public int IndexOfKey(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            for (int index = 0; index < _list.Count; index++)
            {
                KeyValuePair<TKey, TValue> entry = _list[index];
                TKey next = entry.Key;
                if (next.Equals(key))
                {
                    return index;
                }
            }

            return -1;
        }
        #endregion Methods

    }
}
