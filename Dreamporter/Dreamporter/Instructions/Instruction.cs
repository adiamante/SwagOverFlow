using Dreamporter.Builds;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwagOverFlow.Parsing;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Dreamporter.Instructions
{
    public abstract class Instruction : ViewModelBase
    {
        #region Private Members
        Int32 _instructionId;
        String _name, _infoFormat, _description, _preProcessQuery, _postProcessQuery;
        Boolean _isEnabled = true, _requireTags, _doLog = true;
        Int32 _sequence;
        Int32? _parentId;
        #endregion Private Members

        #region Properties
        #region InstructionId
        public Int32 InstructionId
        {
            get { return _instructionId; }
            set { SetValue(ref _instructionId, value); }
        }
        #endregion InstructionId
        #region ParentId
        public Int32? ParentId
        {
            get { return _parentId; }
            set { SetValue(ref _parentId, value); }
        }
        #endregion ParentId
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
        #region Sequence
        public Int32 Sequence
        {
            get { return _sequence; }
            set { SetValue(ref _sequence, value); }
        }
        #endregion Sequence
        #region Parent
        [JsonIgnore]
        public virtual InstructionGroup Parent { get; set; }
        #endregion Parent
        #region Type
        public String Type { get { return this.GetType().Name; } }
        #endregion Type
        #region CacheProperties
        public InstructionCacheProperties CacheProperties { get; set; }
        #endregion CacheProperties
        #region LogPattern
        public String LogPattern
        {
            get { return _infoFormat; }
            set { SetValue(ref _infoFormat, value); }
        }
        #endregion LogPattern
        #endregion Properties

        #region Initialization
        public Instruction()
        {
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        abstract public void Execute(RuntimeContext context, Dictionary<String, String> parameters);

        public void Run(RuntimeContext context, Dictionary<String, String> parameters)
        {
            Boolean doExecute = IsEnabled;
            Instruction temp = this;
            String instructionPath = "", logMessage = "";

            #region instructionPath
            while (temp != null)
            {
                instructionPath = $"{temp.Name}/{instructionPath}";
                temp = temp.Parent;
            }
            instructionPath = instructionPath.TrimEnd('/');

#if DEBUG
            System.Diagnostics.Debug.WriteLine(instructionPath);
#endif
            #endregion instructionPath

            logMessage = MessageTemplateHelper.ParseTemplate(LogPattern ?? "", parameters);

            if (doExecute && !context.InAbortState)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                #region DoLog
                if (DoLog)
                {
                    context.Log(instructionPath, "Start", logMessage);
                }
                #endregion DoLog

                try
                {
                    Execute(context, parameters);
                }
                catch (InstructionException iex)
                {
                    context.InAbortState = iex.Abort;
                    context.InErrorState = true;
                    context.LogError(instructionPath, "Error", (iex.Message + Environment.NewLine + iex.Info).Replace("'", "''"));
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

                    context.LogError(instructionPath, "Error", message.Replace("'", "''"));
                    #endregion Exception
                }

                sw.Stop();

                #region DoLog
                if (DoLog)
                {
                    context.Log(instructionPath, $"End - {sw.Elapsed.ToString("G")}", logMessage);
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
            //output = output.Replace("{$Year$}", DateTime.Now.ToString("yyyy"));
            //output = output.Replace("{$Month$}", DateTime.Now.ToString("MM"));
            //output = output.Replace("{$MonthName$}", DateTime.Now.ToString("MMM"));

            return output;
        }

        #endregion Methods
    }
}
