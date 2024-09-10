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

using System.Security.Cryptography;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.GermanCalibrationLaw
{

    public class EMHCrypt01 : ACrypt
    {

        #region Data

        public String              CurveName    { get; }
        public X9ECParameters      ECP          { get; }
        public ECDomainParameters  ECSpec       { get; }
        public FpCurve             C            { get; }

        #endregion

        #region Constructor(s)

        public EMHCrypt01(GetMeterDelegate                      GetMeter,
                          CheckMeterPublicKeySignatureDelegate  CheckMeterPublicKeySignature)

            : base("ECC secp192r1",
                   GetMeter,
                   CheckMeterPublicKeySignature)

        {

            this.CurveName  = "P-192";
            this.ECP        = ECNamedCurveTable.GetByName(CurveName);
            this.ECSpec     = new ECDomainParameters(ECP.Curve, ECP.G, ECP.N, ECP.H, ECP.GetSeed());
            this.C          = (FpCurve) ECSpec.Curve;

        }

        #endregion


        #region GenerateKeyPairs()

        public ECKeyPair GenerateKeyPairs()
        {

            var generator = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            generator.Init(new ECKeyGenerationParameters(ECSpec, new SecureRandom()));

            var keyPair = generator.GenerateKeyPair();

            return new ECKeyPair(
                       (keyPair.Private as ECPrivateKeyParameters)!,
                       (keyPair.Public  as ECPublicKeyParameters)!
                   );

        }

        #endregion

        #region ParsePrivateKey(PrivateKey)

        public ECPrivateKeyParameters ParsePrivateKey(Byte[] PrivateKey)

            => new (new BigInteger(1, PrivateKey), ECSpec);


        public ECPrivateKeyParameters ParsePrivateKey(String PrivateKey)

            => new (new BigInteger(PrivateKey, 16), ECSpec);

        #endregion

        #region ParsePublicKey (PublicKey, PublicKeyFormat = PublicKeyFormats.DER)

        public ECPublicKeyParameters? ParsePublicKey(Byte[]            PublicKey,
                                                     PublicKeyFormats  PublicKeyFormat   = PublicKeyFormats.DER)
        {

            switch (PublicKeyFormat)
            {

                case PublicKeyFormats.DER:
                    return new ECPublicKeyParameters(
                               "ECDSA",
                               ECP.Curve.DecodePoint(PublicKey),
                               ECSpec
                           );

                case PublicKeyFormats.plain:

                    var publicKey  = PublicKey.ToHexString();

#pragma warning disable IDE0057 // Use range operator
                    var q          = ECP.Curve.CreatePoint(
                                         new BigInteger(publicKey.Substring(0, 48), 16),
                                         new BigInteger(publicKey.Substring(   48), 16)
                                     );
#pragma warning restore IDE0057 // Use range operator

                    return q.IsValid()
                               ? new ECPublicKeyParameters(
                                     "ECDH",
                                     q,
                                     SecObjectIdentifiers.SecP192r1
                                 )
                               : null;

            }

            return null;

        }

        public ECPublicKeyParameters? ParsePublicKey(String            PublicKey,
                                                     PublicKeyFormats  PublicKeyFormat = PublicKeyFormats.DER)
        {

            switch (PublicKeyFormat)
            {

                case PublicKeyFormats.DER:
                    return new ECPublicKeyParameters("ECDSA", ECP.Curve.DecodePoint(PublicKey.FromHEX()), ECSpec);

                case PublicKeyFormats.plain:
#pragma warning disable IDE0057 // Use range operator
                    var q           = ECP.Curve.CreatePoint(
                                          new BigInteger(PublicKey.Substring(0, 48), 16),
                                          new BigInteger(PublicKey.Substring(   48), 16)
                                      );
#pragma warning restore IDE0057 // Use range operator

                    return q.IsValid()
                               ? new ECPublicKeyParameters(
                                     "ECDH",
                                     q,
                                     SecObjectIdentifiers.SecP192r1
                                 )
                               : null;

            }

            return null;

        }

        #endregion

        #region ParsePublicKey (X, Y)

        public ECPublicKeyParameters? ParsePublicKey(Byte[] X, Byte[] Y)
        {

            var q = ECP.Curve.CreatePoint(
                        new BigInteger(X),
                        new BigInteger(Y)
                    );

            return q.IsValid()
                       ? new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1)
                       : null;

        }

        public ECPublicKeyParameters? ParsePublicKey(String X, String Y)
        {

            var q = ECP.Curve.CreatePoint(
                        new BigInteger(X, 16),
                        new BigInteger(Y, 16)
                    );

            return q.IsValid()
                       ? new ECPublicKeyParameters("ECDH", q, SecObjectIdentifiers.SecP192r1)
                       : null;

        }

        #endregion


        #region SignChargingSession(ChargingSession,  PrivateKey, SignatureFormat = SignatureFormats.DER)

        public override ISignResult SignChargingSession(IChargingSession  ChargingSession,
                                                        Byte[]            PrivateKey,
                                                        SignatureFormats  SignatureFormat = SignatureFormats.DER)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SignMeasurement    (MeasurementValue, PrivateKey, SignatureFormat = SignatureFormats.DER)

        public override ISignResult SignMeasurement(IMeasurementValue2  MeasurementValue,
                                                    Byte[]              PrivateKey,
                                                    SignatureFormats    SignatureFormat = SignatureFormats.DER)
        {

            try
            {

                #region Check MeasurementValue

                if (MeasurementValue == null)
                    return new EMHSignResult(Status: SignResult.InvalidMeasurementValue);

                if (MeasurementValue is not IEMHMeasurementValue2 EMHMeasurementValue)
                    return new EMHSignResult(Status: SignResult.InvalidMeasurementValue);

                #endregion

                #region Parse PrivateKey

                ECPrivateKeyParameters? EMHPrivateKey = null;

                try
                {
                    EMHPrivateKey  = ParsePrivateKey(PrivateKey);
                }
                catch (Exception e)
                {
                    return new EMHSignResult(Status:        SignResult.InvalidPublicKey,
                                             ErrorMessage:  e.Message);
                }

                #endregion


                var cryptoBuffer  = new Byte[320];

                var signResult    = new EMHSignResult(

                    MeterId:                      cryptoBuffer.SetHex        (EMHMeasurementValue.Measurement.MeterId,                                        0),
                    Timestamp:                    cryptoBuffer.SetTimestamp32(EMHMeasurementValue.Timestamp,                                                 10),
                    InfoStatus:                   cryptoBuffer.SetHex        (EMHMeasurementValue.InfoStatus,                                                14, false),
                    SecondsIndex:                 cryptoBuffer.SetUInt32     (EMHMeasurementValue.SecondsIndex,                                              15, true),
                    PaginationId:                 cryptoBuffer.SetHex        (EMHMeasurementValue.PaginationId,                                              19, true),
                    OBIS:                         cryptoBuffer.SetHex        (EMHMeasurementValue.Measurement.OBIS,                                          23, false),
                    UnitEncoded:                  cryptoBuffer.SetInt8       (EMHMeasurementValue.Measurement.UnitEncoded,                                   29),
                    Scale:                        cryptoBuffer.SetInt8       (EMHMeasurementValue.Measurement.Scale,                                         30),
                    Value:                        cryptoBuffer.SetUInt64     (EMHMeasurementValue.Value,                                                     31, true),
                    LogBookIndex:                 cryptoBuffer.SetHex        (EMHMeasurementValue.LogBookIndex,                                              39, false),
                    AuthorizationStart:           cryptoBuffer.SetText       (EMHMeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41),
                    AuthorizationStartTimestamp:  cryptoBuffer.SetTimestamp32(EMHMeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169),

                    Status:                       SignResult.InvalidMeasurementValue);


                var sha256Hash = SHA256.HashData(cryptoBuffer);

                // Only the first 24 bytes/192 bits are used!
                signResult.SHA256Value = sha256Hash.ToHexString()[..48];

                var meter = GetMeter(MeasurementValue.Measurement.MeterId);
                if (meter is not null)
                {

                    signResult.SetMeter(meter);

                    var publicKey = meter.PublicKeys.FirstOrDefault();
                    if (publicKey != null && (publicKey.Value?.Trim().IsNotNullOrEmpty() == true))
                    {

                        try
                        {

                            //var EMHPublicKey = ParsePublicKey(publicKey.Value);

                            //var bVerifier   = SignerUtilities.GetSigner("SHA-256withECDSA");
                            var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                            signer.Init(true, EMHPrivateKey);
                            signer.BlockUpdate(sha256Hash, 0, 24);

                            var signature = new EMHSignature(
                                                signer.AlgorithmName,
                                                SignatureFormat, // ToDo: Fix signature format selection!
                                                signer.GenerateSignature().ToHexString()
                                            );

                            MeasurementValue.Signatures.Add(signature);
                            signResult.SetSignatureValue(SignResult.OK, signature);

                        }
                        catch (Exception e)
                        {
                            signResult.SetError(Status:        SignResult.InvalidPublicKey,
                                                ErrorMessage:  e.Message);
                        }

                    }
                    else
                        signResult.SetStatus(SignResult.PublicKeyNotFound);

                }
                else
                    signResult.SetStatus(SignResult.MeterNotFound);


                return signResult;

            }
            catch (Exception e)
            {
                return new EMHSignResult(Status:        SignResult.InvalidMeasurementValue,
                                         ErrorMessage:  e.Message);
            }

        }

        #endregion


        #region VerifyChargingSession(ChargingSession)

        public override ISessionCryptoResult VerifyChargingSession(IChargingSession ChargingSession)
        {

            var sessionResult = SessionVerificationResult.UnknownSessionFormat;

            if (ChargingSession.Measurements != null)
            {
                foreach (var measurement in ChargingSession.Measurements)
                {

                    measurement.ChargingSession = ChargingSession;

                    // Must include at least two measurements (start & stop)
                    if (measurement.Values != null && measurement.Values.Count() > 1)
                    {

                        // Validate...
                        foreach (var measurementValue in measurement.Values)
                        {
                            measurementValue.Measurement = measurement;

                            if (measurementValue is IEMHMeasurementValue emhMeasurementValue)
                                VerifyMeasurement(emhMeasurementValue);

                        }


                        // Find an overall result...
                        sessionResult = SessionVerificationResult.ValidSignature;

                        foreach (var measurementValue in measurement.Values)
                        {
                            if (sessionResult == SessionVerificationResult.ValidSignature &&
                                measurementValue.Result.Status != VerificationResult.ValidSignature)
                            {
                                sessionResult = SessionVerificationResult.InvalidSignature;
                            }
                        }

                    }

                    else
                        sessionResult = SessionVerificationResult.AtLeastTwoMeasurementsRequired;

                }
            }

            return new EMHSessionCryptoResult(sessionResult);

        }

        #endregion

        #region VerifyMeasurement    (MeasurementValue)

        public override IVerificationResult VerifyMeasurement(IMeasurementValue MeasurementValue)
        {

            if (MeasurementValue is IEMHMeasurementValue EMHMeasurementValue)
                return VerifyMeasurement(EMHMeasurementValue);

            return new CryptoResult(VerificationResult.UnknownCTRFormat);

        }

        #endregion

        #region VerifyMeasurement    (SignedMeterValue, PublicKey, Signature)

        public VerificationResult VerifyMeasurement(Byte[] SignedMeterValue,
                                                    Byte[] PublicKey,
                                                    Byte[] Signature)
        {

            try
            {

                var publicKeyHEX  = PublicKey.ToHexString();
                var publicKey     = new ECPublicKeyParameters(
                                        "ECDH",
                                        ECP.Curve.CreatePoint(
                                            new BigInteger(publicKeyHEX.Substring(0, 48), 16),
                                            new BigInteger(publicKeyHEX.Substring(   48), 16)
                                        ),
                                        SecObjectIdentifiers.SecP192r1
                                    );

                var sha256Hash    = SHA256.HashData(SignedMeterValue);
                var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                verifier.Init(false, publicKey);
                // Only the first 24 bytes/192 bits are used!
                verifier.BlockUpdate(sha256Hash, 0, 24);

                // Sometimes the signature is 50 bytes long ending with additional "02e4"?!
                var result = verifier.VerifySignature(new DerSequence(
                                                          new DerInteger(new BigInteger(Signature.ToHexString( 0, 24), 16)),
                                                          new DerInteger(new BigInteger(Signature.ToHexString(24, 24), 16))
                                                      ).GetDerEncoded())
                                  ? VerificationResult.ValidSignature
                                  : VerificationResult.InvalidSignature;

                //Asn1Sequence seq = Asn1Sequence.GetInstance(Signature);
                //BigInteger r = ((DerInteger)seq[0]).Value;
                //BigInteger s = ((DerInteger)seq[1]).Value;

                return result;

            }
            catch
            {
                return VerificationResult.Error;
            }

        }

        #endregion

        #region VerifyMeasurement    (MeasurementValue)

        public IVerificationResult VerifyMeasurement(IEMHMeasurementValue MeasurementValue)
        {

            try
            {

                var cryptoBuffer  = new Byte[320];

                var verificationResult  = new EMHVerificationResult(

                    MeterId:                      cryptoBuffer.SetHex        (MeasurementValue.Measurement.MeterId,                                        0),
                    Timestamp:                    cryptoBuffer.SetTimestamp32(MeasurementValue.Timestamp,                                                 10),
                    InfoStatus:                   cryptoBuffer.SetHex        (MeasurementValue.InfoStatus,                                                14, false),
                    SecondsIndex:                 cryptoBuffer.SetUInt32     (MeasurementValue.SecondsIndex,                                              15, true),
                    PaginationId:                 cryptoBuffer.SetHex        (MeasurementValue.PaginationId,                                              19, true),
                    OBIS:                         cryptoBuffer.SetHex        (MeasurementValue.Measurement.OBIS,                                          23, false),
                    UnitEncoded:                  cryptoBuffer.SetInt8       (MeasurementValue.Measurement.UnitEncoded,                                   29),
                    Scale:                        cryptoBuffer.SetInt8       (MeasurementValue.Measurement.Scale,                                         30),
                    Value:                        cryptoBuffer.SetUInt64     (MeasurementValue.Value,                                                     31, true),
                    LogBookIndex:                 cryptoBuffer.SetHex        (MeasurementValue.LogBookIndex,                                              39, false),
                    AuthorizationStart:           cryptoBuffer.SetText       (MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Id,         41),
                    AuthorizationStartTimestamp:  cryptoBuffer.SetTimestamp32(MeasurementValue.Measurement.ChargingSession.AuthorizationStart.Timestamp, 169),

                    Status:                       VerificationResult.InvalidSignature);


                var signatureExpected = MeasurementValue.Signatures.FirstOrDefault();
                if (signatureExpected != null && (signatureExpected.Value?.Trim().IsNotNullOrEmpty() == true))
                {

                    try
                    {

                        //cryptoResult.Signature = {
                        //    algorithm:  MeasurementValue.Measurement.SignatureInfos.Algorithm,
                        //    format:     MeasurementValue.Measurement.SignatureInfos.Format,
                        //    r:          signatureExpected.R,
                        //    s:          signatureExpected.S
                        //};


                        var sha256Hash = SHA256.HashData(cryptoBuffer);

                        // Only the first 24 bytes/192 bits are used!
                        verificationResult.SHA256Value = sha256Hash.ToHexString()[..48];

                        var meter = GetMeter(MeasurementValue.Measurement.MeterId);
                        if (meter is not null)
                        {

                            verificationResult.SetMeter(meter);

                            var publicKey = meter.PublicKeys.FirstOrDefault();
                            if (publicKey != null && (publicKey.Value?.Trim().IsNotNullOrEmpty() == true))
                            {

                                try
                                {

                                    var EMHPublicKey = ParsePublicKey(publicKey.Value);

                                //    cryptoResult.publicKey            = publicKey.value.toLowerCase();
                                //    cryptoResult.publicKeyFormat      = publicKey.format;
                                //    cryptoResult.publicKeySignatures  = publicKey.signatures;

                                    try
                                    {

                                        //var bVerifier   = SignerUtilities.GetSigner("SHA-256withECDSA");
                                        var verifier      = SignerUtilities.GetSigner("NONEwithECDSA");
                                        verifier.Init(false, EMHPublicKey);
                                        verifier.BlockUpdate(sha256Hash, 0, 24);

                                        var SignatureBytes  = signatureExpected.Value.FromHEX();
                                        var verified        = false;

                                        switch (signatureExpected.Format)
                                        // DER:   3037021900 ab9f84adda460f8410bb26061016d6c8258689caa73b0b fd021a00 fd1eab0aa198b5803358a2a91624dc012d0ef3ee72b3de820a
                                        // plain:            ab9f84adda460f8410bb26061016d6c8258689caa73b0b          fd1eab0aa198b5803358a2a91624dc012d0ef3ee72b3de820a
                                        {

                                            case SignatureFormats.DER:
                                                verified = verifier.VerifySignature(SignatureBytes);
                                                break;

                                            case SignatureFormats.plain: // Shouldn't this be called rs?
                                                verified = verifier.VerifySignature(new DerSequence(
                                                                                        new DerInteger(new BigInteger(SignatureBytes.ToHexString(0, 25), 16)),
                                                                                        new DerInteger(new BigInteger(SignatureBytes.ToHexString(   25), 16))
                                                                                    ).GetDerEncoded());
                                                break;

                                        }


                                        // Success!
                                        if (verified)
                                            verificationResult.SetStatus(VerificationResult.ValidSignature);

                                        else
                                            verificationResult.SetStatus(VerificationResult.InvalidSignature);

                                    }
                                    catch (Exception e)
                                    {
                                        verificationResult.SetError(VerificationResult.InvalidSignature,
                                                                    e.Message);
                                    }

                                }
                                catch (Exception e)
                                {
                                    verificationResult.SetError(VerificationResult.InvalidPublicKey,
                                                                e.Message);
                                }

                            }
                            else
                                verificationResult.SetStatus(VerificationResult.PublicKeyNotFound);

                        }
                        else
                            verificationResult.SetStatus(VerificationResult.MeterNotFound);

                    }
                    catch (Exception e)
                    {
                        verificationResult.SetError(
                                               VerificationResult.InvalidSignature,
                                               e.Message
                                           );
                    }

                }

                else
                    verificationResult.SetStatus(VerificationResult.InvalidSignature);

                return MeasurementValue.Result = verificationResult;

            }
            catch (Exception e)
            {

                return MeasurementValue.Result = new EMHVerificationResult(
                                                     Status:        VerificationResult.UnknownCTRFormat,
                                                     ErrorMessage:  e.Message
                                                 );

            }

        }

        #endregion


        // Helper

        #region DecodeStatus(StatusValue)

        public static IEnumerable<String> DecodeStatus(String StatusValue)
        {

            var statusArray = new List<String>();

            try
            {

                var status = Int32.Parse(StatusValue);

                if ((status &  1) ==  1)
                    statusArray.Add("Fehler erkannt");

                if ((status &  2) ==  2)
                    statusArray.Add("Synchrone Messwertübermittlung");

                // Bit 3 is reserved!

                if ((status &  8) ==  8)
                    statusArray.Add("System-Uhr ist synchron");
                else
                    statusArray.Add("System-Uhr ist nicht synchron");

                if ((status & 16) == 16)
                    statusArray.Add("Rücklaufsperre aktiv");

                if ((status & 32) == 32)
                    statusArray.Add("Energierichtung -A");

                if ((status & 64) == 64)
                    statusArray.Add("Magnetfeld erkannt");

            }
            catch (Exception e)
            {
                statusArray.Add($"Invalid status: {e.Message}");
            }

            return statusArray;

        }

        #endregion


    }

}
