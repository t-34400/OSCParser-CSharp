#nullable enable

using System;
using System.Linq;
using System.Text;

namespace OSCPacket
{
    public partial struct Packet
    {
        private const int INT32_SIZE = 4;
        private const int FLOAT32_SIZE = 4;

        public static bool TryParseOSCPacket(byte[] bytes, out Packet oscPacket)
        {
            if(TryParseAddress(bytes, out var address, out var nextIndex)
                && TryParseType(bytes, nextIndex, out var types, out nextIndex)
                && TryParseValue(bytes, nextIndex, types, out var arguments))
            {
                oscPacket = new Packet() { address=address, arguments=arguments };
                return true;
            }

            oscPacket = default;
            return false;
        }

        private static bool TryParseAddress(byte[] bytes, out string address, out int nextIndex)
        {
            address = "";
            nextIndex = -1;

            var nullIndex = Array.IndexOf<byte>(bytes, 0);
            if (nullIndex < 0)
            {
                return false;
            }

            try
            {
                address = Encoding.ASCII.GetString(bytes, 0, nullIndex);
                nextIndex = (nullIndex + 4) & ~3;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryParseType(byte[] bytes, int startIndex, out ArgumentType[] types, out int nextIndex)
        {
            types = new ArgumentType[0];
            nextIndex = -1;

            var nullIndex = Array.IndexOf<byte>(bytes, 0, startIndex);
            if (nullIndex < 0)
            {
                return false;
            }

            try
            {
                var length = nullIndex - startIndex - 1;
                var typeString = Encoding.ASCII.GetString(bytes, startIndex + 1, length); // An OSC Type Tag String is an OSC-string beginning with the character ‘,’.
                types = typeString.Select(typeChar =>
                        typeChar switch
                        {
                            'i' => ArgumentType.Int32,
                            'f' => ArgumentType.Float32,
                            's' => ArgumentType.OSCString,
                            'b' => ArgumentType.OSCBlob,
                            'T' => ArgumentType.True,
                            'F' => ArgumentType.False,
                            _ => ArgumentType.Unsupported
                        }
                    ).ToArray();

                nextIndex = (nullIndex + 4) & ~3;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryParseValue(byte[] bytes, int startIndex, ArgumentType[] types, out Argument[] arguments)
        {
            arguments = new Argument[0];

            if(types.Contains(ArgumentType.Unsupported))
            {
                return false;
            }

            var currentIndex = startIndex;
            try
            {
                arguments = types.Select(type =>
                    {
                        switch(type)
                        {
                            case ArgumentType.Int32:
                                {
                                    var value = BitConverter.ToInt32(GetNumberByteArray(bytes, currentIndex, INT32_SIZE));
                                    currentIndex += INT32_SIZE;

                                    return new Argument() { type=type, value=value };
                                }
                            case ArgumentType.Float32:
                                {
                                    var value = BitConverter.ToSingle(GetNumberByteArray(bytes, currentIndex, FLOAT32_SIZE));
                                    currentIndex += FLOAT32_SIZE;

                                    return new Argument() { type=type, value=value };
                                }
                            case ArgumentType.OSCString:
                                {
                                    var nullIndex = Array.IndexOf<byte>(bytes, 0, currentIndex);
                                    if(nullIndex < 0)
                                    {
                                        return new Argument() { type=type, value="" };
                                    }

                                    var length = nullIndex - currentIndex;
                                    var value = Encoding.ASCII.GetString(bytes, currentIndex, length);

                                    currentIndex = (nullIndex + 4) & ~3;

                                    return new Argument() { type = type, value = value };
                                }
                            case ArgumentType.OSCBlob:
                                {
                                    var valueSize = BitConverter.ToInt32(GetNumberByteArray(bytes, currentIndex, INT32_SIZE));

                                    var value = new byte[valueSize];
                                    Array.Copy(bytes, currentIndex + INT32_SIZE, value, 0, valueSize);

                                    currentIndex = (currentIndex + INT32_SIZE + valueSize + 4) & ~3;

                                    return new Argument() { type = type, value = value };
                                }
                            case ArgumentType.True:
                                {
                                    return new Argument() { type = ArgumentType.Boolean, value = true };
                                }
                            case ArgumentType.False:
                                {
                                    return new Argument() { type = ArgumentType.Boolean, value = false };
                                }
                            default:
                                {
                                    return new Argument() { type=ArgumentType.Unsupported, value="" };
                                }
                        }
                    })
                    .ToArray();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GetNumberByteArray(byte[] source, int startIndex, int size)
        {
            var valueBytes = new byte[size];
            Array.Copy(source, startIndex, valueBytes, 0, size);

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }

            return valueBytes;
        }
    }
}
