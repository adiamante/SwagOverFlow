using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SwagOverFlow.ViewModels
{
    #region BooleanExpression
    //https://stackoverflow.com/questions/20995865/deserializing-json-to-abstract-class
    public abstract class BooleanExpression : SwagValueItem<BooleanOperationExpression, BooleanExpression>
    {
        public abstract bool Evaluate(Dictionary<String, String> context);
        public abstract Type Type { get; }
    }
    #endregion BooleanExpression

    #region BooleanOperationExpression
    public abstract class BooleanOperationExpression : BooleanExpression, ISwagParent<BooleanExpression>
    {
        protected ObservableCollection<BooleanExpression> _children = new ObservableCollection<BooleanExpression>();

        #region SwagItemChanged
        public event EventHandler<SwagItemChangedEventArgs> SwagItemChanged;

        public void OnSwagItemChanged(SwagItemBase swagItem, PropertyChangedExtendedEventArgs e)
        {
            SwagItemChanged?.Invoke(this, new SwagItemChangedEventArgs() { SwagItem = swagItem, PropertyChangedArgs = e });
            Parent?.OnSwagItemChanged(swagItem, e);
        }
        #endregion SwagItemChanged

        #region Children
        public ObservableCollection<BooleanExpression> Children
        {
            get { return _children; }
            set 
            { 
                SetValue(ref _children, value);
                OnPropertyChanged("HasChildren");
            }
        }
        #endregion Children

        #region HasChildren
        public bool HasChildren { get { return _children.Count > 0; } }
        #endregion HasChildren

        #region Initialization
        public BooleanOperationExpression() : base()
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

            OnPropertyChanged("HasChildren");

            //TODO: This probably get's called twice when adding so need to check on that sometime
            OnSwagItemChanged(this, new PropertyChangedExtendedEventArgs("Children", Children, null, null));
        }
        #endregion Initialization
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


