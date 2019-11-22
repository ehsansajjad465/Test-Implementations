using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TestConsole.Zeeshan
{
    public class TextElement1
    {
        public eLabels? Label { get; set; }
        public eTypes? Type { get; set; }

        public String Value { get; set; }

        public ATMCounter.TextAnnotation ExtractedValue { get; set; }

    }
    public class TextElement
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ColumnLabel { get; set; }

    }
    public class LabelsInfo
    {
        public string RowLable { get; set; }
        public string ColumnLable { get; set; }
    }
    public enum eLabels
    {
        Unknown,
        [Description("MACHINE")]
        MACHINENO,
        [Description("DATE")]
        DATE,
        [Description("TIME")]
        TIME,
        [Description("CASSETTE")]
        CASSETTE,
        [Description("REJECTED")]
        REJECTED,
        [Description("REMAINING")]
        REMAINING,
        [Description("DISPENSED")]
        DISPENSED,
        [Description("TOTAL")]
        TOTAL,
        [Description("GRAND")]
        GRANDTOTAL,


        [Description("DENOMINATION")]
        DENOMINATION,

        [Description("PURGE")]
        PURGE,
        [Description("LAST CLEARED")]
        LASTCLEARED,
        [Description("ENCASHED")]
        ENCASHED,
        [Description("CARDS CAPTURED")]
        CARDSCAPTURED,
        [Description("ACTIVITY COUNT")]
        ACTIVITYCOUNT
    }
    public struct abc
    {

    }
    public enum eTypes
    {
        Unknown,
        [Description("Type 1")]
        Type1,
        [Description("Type 2")]
        Type2,
        [Description("Type 3")]
        Type3,
        [Description("Type 4")]
        Type4
    }
    public class TextAnnotation1
    {
        public string locale { get; set; }
        public string description { get; set; }
        public BoundingPoly boundingPoly { get; set; }
    }
    public class Vertex
    {
        public int x { get; set; }
        public int y { get; set; }
    }
    public class BoundingPoly
    {
        public List<Vertex> vertices { get; set; }
    }

}