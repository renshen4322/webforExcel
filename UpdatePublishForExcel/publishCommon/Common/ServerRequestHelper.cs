using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace publishCommon
{
    public class ServerRequestHelper
    {
        #region GET
        /// <summary>
        /// 接口请求(Get)
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Dictionary<string, object> RequestGet(string URL)
        {
            string ObtainText = RequestGetReuse(URL);
            Dictionary<string, object> List = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ObtainText);
            return List;
        }
        public string RequestGetReuse(string URL)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.Method = "GET";
            httpWebRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            httpWebRequest.Headers.Add("Authorization", "bearer " + HttpContext.Current.Session["Token"]);
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd(); 
                }
            }
        }
        #endregion
        #region PUT
        public string RequestPutReuse(string URL, string Json,string token)
        {
            return RequestReuse(URL, Json, "PUT", "application/json",token);
        }
        #endregion
        #region POST
        /// <summary>
        /// 接口请求(Post)
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="ParamList"></param>
        /// <returns></returns>
        public Dictionary<string, object> RequestPost(string URL, Dictionary<string, string> ParamList)
        {
            string ObtainText = RequestPostReuse(URL, ParamList);
            Dictionary<string, object> List = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ObtainText);
            return List;
        }
        /// <summary>
        /// 接口请求(Post)(泛型)
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="ParamList"></param>
        /// <returns></returns>
        public List<T> RequestPost<T>(string URL, Dictionary<string, string> ParamList)
        {
            string ObtainText = RequestPostReuse(URL, ParamList);
            List<T> List = new JavaScriptSerializer().Deserialize<List<T>>(ObtainText);
            return List;
        }
        public string RequestPostReuse(string URL, Dictionary<string, string> ParamList)
        {
            return RequestReuse(URL, ParamList, "POST", "application/x-www-form-urlencoded");
        }
        public string RequestPostReuse(string URL, string Json)
        {
            return RequestReuse(URL, Json, "POST", "application/json");
        }
        #endregion
        #region DELETE
        /// <summary>
        /// 接口请求(DELETE)
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public string RequestDeleteReuse(string URL, string Json = "")
        {
            return RequestReuse(URL, Json, "DELETE", "application/json");
        }
        #endregion
        #region RequestReuse
        public string RequestReuse(string URL, string Json, string Method, string ContentType,string token=null)
        {
            byte[] postData = new byte[0];
            if (Json != null)
            {
                postData = Encoding.UTF8.GetBytes(Json);
            }
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Method = Method;          
            httpWebRequest.Headers.Add("Authorization", "bearer " + token);
            httpWebRequest.ContentLength = postData.Length;
            Stream stream = httpWebRequest.GetRequestStream();
            stream.Write(postData, 0, postData.Length);
            stream.Close();
            HttpWebResponse httpWebResponse = null;
            try
            {
                using (httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                return sr.ReadToEnd();
            }
        }
        public string RequestReuse(string URL, Dictionary<string, string> paramList, string method, string ContentType)
        {
            string postString = null;
            foreach (var item in paramList)
            {
                postString += WebUtility.UrlEncode(item.Key) + "=" + WebUtility.UrlEncode(item.Value) + "&";
            }
            postString = postString != null ? postString.Substring(0, postString.Length - 1) : "";
            return RequestReuse(URL, postString, method, ContentType);
        }

        #endregion
        #region access_token

        private const string access_token = "AQAAANCMnd8BFdERjHoAwE_Cl-sBAAAAwLJTdgA9xUWK0N9ZaZ6XEgAAAAACAAAAAAAQZgAAAAEAACAAAAAkrmMyHuQfLn3GDSh8b9JysR__esHYfiZdEhrGWLXmRQAAAAAOgAAAAAIAACAAAAD11daOsP4-ouuubGhi8JQg-frT_wvj0K8Do9IEpDAP-xABAADVr1GLcMZQ4V2EfZW21YuaoKcb2M5ZtSBsyPxybFVtXZJomROP9kFqpAH3XT9Em5Cq836YuSRwetQvCWxmwbenw3GplnEKulGkNqLR_YsEt3XU0S7yBftcf6Djjo1X_8_urwfi4diU6uNalO4m-1V3oClyAjqpwX9ON0cmdtWt9XEz7IurK5h6_d_N38Hs6CpmH_75YE_QLSKPbp4Tizr021pQpdD6by0XKZWIn6cTv3fD4_ODpdZRKE86OI9_4KSm6Eq9gafmrfOE1JXzX7LS9XbMUDc_38FYZdjr6tAmZFHkgSehgUWMBYjDBmwUy3lWQYtmm90lPV9eS2coHzqpe4NdAwdBELuWpgsCk273-kAAAACDzHP0PODczYTzBP0vscs-P1cTBOjKnBSGdRKz_Rd8dFddpAoVXxBu7sAW-t9MmW7ch4iEk40T17BAqx8QwBCr";

        #endregion
    }
}
