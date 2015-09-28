using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Web;

namespace DokuShell {

    public class HttpQueryReturnObject {
        public HttpStatusCode Statuscode;
        public string DetailedError;
        public XmlDocument Data;
        public string RawData;
        public int HttpStatusCode {
            get {
                return (int)this.Statuscode;
            }
        }
    }

    public class Server {
        // can we rewrite the helper functions as methods in this class?

        private DateTime clock;
        private Stack<string> urlhistory = new Stack<string>();

        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Protocol { get; set; }
        public string WebRoot { get; set; }

        public XmlDocument LastXmlResult { get; set; }

        public string ApiUrl {
            get {
                if ( !string.IsNullOrEmpty( this.Protocol ) && !string.IsNullOrEmpty( this.Host ) && this.Port > 0 && !string.IsNullOrEmpty( this.WebRoot ) ) {
                    return this.Protocol + "://" + this.Host + ":" + this.Port + "/" + this.WebRoot + "/lib/exe/xmlrpc.php";
                }
                else if ( !string.IsNullOrEmpty( this.Protocol ) && !string.IsNullOrEmpty( this.Host ) && this.Port > 0 ) {
                    return this.Protocol + "://" + this.Host + ":" + this.Port + "/lib/exe/xmlrpc.php";
                } else {
                    return null;
                }
            }
        }

        public string Clock {
            get {
                return this.clock.ToString();
            }
            set {
                this.clock = DateTime.Parse( value );
            }
        }
        public DateTime ClockasDateTime {
            get {
                return this.clock;
            }
            set {
                this.clock = value;
            }
        }

        public string[] UrlHistory {
            get {
                return this.urlhistory.ToArray();
            }
        }

        public void FlushHistory() {
            this.urlhistory.Clear();
        }


        public string UrlBuilder(string QueryString) {

            string[] Pieces = new string[1];
            Pieces[0] = this.ApiUrl;

            string CompletedString = string.Join( "", Pieces );

            this.urlhistory.Push( CompletedString );
            return CompletedString;
        }

        private static bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }

        public void OverrideValidation() {
            ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
			// the following has different behaviors depending on the host powershell version
			// powershell v2: SecurityProtocol = Tls (this means Tls 1.0)
			// powershell v3 might be the same as v2. Currently untested.
			// powershell v4: SecurityProtocol = Tls, Tls11, Tls12
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			
			//[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls -bor [System.Net.SecurityProtocolType]::Tls11 -bor [System.Net.SecurityProtocolType]::Tls12
        }

        //Holds the raw result of the last query
        //Would like to convert this to emulate UrlHistory, but I think we need to get the HttpQuery helper as a method to PaDevice first

        private Stack<string> rawqueryhistory = new Stack<string>();

        public string[] RawQueryHistory {
            get {
                return this.rawqueryhistory.ToArray();
            }
        }

        public void FlushRawQueryHistory() {
            this.rawqueryhistory.Clear();
        }

        public HttpQueryReturnObject HttpQuery(string Url, bool AsXml = true) {
            // this works. there's some logic missing from the original powershell version of this
            // that may or may not be important (it was error handling of some flavor)
            // also, all requests should not be treated as XML for this to be more generic
            // (the powershell version had an "-asxml" flag to handle this)

            HttpWebResponse Response = null;
            HttpStatusCode StatusCode = new HttpStatusCode();
			HttpQueryReturnObject ReturnObject = new HttpQueryReturnObject();

            try {
                HttpWebRequest Request = WebRequest.Create( Url ) as HttpWebRequest;
				Request.Timeout = 20000;

                //if (Response.ContentLength > 0) {

                try {
                    Response = Request.GetResponse() as HttpWebResponse;
                    StatusCode = Response.StatusCode;
                } catch ( WebException we ) {
                    StatusCode = ( (HttpWebResponse)we.Response ).StatusCode;
                }

                ReturnObject.DetailedError = Response.GetResponseHeader( "X-Detailed-Error" );
                // }

            } catch {
                throw new HttpException( ReturnObject.DetailedError );
            }

            if ( Response.StatusCode.ToString() == "OK" ) {
                StreamReader Reader = new StreamReader( Response.GetResponseStream() );
                string Result = Reader.ReadToEnd();
                XmlDocument XResult = new XmlDocument();

                if ( AsXml ) {
                    XResult.LoadXml( Result );
                }

                Reader.Close();
                Response.Close();

                
                ReturnObject.Statuscode = StatusCode;
                if ( AsXml ) { ReturnObject.Data = XResult; }
                ReturnObject.RawData = Result;

                this.rawqueryhistory.Push( Result );
                //this.queryhistory.Push(XResult);

                return ReturnObject;

            } else {

                throw new HttpException( "httperror" );
            }
        }
    }
}