using SwagOverFlow.Iterator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SwagOverFlow.ViewModels
{
    #region SwagOption
    public abstract class SwagOption : SwagItem<SwagOptionGroup, SwagOption>
    {
        public String Name { get; set; }
        public String StringFormat { get; set; }
        public abstract String Value { get; }
        public virtual Dictionary<String, String> Dict
        { 
            get 
            { 
                return new Dictionary<string, string>() { { Name, Value } }; 
            }
        }
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
        public override string Value
        {
            get { return ValueT; }
        }
    }
    #endregion StringOption

    #region DateOption
    public class DateOption : SwagOption<DateTime>
    {
        public override string Value
        {
            get { return ValueT.ToString(StringFormat); }
        }
    }
    #endregion DateOption

    #region BooleanOption
    public class BooleanOption : SwagOption<Boolean>
    {
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

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            throw new NotImplementedException();
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
                    if (dict.ContainsKey(option.Name))
                    {
                        dict[option.Name] = option.Value;
                    }
                    else
                    {
                        dict.Add(option.Name, option.Value);
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

    }
    #endregion SwagOptionGroup
}
