﻿namespace BlockSocNetwork
{
    public class HostIPAddress
    {
        public string Name { get; set; }

        public HostIPAddress(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
