﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WinRT.DataClient.Networking
{
     public interface IHttpRequest
    {
        Uri Uri { get; }
        Encoding Encoding { get; }
        object Tag { get; }
        int ConnectionTimeout { get; }

        /// <summary>
        /// Important: Always close stream after usage! If true, Response and RawResponse will be null in response object
        /// </summary>
        bool ResponseAsStream { get; }
    }

    public class HttpGetRequest : IHttpRequest
    {
        public HttpGetRequest(string uri)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute))
        { }

        public HttpGetRequest(string uri, Dictionary<string, string> query)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute), query)
        { }

        public HttpGetRequest(Uri uri)
            : this(uri, new Dictionary<string, string>())
        { }

        public HttpGetRequest(Uri uri, Dictionary<string, string> query)
        {
            Uri = uri;
            Query = query ?? new Dictionary<string, string>();
            Header = new Dictionary<string, string>();
            Cookies = new List<Cookie>();
            UseCache = true;
            Encoding = Encoding.UTF8;
            ConnectionTimeout = 0;
            ResponseAsStream = false;

#if USE_GZIP
			RequestGZIP = true; 
#endif
        }

        /// <summary>
        /// Max. duration till FIRST response from server. Download of message will never be cancelled. 
        /// In seconds. If 0 then use default timeout (better)
        /// </summary>
        public int ConnectionTimeout { get; set; }

        public bool ResponseAsStream { get; set; }

        public Uri Uri { get; private set; }
        public Dictionary<string, string> Query { get; private set; }
        public List<Cookie> Cookies { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public Dictionary<string, string> Header { get; private set; }


        public bool UseCache { get; set; }
        public Encoding Encoding { get; set; }
        public string ContentType { get; set; }
        public string Accept { get; set; }

        public object Tag { get; set; }
        public ICredentials Credentials { get; set; }

#if USE_GZIP
		public bool RequestGZIP { get; set; }
#endif
    }
}



