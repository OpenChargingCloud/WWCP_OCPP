/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS
{

    /// <summary>
    /// A notify priority charging response.
    /// </summary>
    public class NotifyPriorityChargingResponse : AResponse<CS.NotifyPriorityChargingRequest,
                                                            NotifyPriorityChargingResponse>
    {

        #region Constructor(s)

        #region NotifyPriorityChargingResponse(Request, ...)

        /// <summary>
        /// Create a new notify priority charging response.
        /// </summary>
        /// <param name="Request">The notify priority charging request leading to this response.</param>
        /// 
        /// <param name="Signatures">An optional enumeration of cryptographic signatures for this message.</param>
        /// <param name="CustomData">The custom data object to allow to store any kind of customer specific data.</param>
        public NotifyPriorityChargingResponse(CS.NotifyPriorityChargingRequest  Request,

                                              IEnumerable<Signature>?           Signatures   = null,
                                              CustomData?                       CustomData   = null)

            : base(Request,
                   Result.OK(),
                   Signatures,
                   CustomData)

        { }

        #endregion

        #region NotifyPriorityChargingResponse(Request, Result)

        /// <summary>
        /// Create a new notify priority charging response.
        /// </summary>
        /// <param name="Request">The notify priority charging request leading to this response.</param>
        /// <param name="Result">The result.</param>
        public NotifyPriorityChargingResponse(CS.NotifyPriorityChargingRequest  Request,
                                              Result                            Result)

            : base(Request,
                   Result,
                   Timestamp.Now)

        { }

        #endregion

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (Request, JSON, CustomNotifyPriorityChargingResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a notify priority charging response.
        /// </summary>
        /// <param name="Request">The notify priority charging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomNotifyPriorityChargingResponseParser">A delegate to parse custom notify priority charging responses.</param>
        public static NotifyPriorityChargingResponse Parse(CS.NotifyPriorityChargingRequest                              Request,
                                                           JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseParser   = null)
        {

            if (TryParse(Request,
                         JSON,
                         out var notifyPriorityChargingResponse,
                         out var errorResponse,
                         CustomNotifyPriorityChargingResponseParser))
            {
                return notifyPriorityChargingResponse!;
            }

            throw new ArgumentException("The given JSON representation of a notify priority charging response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Request, JSON, out NotifyPriorityChargingResponse, out ErrorResponse, CustomNotifyPriorityChargingResponseParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a notify priority charging response.
        /// </summary>
        /// <param name="Request">The notify priority charging request leading to this response.</param>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="NotifyPriorityChargingResponse">The parsed notify priority charging response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyPriorityChargingResponseParser">A delegate to parse custom notify priority charging responses.</param>
        public static Boolean TryParse(CS.NotifyPriorityChargingRequest                              Request,
                                       JObject                                                       JSON,
                                       out NotifyPriorityChargingResponse?                           NotifyPriorityChargingResponse,
                                       out String?                                                   ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseParser   = null)
        {

            ErrorResponse = null;

            try
            {

                NotifyPriorityChargingResponse = null;

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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyPriorityChargingResponse = new NotifyPriorityChargingResponse(
                                                     Request,
                                                     Signatures,
                                                     CustomData
                                                 );

                if (CustomNotifyPriorityChargingResponseParser is not null)
                    NotifyPriorityChargingResponse = CustomNotifyPriorityChargingResponseParser(JSON,
                                                                                                NotifyPriorityChargingResponse);

                return true;

            }
            catch (Exception e)
            {
                NotifyPriorityChargingResponse  = null;
                ErrorResponse                   = "The given JSON representation of a notify priority charging response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyPriorityChargingResponseSerializer = null, CustomSignatureSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyPriorityChargingResponseSerializer">A delegate to serialize custom notify priority charging responses.</param>
        /// <param name="CustomSignatureSerializer">A delegate to serialize cryptographic signature objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyPriorityChargingResponse>?  CustomNotifyPriorityChargingResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<Signature>?                       CustomSignatureSerializer                        = null,
                              CustomJObjectSerializerDelegate<CustomData>?                      CustomCustomDataSerializer                       = null)
        {

            var json = JSONObject.Create(

                           Signatures is not null
                               ? new JProperty("signatures",   new JArray(Signatures.Select(signature => signature.ToJSON(CustomSignatureSerializer,
                                                                                                                          CustomCustomDataSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomNotifyPriorityChargingResponseSerializer is not null
                       ? CustomNotifyPriorityChargingResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        /// <summary>
        /// The notify priority charging request failed.
        /// </summary>
        public static NotifyPriorityChargingResponse Failed(CS.NotifyPriorityChargingRequest Request)

            => new (Request,
                    Result.Server());

        #endregion


        #region Operator overloading

        #region Operator == (NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2)

        /// <summary>
        /// Compares two notify priority charging responses for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse1">A notify priority charging response.</param>
        /// <param name="NotifyPriorityChargingResponse2">Another notify priority charging response.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (NotifyPriorityChargingResponse? NotifyPriorityChargingResponse1,
                                           NotifyPriorityChargingResponse? NotifyPriorityChargingResponse2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2))
                return true;

            // If one is null, but not both, return false.
            if (NotifyPriorityChargingResponse1 is null || NotifyPriorityChargingResponse2 is null)
                return false;

            return NotifyPriorityChargingResponse1.Equals(NotifyPriorityChargingResponse2);

        }

        #endregion

        #region Operator != (NotifyPriorityChargingResponse1, NotifyPriorityChargingResponse2)

        /// <summary>
        /// Compares two notify priority charging responses for inequality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse1">A notify priority charging response.</param>
        /// <param name="NotifyPriorityChargingResponse2">Another notify priority charging response.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (NotifyPriorityChargingResponse? NotifyPriorityChargingResponse1,
                                           NotifyPriorityChargingResponse? NotifyPriorityChargingResponse2)

            => !(NotifyPriorityChargingResponse1 == NotifyPriorityChargingResponse2);

        #endregion

        #endregion

        #region IEquatable<NotifyPriorityChargingResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two notify priority charging responses for equality.
        /// </summary>
        /// <param name="Object">A notify priority charging response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyPriorityChargingResponse notifyPriorityChargingResponse &&
                   Equals(notifyPriorityChargingResponse);

        #endregion

        #region Equals(NotifyPriorityChargingResponse)

        /// <summary>
        /// Compares two notify priority charging responses for equality.
        /// </summary>
        /// <param name="NotifyPriorityChargingResponse">A notify priority charging response to compare with.</param>
        public override Boolean Equals(NotifyPriorityChargingResponse? NotifyPriorityChargingResponse)

            => NotifyPriorityChargingResponse is not null &&
                   base.GenericEquals(NotifyPriorityChargingResponse);

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

            => "NotifyPriorityChargingResponse";

        #endregion

    }

}
