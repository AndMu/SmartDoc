using System;
using System.Runtime.Serialization;

namespace Wikiled.SmartDoc.Logic.Learning
{
    [Serializable]
    public class LearningException : Exception
    {
        public LearningException()
        {
        }

        public LearningException(string message)
            : base(message)
        {
        }

        public LearningException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected LearningException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
