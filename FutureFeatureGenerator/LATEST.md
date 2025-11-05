## 1.6.0
### Features
- add new method
```
System
    ArraySegment
        CopyTo(T[])
        CopyTo(T[],int)
        CopyTo(ArraySegment<T>)
        Slice(int)
        Slice(int,int)
        ToArray()
    Math
        Clamp(byte,byte,byte)
        Clamp(decimal,decimal,decimal)
        Clamp(double,double,double)
        Clamp(short,short,short)
        Clamp(int,int,int)
        Clamp(long,long,long)
        Clamp(sbyte,sbyte,sbyte)
        Clamp(float,float,float)
        Clamp(ushort,ushort,ushort)
        Clamp(uint,uint,uint)
        Clamp(ulong,ulong,ulong)
        Clamp(nint,nint,nint)
        Clamp(nuint,nuint,nuint)
    String
        EndsWith(char) 
        IndexOf(char,StringComparison) 
        GetHashCode(StringComparison) 
        StartsWith(char) 
    StringComparer
        FromComparison(StringComparison)
```
- The effect of the UseRealCondition option has changed slightly
- `ObjectDisposedException.ThrowIf(Boolean,Object)` rename to `ObjectDisposedException.ThrowIf(bool,object)`
- `ObjectDisposedException.ThrowIf(Boolean,Type)` rename to `ObjectDisposedException.ThrowIf(bool,Type)`
### Bug Fixes
- correct namespace for GeneratedCodeAttribute in `System.Collections.Generic`