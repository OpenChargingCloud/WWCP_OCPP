/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Extension methods for security event types.
    /// </summary>
    public static class SecurityEventTypeExtensions
    {

        /// <summary>
        /// Indicates whether this security event type is null or empty.
        /// </summary>
        /// <param name="SecurityEvent">A security event type.</param>
        public static Boolean IsNullOrEmpty(this SecurityEventType? SecurityEvent)
            => !SecurityEvent.HasValue || SecurityEvent.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this security event type is null or empty.
        /// </summary>
        /// <param name="SecurityEvent">A security event type.</param>
        public static Boolean IsNotNullOrEmpty(this SecurityEventType? SecurityEvent)
            => SecurityEvent.HasValue && SecurityEvent.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// A security event type.
    /// </summary>
    public readonly struct SecurityEventType : IId,
                                               IEquatable<SecurityEventType>,
                                               IComparable<SecurityEventType>
    {

        #region Properties

        /// <summary>
        /// The security event type.
        /// </summary>
        public String    Text           { get; }

        /// <summary>
        /// The optional description of the security event type.
        /// </summary>
        public String?   Description    { get; }

        /// <summary>
        /// Whether the security event type is critical.
        /// </summary>
        public Boolean?  IsCritical     { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public readonly Boolean IsNullOrEmpty
            => Text.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public readonly Boolean IsNotNullOrEmpty
            => Text.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the security event type.
        /// </summary>
        public readonly UInt64 Length
            => (UInt64) (Text?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new security event type.
        /// </summary>
        /// <param name="Text">The string representation of the security event type.</param>
        /// <param name="Description">An optional description of the security event type.</param>
        /// <param name="IsCritical">Whether the security event type is critical.</param>
        private SecurityEventType(String    Text,
                                  String?   Description   = null,
                                  Boolean?  IsCritical    = null)
        {

            this.Text         = Text;
            this.Description  = Description;
            this.IsCritical   = IsCritical;

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a security event type.
        /// </summary>
        /// <param name="Text">A text representation of a security event type.</param>
        public static SecurityEventType Parse(String Text)
        {

            if (TryParse(Text, out var securityEventType))
                return securityEventType;

            throw new ArgumentException($"Invalid text representation of a security event type: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a security event type.
        /// </summary>
        /// <param name="Text">A text representation of a security event type.</param>
        public static SecurityEventType? TryParse(String Text)
        {

            if (TryParse(Text, out var securityEventType))
                return securityEventType;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out SecurityEvent)

        /// <summary>
        /// Try to parse the given text as a security event type.
        /// </summary>
        /// <param name="Text">A text representation of a security event type.</param>
        /// <param name="SecurityEvent">The parsed security event type.</param>
        public static Boolean TryParse(String Text, out SecurityEventType SecurityEvent)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                SecurityEvent = new SecurityEventType(Text);
                return true;
            }

            SecurityEvent = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this security event type.
        /// </summary>
        public SecurityEventType Clone

            => new (
                   new String(Text?.       ToCharArray()),
                   new String(Description?.ToCharArray()),
                   IsCritical
               );

        #endregion


        #region ToJSON(CustomSecurityEventSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSecurityEventSerializer">A delegate to serialize security event types.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SecurityEventType>? CustomSecurityEventSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("text",          Text),

                           Description is not null
                               ? new JProperty("description",   Description)
                               : null,

                           IsCritical.HasValue
                               ? new JProperty("isCritical",    IsCritical.Value)
                               : null

                       );

            return CustomSecurityEventSerializer is not null
                       ? CustomSecurityEventSerializer(this, json)
                       : json;

        }

        #endregion


        #region Statics

        /// <summary>
        /// The Charge Point firmware is updated.
        /// </summary>
        public readonly static SecurityEventType FirmwareUpdated                        = new ("FirmwareUpdated",
                                                                                               "The Charge Point firmware is updated.",
                                                                                                true);

        /// <summary>
        /// The authentication credentials provided by the Charge Point were rejected by the Central System.
        /// </summary>
        public readonly static SecurityEventType FailedToAuthenticateAtCSMS             = new ("FailedToAuthenticateAtCSMS",
                                                                                               "The authentication credentials provided by the Charge Point were rejected by the Central System.",
                                                                                                false);

        /// <summary>
        /// The authentication credentials provided by the Central System were rejected by the Charge Point.
        /// </summary>
        public readonly static SecurityEventType CSMSFailedToAuthenticate               = new ("CSMSFailedToAuthenticate",
                                                                                               "The authentication credentials provided by the Central System were rejected by the Charge Point.",
                                                                                                false);

        /// <summary>
        /// The system time on the Charge Point was changed.
        /// </summary>
        public readonly static SecurityEventType SettingSystemTime                      = new ("SettingSystemTime",
                                                                                               "The system time on the Charge Point was changed.",
                                                                                                true);

        /// <summary>
        /// The Charge Point has booted.
        /// </summary>
        public readonly static SecurityEventType StartupOfTheDevice                     = new ("StartupOfTheDevice",
                                                                                               "The Charge Point has booted.",
                                                                                                true);

        /// <summary>
        /// The Charge Point was rebooted or reset.
        /// </summary>
        public readonly static SecurityEventType ResetOrReboot                          = new ("ResetOrReboot",
                                                                                               "The Charge Point was rebooted or reset.",
                                                                                                true);

        /// <summary>
        /// The security log was cleared.
        /// </summary>
        public readonly static SecurityEventType SecurityLogWasCleared                  = new ("SecurityLogWasCleared",
                                                                                               "The security log was cleared.",
                                                                                                true);

        /// <summary>
        /// Security parameters, such as keys or the security profile used, were changed.
        /// </summary>
        public readonly static SecurityEventType ReconfigurationOfSecurityParameters    = new ("ReconfigurationOfSecurityParameters",
                                                                                               "Security parameters, such as keys or the security profile used, were changed.",
                                                                                                false);

        /// <summary>
        /// The Flash or RAM memory of the Charge Point is getting full.
        /// </summary>
        public readonly static SecurityEventType MemoryExhaustion                       = new ("MemoryExhaustion",
                                                                                               "The Flash or RAM memory of the Charge Point is getting full.",
                                                                                                true);

        /// <summary>
        /// The Charge Point has received messages that are not valid OCPP messages, if signed messages, signage invalid/incorrect.
        /// </summary>
        public readonly static SecurityEventType InvalidMessages                        = new ("InvalidMessages",
                                                                                               "The Charge Point has received messages that are not valid OCPP messages, if signed messages, signage invalid/incorrect.",
                                                                                                false);

        /// <summary>
        /// The Charge Point has received a replayed message (other than the Central System trying to resend a message because it there was for example a network problem).
        /// </summary>
        public readonly static SecurityEventType AttemptedReplayAttacks                 = new ("AttemptedReplayAttacks",
                                                                                               "The Charge Point has received a replayed message (other than the Central System trying to resend a message because it there was for example a network problem).",
                                                                                                false);

        /// <summary>
        /// The physical tamper detection sensor was triggered.
        /// </summary>
        public readonly static SecurityEventType TamperDetectionActivated               = new ("TamperDetectionActivated",
                                                                                               "The physical tamper detection sensor was triggered.",
                                                                                                true);

        /// <summary>
        /// The firmware signature is not valid.
        /// </summary>
        public readonly static SecurityEventType InvalidFirmwareSignature               = new ("InvalidFirmwareSignature",
                                                                                               "The firmware signature is not valid.",
                                                                                                false);

        /// <summary>
        /// The certificate used to verify the firmware signature is not valid.
        /// </summary>
        public readonly static SecurityEventType InvalidFirmwareSigningCertificate      = new ("InvalidFirmwareSigningCertificate",
                                                                                               "The certificate used to verify the firmware signature is not valid.",
                                                                                                false);

        /// <summary>
        /// The certificate that the Central System uses was not valid or could not be verified.
        /// </summary>
        public readonly static SecurityEventType InvalidCSMSCertificate                 = new ("InvalidCSMSCertificate",
                                                                                               "The certificate that the Central System uses was not valid or could not be verified.",
                                                                                                false);

        /// <summary>
        /// The certificate sent to the Charge Point using the SignCertificate.conf message is not a valid certificate.
        /// </summary>
        public readonly static SecurityEventType InvalidChargePointCertificate          = new ("InvalidChargePointCertificate",
                                                                                               "The certificate sent to the Charge Point using the SignCertificate.conf message is not a valid certificate.",
                                                                                                false);

        /// <summary>
        /// The TLS version used by the Central System is lower than 1.2 and is not allowed by the security specification.
        /// </summary>
        public readonly static SecurityEventType InvalidTLSVersion                      = new ("InvalidTLSVersion",
                                                                                               "The TLS version used by the Central System is lower than 1.2 and is not allowed by the security specification.",
                                                                                                false);

        /// <summary>
        /// The Central System did only allow connections using TLS cipher suites that are not allowed by the security specification.
        /// </summary>
        public readonly static SecurityEventType InvalidTLSCipherSuite                  = new ("InvalidTLSCipherSuite",
                                                                                               "The Central System did only allow connections using TLS cipher suites that are not allowed by the security specification.",
                                                                                                false);

        #endregion


        #region Operator overloading

        #region Operator == (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SecurityEventType SecurityEvent1,
                                           SecurityEventType SecurityEvent2)

            => SecurityEvent1.Equals(SecurityEvent2);

        #endregion

        #region Operator != (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SecurityEventType SecurityEvent1,
                                           SecurityEventType SecurityEvent2)

            => !SecurityEvent1.Equals(SecurityEvent2);

        #endregion

        #region Operator <  (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SecurityEventType SecurityEvent1,
                                          SecurityEventType SecurityEvent2)

            => SecurityEvent1.CompareTo(SecurityEvent2) < 0;

        #endregion

        #region Operator <= (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SecurityEventType SecurityEvent1,
                                           SecurityEventType SecurityEvent2)

            => SecurityEvent1.CompareTo(SecurityEvent2) <= 0;

        #endregion

        #region Operator >  (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SecurityEventType SecurityEvent1,
                                          SecurityEventType SecurityEvent2)

            => SecurityEvent1.CompareTo(SecurityEvent2) > 0;

        #endregion

        #region Operator >= (SecurityEvent1, SecurityEvent2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityEvent1">A security event type.</param>
        /// <param name="SecurityEvent2">Another security event type.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SecurityEventType SecurityEvent1,
                                           SecurityEventType SecurityEvent2)

            => SecurityEvent1.CompareTo(SecurityEvent2) >= 0;

        #endregion

        #endregion

        #region IComparable<SecurityEvent> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two security event types.
        /// </summary>
        /// <param name="Object">A security event type to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is SecurityEventType securityEventType
                   ? CompareTo(securityEventType)
                   : throw new ArgumentException("The given object is not a security event type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SecurityEvent)

        /// <summary>
        /// Compares two security event types.
        /// </summary>
        /// <param name="SecurityEvent">A security event type to compare with.</param>
        public Int32 CompareTo(SecurityEventType SecurityEvent)

            => String.Compare(Text,
                              SecurityEvent.Text,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<SecurityEvent> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two security event types for equality.
        /// </summary>
        /// <param name="Object">A security event type to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SecurityEventType securityEventType &&
                   Equals(securityEventType);

        #endregion

        #region Equals(SecurityEvent)

        /// <summary>
        /// Compares two security event types for equality.
        /// </summary>
        /// <param name="SecurityEvent">A security event type to compare with.</param>
        public Boolean Equals(SecurityEventType SecurityEvent)

            => String.Equals(Text,
                             SecurityEvent.Text,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Text?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Text ?? "";

        #endregion

    }

}
