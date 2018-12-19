using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        int index = 0;
        RequestMethod method;
        public string relativeURI;
        public Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //string[] separatingChars = { "\r\n" };
            //TODO: parse the receivedRequest using the \r\n delimeter   
            //this.contentLines = requestString.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            this.contentLines = Regex.Split(requestString, "\r\n");
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (this.contentLines.Length < 3)
                return false;
            // Parse Request line
            this.requestLines = contentLines[0].Split(' ');
            if (this.ParseRequestLine() == false)
                return false;
            // Validate blank line exists
            if (this.ValidateBlankLine() == false)
                return false;

            // Load header lines into HeaderLines dictionary
            if (this.LoadHeaderLines() == false)
                return false;

            return true;
        }

        private bool ParseRequestLine()
        {
            if (requestLines.Length != 3)
                return false;
            if (requestLines[0] == "GET")
            {
                method = RequestMethod.GET;
            }
            else if (requestLines[0] == "POST")
            {
                method = RequestMethod.POST;
            }
            else if (requestLines[0] == "HEAD")
            {
                method = RequestMethod.HEAD;
            }
            else
            {
                return false;
            }
            if (requestLines[2] == " HTTP/1.0")
            {
                httpVersion = HTTPVersion.HTTP10;
            }
            else if (requestLines[2] == "HTTP/1.1")
            {
                httpVersion = HTTPVersion.HTTP11;
            }
            else if (requestLines[2] == "HTTP/0.9")
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else
            {
                return false;
            }
            //if (ValidateIsURI(relativeURI) == false)
            if (ValidateIsURI(requestLines[1]) == false)
            {
                return false;
            }
            else
            {
                relativeURI = requestLines[1];
            }

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            for (int i = 1; i < index; i++)
            {

                string[] bu = contentLines[i].Split(':');
                headerLines.Add(bu[0], bu[1]);
            }
            return true;
        }

        private bool ValidateBlankLine()
        {
            bool ret = false;
            for (int i = 0; i < contentLines.Length; i++)
            {
                if (contentLines[i] == string.Empty)
                {
                    index = i;
                    ret = true;
                    break;
                }
            }
            return ret;
        }

    }
}
