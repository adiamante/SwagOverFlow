using Dreamporter.Instructions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Utils;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Data;

namespace Dreamporter.WPF.ViewModels
{
    public class GroupInstructionWPF : GroupInstruction
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
        public GroupInstructionWPF() : base()
        {
            _childrenCollectionViewSource = new CollectionViewSource() { Source = _children };
            _childrenCollectionViewSource.View.SortDescriptions.Add(new SortDescription("Sequence", ListSortDirection.Ascending));
            _children.CollectionChanged += _children_CollectionChanged;
        }

        private void _children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Instruction instruction in e.NewItems)
                {
                    instruction.PropertyChanged += Instruction_PropertyChanged;
                }
            }

            _childrenCollectionViewSource.View.Refresh();
        }

        private void Instruction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Sequence")
            {
                _childrenCollectionViewSource.View.Refresh();
            }
        }
        #endregion Initialization
    }

    #region InstructionWPFJsonConverter
    public class InstructionWPFJsonConverter : AbstractJsonConverter<Instruction>
    {
        public static InstructionWPFJsonConverter Instance = new InstructionWPFJsonConverter();

        protected override Instruction Create(Type objectType, JObject jObject)
        {
            String nativeType = jObject["Type"].ToString();
            String assemblyType = nativeType
                .Replace("Dreamporter.Instructions", "Dreamporter.WPF.ViewModels")
                .Replace(", Dreamporter,", ", Dreamporter.WPF,")
                .Replace("GroupInstruction", "GroupInstructionWPF");
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

            Instruction instruction = (Instruction)JsonConvert.DeserializeObject(jObject.ToString(), type);

            switch (instruction)
            {
                case GroupInstruction grp:
                    foreach (JToken token in jaChildren)
                    {
                        JObject jChild = (JObject)token;
                        Instruction child = (Instruction)JsonConvert.DeserializeObject(jChild.ToString(), typeof(Instruction), InstructionWPFJsonConverter.Instance);
                        grp.Children.Add(child);
                    }
                    break;
            }

            return instruction;
        }
    }
    #endregion InstructionWPFJsonConverter
}
