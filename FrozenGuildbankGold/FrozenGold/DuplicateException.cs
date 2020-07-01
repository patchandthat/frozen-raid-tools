using System;
using System.Runtime.Serialization;

namespace FrozenGold
{
    [Serializable]
    public class DuplicateException : FrozenGoldException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DuplicateException()
        {
        }

        public DuplicateException(string message) : base(message)
        {
        }

        public DuplicateException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DuplicateException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}