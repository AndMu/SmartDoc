using System;
using ProtoBuf;

namespace Wikiled.SmartDoc.Logic.Results
{
    [ProtoContract]
    public class DocumentSet
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public DocumentDefinition[] Document { get; set; }

        [ProtoMember(3)]
        public DateTime Created { get; set; }

        [ProtoMember(4)]
        public int TotalRequested { get; set; }
    }
}
