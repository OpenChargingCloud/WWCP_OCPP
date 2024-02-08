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

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public abstract class ACrypt {

        #region Properties

        public String                                Description                     { get; }

        public GetMeterDelegate                      GetMeter                        { get; }

        public CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature    { get; }

        #endregion

        #region Constructor(s)

        public ACrypt(String                                Description,
                      GetMeterDelegate                      GetMeter,
                      CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature)
        {

            this.Description                   = Description;
            this.GetMeter                      = GetMeter;
            this.CheckMeterPublicKeySignature  = CheckMeterPublicKeySignature;

        }

        #endregion


        public abstract ISignResult SignChargingSession(IChargingSession    ChargingSession,
                                                        Byte[]              PrivateKey,
                                                        SignatureFormats    SignatureFormat = SignatureFormats.DER);

        public abstract ISignResult SignMeasurement    (IMeasurementValue2  MeasurementValue,
                                                        Byte[]              PrivateKey,
                                                        SignatureFormats    SignatureFormat = SignatureFormats.DER);


        public abstract ISessionCryptoResult VerifyChargingSession(IChargingSession ChargingSession);


        public abstract IVerificationResult VerifyMeasurement(IMeasurementValue measurementValue);


    }

}
