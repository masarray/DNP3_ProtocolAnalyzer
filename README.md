# DNP3 Protocol Tester Analyzer

Scaffold WPF analyzer untuk pengembangan DNP3 master/outstation tester dengan layout industrial gelap.

## Struktur awal

- `Core`: abstractions, models, session service
- `Infrastructure`: capture feed dan transport profile
- `Protocols/Dnp3`: decoder, point mapping, rules baseline
- `Testing/Dnp3`: katalog skenario test awal
- `MainWindow`: shell analyzer dengan Line Monitor, Point Viewer, Validation Events, dan Status History

## Next step

1. Ganti decoder stub menjadi parser frame DNP3 nyata.
2. Tambahkan live transport adapter TCP/serial.
3. Tambahkan import replay/pcap.
4. Bangun scenario runner untuk integrity poll, class poll, unsolicited, select-operate, dan time sync.
