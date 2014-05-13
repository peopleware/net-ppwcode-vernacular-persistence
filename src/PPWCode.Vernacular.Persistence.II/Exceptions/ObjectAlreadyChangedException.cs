using System;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Exceptions.II;

namespace PPWCode.Vernacular.Persistence.II.Exceptions
{
    public class ObjectAlreadyChangedException : SemanticException
    {
        public ObjectAlreadyChangedException()
        {
        }

        public ObjectAlreadyChangedException(string message)
            : base(message)
        {
        }

        public ObjectAlreadyChangedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ObjectAlreadyChangedException(object sender)
            : base(null, null)
        {
            Sender = sender;
        }

        protected ObjectAlreadyChangedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public object Sender
        {
            get { return Data["Sender"]; }
            private set { Data["Sender"] = value; }
        }

        public override bool Like(SemanticException other)
        {
            bool result = base.Like(other);

            ObjectAlreadyChangedException otherObjectAlreadyChangedException = other as ObjectAlreadyChangedException;
            return result
                   && otherObjectAlreadyChangedException != null
                   && Sender == otherObjectAlreadyChangedException.Sender;
        }

        public override string ToString()
        {
            return string.Format(@"Type: {0}; Sender={1}", GetType().Name, Sender);
        }
    }
}