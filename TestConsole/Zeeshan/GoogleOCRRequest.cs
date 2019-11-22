using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TestConsole.Zeeshan
{
    public class GoogleOCRRequest
    {
        public List<Request> requests { get; set; }
    }
    public class Image
    {
        public string content { get; set; }
    }
    public class Request
    {
        public List<Feature> features { get; set; }
        public Image image { get; set; }
        public ImageContext imageContext { get; set; }
    }
    public class ImageContext
    {
        public CropHintsParams cropHintsParams { get; set; }
    }
    public class Feature
    {
        public int maxResults { get; set; }
        public string type { get; set; }
    }
    public class CropHintsParams
    {
        public List<double> aspectRatios { get; set; }
    }
    public class ParkingData
    {
        public string MachineID { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string SequenceNo { get; set; }

        public List<ParkingElement> RecieptDetails { get; set; }
    }
    public class ParkingElement
    {
        public ParkingLabels? Label { get; set; }

        public String Value { get; set; }
    }
    public enum ParkingLabels
    {
        [Description("DISPENSED")]
        Dispensed,
        [Description("CASSETTE")]
        Cassette,
        [Description("REJECTED")]
        Rejected,
        [Description("REMAINING")]
        Remaining,
        [Description("TOTAL")]
        Total,
        [Description("LAST CLEARED")]
        LastCleared,
        [Description("Unknown")]
        Unknown,
        [Description("Denomination")]
        Denomination,
        [Description("Peices")]
        Peices,
        [Description("Amount")]
        Amount,
    }
}