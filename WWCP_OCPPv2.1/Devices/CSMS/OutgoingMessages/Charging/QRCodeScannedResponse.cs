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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    /// <summary>
    /// The QRCodeScanned response.
    /// </summary>
    public class QRCodeScannedResponse : AResponse<CSMS.QRCodeScannedRequest,
                                                        QRCodeScannedResponse>,
                                         IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/qrCodeScannedResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        #endregion

        #region Constructor(s)

        #region QRCodeScannedResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new QRCodeScanned response.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request leading to this response.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public QRCodeScannedResponse(CSMS.QRCodeScannedRequest     Request,
                                     DateTime?                     ResponseTimestamp   = null,

                                     IEnumerable<KeyPair>?         SignKeys            = null,
                                     IEnumerable<SignInfo>?        SignInfos           = null,
                                     IEnumerable<Signature>?       Signatures          = null,

                                     CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   null,
                   null,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #region QRCodeScannedResponse(Result)

        /// <summary>
        /// Create a new QRCodeScanned response.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public QRCodeScannedResponse(CSMS.QRCodeScannedRequest  Request,
                                     Result                     Result,
                                     DateTime?                  ResponseTimestamp   = null,

                                     NetworkingNode_Id?         DestinationId       = null,
                                     NetworkPath?               NetworkPath         = null,

                                     IEnumerable<KeyPair>?      SignKeys            = null,
                                     IEnumerable<SignInfo>?     SignInfos           = null,
                                     IEnumerable<Signature>?    Signatures          = null,

                                     CustomData?                CustomData          = null)

            : base(Request,
                   Result,
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomQRCodeScannedResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an QRCodeScanned response.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomQRCodeScannedResponseParser">A delegate to parse custom QRCodeScanned responses.</param>
        public static QRCodeScannedResponse Parse(CSMS.QRCodeScannedRequest                            Request,
                                                  JObject                                              JSON,
                                                  CustomJObjectParserDelegate<QRCodeScannedResponse>?  CustomQRCodeScannedResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var qrCodeScannedResponse,
                         out var errorResponse,
                         CustomQRCodeScannedResponseParser))
            {
                return qrCodeScannedResponse;
            }

            throw new ArgumentException("The given JSON representation of an QRCodeScanned response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out QRCodeScannedResponse, out ErrorResponse, CustomQRCodeScannedResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an QRCodeScanned response.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="QRCodeScannedResponse">The parsed QRCodeScanned response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomQRCodeScannedResponseParser">A delegate to parse custom QRCodeScanned responses.</param>
        public static Boolean TryParse(CSMS.QRCodeScannedRequest                            Request,
                                       JObject                                              JSON,
                                       [NotNullWhen(true)]  out QRCodeScannedResponse?      QRCodeScannedResponse,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<QRCodeScannedResponse>?  CustomQRCodeScannedResponseParser   = null)
        {

            try
            {

                QRCodeScannedResponse = null;

                #region Signatures    [optional, OCPP_CSE]

                if (JSON.ParseOptionalHashSet("signatures",
                                              "cryptographic signatures",
                                              Signature.TryParse,
                                              out HashSet<Signature> Signatures,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                QRCodeScannedResponse = new QRCodeScannedResponse(
                                            Request,
                                            null,
                                            null,
                                            null,
                                            Signatures,
                                            CustomData
                                        );

                if (CustomQRCodeScannedResponseParser is not null)
                    QRCodeScannedResponse = CustomQRCodeScannedResponseParser(JSON,
                                                                              QRCodeScannedResponse);

                return true;

            }
            catch (Exception e)
            {
                QRCodeScannedResponse  = null;
                ErrorResponse                = "The given JSON representation of an QRCodeScanned response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomQRCodeScannedResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomQRCodeScannedResponseSerializer">A delegate to serialize custom QRCodeScanned responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<QRCodeScannedResponse>?  CustomQRCodeScannedResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?              CustomSignatureSerializer               = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomQRCodeScannedResponseSerializer is not null
                       ? CustomQRCodeScannedResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The QRCodeScanned failed because of a request error.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request.</param>
        public static QRCodeScannedResponse RequestError(CSMS.QRCodeScannedRequest  Request,
                                                         EventTracking_Id           EventTrackingId,
                                                         ResultCode                 ErrorCode,
                                                         String?                    ErrorDescription    = null,
                                                         JObject?                   ErrorDetails        = null,
                                                         DateTime?                  ResponseTimestamp   = null,

                                                         NetworkingNode_Id?         DestinationId       = null,
                                                         NetworkPath?               NetworkPath         = null,

                                                         IEnumerable<KeyPair>?      SignKeys            = null,
                                                         IEnumerable<SignInfo>?     SignInfos           = null,
                                                         IEnumerable<Signature>?    Signatures          = null,

                                                         CustomData?                CustomData          = null)

            => new (

                   Request,
                   Result.FromErrorResponse(
                       ErrorCode,
                       ErrorDescription,
                       ErrorDetails
                   ),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData

               );


        /// <summary>
        /// The QRCodeScanned failed.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static QRCodeScannedResponse SignatureError(CSMS.QRCodeScannedRequest  Request,
                                                           String                     ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The QRCodeScanned failed.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request.</param>
        /// <param name="Description">An optional error description.</param>
        public static QRCodeScannedResponse Failed(CSMS.QRCodeScannedRequest  Request,
                                                   String?                    Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The QRCodeScanned failed because of an exception.
        /// </summary>
        /// <param name="Request">The QRCodeScanned request.</param>
        /// <param name="Exception">The exception.</param>
        public static QRCodeScannedResponse ExceptionOccured(CSMS.QRCodeScannedRequest  Request,
                                                             Exception                  Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (QRCodeScannedResponse1, QRCodeScannedResponse2)

        /// <summary>
        /// Compares two QRCodeScanned responses for equality.
        /// </summary>
        /// <param name="QRCodeScannedResponse1">An QRCodeScanned response.</param>
        /// <param name="QRCodeScannedResponse2">Another QRCodeScanned response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (QRCodeScannedResponse? QRCodeScannedResponse1,
                                           QRCodeScannedResponse? QRCodeScannedResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(QRCodeScannedResponse1, QRCodeScannedResponse2))
                return true;

            // If one is null, but not both, return false.
            if (QRCodeScannedResponse1 is null || QRCodeScannedResponse2 is null)
                return false;

            return QRCodeScannedResponse1.Equals(QRCodeScannedResponse2);

        }

        #endregion

        #region Operator != (QRCodeScannedResponse1, QRCodeScannedResponse2)

        /// <summary>
        /// Compares two QRCodeScanned responses for inequality.
        /// </summary>
        /// <param name="QRCodeScannedResponse1">An QRCodeScanned response.</param>
        /// <param name="QRCodeScannedResponse2">Another QRCodeScanned response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (QRCodeScannedResponse? QRCodeScannedResponse1,
                                           QRCodeScannedResponse? QRCodeScannedResponse2)

            => !(QRCodeScannedResponse1 == QRCodeScannedResponse2);

        #endregion

        #endregion

        #region IEquatable<QRCodeScannedResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two QRCodeScanned responses for equality.
        /// </summary>
        /// <param name="QRCodeScannedResponse">An QRCodeScanned response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is QRCodeScannedResponse qrCodeScannedResponse &&
                   Equals(qrCodeScannedResponse);

        #endregion

        #region Equals(QRCodeScannedResponse)

        /// <summary>
        /// Compares two QRCodeScanned responses for equality.
        /// </summary>
        /// <param name="QRCodeScannedResponse">An QRCodeScanned response to compare with.</param>
        public override Boolean Equals(QRCodeScannedResponse? QRCodeScannedResponse)

            => QRCodeScannedResponse is not null &&

               base.GenericEquals(QRCodeScannedResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => base.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Empty;

        #endregion

    }

}
