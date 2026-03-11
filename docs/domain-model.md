<!-- ANCHOR: DOMAIN_MODEL_START -->
# Domain Model

## Primary Entities

### `CaptureSession`
Mewakili satu workspace analisis atau pengujian.

Field penting:
- `SessionName`
- `StartedAtUtc`
- `TransportProfile`
- `Packets`

### `PacketRecord`
Mewakili satu input frame mentah sebelum decode penuh.

Field penting:
- `TimestampUtc`
- `Direction`
- `Summary`
- `Payload`

### `DecodedFrame`
Hasil interpretasi awal terhadap packet.

Field penting:
- `TimestampText`
- `Direction`
- `FunctionCode`
- `ObjectSummary`
- `Summary`

Future extension:
- `SourceAddress`
- `DestinationAddress`
- `ApplicationControl`
- `SequenceNumber`
- `FunctionCodeRaw`
- `ObjectHeaders`

### `MeasurementPoint`
Representasi point state terakhir yang ditampilkan di UI.

Field penting:
- `PointAddress`
- `PointType`
- `ValueText`
- `Quality`
- `TimestampText`

Future extension:
- `Index`
- `Group`
- `Variation`
- `Flags`
- `RawValue`
- `LastEventClass`

### `ProtocolEvent`
Peristiwa yang layak ditampilkan ke operator.

Contoh:
- session event
- decoder warning
- unsolicited observed
- missing confirmation
- time skew detected

### `ValidationIssue`
Temuan yang berasal dari evaluasi rule.

Field penting:
- `Severity`
- `Title`
- `Detail`

Severity yang disarankan:
- `Info`
- `Pass`
- `Warn`
- `Error`

### `TestScenario`
Definisi tujuan pengujian.

Field penting:
- `Name`
- `Goal`
- `ExpectedOutcome`

Future extension:
- `Preconditions`
- `Steps`
- `Timeouts`
- `SuccessCriteria`
- `FailureCriteria`

### `SessionSnapshot`
Paket data yang siap dikonsumsi UI.

Isi:
- traffic frames
- points
- events
- status history

## Future Domain Objects

Saat parser dan tester mulai matang, tambahkan:
- `Dnp3LinkFrame`
- `Dnp3TransportFragment`
- `Dnp3ApplicationFragment`
- `Dnp3ObjectHeader`
- `Dnp3PointUpdate`
- `CommandLifecycleRecord`
- `ScenarioExecution`
- `ScenarioStepResult`
- `ReplayArtifact`
<!-- ANCHOR: DOMAIN_MODEL_END -->
