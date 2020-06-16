using Dreamporter.Builds;
using Dreamporter.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Clients;
using SwagOverFlow.Parsing;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Dreamporter.Instructions
{
    public class ForEachTableWebRequestInstruction : WebRequestInstruction
    {
        #region Private Variables
        String _query;
        #endregion Private Variables

        #region Properties
        #region Type
        public override Type Type { get { return typeof(ForEachTableWebRequestInstruction); } }
        #endregion Type
        #region Schema
        public String Query
        {
            get { return _query; }
            set { SetValue(ref _query, value); }
        }
        #endregion Schema
        #endregion Properties

        #region Methods
        public override void Execute(RuntimeContext context, Dictionary<String, String> parameters)
        {
            String cacheAddress = "", cacheKey = "", cacheVersion = "";
            WebRequestClient webReqTemplate = new WebRequestClient();
            String strApply = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            JObject jsonApply = JsonConvert.DeserializeObject<JObject>(strApply, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateParseHandling = DateParseHandling.None });
            JsonHelper.ApplyJson(webReqTemplate, jsonApply);

            String schemaName = (Schema == "" ? null : Schema) ?? Name ?? "";

            String query = Query ?? "";
            query = Instruction.ResolveParameters(query, parameters);

            DataTable dtblParameters = context.Query(query);
            String strWebReqTemplate = JsonHelper.ToJsonString(webReqTemplate);
            DataSet dsResult = null;

            HttpResponseMessage response;
            Boolean useCache = CacheProperties?.Enabled ?? false && context.CacheProvider != null;

            if (useCache)
            {
                #region Get Cache Address and Key
                cacheAddress = MessageTemplateHelper.ParseTemplate(CacheProperties?.AddressPattern ?? "", parameters);
                cacheKey = Instruction.ResolveParameters(CacheProperties?.KeyPattern ?? "", parameters);
                cacheVersion = Instruction.ResolveParameters(CacheProperties?.VersionPattern ?? "", parameters);
                #endregion Get Cache Address and Key

                #region TryUseCache
                CacheRecord responseCache = context.CacheProvider.Find(cacheAddress, cacheKey, cacheVersion);

                if (responseCache != null && DateTime.Now < responseCache.CacheExpiresIn)
                {
                    dsResult = JsonConvert.DeserializeObject<DataSet>(Encoding.ASCII.GetString(responseCache.CacheData), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Cache Used: {cacheAddress}[{cacheKey}]");
#endif
                }
                #endregion TryUseCache
            }

            if (dsResult == null)
            {
                #region Get Merged DataSet based on dtblParameters
                dsResult = new DataSet();
                dsResult.EnforceConstraints = false;

                foreach (DataRow drParamSet in dtblParameters.Rows)
                {
                    Dictionary<String, String> dictParamSet = drParamSet.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => drParamSet[c].ToString());
                    String strWebRequest = Instruction.ResolveParameters(strWebReqTemplate, dictParamSet);
                    strWebRequest = Instruction.ResolveParameters(strWebRequest, parameters);

                    WebRequestClient webRequest = JsonConvert.DeserializeObject<WebRequestClient>(strWebRequest);
                    response = webRequest.GetResponse();
                    Int32 tryAttempts = 1;

                    if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    {
                        //Retry 1
                        response = webRequest.GetResponse();
                        tryAttempts++;
                    }

                    if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    {
                        //Retry 2
                        response = webRequest.GetResponse();
                        tryAttempts++;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InstructionException($"Failed request. Retry Attempts: {tryAttempts}")
                        {
                            Abort = true,
                            Info = JsonConvert.SerializeObject(response, Formatting.Indented)
                        };
                    }

                    DataSet dsTemp = WebRequestHelper.GenerateDataSet(response);
                    dsTemp.EnforceConstraints = false;

                    if (schemaName != "")
                    {
                        foreach (DataTable dt in dsTemp.Tables)
                        {
                            dt.TableName = $"{schemaName}.{dt.TableName}";
                            DataTable dtCopy = dt.Copy();
                            dtCopy.Constraints.Clear();

                            #region MappingType.SimpleContent Handling
                            foreach (DataColumn dc in dtCopy.Columns)
                            {
                                if (dc.ColumnMapping == MappingType.SimpleContent)
                                {
                                    dc.ColumnMapping = MappingType.Element;
                                }
                            }
                            #endregion MappingType.SimpleContent Handling

                            #region ParameterColumns
                            if (ParameterColumns != null)
                            {
                                foreach (KeyValuePair<String, String> pcKvp in ParameterColumns.Reverse())
                                {
                                    if (dictParamSet.ContainsKey(pcKvp.Key))
                                    {
                                        if (!dtCopy.Columns.Contains(pcKvp.Key))
                                        {
                                            DataColumn dc = dtCopy.Columns.Add(pcKvp.Value);
                                            dc.SetOrdinal(0);
                                        }

                                        foreach (DataRow dr in dtCopy.Rows)
                                        {
                                            dr[pcKvp.Value] = dictParamSet[pcKvp.Key];
                                        }
                                    }
                                }
                            }
                            #endregion ParameterColumns

                            dsResult.Merge(dtCopy);
                        }
                    }
                }
                #endregion Get Merged DataSet based on dtblParameters
            }

            if (useCache)
            {
                #region SaveCache
                CacheRecord responseCache = context.CacheProvider.Find(cacheAddress, cacheKey, cacheVersion);

                if (responseCache == null)
                {
                    DateTime expireIn = DateTime.Now.AddHours(1);    //By default expire in an hour
                    Int32 expiresInMinutes = CacheProperties?.ExpiresInMinutes ?? 60;

                    if (parameters.ContainsKey("CacheExpireOverrideTo1Hour") && Boolean.TryParse(parameters["CacheExpireOverrideTo1Hour"], out bool overrideToAnHour) && overrideToAnHour)
                    {
                        expiresInMinutes = 60;
                    }

                    if (expiresInMinutes > 0)
                    {
                        expireIn = DateTime.Now.AddMinutes(expiresInMinutes);
                    }
                    else if (expiresInMinutes < 0)                    //Don't Expire
                    {
                        expireIn = DateTime.MaxValue;
                    }

                    responseCache = new CacheRecord() { CacheAddress = cacheAddress, CacheKey = cacheKey, CacheExpiresIn = expireIn, CacheVersion = cacheVersion };
                    responseCache.CacheData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dsResult));
                    context.CacheProvider.Save(responseCache);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Cache Saved: {cacheAddress}[{cacheKey}]");
#endif
                }
                #endregion SaveCache
            }

            foreach (DataTable dt in dsResult.Tables)
            {
                #region ParameterColumns
                if (ParameterColumns != null)
                {
                    foreach (KeyValuePair<String, String> pcKvp in ParameterColumns.Reverse())
                    {
                        if (parameters.ContainsKey(pcKvp.Key) && !dt.Columns.Contains(pcKvp.Key))
                        {
                            DataColumn dc = dt.Columns.Add(pcKvp.Value);
                            dc.Expression = $"'{parameters[pcKvp.Key]}'";
                            dc.SetOrdinal(0);
                        }
                    }
                }
                #endregion ParameterColumns

                context.AddTables(dt);
            }
        }
        #endregion Methods
    }
}