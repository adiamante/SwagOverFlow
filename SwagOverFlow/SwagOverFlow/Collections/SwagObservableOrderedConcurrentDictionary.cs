using SwagOverFlow.ViewModels;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SwagOverFlow.Collections
{
    public class SwagObservableOrderedConcurrentDictionary<TKey, TValue> : ViewModelBaseExtended, ICollection<KeyValuePair<Int32, KeyValuePair<TKey, TValue>>>, IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        #region Private/Protected Members
        ConcurrentDictionary<TKey, TValue> _dict = new ConcurrentDictionary<TKey, TValue>();
        ConcurrentDictionary<Int32, KeyValuePair<TKey, TValue>> _list = new ConcurrentDictionary<Int32, KeyValuePair<TKey, TValue>>();
        #endregion Private/Protected Members

        #region INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs changes)
        {
            CollectionChanged?.Invoke(this, changes);
        }
        #endregion INotifyCollectionChanged

        #region ICollection
        public void Add(KeyValuePair<int, KeyValuePair<TKey, TValue>> item)
        {
            Add(item.Value.Key, item.Value.Value);
        }

        public bool Contains(KeyValuePair<int, KeyValuePair<TKey, TValue>> item)
        {
            return ((ICollection<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).Contains(item);
        }

        public void CopyTo(KeyValuePair<int, KeyValuePair<TKey, TValue>>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<int, KeyValuePair<TKey, TValue>> item)
        {
            return ((ICollection<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).Remove(item);
        }

        public IEnumerator<KeyValuePair<int, KeyValuePair<TKey, TValue>>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
        #endregion ICollection

        #region IDictionary
        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dict).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dict).Values;

        public int Count => ((ICollection<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<int, KeyValuePair<TKey, TValue>>>)_list).IsReadOnly;

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

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>)_dict).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Remove(item);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dict).GetEnumerator();
        }
        #endregion IDictionary

        #region Methods
        public int Add(TKey key, TValue value)
        {
            if (_dict.ContainsKey(key))
            {
                _dict[key] = value;
                //FIX_THIS This probably hides some underlying issue
                Int32 index = IndexOfKey(key);
                if (index != -1)
                {
                    _list[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
            }
            else
            {
                _dict.TryAdd(key, value);
                _list.TryAdd(_list.Count, new KeyValuePair<TKey, TValue>(key, value));
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
                if (_dict.TryRemove(key, out TValue outDictVal))
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, _list[index].Value), index));
                    _list.TryRemove(index, out KeyValuePair<TKey, TValue> outListValue);
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

        public TValue Get(Int32 index)
        {
            return _list[index].Value;
        }

        #endregion Methods

    }
}
