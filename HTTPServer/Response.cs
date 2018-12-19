using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            if (content != null)
            {
                headerLines.Add("Content-Type: " + contentType);
                headerLines.Add("Content-Length: " + content.Length.ToString());
            }
            headerLines.Add("Date: " + "Wed, 30 Jan 2018 12:48:17 EST");
            if (redirectoinPath != null)
            {
                headerLines.Add("Redirection: " + redirectoinPath);
                code = StatusCode.Redirect;
            }
            // TODO: Create the response string
            string s = GetStatusLine(code);
            responseString = s + "/r/n";
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i] + "/r/n";
            }
            responseString += "/r/n";
            responseString += content;

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            int num = (int)code;
            string statusLine = Configuration.ServerHTTPVersion + " " + num.ToString() + " " + code.ToString();
            return statusLine;
        }
    }
}
