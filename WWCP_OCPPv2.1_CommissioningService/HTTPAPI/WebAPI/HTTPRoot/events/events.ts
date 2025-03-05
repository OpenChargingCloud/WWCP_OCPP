///<reference path="../../../../../../../libs/UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartEventsSSE() {

    //MenuHighlight('Events');

    const pionix222DeviceModel  = [
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "Interval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "900"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "Measurands"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Energy.Active.Import.Register,Voltage,Frequency"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Current.Export,Current.Import,Current.Offered,Energy.Active.Export.Register,Energy.Active.Import.Register,Energy.Reactive.Export.Register,Energy.Reactive.Import.Register,Energy.Active.Export.Interval,Energy.Active.Import.Interval,Energy.Reactive.Export.Interval,Energy.Reactive.Import.Interval,Frequency,Power.Active.Export,Power.Active.Import,Power.Factor,Power.Offered,Power.Reactive.Export,Power.Reactive.Import,Voltage"
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "SendDuringIdle"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "SignReadings"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "TxEndedInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "AlignedDataCtrlr"
            },
            "variable": {
                "name": "TxEndedMeasurands"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Energy.Active.Import.Register,Voltage"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Current.Export,Current.Import,Current.Offered,Energy.Active.Export.Register,Energy.Active.Import.Register,Energy.Reactive.Export.Register,Energy.Reactive.Import.Register,Energy.Active.Export.Interval,Energy.Active.Import.Interval,Energy.Reactive.Export.Interval,Energy.Reactive.Import.Interval,Frequency,Power.Active.Export,Power.Active.Import,Power.Factor,Power.Offered,Power.Reactive.Export,Power.Reactive.Import,Voltage"
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "DisablePostAuthorize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "LifeTime"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "Policy"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "LRU,LFU,FIFO,CUSTOM"
            }
        },
        {
            "component": {
                "name": "AuthCacheCtrlr"
            },
            "variable": {
                "name": "Storage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "20"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "B"
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "AdditionalInfoItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "AuthorizeRemoteStart"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "DisableRemoteAuthorization"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "LocalAuthorizeOffline"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "LocalPreAuthorize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "MasterPassGroupId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "123"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "AuthCtrlr"
            },
            "variable": {
                "name": "OfflineTxForUnknownIdEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "instance": "BytesPerMessage",
                "name": "AllowNewSessionsPendingFirmwareUpdate"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "AvailabilityState"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Available,Occupied,Reserved,Unavailable,Faulted"
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "Model"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "PhaseRotation"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "RST"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "Problem"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "SupplyPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStation"
            },
            "variable": {
                "name": "VendorName"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStatusIndicator"
            },
            "variable": {
                "name": "Active"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ChargingStatusIndicator"
            },
            "variable": {
                "name": "Color"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "FFFF00"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "DateTime"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "dateTime",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "NextTimeOffsetTransitionDateTime"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "dateTime",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "NtpServerUri"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "NtpSource"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "DHCP,manual"
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "TimeAdjustmentReportingThreshold"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "TimeOffset"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "instance": "NextTransition",
                "name": "TimeOffset"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "TimeSource"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Heartbeat"
                }
            ],
            "variableCharacteristics": {
                "dataType": "SequenceList",
                "supportsMonitoring": true,
                "valuesList": "Heartbeat,NTP,GPS,RealTimeClock,MobileNetwork,RadioTimeTransmitter"
            }
        },
        {
            "component": {
                "name": "ClockCtrlr"
            },
            "variable": {
                "name": "TimeZone"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 1
                },
                "name": "Connector"
            },
            "variable": {
                "name": "AvailabilityState"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Available,Occupied,Reserved,Unavailable,Faulted"
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 1
                },
                "name": "Connector"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 1
                },
                "name": "Connector"
            },
            "variable": {
                "name": "ChargeProtocol"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 1
                },
                "name": "Connector"
            },
            "variable": {
                "name": "ConnectorType"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "cType2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 1
                },
                "name": "Connector"
            },
            "variable": {
                "name": "SupplyPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 2
                },
                "name": "Connector"
            },
            "variable": {
                "name": "AvailabilityState"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Available,Occupied,Reserved,Unavailable,Faulted"
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 2
                },
                "name": "Connector"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 2
                },
                "name": "Connector"
            },
            "variable": {
                "name": "ChargeProtocol"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 2
                },
                "name": "Connector"
            },
            "variable": {
                "name": "ConnectorType"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "cType2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "connectorId": 1,
                    "id": 2
                },
                "name": "Connector"
            },
            "variable": {
                "name": "SupplyPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "CustomizationCtrlr"
            },
            "variable": {
                "name": "CustomImplementationEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "CustomizationCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "GetReport",
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "250"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "GetVariables",
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "250"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "SetVariables",
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "name": "ConfigurationValueSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "GetReport",
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "4"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "GetVariables",
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "instance": "SetVariables",
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "name": "ReportingValueSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DeviceDataCtrlr"
            },
            "variable": {
                "name": "ValueSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "NumberOfDisplayMessages"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "PersonalMessageSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "SupportedFormats"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "FTP"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "ASCII,HTML,URI,UTF8"
            }
        },
        {
            "component": {
                "name": "DisplayMessageCtrlr"
            },
            "variable": {
                "name": "SupportedPriorities"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "AlwaysFront,InFront,NormalCycle"
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "AllowReset"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "AvailabilityState"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Unavailable"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Available,Occupied,Reserved,Unavailable,Faulted"
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "EvseId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "ISO15118EvseId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "Power"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "22"
                }
            ],
            "variableCharacteristics": {
                "dataType": "decimal",
                "maxLimit": 22000.0,
                "supportsMonitoring": true,
                "unit": "W"
            }
        },
        {
            "component": {
                "evse": {
                    "id": 1
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "SupplyPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "AllowReset"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "AvailabilityState"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Unavailable"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Available,Occupied,Reserved,Unavailable,Faulted"
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "EvseId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "ISO15118EvseId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "Power"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "22"
                }
            ],
            "variableCharacteristics": {
                "dataType": "decimal",
                "maxLimit": 22000.0,
                "supportsMonitoring": true,
                "unit": "W"
            }
        },
        {
            "component": {
                "evse": {
                    "id": 2
                },
                "name": "EVSE"
            },
            "variable": {
                "name": "SupplyPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "CentralContractValidationAllowed"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "ContractCertificateInstallationEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "ContractValidationOffline"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "CountryName"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "MaxScheduleEntries"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "OrganizationName"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "PnCEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "RequestMeteringReceipt"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "RequestedEnergyTransferMode"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "DC,AC_single_phase,AC_two_phase,AC_three_phase"
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "SeccId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ISO15118Ctrlr"
            },
            "variable": {
                "name": "V2GCertificateInstallationEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "AuthorizeConnectorZeroOnConnectorOne"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ChargeBoxSerialNumber"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ChargePointId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "belaybox"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ChargePointModel"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ChargePointSerialNumber"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ChargePointVendor"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ClientCertificateExpireCheckInitialDelaySeconds"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ClientCertificateExpireCheckIntervalSeconds"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "43200"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "FirmwareVersion"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "ICCID"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "IMSI"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "LogMessages"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "LogMessagesFormat"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "log,html,security"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "log,html,console,console_detailed,security"
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MaxCompositeScheduleDuration"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "31536000"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MaxCustomerInformationDataLength"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "51200"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "minLimit": 512.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MaxMessageSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "32000"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "minLimit": 1.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MessageQueueSizeThreshold"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "minLimit": 1.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MeterSerialNumber"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "MeterType"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "NetworkConnectionProfiles"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "[{\"configurationSlot\": 1, \"connectionData\": {\"messageTimeout\": 30, \"ocppCsmsUrl\": \"wss://api1.ocpp.charging.cloud/belaybox\", \"ocppInterface\": \"Wired0\", \"ocppTransport\": \"JSON\", \"ocppVersion\": \"OCPP20\", \"securityProfile\": 2}}]"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "NumberOfConnectors"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "maxLimit": 128.0,
                "minLimit": 1.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "OcspRequestInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "604800"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "minLimit": 86400.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "RoundClockAlignedTimestamps"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "0"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": false
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "SupportedChargingProfilePurposeTypes"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "ChargePointMaxProfile,TxDefaultProfile,TxProfile"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "ChargePointMaxProfile,TxDefaultProfile,TxProfile"
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "SupportedCiphers12"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:AES128-GCM-SHA256:AES256-GCM-SHA384"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:AES128-GCM-SHA256:AES256-GCM-SHA384"
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "SupportedCiphers13"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "TLS_AES_256_GCM_SHA384:TLS_AES_128_GCM_SHA256"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "TLS_AES_256_GCM_SHA384:TLS_AES_128_GCM_SHA256"
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "SupportedCriteria"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Enabled,Active,Available,Problem"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Enabled,Active,Problem,Available"
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "UpdateCertificateSymlinks"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "0"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": false
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "UseSslDefaultVerifyPaths"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "V2GCertificateExpireCheckInitialDelaySeconds"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "V2GCertificateExpireCheckIntervalSeconds"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "43200"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "WebsocketPingPayload"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "hello there"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "InternalCtrlr"
            },
            "variable": {
                "name": "WebsocketPongTimeout"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "5"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1024"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "DisablePostAuthorize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "Entries"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "LocalAuthListCtrlr"
            },
            "variable": {
                "name": "Storage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "B"
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "ActiveMonitoringBase"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "All,FactoryDefault,HardwiredOnly"
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "ActiveMonitoringLevel"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "instance": "ClearVariableMonitoring",
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "instance": "SetVariableMonitoring",
                "name": "BytesPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "instance": "ClearVariableMonitoring",
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "instance": "SetVariableMonitoring",
                "name": "ItemsPerMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "MonitoringBase"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "All,FactoryDefault,HardwiredOnly"
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "MonitoringLevel"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "MonitoringCtrlr"
            },
            "variable": {
                "name": "OfflineQueuingSeverity"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "ActiveNetworkProfile"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "FieldLength"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "FileTransferProtocols"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "FTP,FTPS,HTTP,HTTPS,SFTP"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "HeartbeatInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1800"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "instance": "TransactionEvent",
                "name": "MessageAttemptInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "10"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "instance": "TransactionEvent",
                "name": "MessageAttempts"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "5"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "instance": "Default",
                "name": "MessageTimeout"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "NetworkConfigurationPriority"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "SequenceList",
                "supportsMonitoring": true,
                "valuesList": "1,2"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "NetworkProfileConnectionAttempts"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "OfflineThreshold"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "PublicKeyWithSignedMeterValue"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "OptionList",
                "supportsMonitoring": true,
                "valuesList": "Never,OncePerTransaction,EveryMeterValue"
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "QueueAllMessages"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "ResetRetries"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "RetryBackOffRandomRange"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "RetryBackOffRepeatTimes"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "RetryBackOffWaitMinimum"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "UnlockOnEVSideDisconnect"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "OCPPCommCtrlr"
            },
            "variable": {
                "name": "WebSocketPingInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "30"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "ReservationCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ReservationCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "ReservationCtrlr"
            },
            "variable": {
                "name": "NonEvseSpecific"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "RegisterValuesWithoutPhases"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "SignReadings"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "TxEndedInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "60"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "TxEndedMeasurands"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Energy.Active.Import.Register,Current.Import"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Current.Export,Current.Import,Current.Offered,Energy.Active.Export.Register,Energy.Active.Import.Register,Energy.Reactive.Export.Register,Energy.Reactive.Import.Register,Energy.Active.Export.Interval,Energy.Active.Import.Interval,Energy.Reactive.Export.Interval,Energy.Reactive.Import.Interval,Frequency,Power.Active.Export,Power.Active.Import,Power.Factor,Power.Offered,Power.Reactive.Export,Power.Reactive.Import,Voltage"
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "TxStartedMeasurands"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Energy.Active.Import.Register,Current.Import"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Current.Export,Current.Import,Current.Offered,Energy.Active.Export.Register,Energy.Active.Import.Register,Energy.Reactive.Export.Register,Energy.Reactive.Import.Register,Energy.Active.Export.Interval,Energy.Active.Import.Interval,Energy.Reactive.Export.Interval,Energy.Reactive.Import.Interval,Frequency,Power.Active.Export,Power.Active.Import,Power.Factor,Power.Offered,Power.Reactive.Export,Power.Reactive.Import,Voltage"
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "TxUpdatedInterval"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "120"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "SampledDataCtrlr"
            },
            "variable": {
                "name": "TxUpdatedMeasurands"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Energy.Active.Import.Register,Current.Import,Voltage,Power.Active.Import,Power.Reactive.Import,Frequency"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "Current.Export,Current.Import,Current.Offered,Energy.Active.Export.Register,Energy.Active.Import.Register,Energy.Reactive.Export.Register,Energy.Reactive.Import.Register,Energy.Active.Export.Interval,Energy.Active.Import.Interval,Energy.Reactive.Export.Interval,Energy.Reactive.Import.Interval,Frequency,Power.Active.Export,Power.Active.Import,Power.Factor,Power.Offered,Power.Reactive.Export,Power.Reactive.Import,Voltage"
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "AdditionalRootCertificateCheck"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "BasicAuthPassword"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "WriteOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "OTArSenOCOMU"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "maxLimit": 40.0,
                "minLimit": 16.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "CertSigningRepeatTimes"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "3"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "CertSigningWaitMinimum"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "30"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "CertificateEntries"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "Identity"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "belaybox"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "MaxCertificateChainSize"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "OrganizationName"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "Pionix"
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SecurityCtrlr"
            },
            "variable": {
                "name": "SecurityProfile"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "2"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "maxLimit": 3.0,
                "minLimit": 1.0,
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "ACPhaseSwitchingSupported"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "false"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "instance": "ChargingProfiles",
                "name": "Entries"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "ExternalControlSignalsEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "LimitChangeSignificance"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "decimal",
                "supportsMonitoring": true,
                "unit": "Percent"
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "NotifyChargingLimitWithSchedules"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "PeriodsPerSchedule"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "Phases3to1"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "ProfileStackLevel"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "42"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "SmartChargingCtrlr"
            },
            "variable": {
                "name": "RateUnit"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "A,W"
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "instance": "Cost",
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "instance": "Tariff",
                "name": "Available"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "name": "Currency"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "instance": "Cost",
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "instance": "Tariff",
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "name": "TariffFallbackMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TariffCostCtrlr"
            },
            "variable": {
                "name": "TotalCostFallbackMessage"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": ""
                }
            ],
            "variableCharacteristics": {
                "dataType": "string",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "ChargingTime"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "decimal",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "EVConnectionTimeOut"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "120"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true,
                "unit": "s"
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "Enabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "true"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "MaxEnergyOnInvalidId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1000"
                }
            ],
            "variableCharacteristics": {
                "dataType": "integer",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "StopTxOnEVSideDisconnect"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "StopTxOnInvalidId"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual",
                    "value": "1"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "TxBeforeAcceptedEnabled"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadWrite",
                    "persistent": true,
                    "type": "Actual"
                }
            ],
            "variableCharacteristics": {
                "dataType": "boolean",
                "supportsMonitoring": true
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "TxStartPoint"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "PowerPathClosed"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "ParkingBayOccupancy,EVConnected,Authorized,PowerPathClosed,EnergyTransfer,DataSigned"
            }
        },
        {
            "component": {
                "name": "TxCtrlr"
            },
            "variable": {
                "name": "TxStopPoint"
            },
            "variableAttribute": [
                {
                    "constant": false,
                    "mutability": "ReadOnly",
                    "persistent": true,
                    "type": "Actual",
                    "value": "EVConnected,Authorized"
                }
            ],
            "variableCharacteristics": {
                "dataType": "MemberList",
                "supportsMonitoring": true,
                "valuesList": "ParkingBayOccupancy,EVConnected,Authorized,PowerPathClosed,EnergyTransfer"
            }
        }
    ];


    const connectionColors   = {};
    const eventsDiv          = document.getElementById('eventsDiv');
    const streamFilterInput  = document.getElementById('eventsFilterDiv').getElementsByTagName('input')[0] as HTMLInputElement;
    streamFilterInput.onchange = () => {

        const allLogLines = eventsDiv.getElementsByClassName('logLine') as HTMLCollectionOf<HTMLDivElement>;

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].innerHTML.indexOf(streamFilterInput.value) > -1)
                allLogLines[i].style.display = 'table-row';
            else
                allLogLines[i].style.display = 'none';
        }

    }

    function GetConnectionColors(connectionId) {

        const colors = connectionColors[connectionId];

        if (colors !== undefined)
            return colors;

        else
        {

            const red   = Math.floor(Math.random() * 80 + 165).toString(16);
            const green = Math.floor(Math.random() * 80 + 165).toString(16);
            const blue  = Math.floor(Math.random() * 80 + 165).toString(16);

            const connectionColor = red + green + blue;

            connectionColors[connectionId]             = new Object();
            connectionColors[connectionId].textcolor   = "000000";
            connectionColors[connectionId].background  = connectionColor;

            return connectionColors[connectionId];

        }

    }

    function CreateLogEntry(timestamp, destination, eventTrackingId, command, message, connectionColorKey) {

        const connectionColor = GetConnectionColors(connectionColorKey);

        const logEntryDiv = document.createElement('div');
        logEntryDiv.className         = "logLine";
        logEntryDiv.style.color       = "#" + connectionColor.textcolor;
        logEntryDiv.style.background  = "#" + connectionColor.background;

        const timestampDiv            = document.createElement('div');
        timestampDiv.className        = "timestamp";
        timestampDiv.innerHTML        = timestamp;
        logEntryDiv.appendChild(timestampDiv);

        const eventTrackingIdDiv      = document.createElement('div');
        eventTrackingIdDiv.className  = "eventTrackingId";
        eventTrackingIdDiv.innerHTML  = eventTrackingId;
        logEntryDiv.appendChild(eventTrackingIdDiv);

        const commandDiv              = document.createElement('div');
        commandDiv.className          = "command";
        commandDiv.innerHTML          = command;
        logEntryDiv.appendChild(commandDiv);

        const messageDiv              = document.createElement('div');
        messageDiv.className          = "message";
        logEntryDiv.appendChild(messageDiv);

        if (Array.isArray(message))
            message = message.reduce(function (a, b) { return a + "<br />" + b; });

        else if (message instanceof HTMLDivElement)
            messageDiv.appendChild(message);

        else
            messageDiv.innerHTML = message;

           // logEntryDiv.innerHTML         = "<div class=\"timestamp\">"       + timestamp       + "</div>" + //  new Date(timestamp).format('dd.mm.yyyy HH:MM:ss') + "</div>" +
           //                         "<div class=\"roamingNetwork\">"  + roamingNetwork  + "</div>" +
           //                         "<div class=\"eventTrackingId\">" + eventTrackingId + "</div>" +
           //                         "<div class=\"command\">"         + command         + "</div>" +
           //                         "<div class=\"message\">"         + message         + "</div>";

        if (logEntryDiv.innerHTML.indexOf(streamFilterInput.value) > -1)
            logEntryDiv.style.display = 'table-row';
        else
            logEntryDiv.style.display = 'none';

        eventsDiv.insertBefore(logEntryDiv, eventsDiv.firstChild);

    }

    function AppendLogEntry(timestamp, roamingNetwork, command, searchPattern, message) {

        const allLogLines = eventsDiv.getElementsByClassName('logLine');
        let   found       = false;

        for (let i = 0; i < allLogLines.length; i++) {
            if (allLogLines[i].getElementsByClassName("command")[0].innerHTML == command) {
                if (allLogLines[i].innerHTML.indexOf(searchPattern) > -1) {
                    found = true;
                    allLogLines[i].getElementsByClassName("message")[0].innerHTML += message;
                    break;
                }
            }
        }

        if (!found) {

            const logEntryDiv = document.createElement('div');
            logEntryDiv.className         = "logLine";
            //logEntryDiv.style.color       = "#" + connectionColor.textcolor;
            //logEntryDiv.style.background  = "#" + connectionColor.background;

            const timestampDiv            = document.createElement('div');
            timestampDiv.className        = "timestamp";
            timestampDiv.innerHTML        = timestamp;
            logEntryDiv.appendChild(timestampDiv);

            const eventTrackingIdDiv      = document.createElement('div');
            eventTrackingIdDiv.className  = "eventTrackingId";
            //eventTrackingIdDiv.innerHTML  = eventTrackingId;
            logEntryDiv.appendChild(eventTrackingIdDiv);

            const commandDiv              = document.createElement('div');
            commandDiv.className          = "command";
            commandDiv.innerHTML          = command;
            logEntryDiv.appendChild(commandDiv);

            const messageDiv              = document.createElement('div');
            messageDiv.className          = "message";
            logEntryDiv.appendChild(messageDiv);

            if (Array.isArray(message))
                message = message.reduce(function (a, b) { return a + "<br />" + b; });

            else if (message instanceof HTMLDivElement)
                messageDiv.appendChild(message);

            else
                messageDiv.innerHTML = "RESPONSE: " + message;

            if (logEntryDiv.innerHTML.indexOf(streamFilterInput.value) > -1)
                logEntryDiv.style.display = 'table-row';
            else
                logEntryDiv.style.display = 'none';

            eventsDiv.insertBefore(logEntryDiv, eventsDiv.firstChild);

        }

    }



    const eventsSource = window.EventSource !== undefined
                             ? new EventSource('../events')
                             : null;

    if (eventsSource !== null)
    {

        eventsSource.onmessage = function (event) {
            console.debug(event);
        };

        eventsSource.onerror = function (event) {
            console.debug(event);
        };


        // {"timestamp":"2024-02-26T21:53:54.019Z","connection":{"localSocket":"127.0.0.1:9920","remoteSocket":"127.0.0.1:64675","customData":{"networkingNodeId":"GD001"}},"message":[2,"100000","BootNotification",{"chargingStation":{"model":"mm","vendorName":"vv"},"reason":"ApplicationReset"}]}


        eventsSource.addEventListener('OnNewTCPConnection',            function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData?.chargeBoxId ?? "-",
                               request.eventTrackingId,
                               "OnNewTCPConnection",
                               request.connection.remoteSocket,
                               // ConnectionColorKey
                               request.connection.remoteSocket);

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNewWebSocketConnection',      function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData?.chargeBoxId ?? "-",
                               request.eventTrackingId,
                               "OnNewWebSocketConnection",
                               request.connection.remoteSocket,
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnJSONMessageRequestReceived2',  function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData.chargeBoxId,
                               request.eventTrackingId,
                               "OnJSONMessageRequestReceived",
                               request.message,
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnJSONMessageResponseSent2',     function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData.chargeBoxId,
                               request.eventTrackingId,
                               "OnJSONMessageResponseSent",
                               request.message,
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnJSONErrorResponseSent',        function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData.chargeBoxId,
                               request.eventTrackingId,
                               "OnJSONErrorResponseSent",
                               JSON.stringify(request),
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnClosesMessageReceived',       function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData.chargeBoxId,
                               request.eventTrackingId,
                               "OnClosesMessageReceived",
                               request.connection.remoteSocket,
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnTCPConnectionClosed',         function (event) {

            try
            {

                const request  = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.connection.customData.chargeBoxId,
                               request.eventTrackingId,
                               "OnTCPConnectionClosed",
                               request.connection.remoteSocket,
                               request.connection.remoteSocket // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        // Certificates

        eventsSource.addEventListener('OnGet15118EVCertificateRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnGet15118EVCertificate",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnGet15118EVCertificateResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnGet15118EVCertificate",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnGetCertificateStatusRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnGetCertificateStatus",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnGetCertificateStatusResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnGetCertificateStatus",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnGetCRLRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnGetCRL",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnGetCRLResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnGetCRL",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnSignCertificateRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnSignCertificate",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnSignCertificateResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnSignCertificate",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);




        // Charging

        eventsSource.addEventListener('OnAuthorizeRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnAuthorize",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnAuthorizeResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnAuthorize",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnClearedChargingLimitRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnClearedChargingLimit",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnClearedChargingLimitResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnClearedChargingLimit",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnMeterValuesRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnMeterValues",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnMeterValuesResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnMeterValues",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyChargingLimitRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyChargingLimit",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyChargingLimitResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyChargingLimit",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyEVChargingNeedsRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyEVChargingNeeds",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyEVChargingNeedsResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyEVChargingNeeds",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyEVChargingScheduleRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyEVChargingSchedule",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyEVChargingScheduleResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyEVChargingSchedule",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyPriorityChargingRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyPriorityCharging",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyPriorityChargingResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyPriorityCharging",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifySettlementRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifySettlement",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifySettlementResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifySettlement",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnPullDynamicScheduleUpdateRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnPullDynamicScheduleUpdate",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnPullDynamicScheduleUpdateResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnPullDynamicScheduleUpdate",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnReportChargingProfilesRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnReportChargingProfiles",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnReportChargingProfilesResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnReportChargingProfiles",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnReservationStatusUpdateRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnReservationStatusUpdate",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnReservationStatusUpdateResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnReservationStatusUpdate",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnStatusNotificationRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnStatusNotification",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnStatusNotificationResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnStatusNotification",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnTransactionEventRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnTransactionEvent",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnTransactionEventResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnTransactionEvent",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);



        // Customer

        eventsSource.addEventListener('OnNotifyCustomerInformationRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyCustomerInformation",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyCustomerInformationResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyCustomerInformation",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyDisplayMessagesRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyDisplayMessages",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyDisplayMessagesResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyDisplayMessages",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);



        // Device Model

        eventsSource.addEventListener('OnLogStatusNotificationRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnLogStatusNotification",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnLogStatusNotificationResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnLogStatusNotification",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyEventRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyEvent",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyEventResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyEvent",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyMonitoringReportRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyMonitoringReport",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyMonitoringReportResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyMonitoringReport",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnNotifyReportRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnNotifyReport",
                               OnNotifyReport(request.request.reportData),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnNotifyReportResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnNotifyReport",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnSecurityEventNotificationRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnSecurityEventNotification",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnSecurityEventNotificationResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnSecurityEventNotification",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);



        // Firmware

        eventsSource.addEventListener('OnBootNotificationRequestReceived',   function (event) {

            try
            {

                const message = JSON.parse((event as MessageEvent).data);

                //timestamp, roamingNetwork, eventTrackingId, command, message, connectionColorKey

                CreateLogEntry(message.timestamp,
                               message.destination,
                               message.eventTrackingId,
                               "OnBootNotification",
                               JSON.stringify(message.request),
                               message.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnBootNotificationResponseSent',      function (event) {

            try
            {

                const message   = JSON.parse((event as MessageEvent).data);
                const request   = message.request;
                const response  = message.response;

                AppendLogEntry(message.timestamp,
                               message.destination,
                               // 1) Search for a logline with this command
                               "OnBootNotification",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + message.eventTrackingId,

                               " &rArr; " +
                               response.status + " (" + response.currentTime + ", " + response.interval + " sec) " + message.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnFirmwareStatusNotificationRequestReceived',   function (event) {

            try {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                    request.destinationNodeId,
                    request.eventTrackingId,
                    "OnFirmwareStatusNotification",
                    JSON.stringify(request.request),
                    request.networkPath[0] // ConnectionColorKey
                );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnFirmwareStatusNotificationResponseSent',      function (event) {

            try {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                    data.chargeBoxId,
                    // 1) Search for a logline with this command
                    "OnFirmwareStatusNotification",
                    // 2) Search for a logline with this pattern
                    "\"eventTrackingId\">" + data.eventTrackingId,

                    " &rArr; " +
                    response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnHeartbeatRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnHeartbeat",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnHeartbeatResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnHeartbeat",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);


        eventsSource.addEventListener('OnPublishFirmwareStatusNotificationRequestReceived',   function (event) {

            try
            {

                const request = JSON.parse((event as MessageEvent).data);

                CreateLogEntry(request.timestamp,
                               request.destinationNodeId,
                               request.eventTrackingId,
                               "OnPublishFirmwareStatusNotification",
                               JSON.stringify(request.request),
                               request.networkPath[0] // ConnectionColorKey
                              );

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);

        eventsSource.addEventListener('OnPublishFirmwareStatusNotificationResponseSent',      function (event) {

            try
            {

                const data      = JSON.parse((event as MessageEvent).data);
                const request   = data.request;
                const response  = data.response;

                AppendLogEntry(response.timestamp,
                               data.chargeBoxId,
                               // 1) Search for a logline with this command
                               "OnPublishFirmwareStatusNotification",
                               // 2) Search for a logline with this pattern
                               "\"eventTrackingId\">" + data.eventTrackingId,

                               " &rArr; " +
                               response.data.status + " (" + response.data.currentTime + ", " + response.data.interval + " sec) " + response.runtime + " ms");

            }
            catch (exception) {
                console.debug(exception);
            }

        }, false);




    }



    const webSocket = new WebSocket('ws://127.0.0.1:7001');

    webSocket.onmessage = (event: MessageEvent): void => {

        console.log("Message from WebSocket server:", event.data);

        // If the message is JSON, you could parse it:
        // const data = JSON.parse(event.data);

    };



    //CreateLogEntry(
    //    "2024-02-26T21:53:54.019Z",
    //    "-",
    //    "1234",
    //    "OnNotifyReport",
    //    OnNotifyReport(pionixDeviceModel),
    //    "-"
    //);


    interface Variable {
        name:                     string;
        instance?:                string;
    }

    interface VariableAttribute {
        type:                     string;
        value:                    string;
        mutability:               string;
        persistent:               boolean;
        constant:                 boolean;
    }

    interface VariableCharacteristics {
        dataType:                 string;
        supportsMonitoring:       boolean;
        unit?:                    string;
        minLimit?:                number;
        maxLimit?:                number;
        valueList?:               string[];
    }

    interface Component {
        name:                     string;
        instance?:                string;
        evse?: {
            id:                   number;
            ConnectorId?:         number;
        };
    }

    interface VariableEntry {
        //component?:               Component;
        instance?:                string;
        //variable:                 Variable;
        variableAttribute:        VariableAttribute[];
        variableCharacteristics:  VariableCharacteristics;
    }

    type ComponentLookup = Map<string, Map<string, Map<number | 'default', Map<string, VariableEntry[]>>>>;


    function OnNotifyReport(reportData: any): HTMLDivElement
    {

        if (!Array.isArray(reportData))
            return;

        try {

            const lookup: ComponentLookup = new Map();

            for (const entry of reportData) {

                // Component Name
                const componentName = entry.component.name;

                if (!lookup.has(componentName))
                    lookup.set(componentName, new Map());

                const componentMap = lookup.get(componentName);


                // Component Instance
                const componentInstance = entry.component.instance || 'default';

                if (!componentMap?.has(componentInstance))
                    componentMap?.set(componentInstance, new Map());

                const componentInstanceMap = componentMap.get(componentInstance);


                // Component EVSE
                const evseId = entry.component.evse?.id || 'default';

                if (!componentInstanceMap?.has(evseId))
                    componentInstanceMap?.set(evseId, new Map());


                // Variable Name
                const variableMap = componentInstanceMap.get(evseId);
                const variableName = entry.variable.name;

                if (!variableMap.has(variableName))
                    variableMap.set(variableName, []);


                // Variable Instance
                const variableInstance = entry.variable.instance || 'default';

                const entries = variableMap?.get(variableName);
                entries?.push({
                    //variable:                 entry.variable,
                    instance: variableInstance,
                    variableAttribute: entry.variableAttribute,
                    variableCharacteristics: entry.variableCharacteristics,
                });

            }

            const deviceModelDiv = document.createElement('div');
            deviceModelDiv.className = "deviceModel";

            for (var component of lookup) {

                const componentDiv = document.createElement('div');
                componentDiv.id = "dmc_" + component[0];
                componentDiv.className = "component";
                deviceModelDiv.appendChild(componentDiv);

                // Component Name
                const componentNameDiv = document.createElement('div');
                componentNameDiv.className = "name";
                componentNameDiv.innerHTML = component[0];
                componentDiv.appendChild(componentNameDiv);

                // Component Instances
                const componentInstancesDiv = document.createElement('div');
                componentInstancesDiv.className = "componentInstances";
                componentDiv.appendChild(componentInstancesDiv);

                for (var componentInstance of component[1]) {

                    // Component Instance
                    const componentInstanceDiv = document.createElement('div');
                    componentInstanceDiv.id = "dmc_" + component[0] + "_" + componentInstance[0];
                    componentInstanceDiv.className = "componentInstance";
                    componentInstancesDiv.appendChild(componentInstanceDiv);

                    // Component Instance Name
                    if (componentInstance[0].toString() !== "default") {
                        const componentInstanceNameDiv = document.createElement('div');
                        componentInstanceNameDiv.className = "name";
                        componentInstanceNameDiv.innerHTML = componentInstance[0] + " (Instance)";
                        componentInstanceDiv.appendChild(componentInstanceNameDiv);
                    }

                    // EVSEs
                    const evsesDiv = document.createElement('div');
                    evsesDiv.className = "evses";
                    componentInstanceDiv.appendChild(evsesDiv);

                    for (var evse of componentInstance[1]) {

                        const evseDiv = document.createElement('div');
                        evseDiv.id = "dmc_" + component[0] + "_" + componentInstance[0] + "_" + evse[0].toString();
                        evseDiv.className = "evse";
                        evsesDiv.appendChild(evseDiv);

                        // EVSE Id or "default"
                        if (evse[0].toString() !== "default") {
                            const evseNameDiv = document.createElement('div');
                            evseNameDiv.className = "name";
                            evseNameDiv.innerHTML = "EVSE #" + evse[0].toString();
                            evseDiv.appendChild(evseNameDiv);
                        }

                        // Variables
                        const variablesDiv = document.createElement('div');
                        variablesDiv.className = "variables";
                        evseDiv.appendChild(variablesDiv);

                        for (var variable of evse[1]) {

                            const variableDiv = document.createElement('div');
                            variableDiv.className = "variable";
                            variablesDiv.appendChild(variableDiv);

                            // Variable Name
                            const variableNameDiv = document.createElement('div');
                            variableNameDiv.className = "name";
                            variableNameDiv.innerHTML = variable[0];
                            variableDiv.appendChild(variableNameDiv);

                            // Variable Instances
                            const instancesDiv = document.createElement('div');
                            instancesDiv.className = "instances";
                            variableDiv.appendChild(instancesDiv);

                            for (var instance of variable[1]) {

                                const instanceDiv = document.createElement('div');
                                instanceDiv.className = "instance";
                                instancesDiv.appendChild(instanceDiv);

                                // Instance
                                if (instance.instance !== "default") {
                                    const instanceNameDiv = document.createElement('div');
                                    instanceNameDiv.className = "name";
                                    instanceNameDiv.innerHTML = instance.instance + " (Variable Instance)";
                                    instanceDiv.appendChild(instanceNameDiv);
                                }

                                const vc = instance.variableCharacteristics;

                                if (vc != null) {

                                    // Variable Characteristics
                                    const characteristicsDiv = document.createElement('div');
                                    characteristicsDiv.className = "characteristics";
                                    instanceDiv.appendChild(characteristicsDiv);

                                    const dataTypeDiv = document.createElement('div');
                                    dataTypeDiv.className = "dataType";
                                    dataTypeDiv.innerHTML = "Data Type: " + (instance.variableCharacteristics?.dataType ?? "-");
                                    characteristicsDiv.appendChild(dataTypeDiv);

                                    const supportsMonitoringDiv = document.createElement('div');
                                    supportsMonitoringDiv.className = "supportsMonitoring";
                                    supportsMonitoringDiv.innerHTML = "Supports Monitoring: " + (instance.variableCharacteristics.supportsMonitoring ? "true" : "false");
                                    characteristicsDiv.appendChild(supportsMonitoringDiv);

                                    if (instance.variableCharacteristics.unit) {
                                        const unitDiv = document.createElement('div');
                                        unitDiv.className = "unit";
                                        unitDiv.innerHTML = "Unit: " + instance.variableCharacteristics.unit;
                                        characteristicsDiv.appendChild(unitDiv);
                                    }

                                    if (instance.variableCharacteristics.minLimit) {
                                        const minLimitDiv = document.createElement('div');
                                        minLimitDiv.className = "minLimit";
                                        minLimitDiv.innerHTML = "Min Limit: " + instance.variableCharacteristics.minLimit;
                                        characteristicsDiv.appendChild(minLimitDiv);
                                    }

                                    if (instance.variableCharacteristics.maxLimit) {
                                        const maxLimitDiv = document.createElement('div');
                                        maxLimitDiv.className = "maxLimit";
                                        maxLimitDiv.innerHTML = "Max Limit: " + instance.variableCharacteristics.maxLimit;
                                        characteristicsDiv.appendChild(maxLimitDiv);
                                    }

                                    if (instance.variableCharacteristics.valueList) {

                                        const valueListDiv = document.createElement('div');
                                        valueListDiv.className = "valueList";
                                        characteristicsDiv.appendChild(valueListDiv);

                                        if (Array.isArray(reportData))
                                            valueListDiv.innerHTML = "Value List: " + instance.variableCharacteristics.valueList;

                                        else
                                            valueListDiv.innerHTML = "Value List: " + instance.variableCharacteristics.valueList;

                                    }

                                }


                                // Variable Attribute(s)
                                const attributeDiv = document.createElement('div');
                                attributeDiv.className = "attribute";
                                instanceDiv.appendChild(attributeDiv);

                                const constantDiv = document.createElement('div');
                                constantDiv.className = "constant";
                                constantDiv.innerHTML = "Constant: " + (instance.variableAttribute[0].constant ? "true" : "false");
                                attributeDiv.appendChild(constantDiv);

                                const mutabilityDiv = document.createElement('div');
                                mutabilityDiv.className = "mutability";
                                mutabilityDiv.innerHTML = "Mutability: " + instance.variableAttribute[0].mutability;
                                attributeDiv.appendChild(mutabilityDiv);

                                const persistentDiv = document.createElement('div');
                                persistentDiv.className = "persistent";
                                persistentDiv.innerHTML = "Persistent: " + (instance.variableAttribute[0].persistent ? "true" : "false");
                                attributeDiv.appendChild(persistentDiv);

                                const typeDiv = document.createElement('div');
                                typeDiv.className = "type";
                                typeDiv.innerHTML = "Type: " + instance.variableAttribute[0].type;
                                attributeDiv.appendChild(typeDiv);

                                const valueDiv = document.createElement('div');
                                valueDiv.className = "value";
                                valueDiv.innerHTML = "Value: '" + instance.variableAttribute[0].value + "'";
                                attributeDiv.appendChild(valueDiv);


                            }

                        }

                    }

                }

            }

            return deviceModelDiv;

        } catch (exception) {
            const x = document.createElement('div')
            x.innerHTML = exception.toString();
            return x;
        }

    }


}
