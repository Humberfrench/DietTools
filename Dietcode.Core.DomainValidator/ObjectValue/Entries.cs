﻿namespace Dietcode.Core.DomainValidator.ObjectValue
{
    public class Entries
    {
        public Entries()
        {
            EntryName = string.Empty;
            EntryKeyValue = string.Empty;
        }

        public string EntryName { get; set; }
        public object EntryKeyValue { get; set; }
    }
}
