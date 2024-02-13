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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Logical Component responsible for configuration relating to information exchange between a charging station and a CSMS.
    /// </summary>
    public class OCPPCommCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// Indicates the configuration profile the station uses at that moment to connect to the network.
        /// </summary>
        public NetworkProfiles?                   ActiveNetworkProfile                { get; }

        /// <summary>
        /// List of supported file transfer protocols.
        /// </summary>
        [Mandatory]
        public IEnumerable<FileTransferProtocol>  FileTransferProtocols               { get; }

        /// <summary>
        /// Interval in seconds of inactivity (no OCPP exchanges) with CSMS after which the Charging Station should send HeartbeatRequest.
        /// </summary>
        public TimeSpan?                          HeartbeatInterval                   { get; set; }

        /// <summary>
        /// How often the Charging Station should try to submit a TransactionEventRequest message when the CSMS fails to process it.
        /// </summary>
        [Mandatory]
        public Int16                              MessageAttempts                     { get; set; }

        /// <summary>
        /// How long in seconds the Charging Station should wait before resubmitting a TransactionEventRequest message that the CSMS failed to process.
        /// </summary>
        [Mandatory]
        public TimeSpan                           MessageAttemptInterval              { get; set; }

        /// <summary>
        /// Message timeout in seconds. The message timeout setting in a Charging Station can be configured in the messageTimeout field in the NetworkConnectionProfile.
        /// </summary>
        [Mandatory]
        public TimeSpan                           DefaultMessageTimeout               { get; }

        /// <summary>
        /// A comma separated ordered list of the priority of the possible Network Connection Profiles.
        /// </summary>
        [Mandatory]
        public String                             NetworkConfigurationPriority        { get; set; }

        /// <summary>
        /// Specifies the number of connection attempts the Charging Station executes before switching to a different profile.
        /// </summary>
        [Mandatory]
        public Int16                              NetworkProfileConnectionAttempts    { get; set; }

        /// <summary>
        /// When the offline period in seconds of a Charging Station exceeds the OfflineThreshold it is recommended to send a StatusNotificationRequest for all its Connectors when the Charging Station is back online.
        /// </summary>
        [Mandatory]
        public TimeSpan                           OfflineThreshold                    { get; set; }

        /// <summary>
        /// This Configuration Variable can be used to configure whether a public key needs to be sent with a signed meter value.
        /// </summary>
        public Boolean?                           PublicKeyWithSignedMeterValue       { get; set; }

        /// <summary>
        /// When this variable is set to true, the Charging Station will queue all message until they are delivered to the CSMS.
        /// </summary>
        public Boolean?                           QueueAllMessages                    { get; set; }

        /// <summary>
        /// Number of times to retry a reset of the Charging Station when a reset was unsuccessful.
        /// </summary>
        [Mandatory]
        public Byte                               ResetRetries                        { get; set; }

        /// <summary>
        /// When the charging station is reconnecting, after a connection loss, it will use this variable for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the charging station keeps connecting with the same back-off time.
        /// </summary>
        public UInt16?                            RetryBackOffRepeatTimes             { get; set; }

        /// <summary>
        /// When the Charging Station is reconnecting, after a connection loss, it will use this variable as the maximum value for the random part of the back-off time. It will add a new random value to every increasing back-off time, including the first connection attempt (with this maximum), for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the Charging Station will keep connecting with the same back-off time.
        /// </summary>
        public TimeSpan?                          RetryBackOffRandomRange             { get; set; }

        /// <summary>
        /// When the charging station is reconnecting, after a connection loss, it will use this variable as the minimum backoff time, the first time it tries to reconnect.
        /// </summary>
        public TimeSpan?                          RetryBackOffWaitMinimum             { get; set; }

        /// <summary>
        /// When set to true, the Charging Station will unlock the cable on Charging Station side when the cable is unplugged at the EV.
        /// </summary>
        [Mandatory]
        public Boolean                            UnlockOnEVSideDisconnect            { get; set; }

        /// <summary>
        /// A value of 0 disables client side websocket ping/pong. In this case there is either no ping/pong or the server initiates the ping and client responds with Pong. Positive values are interpreted as number of seconds between pings. Negative values are not allowed, SetConfiguration is then expected to return a Rejected result. It is recommended to configure WebSocketPingInterval smaller then: MessageAttemptsTransactionEvent * MessageAttemptIntervalTransactionEvent. This will limit the chance of the resend mechanism for transactionrelated messages being triggered by connectivity issues.
        /// </summary>
        public TimeSpan?                          WebSocketPingInterval               { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP communication controller.
        /// </summary>
        /// <param name="ActiveNetworkProfile">Indicates the configuration profile the station uses at that moment to connect to the network.</param>
        /// <param name="FileTransferProtocols">List of supported file transfer protocols.</param>
        /// <param name="HeartbeatInterval">Interval in seconds of inactivity (no OCPP exchanges) with CSMS after which the Charging Station should send HeartbeatRequest.</param>
        /// <param name="MessageAttempts">How often the Charging Station should try to submit a TransactionEventRequest message when the CSMS fails to process it.</param>
        /// <param name="MessageAttemptInterval">How long in seconds the Charging Station should wait before resubmitting a TransactionEventRequest message that the CSMS failed to process.</param>
        /// <param name="DefaultMessageTimeout">Message timeout in seconds. The message timeout setting in a Charging Station can be configured in the messageTimeout field in the NetworkConnectionProfile.</param>
        /// <param name="NetworkConfigurationPriority">A comma separated ordered list of the priority of the possible Network Connection Profiles.</param>
        /// <param name="NetworkProfileConnectionAttempts">Specifies the number of connection attempts the Charging Station executes before switching to a different profile.</param>
        /// <param name="OfflineThreshold">When the offline period in seconds of a Charging Station exceeds the OfflineThreshold it is recommended to send a StatusNotificationRequest for all its Connectors when the Charging Station is back online.</param>
        /// <param name="PublicKeyWithSignedMeterValue">This Configuration Variable can be used to configure whether a public key needs to be sent with a signed meter value.</param>
        /// <param name="QueueAllMessages">When this variable is set to true, the Charging Station will queue all message until they are delivered to the CSMS.</param>
        /// <param name="ResetRetries">Number of times to retry a reset of the Charging Station when a reset was unsuccessful.</param>
        /// <param name="RetryBackOffRepeatTimes">When the charging station is reconnecting, after a connection loss, it will use this variable for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the charging station keeps connecting with the same back-off time.</param>
        /// <param name="RetryBackOffRandomRange">When the Charging Station is reconnecting, after a connection loss, it will use this variable as the maximum value for the random part of the back-off time. It will add a new random value to every increasing back-off time, including the first connection attempt (with this maximum), for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the Charging Station will keep connecting with the same back-off time.</param>
        /// <param name="RetryBackOffWaitMinimum">When the charging station is reconnecting, after a connection loss, it will use this variable as the minimum backoff time, the first time it tries to reconnect.</param>
        /// <param name="UnlockOnEVSideDisconnect">When set to true, the Charging Station will unlock the cable on Charging Station side when the cable is unplugged at the EV.</param>
        /// <param name="WebSocketPingInterval">A value of 0 disables client side websocket ping/pong. In this case there is either no ping/pong or the server initiates the ping and client responds with Pong. Positive values are interpreted as number of seconds between pings. Negative values are not allowed, SetConfiguration is then expected to return a Rejected result. It is recommended to configure WebSocketPingInterval smaller then: MessageAttemptsTransactionEvent * MessageAttemptIntervalTransactionEvent. This will limit the chance of the resend mechanism for transactionrelated messages being triggered by connectivity issues.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public OCPPCommCtrlr(TimeSpan                           DefaultMessageTimeout,
                             IEnumerable<FileTransferProtocol>  FileTransferProtocols,
                             String                             NetworkConfigurationPriority,
                             Int16                              NetworkProfileConnectionAttempts,
                             TimeSpan                           OfflineThreshold,
                             Int16                              MessageAttempts,
                             TimeSpan                           MessageAttemptInterval,
                             Boolean                            UnlockOnEVSideDisconnect,
                             Byte                               ResetRetries,                               // ToDo: Only defined in 2.1.14. but not under 3.1.12. OCPPCommCtrlr

                             NetworkProfiles?                   ActiveNetworkProfile               = null,
                             TimeSpan?                          HeartbeatInterval                  = null,
                             Boolean?                           PublicKeyWithSignedMeterValue      = null,  // ToDo: Should not be here!
                             Boolean?                           QueueAllMessages                   = null,
                             UInt16?                            RetryBackOffRepeatTimes            = null,
                             TimeSpan?                          RetryBackOffRandomRange            = null,
                             TimeSpan?                          RetryBackOffWaitMinimum            = null,
                             TimeSpan?                          WebSocketPingInterval              = null,

                             String?                            Instance                           = null,
                             CustomData?                        CustomData                         = null)

            : base(nameof(OCPPCommCtrlr),
                   Instance,
                   new[] {

                       #region ActiveNetworkProfile

                       new VariableConfig(

                           Name:              "ActiveNetworkProfile",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Indicates the configuration profile the station uses at that moment to connect to the network."),

                           CustomData:        null

                       ),

                       #endregion

                       #region FileTransferProtocols

                       new VariableConfig(

                           Name:              "FileTransferProtocols",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.MemberList
                                                   )
                                               },

                           Description:       I18NString.Create("List of supported file transfer protocols."),

                           CustomData:        null

                       ),

                       #endregion

                       #region HeartbeatInterval

                       new VariableConfig(

                           Name:              "HeartbeatInterval",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Interval in seconds of inactivity (no OCPP exchanges) with CSMS after which the Charging Station should send HeartbeatRequest."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MessageAttempts (TransactionEvent)

                       new VariableConfig(

                           Name:              "MessageAttempts",
                           Instance:          "TransactionEvent",

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("How often the Charging Station should try to submit a TransactionEventRequest message when the CSMS fails to process it."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MessageAttemptInterval (TransactionEvent)

                       new VariableConfig(

                           Name:              "MessageAttemptInterval",
                           Instance:          "TransactionEvent",

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Type:        AttributeTypes.Actual,
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("How long in seconds the Charging Station should wait before resubmitting a TransactionEventRequest message that the CSMS failed to process."),

                           CustomData:        null

                       ),

                       #endregion

                       #region MessageTimeout (default)

                       new VariableConfig(

                           Name:              "MessageTimeout",
                           Instance:          "Default",

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadOnly
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Message timeout in seconds. The message timeout setting in a Charging Station can be configured in the messageTimeout field in the NetworkConnectionProfile."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NetworkConfigurationPriority

                       new VariableConfig(

                           Name:              "NetworkConfigurationPriority",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Type:        AttributeTypes.Actual,
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.SequenceList
                                                       //ValuesList:  List of possible values
                                                   )
                                               },

                           Description:       I18NString.Create("A comma separated ordered list of the priority of the possible Network Connection Profiles. The list of possible available profile slots for the network configuration profiles SHALL be reported, via the valueList characteristic of this variable."),

                           CustomData:        null

                       ),

                       #endregion

                       #region NetworkProfileConnectionAttempts

                       new VariableConfig(

                           Name:              "NetworkProfileConnectionAttempts",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Specifies the number of connection attempts the Charging Station executes before switching to a different profile."),

                           CustomData:        null

                       ),

                       #endregion

                       #region OfflineThreshold

                       new VariableConfig(

                           Name:              "OfflineThreshold",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("When the offline period in seconds of a Charging Station exceeds the OfflineThreshold it is recommended to send a StatusNotificationRequest for all its Connectors when the Charging Station is back online."),

                           CustomData:        null

                       ),

                       #endregion

                       #region PublicKeyWithSignedMeterValue  (Should not be here!)

                       new VariableConfig(

                           Name:              "PublicKeyWithSignedMeterValue",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("This Configuration Variable can be used to configure whether a public key needs to be sent with a signed meter value."),

                           CustomData:        null

                       ),

                       #endregion

                       #region QueueAllMessages

                       new VariableConfig(

                           Name:              "QueueAllMessages",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("When this variable is set to true, the Charging Station will queue all message until they are delivered to the CSMS."),

                           CustomData:        null

                       ),

                       #endregion

                       #region ResetRetries

                       new VariableConfig(

                           Name:              "ResetRetries",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("Number of times to retry a reset of the Charging Station when a reset was unsuccessful."),

                           CustomData:        null

                       ),

                       #endregion

                       #region RetryBackOffRepeatTimes

                       new VariableConfig(

                           Name:              "RetryBackOffRepeatTimes",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("When the charging station is reconnecting, after a connection loss, it will use this variable for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the charging station keeps connecting with the same back-off time."),

                           CustomData:        null

                       ),

                       #endregion

                       #region RetryBackOffRandomRange

                       new VariableConfig(

                           Name:              "RetryBackOffRandomRange",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("When the Charging Station is reconnecting, after a connection loss, it will use this variable as the maximum value for the random part of the back-off time. It will add a new random value to every increasing back-off time, including the first connection attempt (with this maximum), for the amount of times it will double the previous back-off time. When the maximum number of increments is reached, the Charging Station will keep connecting with the same back-off time."),

                           CustomData:        null

                       ),

                       #endregion

                       #region RetryBackOffWaitMinimum

                       new VariableConfig(
                           Name:              "RetryBackOffWaitMinimum",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("When the charging station is reconnecting, after a connection loss, it will use this variable as the minimum backoff time, the first time it tries to reconnect."),

                           CustomData:        null
                       ),

                       #endregion

                       #region UnlockOnEVSideDisconnect (for all EVSEs!)

                       new VariableConfig(

                           Name:              "UnlockOnEVSideDisconnect",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       DataType:    DataTypes.Boolean
                                                   )
                                               },

                           Description:       I18NString.Create("When set to true, the Charging Station will unlock the cable on Charging Station side when the cable is unplugged at the EV. For an EVSE with a fixed cable the mutability SHALL be ReadOnly and the actual value SHALL be false."),

                           CustomData:        null

                       ),

                       #endregion

                       #region WebSocketPingInterval

                       new VariableConfig(

                           Name:              "WebSocketPingInterval",
                           Instance:          null,

                           Attributes:        new[] {
                                                   new VariableAttribute(
                                                       Mutability:  MutabilityTypes.ReadWrite
                                                   )
                                               },

                           Characteristics:   new[] {
                                                   new VariableCharacteristics(
                                                       Unit:        UnitsOfMeasure.TimeSpan(),
                                                       DataType:    DataTypes.Integer
                                                   )
                                               },

                           Description:       I18NString.Create("A value of 0 disables client side websocket ping/pong. In this case there is either no ping/pong or the server initiates the ping and client responds with Pong. Positive values are interpreted as number of seconds between pings. Negative values are not allowed, SetConfiguration is then expected to return a Rejected result. It is recommended to configure WebSocketPingInterval smaller then: MessageAttemptsTransactionEvent * MessageAttemptIntervalTransactionEvent. This will limit the chance of the resend mechanism for transactionrelated messages being triggered by connectivity issues."),

                           CustomData:        null

                       )

                       #endregion

                   },
                   I18NString.Create("Logical Component responsible for configuration relating to information exchange between a charging station and a CSMS."),
                   CustomData)

        {

            this.DefaultMessageTimeout             = DefaultMessageTimeout;
            this.FileTransferProtocols             = FileTransferProtocols;
            this.NetworkConfigurationPriority      = NetworkConfigurationPriority;
            this.NetworkProfileConnectionAttempts  = NetworkProfileConnectionAttempts;
            this.OfflineThreshold                  = OfflineThreshold;
            this.MessageAttempts                   = MessageAttempts;
            this.MessageAttemptInterval            = MessageAttemptInterval;
            this.UnlockOnEVSideDisconnect          = UnlockOnEVSideDisconnect;
            this.ResetRetries                      = ResetRetries;

            this.ActiveNetworkProfile              = ActiveNetworkProfile;
            this.HeartbeatInterval                 = HeartbeatInterval;
            this.PublicKeyWithSignedMeterValue     = PublicKeyWithSignedMeterValue;
            this.QueueAllMessages                  = QueueAllMessages;
            this.RetryBackOffRepeatTimes           = RetryBackOffRepeatTimes;
            this.RetryBackOffRandomRange           = RetryBackOffRandomRange;
            this.RetryBackOffWaitMinimum           = RetryBackOffWaitMinimum;
            this.WebSocketPingInterval             = WebSocketPingInterval;

        }

        #endregion


    }

}
