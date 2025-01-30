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

using org.GraphDefined.Vanaheimr.CLI;
using org.GraphDefined.Vanaheimr.Illias;
using Org.BouncyCastle.Tls;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CSMS.CommandLine
{

    /// <summary>
    /// Install a certificate.
    /// </summary>
    /// <param name="CLI">The command line interface</param>
    //[CLIContext([ DefaultStrings.OCPPv2_0_1,
    //              DefaultStrings.OCPPv2_1 ])]
    public class InstallCertificateCommand(ICSMSCLI CLI) : ACLICommand<ICSMSCLI>(CLI),
                                                           ICLICommand
    {

        #region Data

        public static readonly String CommandName = nameof(InstallCertificateCommand)[..^7].ToLowerFirstChar();

        #endregion

        #region Suggest(Arguments)

        public override IEnumerable<SuggestionResponse> Suggest(String[] Arguments)
        {

            // No suggestions without a defined RemoteSystemId and matching OCPP version!
            if (!cli.RemoteSystemIdIsSet() ||
                 cli.GetRemoteSystemOCPPVersion() != DefaultStrings.OCPPv2_1)
            {
                return [];
            }

            if (Arguments.Length >= 3)
            {

                var result = AutoComplete.AutoCompletePath(Arguments[2]);

                if (result.ExpandedPath.Length > 1 && result.Candidates.Count == 0)
                    return [SuggestionResponse.ParameterCompleted($"{Arguments[0]} {Arguments[1]} {result.ExpandedPath}")];

                if (result.ExpandedPath.Length > 1 && result.Candidates.Count > 0)
                    return result.Candidates.Select(candidate => SuggestionResponse.ParameterPrefix($"{Arguments[0]} {Arguments[1]} {candidate}"));
                //return [ SuggestionResponse.ParameterPrefix($"{Arguments[0]} {Arguments[1]} {result.ExpandedPath[1]}") ];

            }

            if (Arguments.Length == 2 &&
                CommandName.Equals(Arguments[0], StringComparison.OrdinalIgnoreCase))
            {

                var pos = Arguments.Length - 1;

                foreach (var installCertificateUse in InstallCertificateUse.All)
                {

                    if (installCertificateUse.ToString().Equals    (Arguments[pos], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterCompleted($"{Arguments.Take(pos).AggregateWith(" ")} {installCertificateUse.ToString().ToLower()}") ];

                    if (installCertificateUse.ToString().StartsWith(Arguments[pos], StringComparison.OrdinalIgnoreCase))
                        return [ SuggestionResponse.ParameterPrefix   ($"{Arguments.Take(pos).AggregateWith(" ")} {installCertificateUse.ToString().ToLower()}") ];

                }

                return [ SuggestionResponse.ParameterCompleted($"{Arguments.Take(pos).AggregateWith(" ")} {Arguments[Arguments.Length]}") ];

            }

            //ToDo: Autocomplete file system paths!
            // D:\Coding\OpenChargingCloud\OCPPCSMSTest\CSMS\bin\Debug\net9.0\pki\rootCA_ECC.cert
            // D:\Coding\OpenChargingCloud\OCPPCSMSTest\CSMS\bin\Debug\net9.0\pki\serverCA_ECC.cert
            // D:\Coding\OpenChargingCloud\OCPPCSMSTest\CSMS\bin\Debug\net9.0\pki\rootCA_RSA.cert
            // D:\Coding\OpenChargingCloud\OCPPCSMSTest\CSMS\bin\Debug\net9.0\pki\serverCA_RSA.cert

            if (Arguments.Length == 1)
            {

                if (CommandName.Equals    (Arguments[0], StringComparison.OrdinalIgnoreCase))
                    return [ SuggestionResponse.CommandHelp(Help()) ];

                if (CommandName.StartsWith(Arguments[0], StringComparison.OrdinalIgnoreCase))
                    return [ SuggestionResponse.CommandCompleted(CommandName) ];

            }

            return [];

        }

        #endregion

        #region Execute(Arguments, CancellationToken)

        public override async Task<String[]> Execute(String[]           Arguments,
                                                     CancellationToken  CancellationToken)
        {

            // No execution without a defined RemoteSystemId!
            var sourceRoute = cli.GetRemoteSystemSourceRoute();
            if (sourceRoute is null)
                return [];


            if (Arguments.Length >= 3)
            {

                if (!InstallCertificateUse.TryParse(Arguments[1], out var installCertificateUse))
                    return [ $"Unknown certificate use '{Arguments[1]}'!" ];

                if (!File.Exists(Arguments[2]))
                    return [ $"Certificate file '{Arguments[2]}' not found!" ];

                OCPP.Certificate? certificate = null;

                try
                {
                    certificate = OCPP.Certificate.Parse(File.ReadAllText(Arguments[2]));
                }
                catch (Exception e)
                {
                    return [ $"Error parsing certificate file '{Arguments[2]}': {e.Message}" ];
                }

                if (certificate is null)
                    return [ $"Error parsing certificate file '{Arguments[2]}'!" ];

                var response = await cli.OCPP.OUT.InstallCertificate(
                                         new InstallCertificateRequest(
                                             Destination:      sourceRoute,
                                             CertificateType:  installCertificateUse,
                                             Certificate:      certificate
                                         )
                                     );

                return [
                    $"{Arguments.AggregateWith(" ")} => {response.Runtime.TotalMilliseconds} ms",
                    response.ToJSON().ToString(Newtonsoft.Json.Formatting.Indented)
                ];

            }

            return [ $"Usage: {CommandName} <{InstallCertificateUse.All.Select(_ => _.ToString()).AggregateWith("|")}> <filename>" ];

        }

        #endregion

        #region Help()

        public override String Help()
            => $"{CommandName} <{InstallCertificateUse.All.Select(_ => _.ToString()).AggregateWith("|")}> <filename> - Install a certificate";

        #endregion

    }

}
