using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WinRT.DataClient.Networking
{
    public class HttpResponse
    {
        internal HttpResponse()
        {
            Cookies = new List<Cookie>();
        }

        public HttpResponse(IHttpRequest request)
            : this()
        {
            Request = request;
        }

        public static HttpResponse CreateCancelled()
        {
            return new HttpResponse { Canceled = true };
        }

        public static HttpResponse CreateException(Exception exception)
        {
            return new HttpResponse { Exception = exception };
        }

        public IHttpRequest Request { get; internal set; }
        internal HttpWebRequest WebRequest { get; set; }

        public bool IsConnected { get; internal set; }

        public bool IsPending
        {
            get { return !HasException && !Canceled && !Successful; }
        }

        public void Abort()
        {
            if (Request != null && !Successful)
                WebRequest.Abort();
        }

        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                if (_exception is TimeoutException)
                    return; // already set

                if (value is WebException && ((WebException)value).Status == WebExceptionStatus.RequestCanceled)
                {
                    _exception = null;
                    Canceled = true;
                }
                else
                {
                    _exception = value;
                    Canceled = false;
                }
            }
        }

        /// <summary>
        /// Can be used for polling (not recommended)
        /// </summary>
        public bool Processed { get { return Canceled || Successful || HasException; } }

        public bool Canceled { get; private set; }
        public bool Successful { get { return !Canceled && Exception == null && (Response != null || ResponseStream != null); } }
        public bool HasException { get { return Exception != null; } }

        /// <summary>
        /// If Response is null and Exception is null as well the request has been canceled
        /// </summary>
        public string Response { get { return RawResponse == null ? null : Request.Encoding.GetString(RawResponse, 0, RawResponse.Length); } }
        public byte[] RawResponse { get; internal set; }
        public Stream ResponseStream { get; internal set; }

        public List<Cookie> Cookies { get; private set; }

        private HttpStatusCode _code = HttpStatusCode.OK;
        public HttpStatusCode HttpStatusCode
        {
            get { return _code; }
            internal set { _code = value; }
        }


        internal void CreateTimeoutTimer(HttpWebRequest request)
        {
            if (Request.ConnectionTimeout > 0)
            {
#if NETFX_CORE
                request.ContinueTimeout = Request.ConnectionTimeout;
#endif
                //await Task.Delay(Request.ConnectionTimeout * 1000);

                //if (IsPending && !IsConnected)
                //{
                //	Exception = new TimeoutException("The connection timed out.");
                //	Abort();
                //}
            }
        }

    }
}



