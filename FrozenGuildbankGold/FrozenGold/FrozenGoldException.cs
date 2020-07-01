using System;
using System.Runtime.Serialization;

namespace FrozenGold
{
    [Serializable]
    public class FrozenGoldException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public FrozenGoldException()
        {
        }

        public FrozenGoldException(string message) : base(message)
        {
        }

        public FrozenGoldException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FrozenGoldException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}