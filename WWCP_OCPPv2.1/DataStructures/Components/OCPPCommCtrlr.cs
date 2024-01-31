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
    /// HTTP Web Socket client behaviour.
    /// </summary>
    public class OCPPCommCtrlr : ComponentConfig
    {

        #region Properties

        public UInt16    RetryBackOffRepeatTimes    { get; set; }

        public TimeSpan  RetryBackOffRandomRange    { get; set; }

        public TimeSpan  RetryBackOffWaitMinimum    { get; set; }

        public TimeSpan  WebSocketPingInterval      { get; set; }

        #endregion


        #region Constructor(s)

        /// <summary>
        /// Create new constant stream data.
        /// </summary>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public OCPPCommCtrlr(String?      Instance     = null,
                             CustomData?  CustomData   = null)

            : base("OCPPCommCtrlr",
                   Instance,
                   null,
                   new[] {

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
                   I18NString.Create("HTTP Web Socket client behaviour"),
                   CustomData)

        { }

        #endregion


    }

}
