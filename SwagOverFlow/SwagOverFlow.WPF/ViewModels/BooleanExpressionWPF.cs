using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;
using System.Windows.Documents;

namespace SwagOverFlow.WPF.ViewModels
{
    public class BooleanContainerExpressionWPF : BooleanContainerExpression
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        protected ObservableCollection<BooleanExpression> _children;
        #endregion Private/Protected Members

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public BooleanContainerExpressionWPF() : base()
        {
            _children = new ObservableCollection<BooleanExpression>() { _root };
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
            this.PropertyChangedExtended += BooleanContainerExpressionWPF_PropertyChangedExtended;
        }

        private void BooleanContainerExpressionWPF_PropertyChangedExtended(object sender, PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Root":
                    _children = new ObservableCollection<BooleanExpression>() { _root };
                    _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
                    _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
                    _children.CollectionChanged += _children_CollectionChanged;
                    OnPropertyChanged("ChildrenView");
                    break;
            }
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _childrenCollectionViewSource.View.Refresh();
        }
        #endregion Initialization
    }

    public class BooleanAndExpressionWPF : BooleanAndExpression
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public BooleanAndExpressionWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BooleanExpression booleanExpression in e.NewItems)
                {
                    booleanExpression.PropertyChanged += BooleanExpression_PropertyChanged;
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }

        private void BooleanExpression_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Sequence")
            {
                _childrenCollectionViewSource.View.Refresh();
            }
        }
        #endregion Initialization
    }

    public class BooleanOrExpressionWPF : BooleanOrExpression
    {
        #region Private/Protected Members
        CollectionViewSource _childrenCollectionViewSource;
        #endregion Private/Protected Members

        #region Properties
        #region ChildrenView
        [JsonIgnore]
        [NotMapped]
        public ICollectionView ChildrenView
        {
            get { return _childrenCollectionViewSource.View; }
        }
        #endregion ChildrenView
        #endregion Properties

        #region Initialization
        public BooleanOrExpressionWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BooleanExpression booleanExpression in e.NewItems)
                {
                    booleanExpression.PropertyChanged += BooleanExpression_PropertyChanged;
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }

        private void BooleanExpression_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Sequence")
            {
                _childrenCollectionViewSource.View.Refresh();
            }
        }
        #endregion Initialization
    }

    #region BooleanExpressionWPFJsonConverter
    public class BooleanExpressionWPFJsonConverter : AbstractJsonConverter<BooleanExpression>
    {
        public static BooleanExpressionWPFJsonConverter Instance = new BooleanExpressionWPFJsonConverter();

        protected override BooleanExpression Create(Type objectType, JObject jObject)
        {
            String nativeType = jObject["Type"].ToString();
            String assemblyType = nativeType
                .Replace("SwagOverFlow.ViewModels", "SwagOverFlow.WPF.ViewModels")
                .Replace(", SwagOverFlow,", ", SwagOverFlow.WPF,")
                .Replace("Expression", "ExpressionWPF");
            Type type = Type.GetType(assemblyType) ?? Type.GetType(nativeType);
            JObject jRoot = null;
            JArray jaChildren = null; 

            if (jObject.ContainsKey("Parent"))  //Prevent ReadJson Null error
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Root"))  //Prevent ReadJson Null error
            {
                if (jObject["Root"] is JObject)
                {
                    jRoot = (JObject)jObject["Root"];
                }
                jObject.Remove("Root");
            }

            if (jObject.ContainsKey("Children")) //Children are handled below
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }

            BooleanExpression exp = (BooleanExpression)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (exp)
            {
                case BooleanContainerExpression cnt:
                    if (jRoot != null)
                    {
                        BooleanExpression root = (BooleanExpression)JsonConvert.DeserializeObject(jRoot.ToString(), typeof(BooleanExpression), BooleanExpressionWPFJsonConverter.Instance);
                        cnt.Root = root;
                    }
                    break;
                case BooleanOperationExpression op:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        BooleanExpression child = (BooleanExpression)JsonConvert.DeserializeObject(jChild.ToString(), typeof(BooleanExpression), BooleanExpressionWPFJsonConverter.Instance);
                        op.Children.Add(child);
                    }
                    break;
            }

            return exp;
        }
    }
    #endregion BooleanExpressionWPFJsonConverter
}
