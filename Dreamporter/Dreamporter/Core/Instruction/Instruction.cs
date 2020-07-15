using Dreamporter.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwagOverFlow.Parsing;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using SwagOverFlow.Utils;


namespace Dreamporter.Core
{
    public abstract class Instruction : SwagItem<GroupInstruction, Instruction>
    {
        #region Private Members
        Int32 _instructionId;
        String _name, _logPattern, _description, _status;
        Boolean _isEnabled = true, _doLog = true;
        InstructionCacheProperties _cacheProperties = new InstructionCacheProperties();
        BooleanContainerExpression _condition = new BooleanContainerExpression();
        ObservableCollection<Schema> _requiredData = new ObservableCollection<Schema>();
        #endregion Private Members

        #region Properties
        #region InstructionId
        public Int32 InstructionId
        {
            get { return _instructionId; }
            set { SetValue(ref _instructionId, value); }
        }
        #endregion InstructionId
        #region Name
        public String Name
        {
            get { return _name; }
            set { SetValue(ref _name, value); }
        }
        #endregion Name
        #region IsEnabled
        public Boolean IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value); }
        }
        #endregion IsEnabled
        #region DoLog
        public Boolean DoLog
        {
            get { return _doLog; }
            set { SetValue(ref _doLog, value); }
        }
        #endregion DoLog
        #region LogPattern
        public String LogPattern
        {
            get { return _logPattern; }
            set { SetValue(ref _logPattern, value); }
        }
        #endregion LogPattern
        #region Tags
        public string[] Tags { get; set; }
        #endregion Tags
        #region Description
        public String Description
        {
            get { return _description; }
            set { SetValue(ref _description, value); }
        }
        #endregion Description
        #region Type
        public abstract Type Type { get; }
        #endregion Type
        #region CacheProperties
        public InstructionCacheProperties CacheProperties
        {
            get { return _cacheProperties; }
            set { SetValue(ref _cacheProperties, value); }
        }
        #endregion CacheProperties
        #region Status
        public String Status
        {
            get { return _status; }
            set { SetValue(ref _status, value); }
        }
        #endregion Status
        #region Path
        [JsonIgnore]
        public String Path
        {
            get
            {
                String path = "";
                Instruction temp = this;
                while (temp != null)
                {
                    path = $"{temp.Name}/{path}";
                    temp = temp.Parent;
                }
                path = path.Trim('/');
                return path;
            }
        }
        #endregion Path
        #region Condition
        public BooleanContainerExpression Condition
        {
            get { return _condition; }
            set { SetValue(ref _condition, value); }
        }
        #endregion Condition  
        #region RequiredData
        public ObservableCollection<Schema> RequiredData
        {
            get { return _requiredData; }
            set { SetValue(ref _requiredData, value); }
        }
        #endregion RequiredData
        #endregion Properties

        #region Initialization
        public Instruction()
        {
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        abstract public void RunHandler(RunContext context, RunParams rp);

        public void Run(RunContext context, RunParams rp)
        {
            rp.Instruction = Path;
            rp.Params.SafeSet<String, String>("InErrorState", context.InErrorState.ToString());

            Boolean doExecute = IsEnabled && Condition.Evaluate(rp.Params);
            Instruction temp = this;
            String fullPath = rp.FullPath, logMessage = "";

#if DEBUG
            System.Diagnostics.Debug.WriteLine(fullPath);
#endif

            logMessage = MessageTemplateHelper.ParseTemplate(LogPattern ?? "", rp.Params);

            if (doExecute && !context.InAbortState)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                #region DoLog
                if (DoLog)
                {
                    context.Log(fullPath, "Start", logMessage);
                }
                #endregion DoLog

                try
                {
                    RunHandler(context, rp);
                }
                catch (InstructionException iex)
                {
                    context.InAbortState = iex.Abort;
                    context.InErrorState = true;
                    context.LogError(fullPath, "Error", (iex.Message + Environment.NewLine + iex.Info).Replace("'", "''"));
                }
                catch (Exception ex)
                {
                    #region Exception

                    context.InErrorState = true;
                    String message = "";
                    Exception exTemp = ex;
                    while (exTemp != null)
                    {
                        message += exTemp.Message + Environment.NewLine;
                        exTemp = exTemp.InnerException;
                    }

                    message += ex.StackTrace;

                    context.LogError(fullPath, "Error", message.Replace("'", "''"));
                    #endregion Exception
                }

                sw.Stop();

                #region DoLog
                if (DoLog)
                {
                    context.Log(fullPath, $"End - {sw.Elapsed.ToString("G")}", logMessage);
                }
                #endregion DoLog
            }
        }

        public static String ResolveParameters(String input, Dictionary<String, String> parameters)
        {
            if (input == "")
            {
                return "";
            }

            String output = input ?? "";

            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                output = output.Replace($"{{{kvp.Key}}}", kvp.Value);
            }

            //Handle special templates
            output = output.Replace("{Now}", DateTime.Now.ToString());
            output = output.Replace("{*}", JsonHelper.ToJsonString(parameters));

            return output;
        }
        #endregion Methods
    }
}
