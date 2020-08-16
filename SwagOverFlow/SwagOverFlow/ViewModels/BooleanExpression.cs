using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwagOverFlow.ViewModels
{
    #region BooleanExpression
    //https://stackoverflow.com/questions/20995865/deserializing-json-to-abstract-class
    public abstract class BooleanExpression : SwagItem<BooleanGroupExpression, BooleanExpression>
    {
        public abstract bool Evaluate(Dictionary<String, String> context);
        public abstract Type Type { get; }
    }
    #endregion BooleanExpression

    #region BooleanGroupExpression
    [JsonObject]
    public abstract class BooleanGroupExpression : BooleanExpression, ISwagParent<BooleanExpression>, ICollection<BooleanExpression>
    {
        protected ObservableCollection<BooleanExpression> _children = new ObservableCollection<BooleanExpression>();

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public virtual void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedEventArgs e)
        {
            BooleanExpression exp = (BooleanExpression)swagItem;

            switch (e)
            {
                case PropertyChangedExtendedEventArgs exArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{exp.Display}({exArgs.PropertyName})\n\t{exArgs.OldValue} => {exArgs.NewValue}"
                    });
                    break;
                case CollectionPropertyChangedEventArgs colArgs:
                    SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs()
                    {
                        SwagItem = swagItem,
                        PropertyChangedArgs = e,
                        Message = $"{exp.Display}({colArgs.PropertyName})\n\t[OLD] => {colArgs.OldItems}\n\t[NEW] {colArgs.NewItems}"
                    });
                    break;
            }
            
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion SwagItemChanged

        #region Children
        public virtual ObservableCollection<BooleanExpression> Children
        {
            get { return _children; }
            set 
            { 
                SetValue(ref _children, value);
                OnPropertyChanged("HasChildren");
            }
        }
        #endregion Children

        #region ICollection
        public int Count => _children.Count;

        public bool IsReadOnly => false;

        public void Add(BooleanExpression item)
        {
            ((ICollection<BooleanExpression>)_children).Add(item);
        }

        public void Clear()
        {
            ((ICollection<BooleanExpression>)_children).Clear();
        }

        public bool Contains(BooleanExpression item)
        {
            return ((ICollection<BooleanExpression>)_children).Contains(item);
        }

        public void CopyTo(BooleanExpression[] array, int arrayIndex)
        {
            ((ICollection<BooleanExpression>)_children).CopyTo(array, arrayIndex);
        }

        public bool Remove(BooleanExpression item)
        {
            return ((ICollection<BooleanExpression>)_children).Remove(item);
        }

        public IEnumerator<BooleanExpression> GetEnumerator()
        {
            return ((IEnumerable<BooleanExpression>)_children).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_children).GetEnumerator();
        }
        #endregion ICollection

        #region Initialization
        public BooleanGroupExpression() : base()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BooleanExpression item in e.NewItems)
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
                foreach (BooleanExpression item in e.OldItems)
                {
                    foreach (BooleanExpression sibling in Children)
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
    }
    #endregion BooleanGroupExpression

    #region BooleanContainerExpression
    [JsonObject]
    public class BooleanContainerExpression : BooleanGroupExpression
    {
        protected BooleanExpression _root;
        Boolean _isInitialized = false;

        public override Type Type { get { return typeof(BooleanContainerExpression); } }

        #region IsInitialized
        public Boolean IsInitialized
        {
            get { return _isInitialized; }
            set { SetValue(ref _isInitialized, value); }
        }
        #endregion IsInitialized

        #region Children
        [NotMapped]
        [JsonIgnore]
        public override ObservableCollection<BooleanExpression> Children
        {
            get { return _children; }
            set
            {
                SetValue(ref _children, value);
                OnPropertyChanged("HasChildren");
            }
        }
        #endregion Children

        #region Root
        public BooleanExpression Root
        {
            get { return _root; }
            set 
            { 
                SetValue(ref _root, value);
                _children.Clear();
                _children.Add(_root);
            }
        }
        #endregion Root

        public override bool Evaluate(Dictionary<string, string> context)
        {
            if (_root == null)
            {
                return true;
            }

            return _root.Evaluate(context);
        }
    }
    #endregion BooleanContainerExpression

    #region BooleanOperationExpression
    public abstract class BooleanOperationExpression : BooleanGroupExpression
    {

    }
    #endregion BooleanOperationExpression

    #region BooleanAndExpression
    public class BooleanAndExpression : BooleanOperationExpression
    {
        public override Type Type { get { return typeof(BooleanAndExpression); } }

        public override bool Evaluate(Dictionary<string, string> context)
        {
            if (Children.Count == 0)
            {
                return false;
            }

            foreach (BooleanExpression bExpr in Children)
            {
                if (!bExpr.Evaluate(context))
                {
                    return false;
                }
            }
            return true;
        }
    }
    #endregion BooleanAndExpression

    #region BooleanOrExpression
    public class BooleanOrExpression : BooleanOperationExpression
    {
        public override Type Type { get { return typeof(BooleanOrExpression); } }
        public override bool Evaluate(Dictionary<string, string> context)
        {
            if (Children.Count == 0)
            {
                return false;
            }

            foreach (BooleanExpression bExpr in Children)
            {
                if (bExpr.Evaluate(context))
                {
                    return true;
                }
            }
            return false;
        }
    }
    #endregion BooleanOrExpression

    #region BooleanVariableExpression
    public abstract class BooleanVariableExpression<T> : BooleanExpression
    {
        #region Private Members
        String _key;
        T _targetValue;
        #endregion Private Members

        #region Properties
        #region Key
        public String Key
        {
            get { return _key; }
            set { SetValue(ref _key, value); }
        }
        #endregion Key
        #region TargetValue
        public T TargetValue
        {
            get { return _targetValue; }
            set { SetValue(ref _targetValue, value); }
        }
        #endregion TargetValue
        #endregion Properties
    }
    #endregion BooleanVariableExpression

    #region BooleanBooleanVariableExpression
    public class BooleanBooleanVariableExpression : BooleanVariableExpression<bool>
    {
        #region Type
        public override Type Type { get { return typeof(BooleanBooleanVariableExpression); } }
        #endregion Type

        public BooleanBooleanVariableExpression()
        {
            TargetValue = true;
        }

        public override bool Evaluate(Dictionary<string, string> context)
        {
            if (context.ContainsKey(Key))
            {
                if (Boolean.TryParse(context[Key], out Boolean passCondition))
                {
                    return passCondition == TargetValue;
                }
            }

            return !TargetValue;
        }
    }
    #endregion BooleanBooleanVariableExpression

    #region BooleanStringVariableExpression
    public class BooleanStringVariableExpression : BooleanVariableExpression<String>
    {
        #region Type
        public override Type Type { get { return typeof(BooleanStringVariableExpression); } }
        #endregion Type

        public override bool Evaluate(Dictionary<string, string> context)
        {
            if (context.ContainsKey(Key))
            {
                if (context[Key] == TargetValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
    #endregion BooleanStringVariableExpression
}


