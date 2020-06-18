/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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

using System;
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// OCPP authorization data.
    /// </summary>
    public struct AuthorizationData : IEquatable<AuthorizationData>
    {

        #region Properties

        /// <summary>
        /// The identifier to which this authorization applies.
        /// </summary>
        public IdToken    IdTag       { get; }

        /// <summary>
        /// Information about authorization status, expiry and parent id.
        /// For a Differential update the following applies: If this element
        /// is present, then this entry SHALL be added or updated in the
        /// Local Authorization List. If this element is absent, than the
        /// entry for this idtag in the Local Authorization List SHALL be
        /// deleted.
        /// </summary>
        public IdTagInfo  IdTagInfo   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP configuration key value pair.
        /// </summary>
        /// <param name="IdTag">The identifier to which this authorization applies.</param>
        /// <param name="IdTagInfo">Information about authorization status, expiry and parent id. For a Differential update the following applies: If this element is present, then this entry SHALL be added or updated in the Local Authorization List. If this element is absent, than the entry for this idtag in the Local Authorization List SHALL be deleted.</param>
        public AuthorizationData(IdToken    IdTag,
                                 IdTagInfo  IdTagInfo)
        {

            this.IdTag      = IdTag;
            this.IdTagInfo  = IdTagInfo;

        }

        #endregion


        #region Documentation

        // <ns:authorizationData>
        //
        //    <ns:idTag>?</ns:idTag>
        //
        //    <!--Optional:-->
        //    <ns:idTagInfo>
        //
        //       <ns:status>?</ns:status>
        //
        //       <!--Optional:-->
        //       <ns:expiryDate>?</ns:expiryDate>
        //
        //       <!--Optional:-->
        //       <ns:parentIdTag>?</ns:parentIdTag>
        //
        //    </ns:idTagInfo>
        //
        // </ns:authorizationData>

        #endregion

        #region (static) Parse(AuthorizationDataXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="AuthorizationDataXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizationData Parse(XElement             AuthorizationDataXML,
                                              OnExceptionDelegate  OnException = null)
        {

            AuthorizationData _AuthorizationData;

            if (TryParse(AuthorizationDataXML, out _AuthorizationData, OnException))
                return _AuthorizationData;

            return default(AuthorizationData);

        }

        #endregion

        #region (static) Parse(AuthorizationDataText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="AuthorizationDataText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static AuthorizationData Parse(String               AuthorizationDataText,
                                              OnExceptionDelegate  OnException = null)
        {

            AuthorizationData _AuthorizationData;

            if (TryParse(AuthorizationDataText, out _AuthorizationData, OnException))
                return _AuthorizationData;

            return default(AuthorizationData);

        }

        #endregion

        #region (static) TryParse(AuthorizationDataXML,  out AuthorizationData, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="AuthorizationDataXML">The XML to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement               AuthorizationDataXML,
                                       out AuthorizationData  AuthorizationData,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                AuthorizationData = new AuthorizationData(

                                        AuthorizationDataXML.MapValueOrFail(OCPPNS.OCPPv1_6_CP + "idTag",
                                                                            IdToken.Parse),

                                        AuthorizationDataXML.MapElement    (OCPPNS.OCPPv1_6_CP + "idTagInfo",
                                                                            IdTagInfo.Parse)

                                    );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, AuthorizationDataXML, e);

                AuthorizationData = default(AuthorizationData);
                return false;

            }

        }

        #endregion

        #region (static) TryParse(AuthorizationDataText, out AuthorizationData, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP configuration key value pair.
        /// </summary>
        /// <param name="AuthorizationDataText">The text to be parsed.</param>
        /// <param name="AuthorizationData">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                 AuthorizationDataText,
                                       out AuthorizationData  AuthorizationData,
                                       OnExceptionDelegate    OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(AuthorizationDataText).Root,
                             out AuthorizationData,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, AuthorizationDataText, e);
            }

            AuthorizationData = default(AuthorizationData);
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:authorizationData"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "authorizationData",

                   new XElement(OCPPNS.OCPPv1_6_CP + "idTag",  IdTag.ToString()),

                   IdTagInfo != null
                       ? IdTagInfo.ToXML()
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AuthorizationData AuthorizationData1, AuthorizationData AuthorizationData2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(AuthorizationData1, AuthorizationData2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AuthorizationData1 == null) || ((Object) AuthorizationData2 == null))
                return false;

            if ((Object) AuthorizationData1 == null)
                throw new ArgumentNullException(nameof(AuthorizationData1),  "The given configuration key value pair must not be null!");

            return AuthorizationData1.Equals(AuthorizationData2);

        }

        #endregion

        #region Operator != (AuthorizationData1, AuthorizationData2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthorizationData1">An configuration key value pair.</param>
        /// <param name="AuthorizationData2">Another configuration key value pair.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AuthorizationData AuthorizationData1, AuthorizationData AuthorizationData2)
            => !(AuthorizationData1 == AuthorizationData2);

        #endregion

        #endregion

        #region IEquatable<AuthorizationData> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a AuthorizationData.
            if (!(Object is AuthorizationData))
                return false;

            return this.Equals((AuthorizationData) Object);

        }

        #endregion

        #region Equals(AuthorizationData)

        /// <summary>
        /// Compares two configuration key value pairs for equality.
        /// </summary>
        /// <param name="AuthorizationData">An configuration key value pair to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AuthorizationData AuthorizationData)
        {

            if ((Object) AuthorizationData == null)
                return false;

            return IdTag.Equals(AuthorizationData.IdTag) &&

                   ((IdTagInfo == null && AuthorizationData.IdTagInfo == null) ||
                    (IdTagInfo != null && AuthorizationData.IdTagInfo != null && IdTagInfo.Equals(AuthorizationData.IdTagInfo)));

        }

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

                return IdTag.GetHashCode() * 5 ^

                       (IdTagInfo != null
                            ? IdTagInfo.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(IdTag,

                             IdTagInfo != null
                                 ? " => " + IdTagInfo
                                 : "");

        #endregion


    }

}
