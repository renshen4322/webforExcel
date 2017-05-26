using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon
{
    public class HttpRequstHelper
    {
        public static APIResponse OperateRequest(string methodType, string url, Dictionary<string, string> httpParams, string contentType = null)
        {
            try
            {
                if (methodType == EnumExtenstions.GetDescription(MethodType.GET) && httpParams != null && httpParams.Count > 0)
                {
                    var formDataString = FormatParams(httpParams);
                    url += "?" + formDataString;

                }
                // LogHelper.Debug("begin request, url:" + url);
                var req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = methodType;

                if (methodType == EnumExtenstions.GetDescription(MethodType.POST))
                {
                    if (contentType == EnumExtenstions.GetDescription(ContentType.Form))
                    {
                        req.ContentType = "application/x-www-form-urlencoded";

                        // httpParams.Add("CommandId", Guid.NewGuid().ToString());
                        using (var rs = req.GetRequestStream())
                        {
                            var formDataString = FormatParams(httpParams);
                            var formData = Encoding.UTF8.GetBytes(formDataString);
                            rs.Write(formData, 0, formData.Length);
                        }
                    }
                    else if (contentType == EnumExtenstions.GetDescription(ContentType.JSON))
                    {
                        req.ContentType = "application/json";
                        using (var rs = req.GetRequestStream())
                        {
                            var jsonDataString = FormatParamsToJSON(httpParams);
                            var formData = Encoding.UTF8.GetBytes(jsonDataString);
                            rs.Write(formData, 0, formData.Length);
                        }

                    }

                }

                var response = req.GetResponse();

                var result = string.Empty;
                using (var receivedStream = response.GetResponseStream())
                {
                    var streamReader = new StreamReader(receivedStream);
                    result = streamReader.ReadToEnd();
                }
                var httpWebResponse = response as System.Net.HttpWebResponse;
                var respon = new APIResponse
                {
                    StatusCode = (int)httpWebResponse.StatusCode,
                    Body = result
                };
                response.Close();
                return respon;
            }
            catch (WebException ex)
            {

                StreamReader sr = new StreamReader(ex.Response.GetResponseStream());

                var httpWebResponse = ex.Response as System.Net.HttpWebResponse;
                var result = new APIResponse
                {
                    StatusCode = (int)httpWebResponse.StatusCode,
                    Body = sr.ReadToEnd()
                };
                return result;
            }
        }

        public static APIResponse OperateRequest(string methodType, string url, Dictionary<string, string> httpParams, Dictionary<string, string> hearderParams, string contentType = null)
        {
            try
            {

                var req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = methodType;
                if (hearderParams != null)
                {
                    foreach (KeyValuePair<string, string> item in hearderParams)
                    {
                        req.Headers.Add(item.Key, item.Value);
                    }
                }

                if (methodType == EnumExtenstions.GetDescription(MethodType.POST))
                {
                    if (contentType == EnumExtenstions.GetDescription(ContentType.Form))
                    {
                        req.ContentType = "application/x-www-form-urlencoded";

                        // httpParams.Add("CommandId", Guid.NewGuid().ToString());
                        using (var rs = req.GetRequestStream())
                        {
                            var formDataString = FormatParams(httpParams);
                            var formData = Encoding.UTF8.GetBytes(formDataString);
                            rs.Write(formData, 0, formData.Length);
                        }
                    }
                    else if (contentType == EnumExtenstions.GetDescription(ContentType.JSON))
                    {
                        req.ContentType = "application/json";
                        using (var rs = req.GetRequestStream())
                        {
                            var jsonDataString = FormatParamsToJSON(httpParams);
                            var formData = Encoding.UTF8.GetBytes(jsonDataString);
                            rs.Write(formData, 0, formData.Length);
                        }

                    }
                }

                var response = req.GetResponse();

                var result = string.Empty;
                using (var receivedStream = response.GetResponseStream())
                {
                    var streamReader = new StreamReader(receivedStream);
                    result = streamReader.ReadToEnd();
                }
                var httpWebResponse = response as System.Net.HttpWebResponse;
                var respon = new APIResponse
                {
                    StatusCode = (int)httpWebResponse.StatusCode,
                    Body = result

                };
                response.Close();
                return respon;
            }
            catch (WebException ex)
            {

                StreamReader sr = new StreamReader(ex.Response.GetResponseStream());

                var httpWebResponse = ex.Response as System.Net.HttpWebResponse;
                var result = new APIResponse
                {
                    StatusCode = (int)httpWebResponse.StatusCode,
                    Body = sr.ReadToEnd()
                };
                return result;
            }
        }

        public static string FormatParams(Dictionary<string, string> httpParams)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in httpParams)
            {
                builder.Append(kvp.Key + "=" + kvp.Value + "&");
            }
            return builder.ToString().Trim('&');
        }
        public static string FormatParamsToJSON(Dictionary<string, string> httpParams)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            var data = new { };
            foreach (KeyValuePair<string, string> kvp in httpParams)
            {
                builder.Append("\"" + kvp.Key + "\":\"" + kvp.Value + "\",");
            }

            return builder.ToString().Trim(',') + "}";
        }


        public static APIResponse PostByForm(string url, Dictionary<string, string> httpParams)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), url, httpParams, EnumExtenstions.GetDescription(ContentType.Form));
        }
        public static APIResponse PostByJSON(string url, Dictionary<string, string> httpParams)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), url, httpParams, EnumExtenstions.GetDescription(ContentType.JSON));
        }

        public static APIResponse Get(string url, Dictionary<string, string> httpParams)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.GET), url, httpParams);
        }
        public static APIResponse PostByForm(string url, Dictionary<string, string> httpParams, Dictionary<string, string> hearderParams)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), url, httpParams, hearderParams, EnumExtenstions.GetDescription(ContentType.Form));
        }
        public static APIResponse PostByJSON(string url, Dictionary<string, string> httpParams, Dictionary<string, string> hearderParams)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.POST), url, httpParams, hearderParams, EnumExtenstions.GetDescription(ContentType.JSON));
        }
        public static APIResponse Get(string url, Dictionary<string, string> httpParams, Dictionary<string, string> hearderParams = null)
        {
            return OperateRequest(EnumExtenstions.GetDescription(MethodType.GET), url, httpParams, hearderParams);
        }

    }

    public class APIResponse
    {
        public int StatusCode { get; set; }
        public object Body { get; set; }     
    }
    public enum MethodType
    {
        [Description("POST")]
        POST,
        [Description("GET")]
        GET,
        [Description("PUT")]
        PUT
    }
    public enum ContentType
    {
        [Description("application/json")]
        JSON,
        [Description("application/x-www-form-urlencoded")]
        Form
    }
}
