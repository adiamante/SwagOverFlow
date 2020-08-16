using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SwagOverFlow.ViewModels
{
    #region SwagOption
    public abstract class SwagOption : SwagItem<SwagOptionGroup, SwagOption>
    {
        Boolean _isEnabled = true;
        String _name = "", _stringFormat = "", _description = "";

        #region IsEnabled
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value); }
        }
        #endregion IsEnabled
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region StringFormat
        public String StringFormat
        {
            get { return _stringFormat; }
            set { SetValue(ref _stringFormat, value); }
        }
        #endregion StringFormat
        #region Description
        public String Description
        {
            get { return _description; }
            set { SetValue(ref _description, value); }
        }
        #endregion Description
        [NotMapped]
        [JsonIgnore]
        public abstract String Value { get; }
        [NotMapped]
        [JsonIgnore]
        public virtual Dictionary<String, String> Dict
        { 
            get 
            {
                if (IsEnabled)
                {
                    return new Dictionary<string, string>() { { Name, Value } };
                }

                return new Dictionary<string, string>();
            }
        }
        public abstract Type Type { get; }
    }
    #endregion SwagOption

    #region SwagOption<T>
    public abstract class SwagOption<T> : SwagOption
    {
        T _valueT;

        #region ValueT
        public T ValueT
        {
            get { return _valueT; }
            set { SetValue(ref _valueT, value); }
        }
        #endregion ValueT
    }
    #endregion SwagOption<T>

    #region StringOption
    public class StringOption : SwagOption<String>
    {
        public override Type Type { get { return typeof(StringOption); } }
        public StringOption()
        {
            StringFormat = "yyyyMMdd";
        }
        public override string Value
        {
            get { return ValueT; }
        }
    }
    #endregion StringOption

    #region DateOption
    public class DateOption : SwagOption<DateTime>
    {
        public override Type Type { get { return typeof(DateOption); } }
        public override string Value
        {
            get { return ValueT.ToString(StringFormat); }
        }
    }
    #endregion DateOption

    #region BooleanOption
    public class BooleanOption : SwagOption<Boolean>
    {
        public override Type Type { get { return typeof(BooleanOption); } }
        public override string Value
        {
            get { return ValueT.ToString(); }
        }
    }
    #endregion BooleanOption

    #region SwagOptionGroup
    [JsonObject]
    public class SwagOptionGroup : SwagOption, ISwagParent<SwagOption>, ICollection<SwagOption>
    {
        protected ObservableCollection<SwagOption> _children = new ObservableCollection<SwagOption>();
        Boolean _isInitialized = false;
        public override string Value
        {
            get { return Dict.ToString(); }
        }
        public override Type Type { get { return typeof(SwagOptionGroup); } }

        #region IsInitialized
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { SetValue(ref _isInitialized, value); }
        }
        #endregion IsInitialized

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
        {
            SwagOption opt = (SwagOption)swagItem;

            switch (e)
            {
                case PropertyChangedExtendedEventArgs exArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{opt.Name}({exArgs.PropertyName})\n\t{exArgs.OldValue} => {exArgs.NewValue}"
                    });
                    break;
                case CollectionPropertyChangedEventArgs colArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{opt.Name}({colArgs.PropertyName})\n\t[OLD] => {colArgs.OldItems}\n\t[NEW] {colArgs.NewItems}"
                    });
                    break;
            }

            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion SwagItemChanged

        #region Children
        public ObservableCollection<SwagOption> Children
        {
            get { return _children; }
            set
            {
                SetValue(ref _children, value);
                OnPropertyChanged("HasChildren");
            }
        }
        #endregion Children

        #region Dict
        public override Dictionary<String, String> Dict
        {
            get
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();

                if (IsEnabled)
                {
                    foreach (SwagOption option in Children)
                    {
                        if (option.IsEnabled)
                        {
                            foreach (KeyValuePair<string, string> kvp in option.Dict)
                            {
                                if (dict.ContainsKey(kvp.Key))
                                {
                                    dict[kvp.Key] = kvp.Value;
                                }
                                else
                                {
                                    dict.Add(kvp.Key, kvp.Value);
                                }
                            }
                        }
                    }
                }
                
                return dict;
            }
        }
        #endregion Dict

        #region HasChildren
        public bool HasChildren { get { return _children.Count > 0; } }
        #endregion HasChildren

        #region Iterator
        public SwagItemPreOrderIterator<SwagOption> CreateIterator()
        {
            return new SwagItemPreOrderIterator<SwagOption>(this);
        }
        #endregion Iterator

        #region Initialization
        public SwagOptionGroup() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagOption item in e.NewItems)
                {
                    item.Parent = this;
                    if (item.Sequence < 0)
                    {
                        item.Sequence = this.Children.Count - 1;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (SwagOption item in e.OldItems)
                {
                    foreach (SwagOption sibling in Children)
                    {
                        if (item.Sequence < sibling.Sequence)
                        {
                            sibling.Sequence--;
                        }
                    }
                }
            }

            OnSwagItemChanged(this, new CollectionPropertyChangedEventArgs(nameof(Children), this, e.Action, e.OldItems, e.NewItems));
            OnPropertyChanged("HasChildren");
        }
        #endregion Initialization

        #region ICollection

        public int Count => ((ICollection<SwagOption>)_children).Count;

        public bool IsReadOnly => ((ICollection<SwagOption>)_children).IsReadOnly;
        public void Add(SwagOption item)
        {
            ((ICollection<SwagOption>)_children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<SwagOption>)_children).Clear();
        }

        public bool Contains(SwagOption item)
        {
            return ((ICollection<SwagOption>)_children).Contains(item);
        }

        public void CopyTo(SwagOption[] array, int arrayIndex)
        {
            ((ICollection<SwagOption>)_children).CopyTo(array, arrayIndex);
        }

        public bool Remove(SwagOption item)
        {
            return ((ICollection<SwagOption>)_children).Remove(item);
        }

        public IEnumerator<SwagOption> GetEnumerator()
        {
            return ((IEnumerable<SwagOption>)_children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }
        #endregion ICollection
    }
    #endregion SwagOptionGroup

}
