using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SwagOverFlow.ViewModels
{
    #region SwagOption
    public abstract class SwagOption : SwagItem<SwagOptionGroup, SwagOption>
    {
        public String Name { get; set; } = "";
        public String StringFormat { get; set; } = "";
        [NotMapped]
        [JsonIgnore]
        public abstract String Value { get; }
        [NotMapped]
        [JsonIgnore]
        public virtual Dictionary<String, String> Dict
        { 
            get 
            { 
                return new Dictionary<string, string>() { { Name, Value } }; 
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
    public class SwagOptionGroup : SwagOption, ISwagParent<SwagOption>
    {
        protected ObservableCollection<SwagOption> _children = new ObservableCollection<SwagOption>();
        public override string Value
        {
            get { return Dict.ToString(); }
        }
        public override Type Type { get { return typeof(SwagOptionGroup); } }

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
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
                foreach (SwagOption option in Children)
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

            OnPropertyChanged("HasChildren");

            //TODO: This probably get's called twice when adding so need to check on that sometime
            OnSwagItemChanged(this, new PropertyChangedExtendedEventArgs("Children", Children, null, null));
        }
        #endregion Initialization
    }
    #endregion SwagOptionGroup

}
