# AGENTS.md — DNP3 Protocol Analyzer Production Engineering Contract

This file governs every coding agent, assistant, and maintainer modifying DNP3_ProtocolAnalyzer.

Treat this repository as production engineering software for DNP3 analysis, FAT/SAT, commissioning, replay, troubleshooting, and evidence generation. The current shell may still have incomplete protocol/runtime features; that is not permission to implement naive or throwaway architecture.

## Product boundary

- Preserve operator-visible evidence and raw protocol truth.
- Current architecture separates `Core`, `Infrastructure`, `Protocols/Dnp3`, `Testing/Dnp3`, and WPF presentation. Keep those boundaries meaningful.
- Sample sessions/scenarios are test fixtures, not evidence that live parsing or interoperability is proven.
- Do not fabricate decoded values, timestamps, IIN state, command results, object semantics, or transport state.
- Keep uncertainty explicit when bytes cannot be decoded safely.

## Prime directive

Do not begin with a naive parser, synchronous UI implementation, disposable state machine, or one-off mock just to make a screenshot work.

Before editing code:
1. locate the current implementation/state owner;
2. reproduce or characterize the behavior;
3. identify the root cause or violated invariant;
4. identify callers, lifecycle, concurrency, and tests;
5. make the smallest coherent production-quality change;
6. protect the failure mode with regression coverage where practical;
7. validate build, failure paths, responsiveness, and evidence integrity.

If roughly three corrective patches accumulate in the same subsystem without stable behavior, stop and reassess architecture/ownership instead of stacking another workaround.

## Architecture ownership

- WPF views/view-model code must not own DNP3 parser state, transport reassembly, application sequencing, object decoding, or command truth.
- `Protocols/Dnp3` owns protocol semantics and decoding.
- `Infrastructure` owns file/live transport boundaries and external I/O.
- `Core` owns stable shared domain/session abstractions, not UI-specific state.
- `Testing/Dnp3` owns deterministic scenarios/fixtures, not production behavior.
- Keep one authoritative session/evidence model; do not create a second hidden decoded-state cache with different semantics.

## Result / Try / exception policy

Expected runtime outcomes are not exceptional control flow.

Examples: incomplete frame, CRC failure, unknown object variation, invalid qualifier, fragmented transport sequence, timeout, EOF, cancellation, unsupported function, malformed capture, disconnected transport.

For expected outcomes:
- prefer `Try...`, typed result/status records, discriminated enums, or another explicit non-throwing contract;
- validate before indexing/casting/converting;
- contain infrastructure exceptions at file/network/serial/platform boundaries;
- convert them to structured diagnostics or typed failure outcomes;
- never swallow exceptions silently;
- one malformed frame must not terminate the full analysis session when isolation is possible.

Do not force Result wrappers into pure helpers that cannot meaningfully fail.

## DNP3 parser invariants

Treat all input bytes as untrusted.

Validate before use:
- link start bytes and minimum/maximum frame length;
- header/data CRC boundaries and chunk CRCs;
- destination/source address fields;
- transport FIR/FIN/sequence handling;
- application FIR/FIN/CON/UNS/sequence state;
- function code and IIN presence/meaning;
- object group/variation support;
- qualifier/range/count/prefix semantics;
- payload bounds before every decode;
- integer/float/time conversion and finite-value assumptions;
- timestamp quality/availability before presenting source time as authoritative.

Never read past the available span. Preserve raw bytes when semantic decoding is incomplete.

## Reassembly and state machines

- Link, transport, and application reassembly must have explicit state ownership and reset rules.
- Fragment/state promotion must be transactional: validate candidate state before replacing last-known-good state.
- Never repair a sequencing bug using arbitrary delays.
- Reset/recovery behavior must be deterministic and tested.
- Bound retained partial fragments and session histories.
- A corrupt frame must not poison later valid frames when resynchronization is possible.

## Live capture / replay / file boundaries

Future TCP, serial, capture-file, and replay paths must remain interchangeable infrastructure adapters feeding the same protocol pipeline.

- No parser fork per transport.
- Reads must be finite/cancellation-aware where possible.
- Replay timing must not rewrite protocol timestamps.
- File/capture parsing must stream/chunk large inputs rather than load unbounded data into memory.
- Stop/cancel/close must dispose handles/tasks/subscriptions deterministically.

## UI responsiveness and backpressure

The WPF dispatcher must never own blocking I/O or expensive parser work.

- Do not render one row synchronously per low-level frame/object when traffic volume is high.
- Batch/coalesce monitor updates while preserving engineering meaning.
- Keep large grids virtualized/recycling-enabled.
- Do not rebuild the full visual tree/collection for a single point/event update.
- Measurement views are current-state snapshots; forensic traces/event logs require bounded retention or paging.
- Expensive filter/sort/export/report work belongs off the UI thread when material.

The 16.7 ms value is the entire 60 Hz frame budget, not a per-function allowance.

## Diagnostics

- Hot paths emit compact status/counters/events only.
- Use bounded asynchronous diagnostic delivery when volume can spike.
- Aggregate/deduplicate/rate-limit repeated CRC/error/timeout storms.
- Formatting/file persistence belongs off parser/capture hot paths.
- Diagnostic sink failure must not block parsing or corrupt session state.
- Normal unsupported/unknown protocol evidence should not be mislabeled as application crashes.

## Evidence integrity

- Open socket/readable file is not proof of valid DNP3 communication.
- Parsed header is not proof that all application objects are valid.
- Command request is not command success.
- Command success is not final field feedback unless evidence confirms it.
- Prefer source/IED timestamps when valid; label capture-time fallback explicitly.
- Keep raw frame bytes and decode provenance available for findings/reports.
- Findings must be explainable from captured evidence and deterministic rules.

## Scenario/testing discipline

Sample/scenario data must be deterministic and clearly synthetic.

Use it to prove:
- normal integrity/class poll;
- unsolicited + confirm behavior;
- select/operate lifecycle;
- fragmented transport/application messages;
- CRC failure and resynchronization;
- unknown group/variation/qualifier;
- restart/time-sync/IIN conditions;
- malformed/truncated input and recovery.

Do not tune production logic to pass one synthetic scenario while breaking protocol invariants.

## Performance and memory

For performance-sensitive changes, measure when practical:
- frame/object throughput;
- parser/reassembly latency;
- UI publication latency;
- allocation/GC pressure;
- retained evidence size;
- queue depth/backlog/drop policy;
- process CPU/memory;
- cancellation convergence.

Prefer bounded algorithms and reduced rendering over speculative pooling/caching.

## Regression discipline

Every meaningful fix should protect the exact failure mode at the highest stable seam available.

Examples:
- CRC/length bug → exact byte fixture;
- fragmentation bug → multi-fragment sequence case;
- object decode bug → group/variation/qualifier fixture;
- state reset bug → deterministic session sequence;
- UI flood bug → bounded publication/state-coherence test;
- file/replay bug → malformed/large-input regression case.

## CI and definition of done

The repository must maintain a pull-request build gate for the WPF application. A green compile gate is necessary but not sufficient.

A task is complete only when applicable evidence includes:
- root cause identified;
- build succeeds;
- targeted regression tests pass;
- malformed/recovery paths checked;
- UI responsiveness preserved;
- protocol/evidence semantics preserved;
- performance measured when relevant;
- remaining live-device/capture validation limits stated explicitly.

## Required completion report

For substantial work, report:
1. Changed
2. Root cause
3. Architecture/ownership impact
4. Regression protection
5. Performance impact
6. Validation actually run
7. Remaining limitations / field validation still needed

Never claim a check passed if it was not run.

## Priority order

1. protocol correctness and evidence integrity
2. failure containment and deterministic recovery
3. regression compatibility
4. responsiveness and bounded concurrency
5. performance and memory
6. maintainability
7. implementation convenience
8. visual polish

## Final rule

Understand first. Parse defensively. Preserve raw evidence. Keep state explicit and bounded. Contain failures. Keep the UI responsive. Validate the exact failure mode. Ship only what has evidence.
