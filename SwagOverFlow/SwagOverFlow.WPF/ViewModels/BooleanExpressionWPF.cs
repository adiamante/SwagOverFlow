using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace SwagOverFlow.WPF.ViewModels
{
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
        protected override BooleanExpression Create(Type objectType, JObject jObject)
        {
            String nativeType = jObject["Type"].ToString();
            String assemblyType = nativeType
                .Replace("SwagOverFlow.ViewModels", "SwagOverFlow.WPF.ViewModels")
                .Replace(", SwagOverFlow,", ", SwagOverFlow.WPF,")
                .Replace("Expression", "ExpressionWPF");
            Type type = Type.GetType(assemblyType) ?? Type.GetType(nativeType);
            JArray jaChildren = null;

            if (jObject.ContainsKey("Parent"))
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Children"))
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }


            BooleanExpression exp = (BooleanExpression)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (exp)
            {
                case BooleanOperationExpression op:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        BooleanExpression child = (BooleanExpression)JsonConvert.DeserializeObject(jChild.ToString(), typeof(BooleanExpression), new BooleanExpressionWPFJsonConverter());
                        op.Children.Add(child);
                    }
                    break;
            }

            return exp;
        }
    }
    #endregion BooleanExpressionWPFJsonConverter
}
