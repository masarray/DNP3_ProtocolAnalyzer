# DNP3 Protocol Tester Analyzer

Desktop WPF workspace untuk membangun `DNP3 Protocol Tester & Analyzer` yang diarahkan ke pengalaman operator sekelas tool commissioning industrial: line monitor, measurement view, event log, findings, protocol context, dan session history.

## Current UX Direction

UX sekarang mengikuti pola yang sudah terbukti nyaman pada analyzer IEC internal:

- kiri: `Line Monitor` yang menampilkan raw traffic dan terjemahan semantik
- kanan: `Measurement View`, `Event Log`, `Findings`, `Protocol Context`, `Glossary / Legend`
- bawah: `Status History`
- top toolbar: workflow operator seperti `Connection Setup`, `Start Session`, `Load Sample`, `Save/Open Capture`, dan `Export`

## Current Scope

Yang sudah ada saat ini:

- WPF analyzer shell dengan layout industrial
- sample DNP3 session untuk integrity poll, unsolicited, confirm, dan operate
- line monitor dengan field source, destination, function, object, IIN, semantic tag, dan raw hex
- measurement view, event log, findings, protocol context, glossary, dan status history
- baseline rule set dan scenario catalog awal

Yang belum selesai:

- parser DNP3 byte-level nyata
- replay file dan PCAP import
- live TCP/serial capture
- scenario runner dan verdict engine real
- reporting/export final

## Project Structure

- `Core`: abstractions, shared models, workspace service
- `Infrastructure`: sample capture source dan transport profile
- `Protocols/Dnp3`: decoding, point mapping, rules, analyzer pipeline
- `Testing/Dnp3`: scenario catalog
- `docs`: architecture, roadmap, parser plan, rule plan, engine strategy

## Immediate Next Steps

1. Implement real DNP3 parser pipeline: frame sync, CRC, link, transport, application, object decode.
2. Tambahkan replay/session file format untuk membuka hasil capture lapangan.
3. Bangun live transport adapter TCP dan serial.
4. Implement scenario engine untuk integrity poll, class poll, unsolicited, select-operate, time sync, dan restart.
5. Tambahkan report/export industrial untuk FAT/SAT dan commissioning.
