using Dreamporter.Core;
using Newtonsoft.Json;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dreamporter.Core
{
    public class Integration : ViewModelBaseExtended
    {
        #region Private Members
        Int32 _integrationId, _buildId, _testBuildId, _sequence;
        String _name;
        GroupBuild _build = new GroupBuild();
        GroupBuild _testBuild = new GroupBuild();
        Build _selectedBuild;
        GroupInstruction _instructionTemplates = new GroupInstruction();
        SwagOptionGroup _defaultOptions = new SwagOptionGroup();
        OptionsNode _selectedOptions;
        OptionsNode _optionsTree = new OptionsNode();
        ObservableCollection<Schema> _initialSchemas = new ObservableCollection<Schema>();
        ObservableCollection<DataContext> _dataContexts = new ObservableCollection<DataContext>();
        ObservableCollection<TestContext> _testContexts = new ObservableCollection<TestContext>();
        TestContext _selectedTestContext;
        #endregion Private Members

        #region Properties
        #region IntegrationId
        public Int32 IntegrationId
        {
            get { return _integrationId; }
            set { SetValue(ref _integrationId, value); }
        }
        #endregion IntegrationId
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region Sequence
        public Int32 Sequence
        {
            get { return _sequence; }
            set { SetValue(ref _sequence, value); }
        }
        #endregion Sequence
        #region BuildId
        public Int32 BuildId
        {
            get { return _buildId; }
            set { SetValue(ref _buildId, value); }
        }
        #endregion BuildId
        #region Build
        [NotMapped] //Opt out of one to zero to one convention with Group Build
        public GroupBuild Build
        {
            get { return _build; }
            set { SetValue(ref _build, value); }
        }
        #endregion Build
        #region TestBuildId
        public Int32 TestBuildId
        {
            get { return _testBuildId; }
            set { SetValue(ref _testBuildId, value); }
        }
        #endregion TestBuildId
        #region TestBuild
        [NotMapped] //Opt out of one to zero to one convention with Group Build
        public GroupBuild TestBuild
        {
            get { return _testBuild; }
            set { SetValue(ref _testBuild, value); }
        }
        #endregion TestBuild
        #region SelectedBuild
        [JsonIgnore]
        [NotMapped]
        public Build SelectedBuild
        {
            get { return _selectedBuild; }
            set { SetValue(ref _selectedBuild, value); }
        }
        #endregion SelectedBuild
        #region InstructionTemplates
        public GroupInstruction InstructionTemplates
        {
            get { return _instructionTemplates; }
            set { SetValue(ref _instructionTemplates, value); }
        }
        #endregion InstructionTemplates
        #region DefaultOptions
        public SwagOptionGroup DefaultOptions
        {
            get { return _defaultOptions; }
            set { SetValue(ref _defaultOptions, value); }
        }
        #endregion DefaultOptions
        #region OptionsTree
        public OptionsNode OptionsTree
        {
            get { return _optionsTree; }
            set { SetValue(ref _optionsTree, value); }
        }
        #endregion OptionsTree
        #region SelectedOptions
        [JsonIgnore]
        [NotMapped]
        public OptionsNode SelectedOptions
        {
            get { return _selectedOptions; }
            set { SetValue(ref _selectedOptions, value); }
        }
        #endregion SelectedOptions
        #region InitialSchemas
        public ObservableCollection<Schema> InitialSchemas
        {
            get { return _initialSchemas; }
            set { SetValue(ref _initialSchemas, value); }
        }
        #endregion InitialSchemas
        #region DataContexts
        public ObservableCollection<DataContext> DataContexts
        {
            get { return _dataContexts; }
            set { SetValue(ref _dataContexts, value); }
        }
        #endregion DataContexts
        #region TestContexts
        public ObservableCollection<TestContext> TestContexts
        {
            get { return _testContexts; }
            set { SetValue(ref _testContexts, value); }
        }
        #endregion TestContexts
        #region SelectedTestContext
        [JsonIgnore]
        [NotMapped]
        public TestContext SelectedTestContext
        {
            get { return _selectedTestContext; }
            set { SetValue(ref _selectedTestContext, value); }
        }
        #endregion SelectedTestContext
        #endregion Properties

        #region Methods
        public RunContext Run(RunContext rc = null, RunParams rp = null, Boolean inTestMode = false)
        {
            if (rc == null)
            {
                rc = new RunContext();
            }

            if (rp == null)
            {
                rp = GenerateRunParams();
            }

            if  (inTestMode)
            {
                if (SelectedTestContext != null && File.Exists(SelectedTestContext.InitialDB))
                {
                    //Copy to In Memory connection so we don't keep appending
                    rc.OpenFileToMemory($"Data Source = {SelectedTestContext.InitialDB};Version=3;");
                }
                else
                {
                    rc.Open();
                }
                TestBuild.Run(rc, rp);
            }
            else
            {
                rc.Open();
                InitRunContext(rc, inTestMode);
                Build.Run(rc, rp);
            }

            return rc;
        }

        public async Task<RunContext> RunAsync(RunContext rc = null, RunParams rp = null, Boolean inTestMode = false)
        {
            return await Task.Run<RunContext>(() => { return Run(rc, rp, inTestMode); });
        }

        public RunParams GenerateRunParams()
        {
            RunParams rp = new RunParams();

            #region Resolve Options
            Dictionary<String, String> optionsDict = DefaultOptions.Dict;

            if (SelectedOptions != null)
            {
                Stack<OptionsNode> optionsStack = new Stack<OptionsNode>();
                OptionsNode optionsNode = SelectedOptions;
                while (optionsNode != null)     //Get all parent options
                {
                    optionsStack.Push(optionsNode);
                    optionsNode = optionsNode.Parent;
                }

                while (optionsStack.Count > 0)
                {
                    optionsNode = optionsStack.Pop();
                    foreach (KeyValuePair<String, String> opt in optionsNode.Options.Dict)
                    {
                        if (optionsDict.ContainsKey(opt.Key))
                        {
                            optionsDict[opt.Key] = opt.Value;
                        }
                        else
                        {
                            optionsDict.Add(opt.Key, opt.Value);
                        }
                    }
                }
            }

            rp.Params = optionsDict;
            #endregion Resolve Options

            return rp;
        }

        public void InitRunContext(RunContext runContext, Boolean inTestMode = false)
        {
            runContext.InitDataContexts(DataContexts);

            if (!inTestMode)
            {
                foreach (Schema schema in InitialSchemas)
                {
                    DataSet ds = schema.GetDataSet();

                    foreach (DataTable dtbl in ds.Tables)
                    {
                        runContext.AddTables(dtbl);
                    }
                }
            }
        }
        #endregion Methods
    }
}
