using System;

namespace WinRT.DataClient.Model
{
    public class WebdataResult
    {
        public bool Success;
        public string Result;
        public object Tag;
        public Exception Ex;

        public bool Canceled { get; set; }



        public WebdataResult(bool success, string result, object tag, Exception ex)
        {
            Success = success;
            Result = result;
            Tag = tag;
            Ex = ex;
        }
        public WebdataResult(bool success, string result, object tag, Exception ex, bool canceled)
        {
            Success = success;
            Result = result;
            Tag = tag;
            Ex = ex;
            Canceled = canceled;
        }

        public WebdataResult()
        {
        }
    }
}
