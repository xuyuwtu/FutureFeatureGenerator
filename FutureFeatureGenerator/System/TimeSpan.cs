#region
#endregion
namespace System;

internal static partial class FutureTimeSpan
{
    #region HoursPerDay
#if !NET9_0_OR_GREATER
    internal const int HoursPerDay = 24;
#endif
    #endregion

    #region MicrosecondsPerDay
#if !NET9_0_OR_GREATER
    internal const long MicrosecondsPerDay = 86400000000;
#endif
    #endregion

    #region MicrosecondsPerHour
#if !NET9_0_OR_GREATER
    internal const long MicrosecondsPerHour = 3600000000;
#endif
    #endregion

    #region MicrosecondsPerMillisecond
#if !NET9_0_OR_GREATER
    internal const long MicrosecondsPerMillisecond = 1000;
#endif
    #endregion

    #region MicrosecondsPerMinute 
#if !NET9_0_OR_GREATER
    internal const long MicrosecondsPerMinute = 60000000;
#endif
    #endregion

    #region MicrosecondsPerSecond 
#if !NET9_0_OR_GREATER
    internal const long MicrosecondsPerSecond = 1000000;
#endif
    #endregion

    #region MillisecondsPerDay 
#if !NET9_0_OR_GREATER
    internal const long MillisecondsPerDay = 86400000;
#endif
    #endregion

    #region MillisecondsPerHour 
#if !NET9_0_OR_GREATER
    internal const long MillisecondsPerHour = 3600000;
#endif
    #endregion

    #region MillisecondsPerMinute 
#if !NET9_0_OR_GREATER
    internal const long MillisecondsPerMinute = 60000;
#endif
    #endregion

    #region MillisecondsPerSecond 
#if !NET9_0_OR_GREATER
    internal const long MillisecondsPerSecond = 1000;
#endif
    #endregion

    #region MinutesPerDay 
#if !NET9_0_OR_GREATER
    internal const long MinutesPerDay = 1440;
#endif
    #endregion

    #region MinutesPerHour 
#if !NET9_0_OR_GREATER
    internal const long MinutesPerHour = 60;
#endif
    #endregion

    #region NanosecondsPerTick
#if !NET7_0_OR_GREATER
    internal const long NanosecondsPerTick = 100;
#endif
    #endregion

    #region SecondsPerDay 
#if !NET9_0_OR_GREATER
    internal const long SecondsPerDay = 86400;
#endif
    #endregion

    #region SecondsPerHour 
#if !NET9_0_OR_GREATER
    internal const long SecondsPerHour = 3600;
#endif
    #endregion

    #region SecondsPerMinute 
#if !NET9_0_OR_GREATER
    internal const long SecondsPerMinute = 60;
#endif
    #endregion

    #region TicksPerMicrosecond
#if !NET7_0_OR_GREATER
    internal const long TicksPerMicrosecond = 10;
#endif
    #endregion
}
