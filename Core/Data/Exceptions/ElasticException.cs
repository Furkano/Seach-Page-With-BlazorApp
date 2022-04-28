using System.Runtime.Serialization;

namespace Core.Data.Exceptions
{
    [Serializable]
    public class ElasticException : Exception
    {
        public ElasticException()
        {
        }
        public ElasticException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }
        public ElasticException(string message) : base(message)
        {
        }
        public ElasticException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
