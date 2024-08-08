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
    /// The AFRRSignal (Automatic Frequency Restoration Reserve) response.
    /// </summary>
    public class AFRRSignalResponse : AResponse<CSMS.AFRRSignalRequest,
                                                     AFRRSignalResponse>,
                                      IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cs/aFRRSignalResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure of the AFRR signal request.
        /// </summary>
        public GenericStatus  Status        { get; }

        /// <summary>
        /// Optional detailed status information.
        /// </summary>
        [Optional]
        public StatusInfo?    StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region AFRRSignalResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new AFRRSignal response.
        /// </summary>
        /// <param name="Request">The AFRR signal request leading to this response.</param>
        /// <param name="Status">The success or failure of the AFRR signal request.</param>
        /// <param name="StatusInfo">Optional detailed status information.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AFRRSignalResponse(CSMS.AFRRSignalRequest        Request,
                                  GenericStatus                 Status,
                                  StatusInfo?                   StatusInfo          = null,
                                  DateTime?                     ResponseTimestamp   = null,

                                  NetworkingNode_Id?            DestinationId       = null,
                                  NetworkPath?                  NetworkPath         = null,

                                  IEnumerable<KeyPair>?         SignKeys            = null,
                                  IEnumerable<SignInfo>?        SignInfos           = null,
                                  IEnumerable<Signature>?       Signatures          = null,

                                  CustomData?                   CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   DestinationId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region AFRRSignalResponse(Result)

        /// <summary>
        /// Create a new AFRRSignal response.
        /// </summary>
        /// <param name="Request">The AFRR signal request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public AFRRSignalResponse(CSMS.AFRRSignalRequest   Request,
                                  Result                   Result,
                                  DateTime?                ResponseTimestamp   = null,

                                  NetworkingNode_Id?       DestinationId       = null,
                                  NetworkPath?             NetworkPath         = null,

                                  IEnumerable<KeyPair>?    SignKeys            = null,
                                  IEnumerable<SignInfo>?   SignInfos           = null,
                                  IEnumerable<Signature>?  Signatures          = null,

                                  CustomData?              CustomData          = null)

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

        #region (static) Parse   (Request, JSON, CustomAFRRSignalResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an AFRR signal response.
        /// </summary>
        /// <param name="Request">The AFRR signal request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAFRRSignalResponseParser">A delegate to parse custom AFRR signal responses.</param>
        public static AFRRSignalResponse Parse(CSMS.AFRRSignalRequest                            Request,
                                               JObject                                           JSON,
                                               NetworkingNode_Id                                 DestinationId,
                                               NetworkPath                                       NetworkPath,
                                               DateTime?                                         ResponseTimestamp                = null,
                                               CustomJObjectParserDelegate<AFRRSignalResponse>?  CustomAFRRSignalResponseParser   = null,
                                               CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                               CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                               CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var afrrSignalResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAFRRSignalResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return afrrSignalResponse;
            }

            throw new ArgumentException("The given JSON representation of an AFRR signal response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AFRRSignalResponse, out ErrorResponse, CustomAFRRSignalResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an AFRR signal response.
        /// </summary>
        /// <param name="Request">The AFRR signal request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AFRRSignalResponse">The parsed AFRR signal response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAFRRSignalResponseParser">A delegate to parse custom AFRR signal responses.</param>
        public static Boolean TryParse(CSMS.AFRRSignalRequest                            Request,
                                       JObject                                           JSON,
                                       NetworkingNode_Id                                 DestinationId,
                                       NetworkPath                                       NetworkPath,
                                       [NotNullWhen(true)]  out AFRRSignalResponse?      AFRRSignalResponse,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       DateTime?                                         ResponseTimestamp                = null,
                                       CustomJObjectParserDelegate<AFRRSignalResponse>?  CustomAFRRSignalResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?          CustomStatusInfoParser           = null,
                                       CustomJObjectParserDelegate<Signature>?           CustomSignatureParser            = null,
                                       CustomJObjectParserDelegate<CustomData>?          CustomCustomDataParser           = null)
        {

            try
            {

                AFRRSignalResponse = null;

                #region Status        [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "generic status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus Status,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region StatusInfo    [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "detailed status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

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


                AFRRSignalResponse = new AFRRSignalResponse(

                                         Request,
                                         Status,
                                         StatusInfo,
                                         ResponseTimestamp,

                                         DestinationId,
                                         NetworkPath,

                                         null,
                                         null,
                                         Signatures,

                                         CustomData

                                     );

                if (CustomAFRRSignalResponseParser is not null)
                    AFRRSignalResponse = CustomAFRRSignalResponseParser(JSON,
                                                                        AFRRSignalResponse);

                return true;

            }
            catch (Exception e)
            {
                AFRRSignalResponse  = null;
                ErrorResponse       = "The given JSON representation of an AFRR signal response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAFRRSignalResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAFRRSignalResponseSerializer">A delegate to serialize custom AFRR signal responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AFRRSignalResponse>?  CustomAFRRSignalResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?          CustomStatusInfoSerializer           = null,
                              CustomJObjectSerializerDelegate<Signature>?           CustomSignatureSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?          CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",       Status.    AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",   StatusInfo.ToJSON(CustomStatusInfoSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAFRRSignalResponseSerializer is not null
                       ? CustomAFRRSignalResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AFRRSignal failed because of a request error.
        /// </summary>
        /// <param name="Request">The AFRRSignal request.</param>
        public static AFRRSignalResponse RequestError(CSMS.AFRRSignalRequest   Request,
                                                      EventTracking_Id         EventTrackingId,
                                                      ResultCode               ErrorCode,
                                                      String?                  ErrorDescription    = null,
                                                      JObject?                 ErrorDetails        = null,
                                                      DateTime?                ResponseTimestamp   = null,

                                                      NetworkingNode_Id?       DestinationId       = null,
                                                      NetworkPath?             NetworkPath         = null,

                                                      IEnumerable<KeyPair>?    SignKeys            = null,
                                                      IEnumerable<SignInfo>?   SignInfos           = null,
                                                      IEnumerable<Signature>?  Signatures          = null,

                                                      CustomData?              CustomData          = null)

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
        /// The AFRRSignal failed.
        /// </summary>
        /// <param name="Request">The AFRRSignal request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AFRRSignalResponse FormationViolation(CSMS.AFRRSignalRequest  Request,
                                                            String                  ErrorDescription)

            => new (Request,
                    Result.FormationViolation(
                        $"Invalid data format: {ErrorDescription}"
                    ));


        /// <summary>
        /// The AFRRSignal failed.
        /// </summary>
        /// <param name="Request">The AFRRSignal request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AFRRSignalResponse SignatureError(CSMS.AFRRSignalRequest  Request,
                                                        String                  ErrorDescription)

            => new (Request,
                    Result.SignatureError(
                        $"Invalid signature(s): {ErrorDescription}"
                    ));


        /// <summary>
        /// The AFRRSignal failed.
        /// </summary>
        /// <param name="Request">The AFRRSignal request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AFRRSignalResponse Failed(CSMS.AFRRSignalRequest  Request,
                                                String?                 Description   = null)

            => new (Request,
                    Result.Server(Description));


        /// <summary>
        /// The AFRRSignal failed because of an exception.
        /// </summary>
        /// <param name="Request">The AFRRSignal request.</param>
        /// <param name="Exception">The exception.</param>
        public static AFRRSignalResponse ExceptionOccured(CSMS.AFRRSignalRequest  Request,
                                                          Exception               Exception)

            => new (Request,
                    Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AFRRSignalResponse1, AFRRSignalResponse2)

        /// <summary>
        /// Compares two AFRR signal responses for equality.
        /// </summary>
        /// <param name="AFRRSignalResponse1">An AFRR signal response.</param>
        /// <param name="AFRRSignalResponse2">Another AFRR signal response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AFRRSignalResponse? AFRRSignalResponse1,
                                           AFRRSignalResponse? AFRRSignalResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AFRRSignalResponse1, AFRRSignalResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AFRRSignalResponse1 is null || AFRRSignalResponse2 is null)
                return false;

            return AFRRSignalResponse1.Equals(AFRRSignalResponse2);

        }

        #endregion

        #region Operator != (AFRRSignalResponse1, AFRRSignalResponse2)

        /// <summary>
        /// Compares two AFRR signal responses for inequality.
        /// </summary>
        /// <param name="AFRRSignalResponse1">An AFRR signal response.</param>
        /// <param name="AFRRSignalResponse2">Another AFRR signal response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AFRRSignalResponse? AFRRSignalResponse1,
                                           AFRRSignalResponse? AFRRSignalResponse2)

            => !(AFRRSignalResponse1 == AFRRSignalResponse2);

        #endregion

        #endregion

        #region IEquatable<AFRRSignalResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AFRR signal responses for equality.
        /// </summary>
        /// <param name="AFRRSignalResponse">An AFRR signal response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AFRRSignalResponse afrrSignalResponse &&
                   Equals(afrrSignalResponse);

        #endregion

        #region Equals(AFRRSignalResponse)

        /// <summary>
        /// Compares two AFRR signal responses for equality.
        /// </summary>
        /// <param name="AFRRSignalResponse">An AFRR signal response to compare with.</param>
        public override Boolean Equals(AFRRSignalResponse? AFRRSignalResponse)

            => AFRRSignalResponse is not null &&

               Status.Equals(AFRRSignalResponse.Status) &&

             ((StatusInfo is     null && AFRRSignalResponse.StatusInfo is     null) ||
               StatusInfo is not null && AFRRSignalResponse.StatusInfo is not null && StatusInfo.Equals(AFRRSignalResponse.StatusInfo)) &&

               base.GenericEquals(AFRRSignalResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Status.     GetHashCode()       * 5 ^
                      (StatusInfo?.GetHashCode() ?? 0) * 3 ^

                       base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.ToString();

        #endregion

    }

}
