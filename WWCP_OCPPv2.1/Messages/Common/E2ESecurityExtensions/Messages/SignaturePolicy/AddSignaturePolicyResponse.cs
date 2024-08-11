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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.NetworkingNode
{

    /// <summary>
    /// The AddSignaturePolicy response.
    /// </summary>
    public class AddSignaturePolicyResponse : AResponse<AddSignaturePolicyRequest,
                                                        AddSignaturePolicyResponse>,
                                              IResponse
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/cs/addSignaturePolicyResponse");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public JSONLDContext  Context
            => DefaultJSONLDContext;

        /// <summary>
        /// The success or failure status of the request.
        /// </summary>
        [Mandatory]
        public GenericStatus   Status        { get; }

        /// <summary>
        /// An optional element providing more information about the status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new AddSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request leading to this response.</param>
        /// <param name="Status">The success or failure status of the request.</param>
        /// <param name="StatusInfo">An optional element providing more information about the status.</param>
        /// 
        /// <param name="Result">The machine-readable result code.</param>
        /// <param name="ResponseTimestamp">The timestamp of the response message.</param>
        /// 
        /// <param name="DestinationId">The destination identification of the message within the overlay network.</param>
        /// <param name="NetworkPath">The networking path of the message through the overlay network.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this message.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this message.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures of this message.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AddSignaturePolicyResponse(AddSignaturePolicyRequest  Request,
                                          GenericStatus              Status,
                                          StatusInfo?                StatusInfo          = null,

                                          Result?                    Result              = null,
                                          DateTime?                  ResponseTimestamp   = null,

                                          NetworkingNode_Id?         DestinationNodeId   = null,
                                          NetworkPath?               NetworkPath         = null,

                                          IEnumerable<KeyPair>?      SignKeys            = null,
                                          IEnumerable<SignInfo>?     SignInfos           = null,
                                          IEnumerable<Signature>?    Signatures          = null,

                                          CustomData?                CustomData          = null)

            : base(Request,
                   Result ?? Result.OK(),
                   ResponseTimestamp,

                   DestinationNodeId,
                   NetworkPath,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

            unchecked
            {

                hashCode = this.Status.     GetHashCode()       * 5 ^
                          (this.StatusInfo?.GetHashCode() ?? 0) * 3 ^
                           base.GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an AddSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">An optional delegate to parse custom AddSignaturePolicy responses.</param>
        public static AddSignaturePolicyResponse Parse(AddSignaturePolicyRequest                                 Request,
                                                       JObject                                                   JSON,
                                                       NetworkingNode_Id                                         DestinationId,
                                                       NetworkPath                                               NetworkPath,
                                                       DateTime?                                                 ResponseTimestamp                        = null,
                                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null,
                                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {


            if (TryParse(Request,
                         JSON,
                         DestinationId,
                         NetworkPath,
                         out var addSignaturePolicyResponse,
                         out var errorResponse,
                         ResponseTimestamp,
                         CustomAddSignaturePolicyResponseParser,
                         CustomStatusInfoParser,
                         CustomSignatureParser,
                         CustomCustomDataParser))
            {
                return addSignaturePolicyResponse;
            }

            throw new ArgumentException("The given JSON representation of an AddSignaturePolicy response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AddSignaturePolicyResponse, out ErrorResponse, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an AddSignaturePolicy response.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AddSignaturePolicyResponse">The parsed AddSignaturePolicy response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">An optional delegate to parse custom AddSignaturePolicy responses.</param>
        public static Boolean TryParse(AddSignaturePolicyRequest                                 Request,
                                       JObject                                                   JSON,
                                       NetworkingNode_Id                                         DestinationId,
                                       NetworkPath                                               NetworkPath,
                                       [NotNullWhen(true)]  out AddSignaturePolicyResponse?      AddSignaturePolicyResponse,
                                       [NotNullWhen(false)] out String?                          ErrorResponse,
                                       DateTime?                                                 ResponseTimestamp                        = null,
                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null,
                                       CustomJObjectParserDelegate<StatusInfo>?                  CustomStatusInfoParser                   = null,
                                       CustomJObjectParserDelegate<Signature>?                   CustomSignatureParser                    = null,
                                       CustomJObjectParserDelegate<CustomData>?                  CustomCustomDataParser                   = null)
        {

            try
            {

                AddSignaturePolicyResponse = null;

                #region Status         [mandatory]

                if (!JSON.ParseMandatory("status",
                                         "registration status",
                                         GenericStatusExtensions.TryParse,
                                         out GenericStatus RegistrationStatus,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (RegistrationStatus == GenericStatus.Unknown)
                {
                    ErrorResponse = "Unknown registration status '" + (JSON["status"]?.Value<String>() ?? "") + "' received!";
                    return false;
                }

                #endregion

                #region StatusInfo     [optional]

                if (JSON.ParseOptionalJSON("statusInfo",
                                           "status info",
                                           OCPPv2_1.StatusInfo.TryParse,
                                           out StatusInfo? StatusInfo,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Signatures     [optional, OCPP_CSE]

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

                #region CustomData     [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                AddSignaturePolicyResponse = new AddSignaturePolicyResponse(

                                                 Request,
                                                 RegistrationStatus,
                                                 StatusInfo,

                                                 null,
                                                 ResponseTimestamp,

                                                 DestinationId,
                                                 NetworkPath,

                                                 null,
                                                 null,
                                                 Signatures,

                                                 CustomData

                                             );

                if (CustomAddSignaturePolicyResponseParser is not null)
                    AddSignaturePolicyResponse = CustomAddSignaturePolicyResponseParser(JSON,
                                                                                        AddSignaturePolicyResponse);

                return true;

            }
            catch (Exception e)
            {
                AddSignaturePolicyResponse  = null;
                ErrorResponse               = "The given JSON representation of an AddSignaturePolicy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddSignaturePolicyResponseSerializer">A delegate to serialize custom AddSignaturePolicy responses.</param>
        /// <param name="CustomStatusInfoSerializer">A delegate to serialize a custom status infos.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<StatusInfo>?                  CustomStatusInfoSerializer                   = null,
                              CustomJObjectSerializerDelegate<Signature>?                   CustomSignatureSerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?                  CustomCustomDataSerializer                   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("status",        Status.           AsText()),

                           StatusInfo is not null
                               ? new JProperty("statusInfo",    StatusInfo.       ToJSON(CustomStatusInfoSerializer,
                                                                                         CustomCustomDataSerializer))
                               : null,

                           Signatures.Any()
                               ? new JProperty("signatures",    new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.       ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomAddSignaturePolicyResponseSerializer is not null
                       ? CustomAddSignaturePolicyResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The AddSignaturePolicy failed because of a request error.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request.</param>
        public static AddSignaturePolicyResponse RequestError(AddSignaturePolicyRequest  Request,
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
                   GenericStatus.Rejected,
                   null,
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
        /// The AddSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddSignaturePolicyResponse FormationViolation(AddSignaturePolicyRequest  Request,
                                                                    String                     ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FormationViolation(
                                 $"Invalid data format: {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request.</param>
        /// <param name="ErrorDescription">An optional error description.</param>
        public static AddSignaturePolicyResponse SignatureError(AddSignaturePolicyRequest  Request,
                                                                String                     ErrorDescription)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.SignatureError(
                                 $"Invalid signature(s): {ErrorDescription}"
                             ));


        /// <summary>
        /// The AddSignaturePolicy failed.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request.</param>
        /// <param name="Description">An optional error description.</param>
        public static AddSignaturePolicyResponse Failed(AddSignaturePolicyRequest  Request,
                                                        String?                    Description   = null)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.Server(Description));


        /// <summary>
        /// The AddSignaturePolicy failed because of an exception.
        /// </summary>
        /// <param name="Request">The AddSignaturePolicy request.</param>
        /// <param name="Exception">The exception.</param>
        public static AddSignaturePolicyResponse ExceptionOccured(AddSignaturePolicyRequest  Request,
                                                                  Exception                  Exception)

            => new (Request,
                    GenericStatus.Rejected,
                    Result:  Result.FromException(Exception));

        #endregion


        #region Operator overloading

        #region Operator == (AddSignaturePolicyResponse1, AddSignaturePolicyResponse2)

        /// <summary>
        /// Compares two AddSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">An AddSignaturePolicy response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another AddSignaturePolicy response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (AddSignaturePolicyResponse? AddSignaturePolicyResponse1,
                                           AddSignaturePolicyResponse? AddSignaturePolicyResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AddSignaturePolicyResponse1, AddSignaturePolicyResponse2))
                return true;

            // If one is null, but not both, return false.
            if (AddSignaturePolicyResponse1 is null || AddSignaturePolicyResponse2 is null)
                return false;

            return AddSignaturePolicyResponse1.Equals(AddSignaturePolicyResponse2);

        }

        #endregion

        #region Operator != (AddSignaturePolicyResponse1, AddSignaturePolicyResponse2)

        /// <summary>
        /// Compares two AddSignaturePolicy responses for inequality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">An AddSignaturePolicy response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another AddSignaturePolicy response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddSignaturePolicyResponse? AddSignaturePolicyResponse1,
                                           AddSignaturePolicyResponse? AddSignaturePolicyResponse2)

            => !(AddSignaturePolicyResponse1 == AddSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<AddSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two AddSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="Object">An AddSignaturePolicy response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddSignaturePolicyResponse addSignaturePolicyResponse &&
                   Equals(addSignaturePolicyResponse);

        #endregion

        #region Equals(AddSignaturePolicyResponse)

        /// <summary>
        /// Compares two AddSignaturePolicy responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse">An AddSignaturePolicy response to compare with.</param>
        public override Boolean Equals(AddSignaturePolicyResponse? AddSignaturePolicyResponse)

            => AddSignaturePolicyResponse is not null &&

               Status.Equals(AddSignaturePolicyResponse.Status) &&

             ((StatusInfo is     null && AddSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AddSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(AddSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(AddSignaturePolicyResponse);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Status.AsText();

        #endregion

    }

}
