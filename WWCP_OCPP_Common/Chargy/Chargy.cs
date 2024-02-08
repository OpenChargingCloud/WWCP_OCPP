/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using cloud.charging.open.protocols.OCPP;
using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public class ParseResult
    {

        public SessionVerificationResult  Result    { get; set; }

        public Object?                    Output    { get; set; }


    }


    public class Chargy
    {

        public ParseResult Parse(String Input)
        {

            #region XML processing...

            if (Input.StartsWith("<?xml"))
            {
                try
                {

                    var xml = XDocument.Parse(Input);

                    #region XML namespace found...

                    var xmlNamespace  = xml?.Root?.Name.NamespaceName;
                    if (xmlNamespace is not null)
                    {
                        switch (xmlNamespace)
                        {

                            case "http://www.mennekes.de/Mennekes.EdlVerification.xsd":
                                //processedFile.result = await new Mennekes().tryToParseMennekesXML(XMLDocument);
                                break;

                            case "http://transparenz.software/schema/2018/07":
                                //processedFile.result = await new SAFEXML(this).tryToParseSAFEXML(XMLDocument);
                                break;

                            // The SAFE transparency software v1.0 does not understand its own
                            // XML namespace. Therefore we have to guess the format.
                            case "":
                                //processedFile.result = await new SAFEXML(this).tryToParseSAFEXML(XMLDocument);

                                //if (processedFile.result.status)
                                //    processedFile.result = await new XMLContainer(this).tryToParseXMLContainer(XMLDocument);

                                break;

                        }
                    }

                    #endregion

                    #region ..., or plain XML.

                    else
                    {

                        // The SAFE transparency software v1.0 does not understand its own
                        // XML namespace. Therefore we have to guess the format.
                        //processedFile.result = await new SAFEXML(this).tryToParseSAFEXML(XMLDocument);

                        //if (processedFile.result.status)
                        //    processedFile.result = await new XMLContainer(this).tryToParseXMLContainer(XMLDocument);

                    }

                    #endregion

                    return new ParseResult() { Result = SessionVerificationResult.InvalidSessionFormat, Output = xml };

                }
                catch
                {
                    return new ParseResult() { Result = SessionVerificationResult.InvalidSessionFormat };
                }
            }

            #endregion

            return new ParseResult() { Result = SessionVerificationResult.InvalidSessionFormat };

        }


    }

}
