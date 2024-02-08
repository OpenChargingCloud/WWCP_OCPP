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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public class EMHSignResult : IEMHSignResult
    {

        public Meter_Id?            MeterId                        { get; }

        public IMeter?              Meter                          { get; set; }

        public DateTime?            Timestamp                      { get; }

        public String?              InfoStatus                     { get; }

        public UInt32?              SecondsIndex                   { get; }

        public String?              PaginationId                   { get; }

        public OBIS?                OBIS                           { get; }

        public Int32?               UnitEncoded                    { get; }

        public Int32?               Scale                          { get; }

        public UInt64?              Value                          { get; }

        public String?              LogBookIndex                   { get; }

        public String?              AuthorizationStart             { get; }

        public String?              AuthorizationStop              { get; }

        public DateTime?            AuthorizationStartTimestamp    { get; }

        public String?              PublicKey                      { get; }

        public String?              PublicKeyFormat                { get; }

        public String?              PublicKeySignatures            { get; }


        public String?              SHA256Value                    { get; set; }

        public String?              HashValue
            => SHA256Value;

        public IEMHSignature?       EMHSignature                   { get; set; }

        public ISignature           Signature
            => EMHSignature;

        public SignResult?          Status                         { get; set; }

        public String?              ErrorMessage                   { get; set; }


        public EMHSignResult(Meter_Id?       MeterId                       = null,
                             IMeter?         Meter                         = null,
                             DateTime?       Timestamp                     = null,
                             String?         InfoStatus                    = null,
                             UInt32?         SecondsIndex                  = null,
                             String?         PaginationId                  = null,
                             OBIS?           OBIS                          = null,
                             Int32?          UnitEncoded                   = null,
                             Int32?          Scale                         = null,
                             UInt64?         Value                         = null,
                             String?         LogBookIndex                  = null,
                             String?         AuthorizationStart            = null,
                             String?         AuthorizationStop             = null,
                             DateTime?       AuthorizationStartTimestamp   = null,
                             String?         PublicKey                     = null,
                             String?         PublicKeyFormat               = null,
                             String?         PublicKeySignatures           = null,

                             String?         SHA256value                   = null,
                             IEMHSignature?  EMHSignature                  = null,
                             SignResult?     Status                        = null,
                             String?         ErrorMessage                  = null)
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

            this.SHA256Value                   = SHA256value;
            this.EMHSignature                  = EMHSignature;
            this.Status                        = Status;
            this.ErrorMessage                  = ErrorMessage;

        }


        public EMHSignResult SetMeter(IMeter Meter)
        {
            this.Meter = Meter;
            return this;
        }

        public EMHSignResult SetSHA256Value(String SHA256Value)
        {
            this.SHA256Value = SHA256Value;
            return this;
        }

        public EMHSignResult SetStatus(SignResult  Status)
        {
            this.Status = Status;
            return this;
        }

        public EMHSignResult SetSignatureValue(IEMHSignature EMHSignature)
        {
            this.EMHSignature = EMHSignature;
            return this;
        }

        public EMHSignResult SetSignatureValue(SignResult     Status,
                                               IEMHSignature  EMHSignature)
        {
            this.Status        = Status;
            this.EMHSignature  = EMHSignature;
            return this;
        }

        public EMHSignResult SetErrorMessage(String ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
            return this;
        }

        public EMHSignResult SetError(SignResult  Status,
                                      String      ErrorMessage)
        {
            this.Status       = Status;
            this.ErrorMessage = ErrorMessage;
            return this;
        }

    }

}
