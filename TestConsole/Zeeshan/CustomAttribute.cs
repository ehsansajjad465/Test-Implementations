using System;

namespace TestConsole.Zeeshan
{
    public class CustomAttribute
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
    public  class SomeAttribute : Attribute
    {
        public SomeAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; set; }
    }
}