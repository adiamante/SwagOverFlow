using SwagOverFlow.Iterator;
using SwagOverFlow.Utils;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;

namespace Dreamporter.Core
{
    public class TemplateInstruction : Instruction
    {
        String _templateKey;
        GroupInstruction _template;

        #region Type
        public override Type Type { get { return typeof(TemplateInstruction); } }
        #endregion Type
        #region TemplateKey
        public String TemplateKey
        {
            get { return _templateKey; }
            set { SetValue(ref _templateKey, value); }
        }
        #endregion TemplateKey
        #region Template
        public GroupInstruction Template
        {
            get { return _template; }
            set { SetValue(ref _template, value); }
        }
        #endregion Template

        public override void RunHandler(RunContext context, RunParams rp)
        {
            String strTemplate = JsonHelper.ToJsonString(Template);
            String resolvedInstructions = Instruction.ResolveParameters(strTemplate, Options.Dict);
            
            GroupInstruction groupInstruction = JsonHelper.ToObject<GroupInstruction>(resolvedInstructions);

            #region WebRequest
            if (Options.Dict.ContainsKey("WebRequest.Headers"))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    if (ins is WebRequestInstruction wri)
                    {
                        wri.Headers = JsonHelper.ToObject<List<KeyValuePairViewModel<String, String>>>(Options.Dict["WebRequest.Headers"]);
                    }
                }
            }

            if (Options.Dict.ContainsKey("WebRequest.UrlParams"))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    if (ins is WebRequestInstruction wri)
                    {
                        wri.UrlParams = JsonHelper.ToObject<List<KeyValuePairViewModel<String, String>>>(Options.Dict["WebRequest.UrlParams"]);
                    }
                }
            }

            if (Options.Dict.ContainsKey("WebRequest.ParameterColumns"))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    if (ins is WebRequestInstruction wri)
                    {
                        wri.ParameterColumns = JsonHelper.ToObject<List<KeyValuePairViewModel<String, String>>>(Options.Dict["WebRequest.ParameterColumns"]);
                    }
                }
            }
            #endregion WebRequest

            #region Cache
            if (Options.Dict.ContainsKey("Cache.Enabled") && Boolean.TryParse(Options.Dict["Cache.Enabled"], out Boolean cacheEnabled))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    ins.CacheProperties.Enabled = cacheEnabled;
                }
            }

            if (Options.Dict.ContainsKey("Cache.ExpiresInMinutes") && Int32.TryParse(Options.Dict["Cache.ExpiresInMinutes"], out Int32 expiresInMinutes))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    ins.CacheProperties.ExpiresInMinutes = expiresInMinutes;
                }
            }

            if (Options.Dict.ContainsKey("Cache.VersionPattern"))
            {
                SwagItemPreOrderIterator<Instruction> insIterator = new SwagItemPreOrderIterator<Instruction>(groupInstruction);
                for (Instruction ins = insIterator.First(); !insIterator.IsDone; ins = insIterator.Next())
                {
                    ins.CacheProperties.VersionPattern = Options.Dict["Cache.VersionPattern"];
                }
            }
            #endregion Cache

            groupInstruction.Run(context, rp);
        }
    }
}
