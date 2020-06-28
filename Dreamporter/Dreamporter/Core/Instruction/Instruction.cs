using Dreamporter.Core;
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
    public abstract class Instruction : SwagItem<GroupInstruction, Instruction>
    {
        #region Private Members
        Int32 _instructionId;
        String _name, _infoFormat, _description;
        Boolean _isEnabled = true, _doLog = true;
        InstructionCacheProperties _cacheProperties = new InstructionCacheProperties();
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
            get { return _infoFormat; }
            set { SetValue(ref _infoFormat, value); }
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
        #endregion Properties

        #region Initialization
        public Instruction()
        {
        }
        #endregion Initialization

        #region Events
        #endregion Events

        #region Methods
        abstract public void Execute(RunContext context, Dictionary<String, String> parameters);

        public void Run(RunContext context, Dictionary<String, String> parameters)
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

            return output;
        }
        #endregion Methods
    }
}
