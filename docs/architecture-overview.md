<!-- ANCHOR: ARCHITECTURE_OVERVIEW_START -->
# Architecture Overview

Project ini tetap diarahkan sebagai aplikasi offline desktop untuk `DNP3 Protocol Tester Analyzer`, tetapi coding plan-nya disusun seolah-olah setiap layer bisa dikerjakan paralel dan matang dulu sebelum integrasi UI penuh.

## Design Goals

- UI WPF hanya menangani presentasi dan operator workflow
- Logic DNP3 ditempatkan di layer protocol/application, bukan di code-behind
- Session harus mendukung `live`, `replay`, dan `sample/demo`
- Engine tester dan analyzer memakai model domain yang sama
- Struktur proyek harus siap bertumbuh dari stub menjadi parser dan validator nyata

## Runtime Layers

1. `Presentation`
- WPF shell
- View binding untuk line monitor, point viewer, event log, status history
- Command surface untuk start session, load capture, run scenario

2. `Application`
- Mengorkestrasi session
- Menjalankan analyzer pipeline
- Menyusun snapshot untuk UI
- Mengelola scenario execution dan hasil validasi

3. `Domain`
- Model umum analyzer
- Definisi finding, point state, decoded frame, test scenario, validation issue
- Tidak bergantung pada WPF atau transport tertentu

4. `Infrastructure`
- Capture source
- Transport profile
- File import/export
- Replay source
- Integrasi library eksternal bila nanti dibutuhkan

5. `Protocols.Dnp3`
- Decoder frame
- Object mapper
- Point state builder
- Rule set baseline
- Scenario-specific evaluators

## Operating Modes

- `Sample Mode`: untuk demo dan scaffolding UI
- `Replay Mode`: membaca capture yang tersimpan
- `Live Mode`: membaca traffic TCP/serial secara real
- `Test Mode`: menjalankan scenario dan menghasilkan verdict

## Core Principle

Analyzer dan tester tidak dipisahkan menjadi dua aplikasi. Keduanya berbagi:
- session model
- frame decode pipeline
- point state projection
- event stream
- validation result

Yang berbeda hanya workflow:
- analyzer fokus observability
- tester fokus expectation dan verdict
<!-- ANCHOR: ARCHITECTURE_OVERVIEW_END -->
