﻿/*
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

namespace cloud.charging.open.protocols.OCPPv1_6
{

    public delegate Task FileDownloadedDelegate   (DateTime             Timestamp,
                                                   DownloadedFileInfos  DownloadedFileInfo,
                                                   CancellationToken    CancellationToken);

    public delegate Task DownloadErrorDelegate    (DateTime             Timestamp,
                                                   String               Module,
                                                   String               Caller,
                                                   String               ErrorResponse,
                                                   CancellationToken    CancellationToken);

    public delegate Task DownloadExceptionDelegate(DateTime             Timestamp,
                                                   String               Module,
                                                   String               Caller,
                                                   Exception            ExceptionOccurred,
                                                   CancellationToken    CancellationToken);


    public class DownloadedFileInfos(String    FileName,
                                     UInt64    FileSize,
                                     DateTime  DownloadTimestamp)
    {
        public String    FileName             { get; } = FileName;
        public UInt64    FileSize             { get; } = FileSize;
        public DateTime  DownloadTimestamp    { get; } = DownloadTimestamp;

    }


}
