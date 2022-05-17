using System.Runtime.Serialization;

namespace ChatApp.BLL.CustomExceptions
{
    public class UserDoesntExistsException : Exception
    {
        public UserDoesntExistsException()
        {
        }

        public UserDoesntExistsException(string message) : base(message)
        {
        }

        public UserDoesntExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserDoesntExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
