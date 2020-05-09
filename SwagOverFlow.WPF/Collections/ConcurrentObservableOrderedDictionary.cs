using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace SwagOverFlow.WPF.Collections
{

	/// <summary>
	/// Represents a generic collection of key/value pairs that are ordered independently of the key and value.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
	public class ConcurrentObservableOrderedDictionary<TKey, TValue> : ConcurrentObservableDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private const int DefaultInitialCapacity = 0;

		private static readonly string _keyTypeName = typeof(TKey).FullName;
		private static readonly string _valueTypeName = typeof(TValue).FullName;
		private static readonly bool _valueTypeIsReferenceType = !typeof(ValueType).IsAssignableFrom(typeof(TValue));

		private ConcurrentObservableCollection<KeyValuePair<TKey, TValue>> _list;
		private IEqualityComparer<TKey> _comparer;
		private object _syncRoot;
		private int _initialCapacity;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class.
		/// </summary>
		public ConcurrentObservableOrderedDictionary()
			: this(DefaultInitialCapacity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class using the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> can contain.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0</exception>
		public ConcurrentObservableOrderedDictionary(int capacity)
			: this(capacity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class using the specified comparer.
		/// </summary>
		/// <param name="comparer">The <see cref="IEqualityComparer{TKey}">IEqualityComparer&lt;TKey&gt;</see> to use when comparing keys, or <null/> to use the default <see cref="EqualityComparer{TKey}">EqualityComparer&lt;TKey&gt;</see> for the type of the key.</param>
		public ConcurrentObservableOrderedDictionary(IEqualityComparer<TKey> comparer)
			: this(DefaultInitialCapacity, comparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> class using the specified initial capacity and comparer.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection can contain.</param>
		/// <param name="comparer">The <see cref="IEqualityComparer{TKey}">IEqualityComparer&lt;TKey&gt;</see> to use when comparing keys, or <null/> to use the default <see cref="EqualityComparer{TKey}">EqualityComparer&lt;TKey&gt;</see> for the type of the key.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0</exception>
		public ConcurrentObservableOrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (0 > capacity)
				throw new ArgumentOutOfRangeException("capacity", "'capacity' must be non-negative");

			_initialCapacity = capacity;
			_comparer = comparer;
		}

		/// <summary>
		/// Converts the object passed as a key to the key type of the dictionary
		/// </summary>
		/// <param name="keyObject">The key object to check</param>
		/// <returns>The key object, cast as the key type of the dictionary</returns>
		/// <exception cref="ArgumentNullException"><paramref name="keyObject"/> is <null/>.</exception>
		/// <exception cref="ArgumentException">The key type of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not in the inheritance hierarchy of <paramref name="keyObject"/>.</exception>
		private static TKey ConvertToKeyType(object keyObject)
		{
			if (null == keyObject)
			{
				throw new ArgumentNullException("key");
			}
			else
			{
				if (keyObject is TKey)
					return (TKey)keyObject;
			}
			throw new ArgumentException("'key' must be of type " + _keyTypeName, "key");
		}

		/// <summary>
		/// Converts the object passed as a value to the value type of the dictionary
		/// </summary>
		/// <param name="value">The object to convert to the value type of the dictionary</param>
		/// <returns>The value object, converted to the value type of the dictionary</returns>
		/// <exception cref="ArgumentNullException"><paramref name="valueObject"/> is <null/>, and the value type of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is a value type.</exception>
		/// <exception cref="ArgumentException">The value type of the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> is not in the inheritance hierarchy of <paramref name="valueObject"/>.</exception>
		private static TValue ConvertToValueType(object value)
		{
			if (null == value)
			{
				if (_valueTypeIsReferenceType)
					return default(TValue);
				else
					throw new ArgumentNullException("value");
			}
			else
			{
				if (value is TValue)
					return (TValue)value;
			}
			throw new ArgumentException("'value' must be of type " + _valueTypeName, "value");
		}

		/// <summary>
		/// Gets the list object that stores the key/value pairs.
		/// </summary>
		/// <value>The list object that stores the key/value pairs for the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></value>
		/// <remarks>Accessing this property will create the list object if necessary.</remarks>
		public ConcurrentObservableCollection<KeyValuePair<TKey, TValue>> List
		{
			get
			{
				if (null == _list)
				{
					_list = new ConcurrentObservableCollection<KeyValuePair<TKey, TValue>>();
				}
				return _list;
			}
		}

		/// <summary>
		/// Inserts a new entry into the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection with the specified key and value at the specified index.
		/// </summary>
		/// <param name="targetIndex">The zero-based index at which the element should be inserted.</param>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add. The value can be <null/> if the type of the values in the dictionary is a reference type.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="targetIndex"/> is less than 0.<br/>
		/// -or-<br/>
		/// <paramref name="targetIndex"/> is greater than <see cref="Count"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/>.</exception>
		/// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>.</exception>
		public void Insert(int targetIndex, TKey key, TValue value)
		{
			if (targetIndex > Count || targetIndex < 0)
				throw new ArgumentOutOfRangeException("index");

			KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(key, value);

			List.Insert(targetIndex, pair);
			
			DoReadWriteNotify(
				() => _internalCollection.Count,
				(index) => _internalCollection.Add(pair),
				(index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, targetIndex)
			  );
		}

		/// <summary>
		/// Removes the entry at the specified index from the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
		/// </summary>
		/// <param name="index">The zero-based index of the entry to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
		/// -or-<br/>
		/// index is equal to or greater than <see cref="Count"/>.</exception>
		public override void RemoveAt(int index)
		{
			if (index >= Count || index < 0)
				throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");

			TKey key = List[index].Key;

			List.RemoveAt(index);
			base.Remove(key);
		}

		/// <summary>
		/// Gets or sets the value at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the value to get or set.</param>
		/// <value>The value of the item at the specified index.</value>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.<br/>
		/// -or-<br/>
		/// index is equal to or greater than <see cref="Count"/>.</exception>
		public TValue this[int index]
		{
			get
			{
				return List[index].Value;
			}

			set
			{
				if (index >= Count || index < 0)
					throw new ArgumentOutOfRangeException("index", "'index' must be non-negative and less than the size of the collection");

				TKey key = List[index].Key;

				List[index] = new KeyValuePair<TKey, TValue>(key, value);
				base[key] = value;
			}
		}

		/// <summary>
		/// Adds an entry with the specified key and value into the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection with the lowest available index.
		/// </summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add. This value can be <null/>.</param>
		/// <returns>The index of the newly added entry</returns>
		/// <remarks>A key cannot be <null/>, but a value can be.
		/// <para>You can also use the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property to add new elements by setting the value of a key that does not exist in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection; however, if the specified key already exists in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>, setting the <see cref="P:OrderedDictionary{TKey,TValue}.Item(TKey)"/> property overwrites the old value. In contrast, the <see cref="M:Add"/> method does not modify existing elements.</para></remarks>
		/// <exception cref="ArgumentNullException"><paramref name="key"/> is <null/></exception>
		/// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></exception>
		public override void Add(TKey key, TValue value)
		{
			KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(key, value);
			if (base.ContainsKey(key))
			{
				List[IndexOfKey(key)] = pair;
				base[key] = value;
			}
			else
			{
				List.Add(pair);
			}

			DoReadWriteNotify(
				() => _internalCollection.Count,
				(index) => _internalCollection.Add(pair),
				(index) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, pair, Count - 1)
			  );
		}


		/// <summary>
		/// Returns the zero-based index of the specified key in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see></param>
		/// <returns>The zero-based index of <paramref name="key"/>, if <paramref name="ley"/> is found in the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see>; otherwise, -1</returns>
		/// <remarks>This method performs a linear search; therefore it has a cost of O(n) at worst.</remarks>
		public int IndexOfKey(TKey key)
		{
			if (null == key)
				throw new ArgumentNullException("key");

			for (int index = 0; index < List.Count; index++)
			{
				KeyValuePair<TKey, TValue> entry = List[index];
				TKey next = entry.Key;
				if (null != _comparer)
				{
					if (_comparer.Equals(next, key))
					{
						return index;
					}
				}
				else if (next.Equals(key))
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// Removes the entry with the specified key from the <see cref="ConcurrentObservableOrderedDictionary{TKey,TValue}">OrderedDictionary&lt;TKey,TValue&gt;</see> collection.
		/// </summary>
		/// <param name="key">The key of the entry to remove</param>
		/// <returns><see langword="true"/> if the key was found and the corresponding element was removed; otherwise, <see langword="false"/></returns>
		public override bool Remove(TKey key)
		{
			if (null == key)
				throw new ArgumentNullException("key");

			int index = IndexOfKey(key);
			if (index >= 0)
			{
				List.RemoveAt(index);

				if (base.Remove(key))
				{
					//OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, List[index].Value), index));
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Returns a KeyValuePair using a key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public KeyValuePair<TKey, TValue> GetKeyValuePair(TKey key)
		{
			return new KeyValuePair<TKey, TValue>(key, this[key]);
		}

		public void Move(Int32 targetIndex, TKey key)
		{
			TValue value = this[key];
			int originalIndex = IndexOfKey(key);
			if (originalIndex >= 0)
			{
				List.RemoveAt(originalIndex);
			}

			if (targetIndex > Count || targetIndex < 0)
				throw new ArgumentOutOfRangeException("index");
			List.Insert(targetIndex, new KeyValuePair<TKey, TValue>(key, value));

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, new KeyValuePair<TKey, TValue>(key, value), targetIndex, originalIndex));
		}

		#region INotifyCollectionChanged

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}

		#endregion INotifyCollectionChanged

		public override void Clear()
		{
			DoReadWriteNotify(
			  // Get the list of keys and values from the internal list
			  () => ListSelect.Create(_internalCollection.List, x => x.KeyValuePair),
			  // remove the keys from the dictionary, remove the range from the list
			  (items) => ImmutableDictionaryListPair<TKey, TValue>.Empty,
			  // Notify which items were removed
			  (items) => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null, -1)
			);

			List.Clear();
		}
	}
}
