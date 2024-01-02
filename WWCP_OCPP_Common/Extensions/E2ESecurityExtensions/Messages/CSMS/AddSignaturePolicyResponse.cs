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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPP.CS
{

    /// <summary>
    /// An add signature policy response.
    /// </summary>
    public class AddSignaturePolicyResponse : AResponse<CSMS.AddSignaturePolicyRequest,
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
        /// The registration status.
        /// </summary>
        [Mandatory]
        public GenericStatus   Status        { get; }

        /// <summary>
        /// An optional element providing more information about the registration status.
        /// </summary>
        [Optional]
        public StatusInfo?     StatusInfo    { get; }

        #endregion

        #region Constructor(s)

        #region AddSignaturePolicyResponse(Request, Status, StatusInfo = null, ...)

        /// <summary>
        /// Create a new add signature policy response.
        /// </summary>
        /// <param name="Request">The add signature policy request leading to this response.</param>
        /// <param name="Status">The registration status.</param>
        /// <param name="StatusInfo">An optional element providing more information about the registration status.</param>
        /// <param name="ResponseTimestamp">An optional response timestamp.</param>
        /// 
        /// <param name="SignKeys">An optional enumeration of keys to be used for signing this response.</param>
        /// <param name="SignInfos">An optional enumeration of information to be used for signing this response.</param>
        /// <param name="Signatures">An optional enumeration of cryptographic signatures.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public AddSignaturePolicyResponse(CSMS.AddSignaturePolicyRequest  Request,
                                          GenericStatus                   Status,
                                          StatusInfo?                     StatusInfo          = null,
                                          DateTime?                       ResponseTimestamp   = null,

                                          IEnumerable<KeyPair>?           SignKeys            = null,
                                          IEnumerable<SignInfo>?          SignInfos           = null,
                                          IEnumerable<Signature>?         Signatures          = null,

                                          CustomData?                     CustomData          = null)

            : base(Request,
                   Result.OK(),
                   ResponseTimestamp,

                   SignKeys,
                   SignInfos,
                   Signatures,

                   CustomData)

        {

            this.Status      = Status;
            this.StatusInfo  = StatusInfo;

        }

        #endregion

        #region AddSignaturePolicyResponse(Request, Result)

        /// <summary>
        /// Create a new add signature policy response.
        /// </summary>
        /// <param name="Request">The authorize request.</param>
        /// <param name="Result">A result.</param>
        public AddSignaturePolicyResponse(CSMS.AddSignaturePolicyRequest  Request,
                                          Result                          Result)

            : base(Request,
                   Result)

        {

            this.Status = GenericStatus.Rejected;

        }

        #endregion

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (Request, JSON, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of an add signature policy response.
        /// </summary>
        /// <param name="Request">The add signature policy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">A delegate to parse custom add signature policy responses.</param>
        public static AddSignaturePolicyResponse Parse(CSMS.AddSignaturePolicyRequest                            Request,
                                                       JObject                                                   JSON,
                                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null)
        {


            if (TryParse(Request,
                         JSON,
                         out var addSignaturePolicyResponse,
                         out var errorResponse,
                         CustomAddSignaturePolicyResponseParser) &&
                addSignaturePolicyResponse is not null)
            {
                return addSignaturePolicyResponse;
            }

            throw new ArgumentException("The given JSON representation of an add signature policy response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out AddSignaturePolicyResponse, out ErrorResponse, CustomAddSignaturePolicyResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an add signature policy response.
        /// </summary>
        /// <param name="Request">The add signature policy request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="AddSignaturePolicyResponse">The parsed add signature policy response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddSignaturePolicyResponseParser">A delegate to parse custom add signature policy responses.</param>
        public static Boolean TryParse(CSMS.AddSignaturePolicyRequest                            Request,
                                       JObject                                                   JSON,
                                       out AddSignaturePolicyResponse?                           AddSignaturePolicyResponse,
                                       out String?                                               ErrorResponse,
                                       CustomJObjectParserDelegate<AddSignaturePolicyResponse>?  CustomAddSignaturePolicyResponseParser   = null)
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
                                           OCPP.StatusInfo.TryParse,
                                           out StatusInfo StatusInfo,
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
                                           OCPP.CustomData.TryParse,
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
                ErrorResponse             = "The given JSON representation of an add signature policy response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddSignaturePolicyResponseSerializer = null, CustomStatusInfoSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddSignaturePolicyResponseSerializer">A delegate to serialize custom add signature policy responses.</param>
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
        /// The add signature policy failed.
        /// </summary>
        public static AddSignaturePolicyResponse Failed(CSMS.AddSignaturePolicyRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (AddSignaturePolicyResponse1, AddSignaturePolicyResponse2)

        /// <summary>
        /// Compares two add signature policy responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">An add signature policy response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another add signature policy response.</param>
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
        /// Compares two add signature policy responses for inequality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse1">An add signature policy response.</param>
        /// <param name="AddSignaturePolicyResponse2">Another add signature policy response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (AddSignaturePolicyResponse? AddSignaturePolicyResponse1,
                                           AddSignaturePolicyResponse? AddSignaturePolicyResponse2)

            => !(AddSignaturePolicyResponse1 == AddSignaturePolicyResponse2);

        #endregion

        #endregion

        #region IEquatable<AddSignaturePolicyResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two add signature policy responses for equality.
        /// </summary>
        /// <param name="Object">An add signature policy response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is AddSignaturePolicyResponse addSignaturePolicyResponse &&
                   Equals(addSignaturePolicyResponse);

        #endregion

        #region Equals(AddSignaturePolicyResponse)

        /// <summary>
        /// Compares two add signature policy responses for equality.
        /// </summary>
        /// <param name="AddSignaturePolicyResponse">An add signature policy response to compare with.</param>
        public override Boolean Equals(AddSignaturePolicyResponse? AddSignaturePolicyResponse)

            => AddSignaturePolicyResponse is not null &&

               Status.Equals(AddSignaturePolicyResponse.Status) &&

             ((StatusInfo is     null && AddSignaturePolicyResponse.StatusInfo is     null) ||
              (StatusInfo is not null && AddSignaturePolicyResponse.StatusInfo is not null && StatusInfo.Equals(AddSignaturePolicyResponse.StatusInfo))) &&

               base.GenericEquals(AddSignaturePolicyResponse);

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

            => Status.AsText();

        #endregion

    }

}
