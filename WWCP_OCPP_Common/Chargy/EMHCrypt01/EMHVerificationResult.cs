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

using cloud.charging.open.protocols.OCPP;
using System;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public interface IEMHVerificationResult : IVerificationResult
    {

        Meter_Id?        MeterId                          { get; }
        IMeter           Meter                            { get; }
        DateTime?        Timestamp                        { get; }
        String           InfoStatus                       { get; }
        UInt32?          SecondsIndex                     { get; }
        String           PaginationId                     { get; }
        OBIS?            OBIS                             { get; }
        Int32?           UnitEncoded                      { get; }
        Int32?           Scale                            { get; }
        UInt64?          Value                            { get; }
        String           LogBookIndex                     { get; }
        String           AuthorizationStart               { get; }
        String           AuthorizationStop                { get; }
        DateTime?        AuthorizationStartTimestamp      { get; }
        String           PublicKey                        { get; }
        String           PublicKeyFormat                  { get; }
        //String           PublicKeySignatures              { get; }

        IEMHSignature    Signature                        { get; }

        String           SHA256Value                      { get; }


    }

    public class EMHVerificationResult : IEMHVerificationResult
    {

        public Meter_Id?            MeterId                        { get; }

        public IMeter               Meter                          { get; set; }

        public DateTime?            Timestamp                      { get; }

        public String               InfoStatus                     { get; }

        public UInt32?              SecondsIndex                   { get; }

        public String               PaginationId                   { get; }

        public OBIS?                OBIS                           { get; }

        public Int32?               UnitEncoded                    { get; }

        public Int32?               Scale                          { get; }

        public UInt64?              Value                          { get; }

        public String               LogBookIndex                   { get; }

        public String               AuthorizationStart             { get; }

        public String               AuthorizationStop              { get; }

        public DateTime?            AuthorizationStartTimestamp    { get; }

        public String               PublicKey                      { get; }

        public String               PublicKeyFormat                { get; }

        public String               PublicKeySignatures            { get; }

        public IEMHSignature        Signature                      { get; }


        public String               SHA256Value                    { get; set; }

        public VerificationResult?  Status                         { get; set; }

        public String               ErrorMessage                   { get; set; }


        public EMHVerificationResult(Meter_Id?            MeterId                             = null,
                                     IMeter               Meter                               = null,
                                     DateTime?            Timestamp                           = null,
                                     String               InfoStatus                          = null,
                                     UInt32?              SecondsIndex                        = null,
                                     String               PaginationId                        = null,
                                     OBIS?                OBIS                                = null,
                                     Int32?               UnitEncoded                         = null,
                                     Int32?               Scale                               = null,
                                     UInt64?              Value                               = null,
                                     String               LogBookIndex                        = null,
                                     String               AuthorizationStart                  = null,
                                     String               AuthorizationStop                   = null,
                                     DateTime?            AuthorizationStartTimestamp         = null,
                                     String               PublicKey                           = null,
                                     String               PublicKeyFormat                     = null,
                                     String               PublicKeySignatures                 = null,
                                     IEMHSignature        Signature                           = null,

                                     String               SHA256value                         = null,
                                     VerificationResult?  Status                              = null,
                                     String               ErrorMessage                        = null)
        {

            this.MeterId                       = MeterId;
            this.Meter                         = Meter;
            this.Timestamp                     = Timestamp;
            this.InfoStatus                    = InfoStatus;
            this.SecondsIndex                  = SecondsIndex;
            this.PaginationId                  = PaginationId;
            this.OBIS                          = OBIS;
            this.UnitEncoded                   = UnitEncoded;
            this.Scale                         = Scale;
            this.Value                         = Value;
            this.LogBookIndex                  = LogBookIndex;
            this.AuthorizationStart            = AuthorizationStart;
            this.AuthorizationStop             = AuthorizationStop;
            this.AuthorizationStartTimestamp   = AuthorizationStartTimestamp;
            this.PublicKey                     = PublicKey;
            this.PublicKeyFormat               = PublicKeyFormat;
            this.PublicKeySignatures           = PublicKeySignatures;
            this.Signature                     = Signature;

            this.SHA256Value                   = SHA256value;
            this.Status                        = Status;
            this.ErrorMessage                  = ErrorMessage;

        }


        public EMHVerificationResult SetMeter(IMeter Meter)
        {
            this.Meter = Meter;
            return this;
        }

        public EMHVerificationResult SetSHA256Value(String SHA256Value)
        {
            this.SHA256Value = SHA256Value;
            return this;
        }

        public EMHVerificationResult SetStatus(VerificationResult Status)
        {
            this.Status = Status;
            return this;
        }

        public EMHVerificationResult SetErrorMessage(String ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
            return this;
        }

        public EMHVerificationResult SetError(VerificationResult  Status,
                                              String              ErrorMessage)
        {
            this.Status       = Status;
            this.ErrorMessage = ErrorMessage;
            return this;
        }

    }

}
