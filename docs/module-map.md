<!-- ANCHOR: MODULE_MAP_START -->
# Module Map

## Solution Layout

- `Core/Abstractions`
- `Core/Models`
- `Core/Services`
- `Infrastructure/Capture`
- `Infrastructure/Transport`
- `Protocols/Dnp3/Decoding`
- `Protocols/Dnp3/Mapping`
- `Protocols/Dnp3/Rules`
- `Testing/Dnp3`
- `docs`

## Module Responsibilities

### `Core/Abstractions`
- `IFrameDecoder`
- `IProtocolAnalyzer`
- `IProtocolRuleSet`
- `ISessionService`

Tujuan:
- mendefinisikan kontrak inti
- menjaga agar UI tidak bergantung pada implementasi spesifik DNP3 secara langsung

### `Core/Models`
- `CaptureSession`
- `PacketRecord`
- `DecodedFrame`
- `MeasurementPoint`
- `ProtocolEvent`
- `ValidationIssue`
- `TestScenario`
- `SessionSnapshot`

Tujuan:
- jadi bahasa bersama di seluruh layer

### `Core/Services`
- `AnalyzerWorkspaceService`

Tujuan:
- adapter antara pipeline analyzer dengan kebutuhan presentasi WPF

### `Infrastructure/Capture`
- feed sample
- replay provider
- file-backed capture reader
- future: PCAP import adapter

### `Infrastructure/Transport`
- profile TCP
- profile serial
- future: connection factory

### `Protocols/Dnp3/Decoding`
- parse frame mentah
- identifikasi function code
- identifikasi object group/variation
- ekstrak fragment summary

### `Protocols/Dnp3/Mapping`
- memetakan decoded object menjadi point state
- memproyeksikan update ke point viewer

### `Protocols/Dnp3/Rules`
- baseline validation
- scenario rule packs
- protocol compliance checks

### `Testing/Dnp3`
- scenario catalog
- future: scenario runner
- future: step evaluator

## Dependency Direction

- `Presentation -> Core.Services`
- `Core.Services -> Core.Abstractions + Core.Models`
- `Protocols.Dnp3 -> Core.Abstractions + Core.Models`
- `Infrastructure -> Core.Models`
- `Testing -> Core.Models`

Tidak boleh:
- `Core.Models -> WPF`
- `Protocols.Dnp3 -> MainWindow`
- `Rules -> UI controls`
<!-- ANCHOR: MODULE_MAP_END -->
