# OSCParser-CSharp
A C# parser for OSC (Open Sound Control) binary data. A utility to parse OSC messages and extract address patterns, type tags, arguments, and more.

## Prerequisites
Refer to the official [OpenSoundControl Specification 1.0](https://opensoundcontrol.stanford.edu/spec-1_0.html) for detailed OSC specifications.

## Environment
C# 8 or later (If using an earlier version, rewrite the switch expressions)

## Supported Type Formats
- Standard Formats
  - int32
  - float32
  - OSC-string
  - OSC-blob
- Non-Standard Formats
  - True
  - False

## Usage
- Call `Packet.TryParseOSCPacket()` with the received byte array as an argument. If the parsing is successful, `true` is returned, and the parsed `Packet` data is stored in the second argument.
  - The Packet's address contains the address pattern.
  - The Packet's arguments contain the OSC arguments.
    - Since the value of each argument is upcasted to `object`, cast down by checking the `type` field or using the `is` keyword.
    - Refer to the [Packet.Argument](./OSCPacket.cs) class's `ToString()` method for supported types.
```csharp
using OSCPacket;

// ...

byte[] bytes = // Assign the received byte array.
if (Packet.TryParseOSCPacket(bytes, out Packet oscPacket))
{
    string address = oscPacket.address;

    Argument firstArgument = oscPacket.arguments[0];
    if (firstArgument.value is int i)
    {
        // ...
    }

    Argument secondArgument = oscPacket.arguments[1];
    if (secondArgument.type == ArgumentType.Float32)
    {
        float value = (float) secondArgument.value;
        // ...
    }
}
```

## License
[MIT License](./LICENSE)
