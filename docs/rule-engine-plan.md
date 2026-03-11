<!-- ANCHOR: RULE_ENGINE_PLAN_START -->
# Rule Engine Plan

## Objective

Rule engine dipakai untuk mengubah hasil decode menjadi insight yang bernilai:
- warning
- error
- pass condition
- scenario verdict

## Rule Categories

### Baseline Protocol Rules
- frame malformed
- CRC invalid
- response unexpected
- unsolicited not enabled but observed
- missing confirm/ack when expected

### Data Quality Rules
- point quality invalid
- stale timestamp
- invalid flag combination
- counter rollback

### Command Lifecycle Rules
- select sent without operate
- operate without prior select
- timeout between select dan operate
- missing command response

### Polling Rules
- class poll missing response
- integrity poll incomplete
- event backlog not drained

### Time Sync Rules
- time sync command missing confirmation
- excessive skew
- stale time object

## Engine Design

Disarankan ada 3 lapis:

1. `Protocol Facts`
- frame list
- point updates
- command lifecycle facts
- indication flags

2. `Rules`
- evaluasi berbasis facts
- tidak bergantung pada UI

3. `Presentation Projection`
- hasil rule diubah ke `ValidationIssue`
- ditampilkan ke event log dan status history

## Suggested Interfaces

- `IProtocolRuleSet`
- future: `IRuleFactBuilder`
- future: `IScenarioEvaluator`
- future: `IVerdictAggregator`

## Scenario Evaluation Model

Setiap scenario minimal punya:
- precondition
- observation window
- expected sequence
- fail condition
- pass condition

## Initial Scenario Pack

- `Integrity Poll`
- `Class Poll`
- `Unsolicited`
- `Select Operate`
- `Time Sync`
- `Restart`

## Recommended Output Shape

Setiap hasil rule idealnya punya:
- `Severity`
- `Code`
- `Title`
- `Detail`
- `FrameReference`
- `PointReference`
- `ScenarioName`

Ini akan memudahkan nanti kalau ingin export report yang rapi.
<!-- ANCHOR: RULE_ENGINE_PLAN_END -->
