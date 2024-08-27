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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An access right for a device model component.
    /// </summary>
    public class ComponentAccessRights : ACustomData,
                                         IEquatable<ComponentAccessRights>
    {

        #region Properties

        /// <summary>
        /// The device model component.
        /// </summary>
        [Mandatory]
        public Component                 Component          { get; }

        /// <summary>
        /// The component access rights.
        /// </summary>
        [Mandatory]
        public IEnumerable<AccessRight>  AccessRights       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new component access right.
        /// </summary>
        /// <param name="Component">A device model component.</param>
        /// <param name="AccessRights">An enumeration of component access rights.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ComponentAccessRights(Component                 Component,
                                     IEnumerable<AccessRight>  AccessRights,
                                     CustomData?               CustomData   = null)

            : base(CustomData)

        {

            this.Component     = Component;
            this.AccessRights  = AccessRights.Distinct();

            unchecked
            {

                hashCode = this.Component.   GetHashCode()  * 5 ^
                           this.AccessRights.CalcHashCode() * 3 ^
                           base.             GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse       (JSON, CustomComponentAccessRightParser = null)

        /// <summary>
        /// Parse the given JSON representation of an component access right.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomComponentAccessRightParser">A delegate to parse custom component access rights.</param>
        public static ComponentAccessRights Parse(JObject                                            JSON,
                                                CustomJObjectParserDelegate<ComponentAccessRights>?  CustomComponentAccessRightParser   = null)
        {

            if (TryParse(JSON,
                         out var componentAccessRights,
                         out var errorResponse,
                         CustomComponentAccessRightParser) &&
                componentAccessRights is not null)
            {
                return componentAccessRights;
            }

            throw new ArgumentException("The given JSON representation of an component access right is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse    (JSON, out ComponentAccessRight, out ErrorResponse, CustomComponentAccessRightParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component access right.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentAccessRight">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out ComponentAccessRights?  ComponentAccessRight,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)

            => TryParse(JSON,
                        out ComponentAccessRight,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a component access right.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentAccessRight">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomComponentAccessRightParser">A delegate to parse custom component access rights.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       [NotNullWhen(true)]  out ComponentAccessRights?      ComponentAccessRight,
                                       [NotNullWhen(false)] out String?                     ErrorResponse,
                                       CustomJObjectParserDelegate<ComponentAccessRights>?  CustomComponentAccessRightParser   = null)
        {

            try
            {

                ComponentAccessRight = default;

                #region Component       [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "device model component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region AccessRights    [mandatory]

                if (!JSON.ParseMandatoryHashSet("accessRights",
                                                "component access rights",
                                                AccessRight.TryParse,
                                                out HashSet<AccessRight> AccessRights,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData      [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ComponentAccessRight = new ComponentAccessRights(
                                           Component,
                                           AccessRights,
                                           CustomData
                                       );

                if (CustomComponentAccessRightParser is not null)
                    ComponentAccessRight = CustomComponentAccessRightParser(JSON,
                                                                            ComponentAccessRight);

                return true;

            }
            catch (Exception e)
            {
                ComponentAccessRight  = default;
                ErrorResponse        = "The given JSON representation of a component access right is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomComponentAccessRightSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomComponentAccessRightsSerializer">A delegate to serialize component access rights.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ComponentAccessRights>?  CustomComponentAccessRightsSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?              CustomComponentSerializer               = null,
                              CustomJObjectSerializerDelegate<EVSE>?                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<CustomData>?             CustomCustomDataSerializer              = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("component",      Component. ToJSON(
                                                                                CustomComponentSerializer,
                                                                                CustomEVSESerializer,
                                                                                CustomCustomDataSerializer
                                                                            )),

                                 new JProperty("accessRights",   new JArray(AccessRights.Select(accessRight => accessRight.ToString()))),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomComponentAccessRightsSerializer is not null
                       ? CustomComponentAccessRightsSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ComponentAccessRights Clone()

            => new (

                   Component.   Clone(),
                   AccessRights.Select(accessRight => accessRight.Clone),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (ComponentAccessRight1, ComponentAccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentAccessRight1">A component access right.</param>
        /// <param name="ComponentAccessRight2">Another component access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ComponentAccessRights? ComponentAccessRight1,
                                           ComponentAccessRights? ComponentAccessRight2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ComponentAccessRight1, ComponentAccessRight2))
                return true;

            // If one is null, but not both, return false.
            if (ComponentAccessRight1 is null || ComponentAccessRight2 is null)
                return false;

            return ComponentAccessRight1.Equals(ComponentAccessRight2);

        }

        #endregion

        #region Operator != (ComponentAccessRight1, ComponentAccessRight2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentAccessRight1">A component access right.</param>
        /// <param name="ComponentAccessRight2">Another component access right.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ComponentAccessRights? ComponentAccessRight1,
                                           ComponentAccessRights? ComponentAccessRight2)

            => !(ComponentAccessRight1 == ComponentAccessRight2);

        #endregion

        #endregion

        #region IEquatable<ComponentAccessRight> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two component access rights for equality.
        /// </summary>
        /// <param name="Object">A component access right to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ComponentAccessRights componentAccessRights &&
                   Equals(componentAccessRights);

        #endregion

        #region Equals(ComponentAccessRight)

        /// <summary>
        /// Compares two component access rights for equality.
        /// </summary>
        /// <param name="ComponentAccessRight">A component access right to compare with.</param>
        public Boolean Equals(ComponentAccessRights? ComponentAccessRights)

            => ComponentAccessRights is not null &&

               Component.Equals(ComponentAccessRights.Component) &&

               AccessRights.Count() == ComponentAccessRights.AccessRights.Count() &&
               AccessRights.All(accessRight => ComponentAccessRights.AccessRights.Contains(accessRight)) &&

               base.Equals(ComponentAccessRights);

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

            => $"{Component}: {AccessRights.AggregateWith(", ")}";

        #endregion

    }

}
