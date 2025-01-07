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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPP;
using cloud.charging.open.protocols.OCPPv2_1.NetworkingNode;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.CS
{

    #region (class) ChargingStationConnector

    /// <summary>
    /// A connector at a charging station.
    /// </summary>
    public class ChargingStationConnector
    {

        #region Data

        private readonly ChargingStationEVSE                                  parentEVSE;
        public           ConcurrentDictionary<String, List<ComponentConfig>>  ComponentConfigs   = [];

        #endregion

        #region Properties
        public Connector_Id   Id               { get; }
        public ConnectorType  ConnectorType    { get; }

        #endregion

        #region Controllers

        #region Generic

        public IEnumerable<T> GetComponentConfigs<T>(String         Name,
                                                     Connector_Id?  ConnectorId   = null,
                                                     String?        Instance      = null)

            where T : ComponentConfig

            => ComponentConfigs.TryGetValue(Name, out var controllerList)
                   ? controllerList.Where  (controller => controller.Name              == Name           &&
                                                          controller.EVSE?.Id          == parentEVSE?.Id &&
                                                          controller.EVSE?.ConnectorId == Id             &&
                                                          ( Instance.IsNullOrEmpty() || controller.Instance == Instance)).
                                    Cast<T>()
                   : [];

        #endregion

        #endregion

        #region ChargingStationConnector(Id, ConnectorType, ParentEVSE = null)

        public ChargingStationConnector(ChargingStationEVSE  ParentEVSE,
                                        Connector_Id         Id,
                                        ConnectorType        ConnectorType)
        {

            this.parentEVSE     = ParentEVSE;
            this.Id             = Id;
            this.ConnectorType  = ConnectorType;

        }

        #endregion



        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("id", Id.ToString())

                       );

            return json;

        }

    }

    #endregion

}
