/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
    /// An OCPP identification tag info.
    /// </summary>
    public struct IdTagInfo : IEquatable<IdTagInfo>
    {

        #region Properties

        /// <summary>
        /// The authentication result.
        /// </summary>
        public AuthorizationStatus  Status        { get; }

        /// <summary>
        /// An optional date at which the idTag should be removed
        /// from the authorization cache.
        /// </summary>
        public DateTime?            ExpiryDate    { get; }

        /// <summary>
        /// An optional the parent-identifier.
        /// </summary>
        public IdToken?             ParentIdTag   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP identification tag info.
        /// </summary>
        /// <param name="Status">The authentication result.</param>
        /// <param name="ExpiryDate">An optional date at which the idTag should be removed from the authorization cache.</param>
        /// <param name="ParentIdTag">An optional the parent-identifier.</param>
        public IdTagInfo(AuthorizationStatus  Status,
                         DateTime?            ExpiryDate   = null,
                         IdToken?             ParentIdTag  = null)
        {

            this.Status       = Status;
            this.ExpiryDate   = ExpiryDate  ?? new DateTime?();
            this.ParentIdTag  = ParentIdTag ?? new IdToken?();

        }

        #endregion


        #region Documentation

        // <ns:idTagInfo>
        //
        //    <ns:status>?</ns:status>
        //
        //    <!--Optional:-->
        //    <ns:expiryDate>?</ns:expiryDate>
        //
        //    <!--Optional:-->
        //    <ns:parentIdTag>?</ns:parentIdTag>
        //
        // </ns:idTagInfo>

        #endregion

        #region (static) Parse(IdTagInfoXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP identification tag info.
        /// </summary>
        /// <param name="IdTagInfoXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTagInfo Parse(XElement             IdTagInfoXML,
                                      OnExceptionDelegate  OnException = null)
        {

            IdTagInfo _IdTagInfo;

            if (TryParse(IdTagInfoXML, out _IdTagInfo, OnException))
                return _IdTagInfo;

            return default(IdTagInfo);

        }

        #endregion

        #region (static) Parse(IdTagInfoText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP identification tag info.
        /// </summary>
        /// <param name="IdTagInfoText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static IdTagInfo Parse(String               IdTagInfoText,
                                      OnExceptionDelegate  OnException = null)
        {

            IdTagInfo _IdTagInfo;

            if (TryParse(IdTagInfoText, out _IdTagInfo, OnException))
                return _IdTagInfo;

            return default(IdTagInfo);

        }

        #endregion

        #region (static) TryParse(IdTagInfoXML,  out IdTagInfo, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP identification tag info.
        /// </summary>
        /// <param name="IdTagInfoXML">The XML to parse.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             IdTagInfoXML,
                                       out IdTagInfo        IdTagInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                IdTagInfo = new IdTagInfo(

                                    IdTagInfoXML.MapEnumValues     (OCPPNS.OCPPv1_6_CS + "status",
                                                                    XML_IO.AsAuthorizationStatus),

                                    IdTagInfoXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "expiryDate",
                                                                    DateTime.Parse),

                                    IdTagInfoXML.MapValueOrNull    (OCPPNS.OCPPv1_6_CS + "parentIdTag",
                                                                    IdToken.Parse)

                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, IdTagInfoXML, e);

                IdTagInfo = default(IdTagInfo);
                return false;

            }

        }

        #endregion

        #region (static) TryParse(IdTagInfoText, out IdTagInfo, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP identification tag info.
        /// </summary>
        /// <param name="IdTagInfoText">The text to parse.</param>
        /// <param name="IdTagInfo">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               IdTagInfoText,
                                       out IdTagInfo        IdTagInfo,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(IdTagInfoText).Root,
                             out IdTagInfo,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, IdTagInfoText, e);
            }

            IdTagInfo = default(IdTagInfo);
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:idTagInfo"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "idTagInfo",

                   new XElement(OCPPNS.OCPPv1_6_CS + "connectorStandard",  XML_IO.AsText(Status)),

                   ExpiryDate.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "expiryDate",   ExpiryDate.Value.ToIso8601())
                       : null,

                   ParentIdTag.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "parentIdTag",  ParentIdTag.Value.ToString())
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(IdTagInfo1, IdTagInfo2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) IdTagInfo1 == null) || ((Object) IdTagInfo2 == null))
                return false;

            if ((Object) IdTagInfo1 == null)
                throw new ArgumentNullException(nameof(IdTagInfo1),  "The given id tag info must not be null!");

            return IdTagInfo1.Equals(IdTagInfo2);

        }

        #endregion

        #region Operator != (IdTagInfo1, IdTagInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="IdTagInfo1">An id tag info.</param>
        /// <param name="IdTagInfo2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (IdTagInfo IdTagInfo1, IdTagInfo IdTagInfo2)
            => !(IdTagInfo1 == IdTagInfo2);

        #endregion

        #endregion

        #region IEquatable<IdTagInfo> Members

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

            // Check if the given object is a id tag info.
            if (!(Object is IdTagInfo))
                return false;

            return this.Equals((IdTagInfo) Object);

        }

        #endregion

        #region Equals(IdTagInfo)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="IdTagInfo">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IdTagInfo IdTagInfo)
        {

            if ((Object) IdTagInfo == null)
                return false;

            return Status.Equals(IdTagInfo.Status) &&

                   ((!ExpiryDate.HasValue  && !IdTagInfo.ExpiryDate. HasValue) ||
                     (ExpiryDate.HasValue  &&  IdTagInfo.ExpiryDate. HasValue && ExpiryDate. Value.Equals(IdTagInfo.ExpiryDate. Value))) &&

                   ((!ParentIdTag.HasValue && !IdTagInfo.ParentIdTag.HasValue) ||
                     (ParentIdTag.HasValue &&  IdTagInfo.ParentIdTag.HasValue && ParentIdTag.Value.Equals(IdTagInfo.ParentIdTag.Value)));

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

                return (ParentIdTag.HasValue
                            ? ParentIdTag.GetHashCode() * 7
                            : 0) ^

                       (ExpiryDate.HasValue
                            ? ExpiryDate. GetHashCode() * 5
                            : 0) ^

                       Status.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Status,

                             ParentIdTag.HasValue
                                 ? " (" + ParentIdTag.Value + ")"
                                 : "",

                             ExpiryDate.HasValue
                                 ? " valid till " + ExpiryDate.Value.ToIso8601()
                                 : "");

        #endregion


    }

}
