using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.CustomExceptions
{
    public class ItemWithSuchNameAlreadyExists : Exception
    {
        public ItemWithSuchNameAlreadyExists()
        {
        }

        public ItemWithSuchNameAlreadyExists(string? message) : base(message)
        {
        }

        public ItemWithSuchNameAlreadyExists(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ItemWithSuchNameAlreadyExists(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
