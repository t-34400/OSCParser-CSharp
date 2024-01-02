#nullable enable

using System.Linq;

namespace OSCPacket
{
    public partial struct Packet
    {
        public string address;
        public Argument[] arguments;

        public override string ToString()
        {
            return $"Address: {address}, Arguments: {string.Join(", ", arguments.Select(arg => arg.ToString()))}";
        }

        public enum ArgumentType
        {
            Unsupported = -1,

            // Standard argument types
            Int32,
            Float32,
            OSCString,
            OSCBlob,

            // Nonstandard argument types
            Boolean,

            // OSC format types
            True,
            False,
        }

        public struct Argument
        {
            public ArgumentType type;
            public object value;

            public override string ToString() =>  type switch
                    {
                        ArgumentType.Int32 => ((int) value).ToString(),
                        ArgumentType.Float32 => ((float) value).ToString(),
                        ArgumentType.OSCString => (string) value,
                        ArgumentType.OSCBlob => ((byte[]) value).ToString(),
                        ArgumentType.Boolean => ((bool) value).ToString(),
                        _ => ""
                    };
        }
    }
}
