# OCPP Incoming Charging Messages

## Charging Stations -> Local Controllers, CSMSs

- StatusNotification


### ChargingProfiles

- ReportChargingProfiles


### Transaction

- Authorize
- MeterValues
- ReservationStatusUpdate
- TransactionEvent



## CSMSs -> Charging Stations, Local Controllers

- GetCompositeSchedule
- NotifyAllowedEnergyTransfer
- UnlockConnector
- UpdateDynamicSchedule
- UsePriorityCharging


### ChargingProfiles

- ClearChargingProfile
- GetChargingProfiles
- SetChargingProfile


### Transaction

- CancelReservation
- GetTransactionStatus
- QRCodeScanned
- RequestStartTransaction
- RequestStopTransaction
- ReserveNow
