﻿using Dreamporter.Builds;
using Dreamporter.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SwagOverFlow.Clients;
using SwagOverFlow.Parsing;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Dreamporter.Instructions
{
    public class WebRequestInstruction : Instruction
    {
        #region Private Variables
        String _schema;
        String _limitField = "limit", _offSetField = "offset", _pageField = "page";
        String _baseURL, _path, _postContent;
        WebRequestType _requestType;
        WebRequestPostContentType _postContentType;
        Int32 _limit;
        Boolean _forceFixXml;
        #endregion Private Variables

        #region Properties

        #region Schema
        public String Schema
        {
            get { return _schema; }
            set { SetValue(ref _schema, value); }
        }
        #endregion Schema
        #region BaseURL
        public String BaseURL
        {
            get { return _baseURL; }
            set { SetValue(ref _baseURL, value); }
        }
        #endregion BaseURL
        #region Path
        public String Path
        {
            get { return _path; }
            set { SetValue(ref _path, value); }
        }
        #endregion Path
        #region PostContent
        public String PostContent
        {
            get { return _postContent; }
            set { SetValue(ref _postContent, value); }
        }
        #endregion PostContent
        #region RequestType
        [JsonConverter(typeof(StringEnumConverter))]
        public WebRequestType RequestType
        {
            get { return _requestType; }
            set { SetValue(ref _requestType, value); }
        }
        #endregion RequestTypeS
        #region PostContentType
        [JsonConverter(typeof(StringEnumConverter))]
        public WebRequestPostContentType PostContentType
        {
            get { return _postContentType; }
            set { SetValue(ref _postContentType, value); }
        }
        #endregion PostContentType
        #region Headers
        public KeyValuePair<String, String>[] Headers { get; set; }
        #endregion Headers
        #region UrlParams
        public KeyValuePair<String, String>[] UrlParams { get; set; }
        #endregion UrlParams
        #region Limit
        public Int32 Limit
        {
            get { return _limit; }
            set { SetValue(ref _limit, value); }
        }
        #endregion Limit
        #region ForceFixXml
        public Boolean ForceFixXml
        {
            get { return _forceFixXml; }
            set { SetValue(ref _forceFixXml, value); }
        }

        #endregion ForceFixXml
        #region ParameterColumns
        public KeyValuePair<String, String>[] ParameterColumns { get; set; }
        #endregion ParameterColumns
        #region LimitField
        public String LimitField
        {
            get { return _limitField; }
            set { SetValue(ref _limitField, value); }
        }
        #endregion LimitField
        #region OffsetField
        public String OffsetField
        {
            get { return _offSetField; }
            set { SetValue(ref _offSetField, value); }
        }
        #endregion OffsetField
        #region PageField
        public String PageField
        {
            get { return _pageField; }
            set { SetValue(ref _pageField, value); }
        }
        #endregion PageField

        #endregion Properties

        #region Methods
        public override void Execute(RuntimeContext context, Dictionary<String, String> parameters)
        {
            String cacheAddress = "", cacheKey = "", cacheVersion = "";
            WebRequestClient webRequestClient = new WebRequestClient();
            String strApply = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            DataSet dsResult = null;

            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                strApply = strApply.Replace($"{{{kvp.Key}}}", kvp.Value);
            }

            JObject jsonApply = JsonConvert.DeserializeObject<JObject>(strApply, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            JsonHelper.ApplyJson(webRequestClient, jsonApply);

            String schemaName = (Schema == "" ? null : Schema) ?? Name ?? "";

            HttpResponseMessage response = null;
            Boolean useCache = CacheProperties?.Enabled ?? false && context.CacheProvider != null;

            //if (parameters.ContainsKey("IgnoreCache") && Boolean.TryParse(parameters["IgnoreCache"], out bool ignoreCache) && ignoreCache)
            //{
            //    useCache = false;
            //}

            if (useCache)
            {
                #region Get Cache Address and Key
                //cacheAddress = Instruction.ResolveParameters(CacheProperties?.AddressPattern ?? "", parameters);
                cacheAddress = MessageTemplateHelper.ParseTemplate(CacheProperties?.AddressPattern ?? "", parameters);
                cacheKey = Instruction.ResolveParameters(CacheProperties?.KeyPattern ?? "", parameters);
                cacheVersion = Instruction.ResolveParameters(CacheProperties?.VersionPattern ?? "", parameters);
                #endregion Get Cache Address and Key

                #region TryUseCache
                CacheRecord responseCache = context.CacheProvider.Find(cacheAddress, cacheKey, cacheVersion);

                if (responseCache != null && DateTime.Now < responseCache.CacheExpiresIn)
                {
                    dsResult = JsonConvert.DeserializeObject<DataSet>(Encoding.ASCII.GetString(responseCache.CacheData), new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });

                    #region Removed for compatibility
                    //response = new HttpResponseMessage();
                    //JObject jResponse = JObject.Parse(Encoding.ASCII.GetString(responseCache.CacheData));
                    //response.Content = new StringContent(jResponse["ResponseRaw"].ToString());
                    //JArray jHeaders = (JArray)jResponse["ResponseInfo"]["Content"]["Headers"];

                    //foreach (JToken jKvp in jHeaders)
                    //{
                    //    String headerKey = jKvp["Key"].ToString();
                    //    String headerValue = jKvp["Value"].ToString();

                    //    switch (headerKey)
                    //    {
                    //        case "Content-Type":
                    //            String[] split = headerValue.TrimStart('[').TrimEnd(']').Replace("\"", "").Trim().Split(';');
                    //            response.Content.Headers.ContentType.MediaType = split[0].Trim();
                    //            if (split.Length > 1)
                    //            {
                    //                response.Content.Headers.ContentType.CharSet = split[1].Trim().Replace("charset=", "");
                    //            }
                    //            break;
                    //        default:
                    //            response.Content.Headers.TryAddWithoutValidation(headerKey, headerValue);
                    //            break;
                    //    }
                    //}
                    //jResponse["ResponseInfo"]["Content"]["Headers"] = null;
                    //jResponse["ResponseInfo"]["RequestMessage"] = null;

                    //JsonHelper.ApplyJson(response, (JObject)jResponse["ResponseInfo"]);
                    #endregion Removed for compatibility

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Cache Used: {cacheAddress}[{cacheKey}]");
#endif
                }
                #endregion TryUseCache
            }

            Int32 tryAttempts = 1;
            if (dsResult == null)
            {
                #region GetResponse
                response = webRequestClient.GetResponse();

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    //Retry 1
                    response = webRequestClient.GetResponse();
                    tryAttempts++;
                }

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    //Retry 2
                    response = webRequestClient.GetResponse();
                    tryAttempts++;
                }

                #region GZip handling
                if (response.Content.Headers.ContentEncoding.ToString().ToLower().Contains("gzip"))
                {
                    GZipStream gStream = new GZipStream(response.Content.ReadAsStreamAsync().Result, CompressionMode.Decompress);
                    StreamReader sReader = new StreamReader(gStream);
                    HttpResponseMessage responseTemp = new HttpResponseMessage();
                    responseTemp.Content = new StringContent(sReader.ReadToEnd());
                    foreach (KeyValuePair<String, IEnumerable<String>> header in response.Content.Headers)
                    {
                        if (header.Key != "Content-Encoding")
                        {
                            responseTemp.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                    responseTemp.Content.Headers.ContentType = response.Content.Headers.ContentType;
                    response = responseTemp;
                }
                #endregion GZip handling

                #endregion GetResponse

                if (!response.IsSuccessStatusCode)
                {
                    throw new InstructionException($"Failed request. Retry Attempts: {tryAttempts}")
                    {
                        Abort = false,
                        Info = JsonConvert.SerializeObject(response, Formatting.Indented)
                    };
                }

                WebRequestContentType responseContentType = WebRequestContentType.JSON;

                switch (response.Content.Headers.ContentType.MediaType)
                {
                    case "text/xml":
                        responseContentType = WebRequestContentType.XML;
                        break;
                }

                dsResult = WebRequestHelper.GenerateDataSet(response, responseContentType);
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

                    //JObject jResponse = new JObject();
                    //jResponse["ResponseInfo"] = JObject.Parse(JsonConvert.SerializeObject(response));
                    //jResponse["ResponseRaw"] = response.Content.ReadAsStringAsync().Result;

                    //responseCache.CacheData = Encoding.ASCII.GetBytes(jResponse.ToString());
                    responseCache.CacheData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(dsResult));
                    context.CacheProvider.Save(responseCache);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"Cache Saved: {cacheAddress}[{cacheKey}]");
#endif
                }
                #endregion SaveCache
            }

#if DEBUG
            //context.AddRequestInfo($"{schemaName}", JsonConvert.SerializeObject(webRequestClient, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            //context.AddResponseInfo($"{schemaName}", JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));

            //switch (responseContentType)
            //{
            //    case WebRequestContentType.JSON:
            //        context.AddResponseJson($"{schemaName}", response.Content.ReadAsStringAsync().Result);
            //        break;
            //}
#endif

            foreach (DataTable dt in dsResult.Tables)
            {
                dt.TableName = $"{schemaName}.{dt.TableName}";

                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnMapping == MappingType.SimpleContent)
                    {
                        dc.ColumnMapping = MappingType.Element;
                    }
                }

                if (ParameterColumns != null)
                {
                    foreach (KeyValuePair<String, String> pcKvp in ParameterColumns.Reverse())
                    {
                        if (parameters.ContainsKey(pcKvp.Key))
                        {
                            DataColumn dc = dt.Columns.Add(pcKvp.Value);
                            dc.Expression = $"'{parameters[pcKvp.Key]}'";
                            dc.SetOrdinal(0);
                        }
                    }
                }

                context.AddTables(dt);
            }
        }
        #endregion Methods
    }
}
