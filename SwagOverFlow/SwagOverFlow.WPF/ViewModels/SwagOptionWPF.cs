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
    public class SwagOptionGroupWPF : SwagOptionGroup
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
        public SwagOptionGroupWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SwagOption swagOptoin in e.NewItems)
                {
                    swagOptoin.PropertyChanged += swagOptionGroup_PropertyChanged;
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }

        private void swagOptionGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Sequence")
            {
                _childrenCollectionViewSource.View.Refresh();
            }
        }
        #endregion Initialization
    }

    #region SwagOptionWPFJsonConverter
    public class SwagOptionWPFJsonConverter : AbstractJsonConverter<SwagOption>
    {
        public static SwagOptionWPFJsonConverter Instance = new SwagOptionWPFJsonConverter();
        protected override SwagOption Create(Type objectType, JObject jObject)
        {
            String nativeType = jObject["Type"].ToString();
            String assemblyType = nativeType
                .Replace("SwagOverFlow.ViewModels", "SwagOverFlow.WPF.ViewModels")
                .Replace(", SwagOverFlow,", ", SwagOverFlow.WPF,")
                .Replace("SwagOptionGroup", "SwagOptionGroupWPF");
            Type type = Type.GetType(assemblyType) ?? Type.GetType(nativeType);
            JArray jaChildren = null;

            if (jObject.ContainsKey("Parent"))  //Prevent ReadJson Null error
            {
                jObject.Remove("Parent");
            }

            if (jObject.ContainsKey("Children")) //Children are handled below
            {
                jaChildren = (JArray)jObject["Children"];
                jObject.Remove("Children");
            }

            SwagOption exp = (SwagOption)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (exp)
            {
                case SwagOptionGroup sog:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        SwagOption child = (SwagOption)JsonConvert.DeserializeObject(jChild.ToString(), typeof(SwagOption), SwagOptionWPFJsonConverter.Instance);
                        sog.Children.Add(child);
                    }
                    break;
            }

            return exp;
        }
    }
    #endregion SwagOptionWPFJsonConverter
}
