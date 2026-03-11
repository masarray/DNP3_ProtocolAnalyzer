<!-- ANCHOR: PARSER_PIPELINE_START -->
# Parser Pipeline

## Current State

Saat ini project memakai decoder stub berbasis summary string. Itu cukup untuk UI scaffolding, tetapi belum cukup untuk analyzer nyata.

## Target Pipeline

1. `Input Acquisition`
- live TCP bytes
- serial bytes
- replay file
- sample feed

2. `Frame Boundary Detection`
- identifikasi awal frame DNP3
- sinkronisasi start bytes
- validasi panjang frame
- validasi CRC per blok

3. `Link Layer Decode`
- source
- destination
- control field
- direction
- PRM/FCB/FCV/DFC

4. `Transport Layer Decode`
- FIR
- FIN
- sequence
- fragment grouping

5. `Application Layer Decode`
- application control
- function code
- indication bits
- response classification

6. `Object Header Decode`
- group
- variation
- qualifier
- range/index

7. `Object Value Decode`
- binary input
- analog input
- counter
- frozen counter
- CROB/control
- time objects

8. `Semantic Projection`
- frame summary
- point state update
- protocol event
- validation input

## Recommended Class Flow

- `IFrameSource`
- `Dnp3FrameBoundaryScanner`
- `Dnp3LinkDecoder`
- `Dnp3TransportReassembler`
- `Dnp3ApplicationDecoder`
- `Dnp3ObjectDecoder`
- `Dnp3PointProjector`
- `Dnp3EventProjector`

## Output Strategy

Jangan decode langsung ke UI model. Gunakan dua tahap:

1. decode ke model protokol DNP3
2. project ke model analyzer umum

Keuntungannya:
- parser bisa diuji tanpa UI
- rule engine bisa memakai objek yang lebih kaya
- perubahan tampilan tidak merusak decoder

## Milestones

### Milestone A
- link header decode
- function code detection
- object header summary

### Milestone B
- binary / analog / counter object decode
- point projection

### Milestone C
- unsolicited correlation
- select-operate lifecycle
- time sync interpretation

### Milestone D
- replay import
- parser regression tests
<!-- ANCHOR: PARSER_PIPELINE_END -->
