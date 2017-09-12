using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Wikiled.SmartDoc.Logic.Results
{
    [ProtoContract]
    public class DocumentDefinition
    {
        [ProtoMember(1)]
        public Dictionary<string, int> WordsTable { get; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        [ProtoMember(2)]
        public string[] Labels { get; set; }

        [ProtoMember(3)]
        public uint Crc32 { get; set; }

        [ProtoMember(4)]
        public string Path { get; set; }
    }
}
