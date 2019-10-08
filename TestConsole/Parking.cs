using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestConsole
{
    public enum ParkingLabels
    {
        [Description("Unknown")]
        Unknown,
        [Description("EUR")]
        Denomination,
        [Description("CNT")]
        Count,
        [Description("TOTAL")]
        Total
    }

    public enum ParkingPhysicalCashLabels
    {
        [Description("Unknown")]
        Unknown,
        [Description("MACHINEID:")]
        MachineID,
        [Description("DATE:")]
        Date,
        [Description("CUSTOMER:")]
        Customer,
        [Description("TIME:")]
        Time,
        [Description("LOCATION:")]
        Location,
        [Description("BRANCH:")]
        Branch,
        [Description("DENOMINATION20CENT")]
        Denomination20Cent,
        [Description("DENOMINATION50CENT")]
        Denomination50Cent,
        [Description("DENOMINATION1EURO")]
        Denomination1Euro,
        [Description("DENOMINATION2EURO")]
        Denomination2Euro,
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

    public class ParkingPhysicalCashData
    {
        public List<ParkingPhysicalCashElement> RecieptDetails { get; set; }
    }

    public class ParkingPhysicalCashElement
    {
        public ParkingPhysicalCashLabels? Label { get; set; }

        public String Value { get; set; }
    }
    public class Parking
    {

        public static void ParkingLogic()
        {
            var possibleKeys = new String[] { "MACHINE ID", "SEQUENCE NO", "EUR" };

            string dateRegex = @"([0-2][0-9]|(3)[0-1])(\.)(((0)[0-9])|((1)[0-2]))(\.)\d{2,4} ";
            string timeRegex = @"([01]\d|2[0-3]):([0-5]\d)";


            var ocrRequest = OCRHelper.BuildRequest(null);
            var temp = OCRHelper.GetOCRResult(ocrRequest).Result;

            var textAnnotations = temp.responses.FirstOrDefault().textAnnotations;

            ParkingData parkingData = new ParkingData();
            #region Date & Time Extraction
            var wholeTextArray = textAnnotations[0].description.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);


            foreach (var item in wholeTextArray)
            {
                if (String.IsNullOrEmpty(parkingData.MachineID) && item.StartsWith("-"))
                    parkingData.MachineID = item.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                if (String.IsNullOrEmpty(parkingData.Date))
                    parkingData.Date = Regex.Match(item, dateRegex).Value;

                if (String.IsNullOrEmpty(parkingData.Time))
                    parkingData.Time = Regex.Match(item, timeRegex).Value;

                if (String.IsNullOrEmpty(parkingData.SequenceNo) && item.ToUpper().Contains("SEQUENCE NO"))
                    parkingData.SequenceNo = item.ToUpper().Split(new[] { "SEQUENCE NO" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Trim('.');

                if (!String.IsNullOrEmpty(parkingData.MachineID) &&
                    !String.IsNullOrEmpty(parkingData.SequenceNo) &&
                    !String.IsNullOrEmpty(parkingData.Date) &&
                    !String.IsNullOrEmpty(parkingData.Time)
                    )
                    break;
            }

            #endregion


            List<ParkingElement> parkingElements = new List<ParkingElement>();
            int index = 1;
            while (index < textAnnotations.Count)
            {
                var textBlock = textAnnotations[index];



                var y1 = textBlock.boundingPoly.vertices[0].y;
                var y2 = textBlock.boundingPoly.vertices[2].y;

                var x1 = textBlock.boundingPoly.vertices[1].x;
                var x2 = textBlock.boundingPoly.vertices[3].x;

                var matchingY = textAnnotations.Where(textB =>
                                                           (textB.boundingPoly.vertices[0].y == y1 && textB.boundingPoly.vertices[2].y == y2) ||
                                                           (Math.Abs(textB.boundingPoly.vertices[0].y - y1) <= 25 && Math.Abs(textB.boundingPoly.vertices[2].y - y2) <= 25))
                                                   .OrderBy(x => x.boundingPoly.vertices[0].x);

                var matchingX = textAnnotations.Where(textB =>
                                               (
                                                   (textB.boundingPoly.vertices[1].x == x1 && textB.boundingPoly.vertices[3].x == x2) ||
                                                   (Math.Abs(textB.boundingPoly.vertices[1].x - x1) <= 100 && Math.Abs(textB.boundingPoly.vertices[3].x - x2) <= 100)
                                               )
                                                && textB.boundingPoly.vertices[0].y > y1 && textB.boundingPoly.vertices[2].y > y2)
                                       .OrderBy(x => x.boundingPoly.vertices[1].y);

                if (matchingX.Any())
                {
                    var cleaned = matchingX.Where(x => x.description != "-");

                    var matchingWithIndex = cleaned.Select((ax, i) => new { Index = i, Label = textBlock.description.TryFromEnumStringValue<ParkingLabels>(), TextAnnotation = ax });
                    var finalResult = matchingWithIndex.Where(x => x.Label != ParkingLabels.Unknown);

                    if (finalResult.Any())
                    {
                        int skipIndex = 0;
                        while (skipIndex < 4)
                        {

                            var item = finalResult.Skip(skipIndex).FirstOrDefault();

                            //foreach (var item in matchingWithIndex)
                            //{
                            if (item != null)
                            {
                                ParkingElement parking = new ParkingElement();
                                parking.Label = item.Label;

                                if (item.Label == ParkingLabels.Count)
                                {

                                    parking.Value = item.TextAnnotation.description;
                                }
                                else
                                {

                                    parking.Value = String.Join("", finalResult.Skip(skipIndex * 3).Take(3).OrderBy(x => x.TextAnnotation.boundingPoly.vertices[0].x).Select(x => x.TextAnnotation.description));
                                }

                                parkingElements.Add(parking);

                            }

                            skipIndex++;
                            //}
                        }
                    }
                }

                index++;
            }

            parkingData.RecieptDetails = parkingElements;





            #region Denomination


            //var itemWithIndex = wholeTextArray.Select((x, i) => new { Index = i, Value = x });
            //var seperatorLine = itemWithIndex.Where(x => x.Value.Contains("------")).FirstOrDefault();

            //var denomination1 = itemWithIndex.Skip(seperatorLine.Index + 1).FirstOrDefault();
            //var denomination2 = itemWithIndex.Skip(seperatorLine.Index + 2).FirstOrDefault();
            //var denomination3 = itemWithIndex.Skip(seperatorLine.Index + 3).FirstOrDefault();
            //var denomination4 = itemWithIndex.Skip(seperatorLine.Index + 4).FirstOrDefault();

            //var count1 = itemWithIndex.Skip(seperatorLine.Index + 5).FirstOrDefault();
            //var count2 = itemWithIndex.Skip(seperatorLine.Index + 6).FirstOrDefault();
            //var count3 = itemWithIndex.Skip(seperatorLine.Index + 7).FirstOrDefault();
            //var count4 = itemWithIndex.Skip(seperatorLine.Index + 8).FirstOrDefault();


            ////int index = 1;
            ////while (index < textAnnotations.Count)
            ////{
            ////    var textBlock = textAnnotations[index];
            ////    textBlock = textAnnotations.Where(x => x.description.ToUpper().Contains("EUR")).LastOrDefault();
            ////    if(textBlock.description.ToUpper().Contains("EUR"))
            ////    {
            ////        var x1 = textBlock.boundingPoly.vertices[1].x;
            ////        var x2 = textBlock.boundingPoly.vertices[2].x;

            ////        var y1 = textBlock.boundingPoly.vertices[2].y;
            ////        var y2 = textBlock.boundingPoly.vertices[3].y;

            ////        var matchingX = textAnnotations.Skip(1).Where(textB =>
            ////                                            ((textB.boundingPoly.vertices[1].x == x1 && textB.boundingPoly.vertices[2].x == x2) ||
            ////                                              //(textB.boundingPoly.vertices[0].y < y1 && textB.boundingPoly.vertices[1].y < y2))
            ////                                            (x1 - Math.Abs(textBlock.boundingPoly.vertices[1].x) <= 5 && Math.Abs(x2 - textBlock.boundingPoly.vertices[2].x) <= 5)) &&
            ////                                            (y1 < textB.boundingPoly.vertices[2].y && y2 < textB.boundingPoly.vertices[3].y)
            ////                                            );
            ////                                    //.OrderBy(x => x.boundingPoly.vertices[0].y);


            ////    }

            ////    index++;
            ////}

            #endregion
        }

        public static void ParkingPhysicalCashLogic()
         {

            var possibleKeys = new String[] { "MACHINE ID", "SEQUENCE NO", "EUR" };

            string dateRegex = @"([0-2][0-9]|(3)[0-1])(\.)(((0)[0-9])|((1)[0-2]))(\.)\d{2,4} ";
            string timeRegex = @"([01]\d|2[0-3]):([0-5]\d)";


            var ocrRequest = OCRHelper.BuildRequest(null);
            var temp = OCRHelper.GetOCRResult(ocrRequest).Result;

            var textAnnotations = temp.responses.FirstOrDefault().textAnnotations;

            ParkingPhysicalCashData physicalCashData = new ParkingPhysicalCashData();



            List<ParkingPhysicalCashElement> parkingElements = new List<ParkingPhysicalCashElement>();
            int index = 1;
            while (index < textAnnotations.Count)
            {
                var textBlock = textAnnotations[index];



                var y1 = textBlock.boundingPoly.vertices[0].y;
                var y2 = textBlock.boundingPoly.vertices[2].y;

                var x1 = textBlock.boundingPoly.vertices[1].x;
                var x2 = textBlock.boundingPoly.vertices[3].x;

                var matchingY = textAnnotations.Where(textB =>
                                                           (textB.boundingPoly.vertices[0].y == y1 && textB.boundingPoly.vertices[2].y == y2) ||
                                                           (Math.Abs(textB.boundingPoly.vertices[0].y - y1) <= 25 && Math.Abs(textB.boundingPoly.vertices[2].y - y2) <= 25))
                                                   .OrderBy(x => x.boundingPoly.vertices[0].x);

                var matchingX = textAnnotations.Where(textB =>
                                               (
                                                   (textB.boundingPoly.vertices[1].x == x1 && textB.boundingPoly.vertices[3].x == x2) ||
                                                   (Math.Abs(textB.boundingPoly.vertices[1].x - x1) <= 100 && Math.Abs(textB.boundingPoly.vertices[3].x - x2) <= 100)
                                               )
                                                && textB.boundingPoly.vertices[0].y > y1 && textB.boundingPoly.vertices[2].y > y2)
                                       .OrderBy(x => x.boundingPoly.vertices[1].y);

                //var labelsToFind = new String[] { "MACHINEID:", "Date:" };

                //var labelsToFind = typeof(ParkingPhysicalCashLabels)
                //    .GetFields()
                //    .Select(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false).First())
                //        .Cast<DescriptionAttribute>()
                //        .Where(x => String.Equals(x.Description, ParkingPhysicalCashLabels.Unknown.ToString(),
                //            StringComparison.OrdinalIgnoreCase))
                //        .Select(x => x.Description).ToArray();

                var labelsToFind = ParkingPhysicalCashLabels.Time.ToList();

                if (matchingY.Any())
                {
                    var line = String.Join("", matchingY.Select(x => x.description));
                    

                    var labelsFound = labelsToFind.Where(x => line.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0)
                        .Select(x =>  new { Label = x, Index = line.IndexOf(x, StringComparison.OrdinalIgnoreCase)}).ToArray();


                    for (int i=0; i < labelsFound.Length; i++)
                    {
                        var labelEnum = labelsFound[i].Label.TryFromEnumStringValue<ParkingPhysicalCashLabels>();
                        var labelValue = line.Substring(labelsFound[i].Index + labelsFound[i].Label.Length,
                            (i < labelsFound.Length - 1) ? (labelsFound[i + 1].Index - labelsFound[i].Label.Length) : line.Length - (labelsFound[i].Index + labelsFound[i].Label.Length));

                        if (labelEnum != ParkingPhysicalCashLabels.Unknown)
                        {
                            ParkingPhysicalCashElement parking = new ParkingPhysicalCashElement();
                            parking.Label = labelEnum;
                            parking.Value = labelValue;
                            if(parkingElements.All(x => x.Label != parking.Label))
                                parkingElements.Add(parking);
                        }
                    }

                    

                }

                index++;

                //}
            }
            physicalCashData.RecieptDetails = parkingElements;

        }




    }
}

