using System;
using FrozenGold;

namespace FrozenGold.Web
{
    public class TransactionDto
    {
        public TransactionType Type { get; set; }
        public uint CopperAmount { get; set; }
        public string PlayerFrom { get; set; }
        public string PlayerTo { get; set; }
        public DateTimeOffset WhenServerTime { get; set; }
    }
}
