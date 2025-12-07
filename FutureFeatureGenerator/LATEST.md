## 1.7.0
### Features
- add
```
System
    Collections.Generic
        KeyValuePair
    Math
        Tau
    MathF
        Tau
    Runtime
        CompilerServices
            RawArrayData
            RawData
        InteropServices
            MemoryMarshal
                GetArrayDataReference(T[])
    TimeSpan
        HoursPerDay
        MicrosecondsPerDay
        MicrosecondsPerHour
        MicrosecondsPerMillisecond
        MicrosecondsPerMinute
        MicrosecondsPerSecond
        MillisecondsPerDay
        MillisecondsPerHour
        MillisecondsPerMinute
        MillisecondsPerSecond
        MinutesPerDay
        MinutesPerHour
        NanosecondsPerTick
        SecondsPerDay
        SecondsPerHour
        SecondsPerMinute
        TicksPerMicrosecond
```
- change
```
;old
System.Collections.Generic
    KeyValuePair
        Deconstruct()
;new
System.Collections.Generic
    KeyValuePair`2
        Deconstruct()
```
### Bug Fixes
- ArgumentException occurred during the initial build of a multi-threaded environment