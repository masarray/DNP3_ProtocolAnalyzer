<!-- ANCHOR: IMPLEMENTATION_ROADMAP_START -->
# Implementation Roadmap

## Phase 1: Foundation

Target:
- struktur project stabil
- UI shell siap
- sample session berjalan

Status:
- selesai

Deliverables:
- WPF shell
- core abstractions
- sample capture feed
- DNP3 analyzer stub
- baseline docs

## Phase 2: Protocol Model Expansion

Target:
- model DNP3 internal mulai nyata

Work items:
- tambah model link/application/object header
- pisahkan summary decode dari raw protocol decode
- siapkan projector ke `DecodedFrame`

## Phase 3: Replay Analyzer

Target:
- analyzer bisa membaca replay data

Work items:
- format replay sederhana
- file loader
- session persistence
- parser regression sample set

## Phase 4: Real Decoder

Target:
- decode DNP3 frame aktual

Work items:
- boundary scanner
- CRC validation
- link decode
- transport reassembly
- application decode
- object decode awal

## Phase 5: Point Projection

Target:
- point viewer merefleksikan object update nyata

Work items:
- binary input
- analog input
- counter
- control relay output projection

## Phase 6: Tester Engine

Target:
- skenario test mulai menghasilkan verdict

Work items:
- scenario runner
- expectation matcher
- lifecycle tracker
- verdict summary

## Phase 7: Live Transport

Target:
- analyzer bisa dipakai untuk live session offline

Work items:
- TCP channel profile
- serial channel profile
- connection setup workflow
- capture buffering

## Phase 8: Reporting

Target:
- hasil test dan analisis bisa disimpan dan dibuka lagi

Work items:
- session export
- finding export
- summary report
- comparison session

## Practical Work Split Between Codex Web and Local App

### Codex Web
- arsitektur
- domain model
- parser design
- rule design
- class breakdown
- test plan

### Codex App Local
- implementasi file nyata
- build dan smoke test
- integrasi WPF
- runtime verification
<!-- ANCHOR: IMPLEMENTATION_ROADMAP_END -->
