# DNP3 Industrial Engine Strategy

## Objective

Bangun engine `DNP3 Protocol Tester & Analyzer` yang layak dipakai engineer commissioning, FAT/SAT, dan forensic troubleshooting relay seperti SIPROTEC, dengan dua mode utama yang berbagi pipeline inti:

- `Analyzer Mode`: observability, decode, korelasi event, troubleshooting
- `Tester Mode`: skenario aktif, expected sequence, verdict, dan report

Target UX boleh meniru analyzer IEC yang sudah matang, tetapi target engine harus disusun sebagai produk industrial, bukan demo parser.

## Product Pillars

### 1. Protocol Fidelity
- decode frame DNP3 aktual dari byte stream
- validasi CRC, length, fragment, sequence, FIR/FIN, IIN
- dukung variasi object penting untuk relay protection dan SCADA

### 2. Operator Clarity
- raw traffic tetap tersedia
- setiap frame punya translasi semantik yang mudah dibaca engineer
- event, point update, dan findings dapat ditelusuri balik ke frame sumber

### 3. Test Determinism
- skenario test harus punya precondition, stimulus, observation window, expected result, fail condition
- hasil test harus repeatable pada replay maupun live execution

### 4. Industrial Workflow
- support live session, replay, save/open session, export finding/report
- tetap usable untuk FAT, SAT, troubleshooting vendor relay, dan audit pasca gangguan

## Runtime Architecture

## Layer 1: Acquisition

Sumber input:
- live DNP3 TCP client/server monitor
- serial capture / serial master session
- replay file internal
- future PCAP import

Komponen yang disarankan:
- `IFrameSource`
- `Dnp3TcpCaptureAdapter`
- `Dnp3SerialCaptureAdapter`
- `Dnp3ReplaySource`
- `PcapImportAdapter`

Tanggung jawab:
- timestamp frame
- direction tagging TX/RX
- buffering dan backpressure
- session lifecycle events

## Layer 2: Frame Processing

Pipeline inti:
1. byte stream normalization
2. frame boundary detection
3. CRC validation per block
4. link layer decode
5. transport fragment reassembly
6. application layer decode
7. object header decode
8. object value decode

Komponen yang disarankan:
- `Dnp3FrameBoundaryScanner`
- `Dnp3CrcValidator`
- `Dnp3LinkDecoder`
- `Dnp3TransportReassembler`
- `Dnp3ApplicationDecoder`
- `Dnp3ObjectHeaderDecoder`
- `Dnp3ObjectValueDecoder`

## Layer 3: Domain Projection

Engine tidak boleh decode langsung ke UI model. Pakai dua tahap:

1. protocol domain model
2. analyzer/tester projection model

Protocol domain objects minimal:
- `Dnp3LinkFrame`
- `Dnp3TransportFragment`
- `Dnp3ApplicationFragment`
- `Dnp3ObjectHeader`
- `Dnp3DecodedObject`
- `Dnp3IinState`

Projection output minimal:
- `DecodedFrame`
- `MeasurementPoint`
- `ProtocolEvent`
- `FindingItem`
- `ProtocolContextItem`

## Layer 4: Fact Building

Sebelum rule engine jalan, bangun kumpulan facts yang stabil dan bisa diuji.

Contoh facts:
- unsolicited enabled / disabled state
- pending confirm expectation
- integrity poll request-response pair
- class poll backlog state
- select-operate pending lifecycle
- time sync request/ack/skew
- device restart observed
- point value transition history

Komponen yang disarankan:
- `IRuleFactBuilder`
- `Dnp3FactSnapshot`
- `CommandLifecycleFactBuilder`
- `PollingFactBuilder`
- `TimeSyncFactBuilder`

## Layer 5: Rule Engine

Rule dibagi menjadi tiga kelompok besar.

### A. Baseline Protocol Rules
- malformed frame
- CRC invalid
- bad fragment continuity
- unsolicited observed before enable
- missing confirm after unsolicited
- unexpected response function
- restart indication not followed by reinitialization workflow

### B. Operational Analyzer Rules
- stale analog timestamp
- quality flag abnormal
- event flood / burst
- counter rollback
- point chatter
- command response mismatch
- inconsistent object variation for the same point

### C. Test Scenario Rules
- integrity poll complete/incomplete
- class 1/2/3 poll sequencing
- unsolicited end-to-end confirmation
- select-operate success/timeout/negative ack
- direct operate result
- cold restart / warm restart behavior
- time sync confirmation and skew

## Layer 6: Tester Orchestration

Tester engine harus aktif mengirim stimulus dan memantau outcome.

Komponen yang disarankan:
- `ITestScenarioRunner`
- `ScenarioExecutionContext`
- `ScenarioStimulusDispatcher`
- `ScenarioExpectationMatcher`
- `VerdictAggregator`

Setiap scenario minimal punya:
- `Name`
- `Mode`: passive analyzer / active tester
- `Preconditions`
- `Stimuli`
- `ObservationWindow`
- `ExpectedSequence`
- `PassCriteria`
- `FailCriteria`
- `ArtifactsProduced`

## Industrial Scenario Pack Priorities

### Phase A: Analyzer-First
- integrity poll correlation
- unsolicited event correlation
- confirm tracking
- restart indication tracking
- point projection for binary and analog

### Phase B: Commissioning Essentials
- class poll analyzer
- select-operate lifecycle
- direct operate lifecycle
- device restart sequence
- time sync flow

### Phase C: Advanced Testing
- freeze / counter scenarios
- analog deadband behavior observation
- event buffer drain behavior
- sequence anomaly detection
- comparison between baseline capture and current session

## Data Model Priorities

Field minimal yang harus tersedia pada decoded domain:
- timestamp
- direction
- raw bytes
- link source / destination
- link control bits
- FIR / FIN / sequence
- application control
- function code
- IIN bits
- object group / variation / qualifier
- start-stop index or count range
- decoded values and flags
- frame references

Tanpa field tersebut, analyzer akan selalu terbatas pada demo UI.

## UX-to-Engine Mapping

Agar UX mirip analyzer IEC tetap bernilai, setiap panel harus punya sumber data engine yang jelas.

### Line Monitor
Sumber:
- decoded frames
- semantic labels
- raw bytes

### Measurement View
Sumber:
- point projection engine
- latest known state
- freshness / quality / event class

### Event Log
Sumber:
- semantic event projector
- command lifecycle tracker
- unsolicited and poll correlation

### Findings
Sumber:
- baseline rules
- scenario rules
- reliability heuristics

### Protocol Context
Sumber:
- session settings
- last known IIN
- fragment state
- active classes and device state

### Status History
Sumber:
- capture lifecycle
- tester milestones
- export and replay actions

## Verification Strategy

### Unit Tests
- CRC validation
- boundary scanner
- link decode
- transport reassembly
- object decoder per group/variation
- fact builder
- scenario matcher

### Regression Samples
- golden replay files dari relay SIPROTEC
- expected decoded summary snapshots
- expected finding/verdict snapshots

### System Tests
- sample relay simulator
- real TCP loopback
- serial harness when available
- replay-open-export roundtrip

## Delivery Strategy

### Milestone 1
- analyzer UX solid
- replay sample sessions
- real frame/link/application summary

### Milestone 2
- binary + analog projection
- findings baseline
- save/open session

### Milestone 3
- active tester for integrity/class/unsolicited/select-operate/time sync
- verdict summary and report export

### Milestone 4
- field-ready workflows
- comparison sessions
- performance hardening for long captures

## Non-Functional Requirements

- tahan untuk long-running capture
- trimming/virtualization untuk koleksi UI besar
- deterministic timestamps and ordering
- clear error reporting untuk malformed traffic
- minimal operator clicks for commissioning workflow
- export evidence yang mudah dibawa ke FAT/SAT report

## Recommended Next Implementation Order

1. real decoded protocol domain model
2. frame boundary + CRC + link decode
3. application + object header decode
4. point projector untuk binary/analog/counter/CROB
5. replay file format dan loader
6. fact builder
7. baseline findings
8. scenario runner
9. live TCP/serial adapter
10. export/report pipeline
