using System;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Security;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
namespace DokuShell {
	
	public class RpcParam {
		public string Value;
		public string DataType;
	}
	
	public class RpcMethod {
		public string Name;
		
		private List<RpcParam> parameters;
		public List<RpcParam> Parameters {
			get { return this.parameters; }
			set {
					this.parameters = new List<RpcParam>();
				foreach (RpcParam member in value) {
					this.parameters.Add(member);
				}
				//this.parameters = value;
			}
		}
		
		//public XElement Xml () {
		public XDocument Xml() {
			// Create root
			XDocument XmlObject = new XDocument();
			
			XmlObject.Declaration = new XDeclaration("1.0", "ISO-8859-1", "yes");
			
			// create entry nod and define name attribute
			XElement xmlEntry = new XElement("methodCall");
			XmlObject.Add(xmlEntry);
			
			// Set MethodName
			XElement xmlMethodName = new XElement("methodName",this.Name);
			XmlObject.Element("methodCall").Add(xmlMethodName);
			
			// Compile Parameters
			XElement xmlParams = new XElement("params");
			

			foreach (RpcParam param in this.Parameters) {
				xmlParams.Add(
					new XElement("param",
						new XElement(param.DataType,param.Value)
					)
				);
			}
			
			XmlObject.Element("methodCall").Add(xmlParams);
			
			// Return Xml
			//return XmlObject.Element("methodCall");
			return XmlObject;
		}
		
		public string PrintPrettyXml() {
			string[] Pieces = new string[2];
            Pieces[0] = Xml().Declaration.ToString();
            Pieces[1] = Xml().ToString();
			
			string CompletedString = string.Join("\n", Pieces);
			return CompletedString;
		}

		public string PrintPlainXml() {
			string[] Pieces = new string[2];
            Pieces[0] = Xml().Declaration.ToString();
            Pieces[1] = Xml().ToString(SaveOptions.DisableFormatting);
			
			string CompletedString = string.Join("", Pieces);
			return CompletedString;
		}
		
		
		/*
		<?xml version="1.0" encoding="ISO-8859-1"?>
  <methodCall>
    <methodName>dokuwiki.login</methodName>
    <params>
      <param>
        <string>
          brian
        </string>
      </param>
      <param>
        <string>
          nommowvil69+
        </string>
      </param>
    </params>
  </methodCall>
  
		
                    /*
			// Create root
			XDocument XmlObject = new XDocument();
			
			/*
			// create entry nod and define name attribute
			XElement xmlEntry = new XElement("entry");
			xmlEntry.SetAttributeValue("name",this.Name);
			XmlObject.Add(xmlEntry);

			// Set Disable Server Response Inspection

			XElement xmlDisableSRI = new XElement("option",
				createXmlBool( "disable-server-response-inspection", this.DisableSRI )
			);

			XmlObject.Element("entry").Add(xmlDisableSRI);

			// ---------------------------------------- Zones ----------------------------------------- //
			XmlObject.Element("entry").Add( createXmlWithMembers( "from", this.SourceZone, true ));			// Source Zones
			XmlObject.Element("entry").Add( createXmlWithMembers( "to", this.DestinationZone, true ));	// Destination Zones
			// ---------------------------------------------------------------------------------------- //

			// -------------------------------------------- Addresses --------------------------------------------- //
			XmlObject.Element("entry").Add( createXmlWithMembers( "source", this.SourceAddress, true ));						// Source Addresses
			XmlObject.Element("entry").Add( createXmlWithMembers( "destination", this.DestinationAddress, true ));	// Destination Address
			// ---------------------------------------------------------------------------------------------------- //

			XmlObject.Element("entry").Add( createXmlWithMembers( "source-user", this.SourceUser, true ));      // Source User
			XmlObject.Element("entry").Add( createXmlWithMembers( "category", this.UrlCategory, true ));       // Url Category
			XmlObject.Element("entry").Add( createXmlWithMembers( "application", this.Application, true ));    // Applications
			XmlObject.Element("entry").Add( createXmlWithMembers( "service", this.Service, true ));            // Services
			XmlObject.Element("entry").Add( createXmlWithMembers( "hip-profiles", this.HipProfile, true ));     // Hip Profiles

			// ----------------------------------- Action ----------------------------------- //
			XElement xmlAction = new XElement("action");
			if (this.Allow) { xmlAction.Value = "allow"; } 																		// Allow
			           else { xmlAction.Value = "deny";  }																		// Deny
			XmlObject.Element("entry").Add(xmlAction);
			// ------------------------------------------------------------------------------ //

			XmlObject.Element("entry").Add( createXmlBool( "log-start", this.LogAtSessionStart));				  				// Log At Start
			XmlObject.Element("entry").Add( createXmlBool( "log-end", this.LogAtSessionEnd));					  					// Log At End

			// ------------------------------------- Address Negation ------------------------------------- //
			XmlObject.Element("entry").Add( createXmlBool( "negate-source", this.SourceNegate ));			      // Source Negate
			XmlObject.Element("entry").Add( createXmlBool( "negate-destination", this.DestinationNegate ));	// Destination Negate
			// -------------------------------------------------------------------------------------------- //

			// ---------------------------------- Disabled ---------------------------------- //
			XmlObject.Element("entry").Add( createXmlBool( "disabled", this.Disabled));

			XmlObject.Element("entry").Add( createXmlWithoutMembers( "log-setting", this.LogForwarding));		  		// Log Forwarding
			XmlObject.Element("entry").Add( createXmlWithoutMembers( "schedule", this.Schedule));				  				// Schedule
			XmlObject.Element("entry").Add( createXmlWithMembers( "tag", this.Tags ));				            // Tags

			// Set Qos Marking
			if (!(String.IsNullOrEmpty(this.QosMarking)) && !(String.IsNullOrEmpty(this.QosType))) {
				XElement QosXml = new XElement("qos", new XElement( "marking" ));
				
				if (this.QosType == "ip-dscp") {
					XElement QosMarkingXml = new XElement("ip-dscp",this.QosMarking);
					QosXml.Element("marking").Add(QosMarkingXml);
				} else {
					XElement QosMarkingXml = new XElement("ip-precedence",this.QosMarking);
					QosXml.Element("marking").Add(QosMarkingXml);
				}
				
				XmlObject.Element("entry").Add(QosXml);
			}

			// ------------------------------ Security Profiles ----------------------------- //
			if (this.ProfileExists) {
				XElement xmlProfileSetting = new XElement("profile-setting",
					new XElement("profiles")
				);

				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "virus", this.AntivirusProfile ));
				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "spyware", this.AntiSpywareProfile ));
				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "vulnerability", this.VulnerabilityProfile ));
				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "url-filtering", this.UrlFilteringProfile ));
				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "file-blocking", this.FileBlockingProfile ));
				xmlProfileSetting.Element("profiles").Add( createXmlWithSingleMember( "data-filtering", this.DataFilteringProfile ));

				XmlObject.Element("entry").Add(xmlProfileSetting);
			}

			if (!(String.IsNullOrEmpty(this.ProfileGroup))) {
				XElement xmlProfileSetting = new XElement("profile-setting");

				xmlProfileSetting.Add( createXmlWithSingleMember( "group", this.ProfileGroup ));

				XmlObject.Element("entry").Add(xmlProfileSetting);
			}
			// ------------------------------------------------------------------------------ //

			XmlObject.Element("entry").Add( createXmlWithoutMembers( "description", this.Description ));	// Description

      return XmlObject.Element("entry");
	  }

		public string PrintCli () {
			List<string> CliList = new List<string>();
			
			// Start command and add name
			CliList.Add("set rulebase security rules ");
			CliList.Add(this.Name);
			
			// ---------------------------- Description and Tags ---------------------------- //
			CliList.Add(createCliWithoutMembers( "description", this.Description));		  	  	// Description
			CliList.Add(createCliWithMembers( "tag", this.Tags ));							  						// Tags
			// ------------------------------------------------------------------------------ //

			
			// --------------------------- Users and Hip Profiles --------------------------- //
			CliList.Add(createReqCliWithMembers( "source-user", this.SourceUser ));           // Source User
			CliList.Add(createReqCliWithMembers( "hip-profiles", this.HipProfile ));          // Hip Profiles
			// ------------------------------------------------------------------------------ //


			// ------------------------------ Address Negation ------------------------------ //
			CliList.Add(createCliBool( "negate-source", this.SourceNegate));				  				// Source Negate
			CliList.Add(createCliBool( "negate-destination", this.DestinationNegate));		  	// Destination Negate
			// ------------------------------------------------------------------------------ //


			// ----------------------------------- Zones ------------------------------------ //
			CliList.Add(createReqCliWithMembers( "from", this.SourceZone ));	      			    // Source Zones
			CliList.Add(createReqCliWithMembers( "to", this.DestinationZone ));				  			// Destination Zones
			// ------------------------------------------------------------------------------ //


			// --------------------------------- Addresses ---------------------------------- //
			CliList.Add(createReqCliWithMembers( "source", this.SourceAddress ));						  // Source Addresses
			CliList.Add(createReqCliWithMembers( "destination", this.DestinationAddress ));	  // Destination Addresses
			// ------------------------------------------------------------------------------ //


			// ------------------ Applications, Services, and URL Category ------------------ //
			CliList.Add(createReqCliWithMembers( "application", this.Application ));          // Applications
			CliList.Add(createReqCliWithMembers( "service", this.Service ));                  // Services
			CliList.Add(createReqCliWithMembers( "category", this.UrlCategory ));             // Url Category
			// ------------------------------------------------------------------------------ //

			// ----------------------------------- Action ----------------------------------- //
			if (this.Allow) { CliList.Add(" action allow");	}                                 // Allow
			           else { CliList.Add(" action deny");  }                                 // Deny
			// ------------------------------------------------------------------------------ //


			// -------------------------------- Log Settings -------------------------------- //
			CliList.Add(createCliBool( "log-start", this.LogAtSessionStart));				  				// Log At Start
			CliList.Add(createCliBool( "log-end", this.LogAtSessionEnd));					  					// Log At End
			CliList.Add(createCliWithoutMembers( "log-setting", this.LogForwarding));		  		// Log Forwarding
			// ------------------------------------------------------------------------------ //


			// ----------------------------- Schedule and DSRI ------------------------------ //
			CliList.Add(createCliWithoutMembers( "schedule", this.Schedule));				  				// Schedule
			string cmdDisableSRI = "option disable-server-response-inspection";
			CliList.Add(createCliBool( cmdDisableSRI, this.DisableSRI));										  // Disable SRI
			// ------------------------------------------------------------------------------ //


			// ------------------------------ Security Profiles ----------------------------- //
			if (this.ProfileExists) {
				CliList.Add(" profile-setting profiles");
				CliList.Add(createCliWithoutMembers( "virus", this.AntivirusProfile ));
				CliList.Add(createCliWithoutMembers( "spyware", this.AntiSpywareProfile ));
				CliList.Add(createCliWithoutMembers( "vulnerability", this.VulnerabilityProfile ));
				CliList.Add(createCliWithoutMembers( "url-filtering", this.UrlFilteringProfile ));
				CliList.Add(createCliWithoutMembers( "file-blocking", this.FileBlockingProfile ));
				CliList.Add(createCliWithoutMembers( "data-filtering", this.DataFilteringProfile ));
			}

			if (!(String.IsNullOrEmpty(this.ProfileGroup))) {
				CliList.Add(" profile-settings group ");
				CliList.Add(this.ProfileGroup);
			}			
			// ------------------------------------------------------------------------------ //


			// -------------------------------- QoS Settings -------------------------------- //
			if (!(String.IsNullOrEmpty(this.QosType)) && !(String.IsNullOrEmpty(this.QosMarking))) {
				string cliQos = " qos marking " + this.QosType + " " + this.QosMarking;
				CliList.Add(cliQos);
			}
			// ------------------------------------------------------------------------------ //			
			
			string CliString = string.Join("",CliList.ToArray());  // Smash it all together
			return CliString;
		}
   
    }*/
		public RpcMethod () {
			this.Parameters = new List<RpcParam>();
		}
	}

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
