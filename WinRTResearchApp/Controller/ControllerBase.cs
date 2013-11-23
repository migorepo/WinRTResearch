using System;
using WinRT.DataClient.Model;
using WinRT.DataClient.Networking;

namespace WinRTResearchApp.Controller
{
    public class ControllerBase<T> where T : class, new()
    {
        private static volatile T _instance;
        private static readonly object SyncRoot = new Object();

        #region Singleton Instance
        public static T Instance
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new T();

                    }
                }
                return _instance;
            }
        }
        #endregion


        /// <summary>
        /// Method for building a HttpGet web request
        /// </summary>
        /// <param name="baseUrl"> </param>
        /// <param name="query"> </param>
        /// <returns></returns>
        protected HttpGetRequest GenerateGetRequest(string baseUrl, string query = null)
        {
            string url = string.Format(baseUrl, query);
            var uri = new Uri(url);
            var request = new HttpGetRequest(uri);
            request.Header.Add(null, null); // add headre if any...
            request.ContentType = "application/json";
            request.Accept = "application/json";

            return request;
        }

        /// <summary>
        /// Method for building resultant object
        /// </summary>
        /// <param name="result">result</param>
        /// <param name="cancled">cancled</param>
        /// <param name="ex">exception</param>
        /// <param name="success">success</param>
        /// <param name="tag">result tag</param>
        /// <returns></returns>
        protected WebdataResult BuildWebResult(string result, bool cancled, Exception ex, bool success, object tag = null)
        {
            var resultantObj = new WebdataResult
            {
                Result = result,
                Canceled = cancled,
                Ex = ex,
                Success = success,
                Tag = tag
            };
            return resultantObj;
        }

    }
}
