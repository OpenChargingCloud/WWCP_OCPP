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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A Charging Station Management System (CSMS) for testing.
    /// </summary>
    public class TestCSMS : ACSMS
    {

        #region Data

        #endregion

        #region Properties

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Charging Station Management System (CSMS) for testing.
        /// </summary>
        /// <param name="Id">The unique identification of this charging station management system.</param>
        /// <param name="RequireAuthentication">Require a HTTP Basic Authentication of all connecting networking nodes/charging stations.</param>
        public TestCSMS(NetworkingNode_Id         Id,

                        Boolean                   RequireAuthentication   = true,

                        IPPort?                   HTTPUploadPort          = null,
                        IPPort?                   HTTPDownloadPort        = null,

                        AsymmetricCipherKeyPair?  ClientCAKeyPair         = null,
                        X509Certificate?          ClientCACertificate     = null,

                        SignaturePolicy?          SignaturePolicy         = null,

                        TimeSpan?                 DefaultRequestTimeout   = null,
                        DNSClient?                DNSClient               = null)

            : base(Id,

                   RequireAuthentication,

                   HTTPUploadPort,
                   HTTPDownloadPort,

                   ClientCAKeyPair,
                   ClientCACertificate,

                   SignaturePolicy,

                   DefaultRequestTimeout,
                   DNSClient)

        {

        }

        #endregion


        #region HandleErrors(Module, Caller, ErrorResponse)

        public override Task HandleErrors(String  Module,
                                          String  Caller,
                                          String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ErrorResponse
                   );

        }

        #endregion

        #region HandleErrors(Module, Caller, ExceptionOccured)

        public override Task HandleErrors(String     Module,
                                          String     Caller,
                                          Exception  ExceptionOccured)
        {

            DebugX.LogException(ExceptionOccured, $"{Module}.{Caller}");

            return base.HandleErrors(
                       Module,
                       Caller,
                       ExceptionOccured
                   );

        }

        #endregion


    }

}
