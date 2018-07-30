using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightNovelSniffer_CLI
{
    internal class FileWriter : StreamWriter
    {
        private List<string> lines = new List<string>{""};

        public FileWriter(Stream stream) : base(stream)
        {
        }

        public FileWriter(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public FileWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize)
        {
        }

        public FileWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) : base(stream, encoding, bufferSize, leaveOpen)
        {
        }

        public FileWriter(string path) : base(path)
        {
        }

        public FileWriter(string path, bool append) : base(path, append)
        {
        }

        public FileWriter(string path, bool append, Encoding encoding) : base(path, append, encoding)
        {
        }

        public FileWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize)
        {
        }

        public override void Write(string str)
        {
            if (str.Equals("\r\n"))
            {
                lines[lines.Count - 1] = lines[lines.Count - 1] + "\r\n";
                lines.Add("");
            }
            else
            {
                string line = str.StartsWith("\r") ? "" : lines[lines.Count - 1];
                lines[lines.Count - 1] = line + str.TrimStart('\r');
                
                if (str.EndsWith("\r\n"))
                    lines.Add("");
            }
        }

        public override void Close()
        {
            foreach (string line in lines)
                base.Write(line);
            base.Close();
        }
    }
}
