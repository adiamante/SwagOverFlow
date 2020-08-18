using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SwagOverFlow.Data.Clients
{
    #region WebRequestType
    public enum WebRequestType
    {
        GET,
        GET_OFFSET,
        GET_PAGE,
        CANCEL,
        DELETE,
        SEND,
        POST,
        POST_OFFSET,
        POST_PAGE,
        PUT
    }
    #endregion WebRequestType

    #region WebRequestPostContentType
    public enum WebRequestPostContentType
    {
        JSON,
        XML,
        XML_STRING,
        SOAP,
        SOAP_STRING,
        STRING,
        FORM_URL_ENCODED
    }
    #endregion WebRequestPostContentType

    #region WebRequestContentType
    public enum WebRequestContentType
    {
        JSON,
        XML,
    }
    #endregion WebRequestContentType

    public class WebRequestClient : ViewModelBase
    {
        #region Private Variables
        String _name, _baseURL, _requestPath, _postContent, _limitField = "limit", _offSetField = "offset", _pageField = "page";
        WebRequestType _requestType;
        WebRequestPostContentType _postContentType;
        Int32 _limit;
        Boolean _forceFixXml;
        #endregion Private Variables

        #region Properties

        #region BaseURL
        public String BaseURL
        {
            get { return _baseURL; }
            set { SetValue(ref _baseURL, value); }
        }
        #endregion BaseURL

        #region RequestPath
        public String RequestPath
        {
            get { return _requestPath; }
            set { SetValue(ref _requestPath, value); }
        }
        #endregion RequestPath

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
        public Dictionary<String, String> Headers { get; set; } = new Dictionary<string, string>();
        #endregion Headers

        #region UrlParams
        public Dictionary<String, String> UrlParams { get; set; } = new Dictionary<string, string>();
        #endregion UrlParams

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

        #region Limit
        public Int32 Limit
        {
            get { return _limit; }
            set { SetValue(ref _limit, value); }
        }
        #endregion Limit

        #region URLParamsString
        [JsonIgnore]
        public String URLParamsString
        {
            get
            {
                if (UrlParams == null || UrlParams.Count == 0)
                {
                    return "";
                }
                return String.Join("&", UrlParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }
        }
        #endregion URLParamsString

        #region ForceFixXml
        public Boolean ForceFixXml
        {
            get { return _forceFixXml; }
            set { SetValue(ref _forceFixXml, value); }
        }
        #endregion ForceFixXml

        #region FullURL
        public String FullURL
        {
            get
            {
                String strParam = URLParamsString;
                RequestPath = RequestPath.Replace('\\', '/');
                String urlPath = string.Format("{0}{1}{2}{3}", RequestPath, RequestPath.EndsWith("/") || strParam == "" ? "" : "/", strParam != "" ? "?" : "", strParam);
                return $"{(BaseURL.TrimEnd(new char[] { '/' }))}/{(urlPath.TrimStart(new char[] { '/' }))}";
            }
        }
        #endregion FullURL

        #endregion Properties

        #region Initialization
        public WebRequestClient()
        {

            _name = _baseURL = _requestPath = _postContent = "";
            _forceFixXml = false;
        }
        #endregion Initialization

        #region GetResponse
        public HttpResponseMessage GetResponse()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(FullURL);
#endif

            #region Local Variables
            WebRequestPostContentType postContentType = PostContentType;
            String strPostContent = PostContent;
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseURL);
            HttpResponseMessage httpResponse = null;
            #endregion Local Variables

            #region Resolve Headers
            foreach (KeyValuePair<String, String> kvp in Headers)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(kvp.Key, kvp.Value);
            }
            #endregion Resolve Headers

            #region Resolve URL Path
            String strParam = URLParamsString;
            RequestPath = RequestPath.Replace('\\', '/');
            String urlPath = string.Format("{0}{1}{2}{3}", RequestPath, RequestPath.EndsWith("/") || strParam == "" ? "" : "/", strParam != "" ? "?" : "", strParam);
            #endregion Resolve URL Path

            switch (RequestType)
            {
                case WebRequestType.GET_OFFSET:
                case WebRequestType.GET_PAGE:
                case WebRequestType.POST_OFFSET:
                case WebRequestType.POST_PAGE:
                    Int32 limit = 100, offset = 0, page = 0, maxPages = 20, maxChildren = 0;
                    JObject jObjectMerged = new JObject(), jObjectTemp;
                    Dictionary<string, string> dictPostContent = null; //Used for POST_OFFSET and POST_PAGE

                    #region Initialization by Request Type
                    switch (RequestType)
                    {
                        case WebRequestType.GET_OFFSET:
                            if (UrlParams.ContainsKey(LimitField))
                            {
                                Int32.TryParse(UrlParams[LimitField], out limit);
                            }
                            else
                            {
                                UrlParams.Add(LimitField, limit.ToString());
                            }

                            if (UrlParams.ContainsKey(OffsetField))
                            {
                                Int32.TryParse(UrlParams[OffsetField], out offset);
                            }
                            else
                            {
                                UrlParams.Add(OffsetField, offset.ToString());
                            }
                            break;
                        case WebRequestType.GET_PAGE:
                            if (UrlParams.ContainsKey(LimitField))
                            {
                                Int32.TryParse(UrlParams[LimitField], out limit);
                            }
                            else
                            {
                                UrlParams.Add(LimitField, limit.ToString());
                            }

                            if (UrlParams.ContainsKey(PageField))
                            {
                                Int32.TryParse(UrlParams[PageField], out page);
                            }
                            else
                            {
                                UrlParams.Add(PageField, page.ToString());
                            }
                            break;
                        case WebRequestType.POST_OFFSET:
                            dictPostContent = new Dictionary<string, string>
                            {
                                { LimitField , Limit.ToString() },
                                { OffsetField , offset.ToString()}
                            };
                            break;
                        case WebRequestType.POST_PAGE:
                            dictPostContent = new Dictionary<string, string>
                            {
                                { LimitField , Limit.ToString() },
                                { PageField , page.ToString()}
                            };
                            break;
                    }
                    #endregion Initialization by Request Type

                    do
                    {
                        #region Loop Initialization
                        switch (RequestType)
                        {
                            case WebRequestType.GET_OFFSET:
                                UrlParams[OffsetField] = offset.ToString();
                                break;
                            case WebRequestType.GET_PAGE:
                                UrlParams[PageField] = page.ToString();
                                break;
                            case WebRequestType.POST_OFFSET:
                                dictPostContent[OffsetField] = offset.ToString();
                                break;
                            case WebRequestType.POST_PAGE:
                                dictPostContent[PageField] = page.ToString();
                                break;
                        }
                        #endregion Loop Initialization

                        strParam = URLParamsString;
                        urlPath = string.Format("{0}{1}{2}{3}", RequestPath, RequestPath.EndsWith("/") ? "" : "/", strParam != "" ? "?" : "", strParam);

                        #region Get the Response by Request Type
                        switch (RequestType)
                        {
                            case WebRequestType.GET_OFFSET:
                            case WebRequestType.GET_PAGE:
                                httpResponse = httpClient.GetAsync(urlPath).Result;
                                break;
                            case WebRequestType.POST_OFFSET:
                            case WebRequestType.POST_PAGE:
                                FormUrlEncodedContent postContent = new FormUrlEncodedContent(dictPostContent);
                                httpResponse = httpClient.PostAsync(urlPath, postContent).Result;
                                break;
                        }
                        #endregion Get the Response by Request Type

                        #region Break out if StatusCode is not OK
                        if (httpResponse.StatusCode != HttpStatusCode.OK)
                        {
                            break;
                        }
                        #endregion Break out if StatusCode is not OK

                        String strResponse = httpResponse.Content.ReadAsStringAsync().Result;
                        if (strResponse.StartsWith("[") && strResponse.EndsWith("]"))
                        {
                            strResponse = $"{{\"root\":{strResponse}}}";
                        }
                        jObjectTemp = JObject.Parse(strResponse);
                        IEnumerable<JToken> allTokens = jObjectTemp.SelectTokens("*");
                        maxChildren = allTokens.Select(t => t.Count()).Max();
                        jObjectMerged.Merge(jObjectTemp);
                        page++;
                        offset += limit;
                    } while (maxChildren > 1 && maxChildren >= limit && page <= maxPages);

                    httpResponse.Content = new StringContent(jObjectMerged.ToString(), Encoding.UTF8, "application/json");
                    break;
                case WebRequestType.CANCEL:
                    httpClient.CancelPendingRequests();
                    break;
                case WebRequestType.DELETE:
                    httpResponse = httpClient.DeleteAsync(urlPath).Result;
                    break;
                case WebRequestType.GET:
                default:
                    httpResponse = httpClient.GetAsync(urlPath).Result;
                    break;
                case WebRequestType.POST:
                case WebRequestType.PUT:
                case WebRequestType.SEND:
                    #region Resolve MediaType
                    String mediaType;

                    switch (postContentType)
                    {
                        case WebRequestPostContentType.STRING:
                        case WebRequestPostContentType.FORM_URL_ENCODED:
                        default:
                            mediaType = "text/plain";
                            break;
                        case WebRequestPostContentType.JSON:
                            mediaType = "application/json";
                            break;
                        case WebRequestPostContentType.XML:
                        case WebRequestPostContentType.SOAP:
                        case WebRequestPostContentType.SOAP_STRING:
                            mediaType = "text/xml";
                            break;
                    }
                    #endregion Resolve MediaType

                    #region Resolve Content
                    HttpContent content;

                    switch (postContentType)
                    {
                        case WebRequestPostContentType.XML:
                        case WebRequestPostContentType.JSON:
                        case WebRequestPostContentType.STRING:
                        default:
                            content = new StringContent(strPostContent, Encoding.UTF8, mediaType);
                            break;
                        case WebRequestPostContentType.FORM_URL_ENCODED:
                            Dictionary<String, String> formData = strPostContent.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(part => part.Split('='))
                               .ToDictionary(split => split[0], split => split[1]);
                            content = new FormUrlEncodedContent(formData);
                            break;
                        case WebRequestPostContentType.SOAP:
                        case WebRequestPostContentType.SOAP_STRING:
                            content = new StringContent(String.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                        <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
                            <s:Body>
                                {0}
                            </s:Body>
                        </s:Envelope>", strPostContent), Encoding.UTF8, mediaType);
                            break;
                    }
                    #endregion Resolve Content

                    switch (RequestType)
                    {
                        case WebRequestType.POST:
                            httpResponse = httpClient.PostAsync(urlPath, content).Result;
                            break;
                        case WebRequestType.PUT:
                            httpResponse = httpClient.PutAsync(urlPath, content).Result;
                            break;
                        case WebRequestType.SEND:
                            HttpRequestMessage httpContent = new HttpRequestMessage(HttpMethod.Post, urlPath) { Content = content, Version = HttpVersion.Version11 };
                            httpResponse = httpClient.SendAsync(httpContent).Result;
                            break;
                    }
                    break;
            }

            return httpResponse;
        }
        #endregion GetResponse

        #region GetResponseDataSet
        public DataSet GetResponseDataSet()
        {
            HttpResponseMessage response = GetResponse();
            return WebRequestHelper.GenerateDataSet(response);
        }
        #endregion GetResponseDataSet
    }

    #region WebRequestHelper
    public static class WebRequestHelper
    {

        /// <summary>
        /// Generates as DataSet given an HttpResponseMessage input
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static DataSet GenerateDataSet(HttpResponseMessage httpResponse, WebRequestContentType contentType = WebRequestContentType.JSON)
        {
            return GenerateDataSet(httpResponse.Content.ReadAsStringAsync().Result, contentType);
        }

        /// <summary>
        /// Generates a DataSet given a string
        /// </summary>
        /// <param name="strResponse"></param>
        /// <returns></returns>
        public static DataSet GenerateDataSet(String strResponse, WebRequestContentType contentType = WebRequestContentType.JSON)
        {
            DataSet dsResponse = new DataSet();

            if (strResponse != "")
            {
                XmlDocument xmlDocument = new XmlDocument();

                switch (contentType)
                {
                    case WebRequestContentType.XML:
                        xmlDocument.LoadXml(strResponse);
                        break;
                    case WebRequestContentType.JSON:
                    default:
                        if (strResponse.StartsWith("[") && strResponse.EndsWith("]"))
                        {
                            strResponse = $"{{\"root\":{strResponse}}}";
                        }
                        //Conversion to JObject is to prevent automatic DateTime columns because they break when the value is an empty string
                        JObject jResponse = JsonConvert.DeserializeObject<JObject>(strResponse, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

                        //Handle Array with one value
                        if (jResponse.Count == 1 && jResponse["root"] is JArray && ((JArray)jResponse["root"]).Count == 1 && ((JArray)jResponse["root"])[0] is JValue)
                        {
                            DataTable dt = new DataTable("root");
                            dt.Columns.Add("root_Text");
                            dt.Rows.Add(new object[] { ((JArray)jResponse["root"])[0] });
                            dsResponse.Tables.Add(dt);
                            return dsResponse;
                        }
                        else
                        {
                            xmlDocument = (XmlDocument)JsonConvert.DeserializeXmlNode(jResponse.ToString(), "root");
                        }
                        break;
                }

                dsResponse.ReadXmlTryFix(xmlDocument);
            }

            return dsResponse;
        }

        #region XML

        public static bool ReadXmlTryFix(this DataSet ds, XmlDocument doc, Boolean forceFixXML = false)
        {
            try
            {
                if (forceFixXML)
                {
                    XDocument xDoc = XDocument.Parse(doc.OuterXml);

                    FixElemNestedRelation(xDoc.Root);
                    FixSimpleContent(xDoc.Root);

                    ds.ReadXml(new StringReader(xDoc.Root.ToString()));
                }
                else
                {
                    ds.ReadXml(new XmlNodeReader(doc));
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("SimpleContent") || ex.Message.Contains("nested"))
                {
                    XDocument xDoc = XDocument.Parse(doc.OuterXml);
                    Exception exCurrent = ex;

                    FlattenDigitTags(xDoc.Root);

                    #region FixSimpleContent
                    if (exCurrent.Message.Contains("SimpleContent"))
                    {
                        FixSimpleContent(xDoc.Root);
                    }
                    #endregion FixSimpleContent

                    #region Get currentNestedName
                    string lastNestedName = "", currentNestedName = "";
                    Regex pattern = new Regex(@"(named '(?<name>.+?)')|(DataTable '(?<name>.+?)')");
                    Match match = pattern.Match(exCurrent.Message);
                    if (match.Success)
                    {
                        currentNestedName = match.Groups["name"].Value;
                    }
                    #endregion Get currentNestedName

                    do
                    {
                        #region FixElemNestedRelation
                        if (currentNestedName != "")
                        {
                            FixElemNestedRelation(xDoc.Root, currentNestedName);
                        }
                        #endregion FixElemNestedRelation

                        try
                        {
                            #region Clear Table Relations then Remove all Tables from DataSet
                            for (int t = 0; t < ds.Tables.Count; t++)
                            {
                                DataTable dt = ds.Tables[t];
                                for (int r = dt.ChildRelations.Count - 1; r >= 0; r--)
                                {
                                    dt.ChildRelations[r].ChildTable.Constraints.Remove(dt.ChildRelations[r].RelationName);
                                    dt.ChildRelations.RemoveAt(r);
                                }
                                dt.ChildRelations.Clear();
                                dt.ParentRelations.Clear();
                                dt.Constraints.Clear();
                            }
                            ds.Tables.Clear();
                            #endregion Clear Table Relations then Remove all Tables from DataSet

                            ds.ReadXml(new StringReader(xDoc.Root.ToString()));
                            lastNestedName = currentNestedName;
                        }
                        catch (Exception ex2)
                        {
                            #region If nesting error get name else throw exception
                            //Potential infinite loop if consecutive exceptions keep having different messages
                            exCurrent = ex2;
                            lastNestedName = currentNestedName;
                            match = pattern.Match(exCurrent.Message);
                            if (match.Success)
                            {
                                currentNestedName = match.Groups["name"].Value;
                            }
                            else
                            {
                                throw ex2;
                            }
                            #endregion If nesting error get name else throw exception
                        }
                    }
                    while (exCurrent.Message.Contains("nested") && lastNestedName != currentNestedName);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static void FixElemNestedRelation(this XElement elem)
        {
            foreach (XNode child in elem.Nodes())
            {
                if (child is XElement)
                {
                    (child as XElement).FixElemNestedRelationChild(elem.Name.ToString(), 1);
                }
            }
        }

        public static void FixElemNestedRelation(this XElement elem, string targetTag)
        {
            foreach (XNode child in elem.Nodes())
            {
                if (child is XElement)
                {
                    (child as XElement).FixElemNestedRelationChild(targetTag, 1);
                }
            }
        }

        public static void FixElemNestedRelationChild(this XElement elem, String tag, int level)
        {
            if (elem.Name.ToString() == tag && level > 1)
            {
                elem.Name = XName.Get(string.Format("{0}_{1}", tag, level));
            }

            foreach (XNode child in elem.Nodes())
            {
                if (child is XElement)
                {
                    FixElemNestedRelationChild(child as XElement, tag, level + 1);
                }
            }

            //FixElemNestedRelation(elem);
        }

        public static void FixSimpleContent(this XElement elem)
        {
            foreach (XElement child in elem.Descendants())
            {
                if (!child.HasElements && child.HasAttributes)
                {
                    if (child.Value != "")
                    {
                        String myValue = child.Value;
                        child.Value = "";
                        child.Add(new XElement("Value", myValue));
                    }

                    foreach (XAttribute xAttr in child.Attributes())
                    {
                        child.Add(new XElement(child.Name.ToString() + "_" + xAttr.Name, xAttr.Value));
                    }
                    child.Attributes().Remove();
                }
            }
        }

        public static void FlattenDigitTags(this XElement elem)
        {
            List<String> parentNames = new List<string>();
            foreach (XElement child in elem.Descendants())
            {
                String childName = child.Name.ToString();
                if (childName.StartsWith("_x"))
                {
                    //Ex: _x0031_ => 31 (hex) => Digit One "1"
                    Int32 hex = Convert.ToInt32(childName.Substring(4, 2), 16);
                    String originalName = Char.ConvertFromUtf32(hex) + childName.Substring(7);

                    String parentName = child.Parent.Name.ToString();
                    String newName = "";
                    if (parentName.EndsWith("s"))
                    {
                        newName = parentName.Substring(0, parentName.Length - 1);
                    }
                    else
                    {
                        newName = parentName + "Item";
                    }

                    child.Name = XName.Get(newName);
                    if (!parentNames.Contains(parentName))
                    {
                        parentNames.Add(parentName);
                    }
                }
            }

            foreach (String parentName in parentNames)
            {
                FixElemNestedRelation(elem, parentName);
            }
        }
        #endregion XML

    }
    #endregion WebRequestHelper

    /* Simple Content Nesting Error Example (modifierOptionReferences is in two different heirarachis and is in modifierGroupReference multiple times)
      <root>
          <modifierGroupReferences>
          <modifierGroupReference>
              <referenceId>1</referenceId>
              <name>Catering - 12 Mini empanadas</name>
              <guid>07c7aa48-ecb5-4c63-8089-d52962738489</guid>
              <masterId>400000004288464285</masterId>
              <visibility>POS</visibility>
              <visibility>KIOSK</visibility>
              <visibility>TOAST_ONLINE_ORDERING</visibility>
              <pricingStrategy>NONE</pricingStrategy>
              <pricingRules />
              <defaultOptionsChargePrice>YES</defaultOptionsChargePrice>
              <defaultOptionsSubstitutionPricing>NO</defaultOptionsSubstitutionPricing>
              <minSelections>1</minSelections>
              <maxSelections>12</maxSelections>
              <requiredMode>REQUIRED</requiredMode>
              <isMultiSelect>true</isMultiSelect>
              <preModifierGroupReference />
              <modifierOptionReferences>2</modifierOptionReferences> => Simple Content if multiple instance in under one element
              <modifierOptionReferences>3</modifierOptionReferences> => Simple Content if multiple instance in under one element
              <modifierOptionReferences>4</modifierOptionReferences> => Simple Content if multiple instance in under one element
            </modifierGroupReference>
          </modifierGroupReferences>
          <modifierOptionReferences> => Not Simple Content
          <modifierOptionReference>
              <referenceId>2</referenceId>
              <name>Cheese Mini Empanada</name>
              <guid>fec563db-6aa7-4493-9a6f-cdf96e34c12f</guid>
              <masterId>400000003720861731</masterId>
              <description></description>
              <image />
              <visibility>POS</visibility>
              <visibility>KIOSK</visibility>
              <visibility>TOAST_ONLINE_ORDERING</visibility>
              <price>0</price>
              <pricingStrategy>BASE_PRICE</pricingStrategy>
              <pricingRules />
              <salesCategory>
                <name>Food Mods</name>
                <guid>d158fc1d-dc20-4659-9aea-7950576bfe96</guid>
              </salesCategory>
              <taxInfo>43a86a8c-1582-4b8c-89e5-78e3fb2d2518</taxInfo>
              <modifierOptionTaxInfo>
                <taxRateGuids>43a86a8c-1582-4b8c-89e5-78e3fb2d2518</taxRateGuids>
                <overrideItemTaxRates>false</overrideItemTaxRates>
              </modifierOptionTaxInfo>
              <plu></plu>
              <sku></sku>
              <calories />
              <unitOfMeasure>NONE</unitOfMeasure>
              <isDefault>false</isDefault>
              <allowsDuplicates>true</allowsDuplicates>
            </modifierOptionReference>
	        </modifierOptionReferences>
        </root>
    */
}
