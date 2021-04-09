# CastIntoGenerator

A simple tool made to implement an in house pattern for casting c# classes into derived or otherwise compatible versions of themselfs

```csharp
public static T CastInto<T>(this TYPE input, T output) where T : TYPE
        {
            BASETYPE.CastInto(input, output); // (Only in case TYPE dervies from BASETYPE, use the direct parent of the type and fill the chain accordingly to the top)

            output.PROPERTY1 = input.PROPERTY1; // (Only the properties specific to this TYPE, not ones in base types)
            output.PROPERTY2 = input.PROPERTY2;
            output.PROPERTY3 = input.PROPERTY3;
            output.PROPERTY4 = input.PROPERTY4;
            // ....

            return output;
        }
```

TYPE can be either an interface or a class, depending on what you have implemented.

usage

```csharp
SampleBaseClass instanceOfBaseClass; // let's say we have an instance of SampleBaseClass with some properties

// this creates an instance of SampleDerivedClass with all the properties from SampleBaseClass 
SampleDerivedClass newClass = instanceOfBaseClass.CastInto(new SampleDerivedClass());

// this instead creates a copy of instanceOfBaseClass
SampleBaseClass instanceOfBaseClass2 = instanceOfBaseClass.CastInto(new SampleBaseClass());
```

the tool works by creating the assignment lines from a selection of the property lines in the class or interface declaration. (by Copy/Pasting the source code)
only public properties are considered.

* select the properties in your type declaration
* ctrl+c 
* use the tool
* ctrl+v where you want to keep the extension method 
