using System.IO;

namespace WinRT.DataClient.Networking
{
    public class HttpPostFile
    {

        public HttpPostFile(string name, string filename, string path)
        {
            Name = name;
            Filename = System.IO.Path.GetFileName(filename);
            Path = path;
            CloseStream = true;
        }

        public HttpPostFile(string name, string path)
        {
            Name = name;
            Filename = System.IO.Path.GetFileName(path);
            Path = path;
            CloseStream = true;
        }


        public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
        {
            Name = name;
            Filename = filename;
            Stream = stream;
            CloseStream = closeStream;
        }

        public string Name { get; private set; }
        public string Filename { get; private set; }
        public string Path { get; private set; } // use Path OR Stream
        public Stream Stream { get; private set; }
        public bool CloseStream { get; private set; }

        /// <summary>
        /// default: application/octet-stream
        /// </summary>
        public string ContentType { get; set; }
    }
}
