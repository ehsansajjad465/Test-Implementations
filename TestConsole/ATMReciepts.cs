using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public static class ATMReciepts
    {
        public static List<TextElement> ExtractATMReciept(List<TextAnnotation> textAnnotations)
        {
            var possibleKeys = new String[] { "MACHINE NO.", "MACHINE ID", "DATE - TIME", "CASSETTE", "REJECTED", "REMAINING", "DISPENSED", "TOTAL", "TYPE 1", "TYPE 2", "TYPE 3", "TYPE 4", "LAST CLEARED" };

            #region Date & Time Extraction
            var wholeTextArray = textAnnotations[0].description.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            TextElement dateElement = null;
            TextElement timeElement = null;
            foreach (var textItem in wholeTextArray)
            {
                if (textItem.ToUpper().Contains("DATE"))
                {
                    var splitedDateTime = textItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitedDateTime.Any() && splitedDateTime.Length > 2)
                    {
                        dateElement = new TextElement();
                        dateElement.Label = new LabelFormMap() { RecieptLabel = new RecieptLabel() { RowLabel = "DATE" } };// eLabels.DATE;
                        dateElement.Value = splitedDateTime[splitedDateTime.Length - 2];

                        timeElement = new TextElement();
                        timeElement.Label = new LabelFormMap() { RecieptLabel = new RecieptLabel() { RowLabel = "TIME" } };//eLabels.TIME;
                        timeElement.Value = splitedDateTime[splitedDateTime.Length - 1];

                        break;
                    }
                }
            }


            #endregion

            List<TextElement> elements = new List<TextElement>();

            if (dateElement != null)
                elements.Add(dateElement);
            if (timeElement != null)
                elements.Add(timeElement);


            int index = 1;
            while (index < textAnnotations.Count)
            {
                var textBlock = textAnnotations[index];



                var y1 = textBlock.boundingPoly.vertices[0].y;
                var y2 = textBlock.boundingPoly.vertices[2].y;


                var matchingY = textAnnotations.Where(textB =>
                                                        (textB.boundingPoly.vertices[0].y == y1 && textB.boundingPoly.vertices[2].y == y2) ||
                                                        (Math.Abs(textB.boundingPoly.vertices[0].y - y1) <= 25 && Math.Abs(textB.boundingPoly.vertices[2].y - y2) <= 25))
                                                .OrderBy(x => x.boundingPoly.vertices[0].x);
                ;
                if (String.Equals(textBlock.description, "Type", StringComparison.OrdinalIgnoreCase))
                {
                    index = index + matchingY.Count();
                }

                if (matchingY.Any())
                {
                    //var matchingWithIndex = matchingY.Select((ax, i) => new { Index = i, Label = ax.description.TryFromEnumStringValue<eLabels>(), TextAnnotation = ax });
                    var matchingWithIndex = matchingY.Select((ax, i) => new { Index = i, Label = ax.description.TryConvertToRecieptMetaData(null), TextAnnotation = ax });

                    TextElement element = new TextElement();

                    var itemsWithValue = matchingWithIndex.Where(x => x.Label != null);//eLabels.Unknown);

                    if (itemsWithValue.Any())
                    {


                        var labelItem = itemsWithValue.FirstOrDefault();
                        element.Label = labelItem?.Label;

                        if (element.Label == null)//eLabels.Unknown)
                        {
                            index = index + matchingY.Count();
                            continue;
                        }
                        //if (element.Label == eLabels.DATE)
                        if (String.Equals(element.Label.RecieptLabel.RowLabel,"DATE",StringComparison.OrdinalIgnoreCase))
                        {
                            index = index + matchingY.Count();
                            continue;
                        }


                        int skipIndex = labelItem.Index + 1;

                        if (matchingY.Count() <= 3)
                        {
                            element.Value = matchingY.LastOrDefault()?.description;
                        }
                        else
                        {
                            element.Value = matchingY.Skip(skipIndex).FirstOrDefault()?.description;
                        }

                        TextElement element2 = new TextElement();
                        if (matchingY.Count() > 3)
                        {
                            element2.Label = element.Label;
                            element2.Value = matchingY.Skip(skipIndex + 1).FirstOrDefault()?.description;
                        }
                        var currentMatchAdded = false;
                        if (!possibleKeys.Any(x => x.Contains(element.Value.ToUpper())))
                        {
                            elements.Add(element);
                            currentMatchAdded = true;
                        }
                        if (element2?.Value != null && !possibleKeys.Any(x => x.Contains(element2.Value.ToUpper())))
                        {
                            elements.Add(element2);
                            currentMatchAdded = true;
                        }

                        if (currentMatchAdded)
                        {
                            foreach (var processed in matchingY.ToList())
                                textAnnotations.Remove(processed);
                        }
                        else
                        {
                            index++;
                        }

                    }
                    else
                    {
                        index++;
                    }
                }

            }

            return elements;
        }

        public static List<TextElement> ExtractATMReciept(RecieptMetaData recieptMetaData,List<TextAnnotation> textAnnotations)
        {
            var possibleKeys = recieptMetaData.Attributes.Select(x => x.RecieptLabel.RowLabel);
                //new String[] { "MACHINE NO.", "MACHINE ID", "DATE - TIME", "CASSETTE", "REJECTED", "REMAINING", "DISPENSED", "TOTAL", "TYPE 1", "TYPE 2", "TYPE 3", "TYPE 4", "LAST CLEARED" };

            #region Date & Time Extraction
            var wholeTextArray = textAnnotations[0].description.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            TextElement dateElement = null;
            TextElement timeElement = null;
            foreach (var textItem in wholeTextArray)
            {
                if (textItem.ToUpper().Contains("DATE"))
                {
                    var splitedDateTime = textItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitedDateTime.Any() && splitedDateTime.Length > 2)
                    {
                        dateElement = new TextElement();
                        dateElement.Label = new LabelFormMap() { RecieptLabel = new RecieptLabel() { RowLabel = "DATE" } };//eLabels.DATE;
                        dateElement.Value = splitedDateTime[splitedDateTime.Length - 2];

                        timeElement = new TextElement();
                        timeElement.Label = new LabelFormMap() { RecieptLabel = new RecieptLabel() { RowLabel = "TIME" } };//eLabels.TIME;
                        timeElement.Value = splitedDateTime[splitedDateTime.Length - 1];

                        break;
                    }
                }
            }


            #endregion

            List<TextElement> elements = new List<TextElement>();

            if (dateElement != null)
                elements.Add(dateElement);
            if (timeElement != null)
                elements.Add(timeElement);


            int index = 1;
            while (index < textAnnotations.Count)
            {
                var textBlock = textAnnotations[index];



                var y1 = textBlock.boundingPoly.vertices[0].y;
                var y2 = textBlock.boundingPoly.vertices[2].y;


                var matchingY = textAnnotations.Where(textB =>
                                                        (textB.boundingPoly.vertices[0].y == y1 && textB.boundingPoly.vertices[2].y == y2) ||
                                                        (Math.Abs(textB.boundingPoly.vertices[0].y - y1) <= 25 && Math.Abs(textB.boundingPoly.vertices[2].y - y2) <= 25))
                                                .OrderBy(x => x.boundingPoly.vertices[0].x);
                ;
                if (String.Equals(textBlock.description, "Type", StringComparison.OrdinalIgnoreCase))
                {
                    index = index + matchingY.Count();
                }

                if (matchingY.Any())
                {
                    //var matchingWithIndex = matchingY.Select((ax, i) => new { Index = i, Label = ax.description.TryFromEnumStringValue<eLabels>(), TextAnnotation = ax });

                    var matchingWithIndex = matchingY.Select((ax, i) => new { Index = i, Label = ax.description.TryConvertToRecieptMetaData(recieptMetaData), TextAnnotation = ax });

                    TextElement element = new TextElement();

                    var itemsWithValue = matchingWithIndex.Where(x => x.Label != null); //!= eLabels.Unknown);

                    if (itemsWithValue.Any())
                    {


                        var labelItem = itemsWithValue.FirstOrDefault();
                        element.Label = labelItem?.Label;

                        if (element.Label == null)//eLabels.Unknown)
                        {
                            index = index + matchingY.Count();
                            continue;
                        }
                        if (String.Equals(element.Label.RecieptLabel.RowLabel,"Data",StringComparison.OrdinalIgnoreCase))
                        //if (element.Label == eLabels.DATE)
                        {
                            index = index + matchingY.Count();
                            continue;
                        }


                        int skipIndex = labelItem.Index + 1;

                        if (matchingY.Count() <= 3)
                        {
                            element.Value = matchingY.LastOrDefault()?.description;
                        }
                        else
                        {
                            element.Value = matchingY.Skip(skipIndex).FirstOrDefault()?.description;
                        }

                        TextElement element2 = new TextElement();
                        if (matchingY.Count() > 3)
                        {
                            element2.Label = element.Label;
                            element2.Value = matchingY.Skip(skipIndex + 1).FirstOrDefault()?.description;
                        }
                        var currentMatchAdded = false;
                        if (!possibleKeys.Any(x => x.Contains(element.Value.ToUpper())))
                        {
                            elements.Add(element);
                            currentMatchAdded = true;
                        }
                        if (element2?.Value != null && !possibleKeys.Any(x => x.Contains(element2.Value.ToUpper())))
                        {
                            elements.Add(element2);
                            currentMatchAdded = true;
                        }

                        if (currentMatchAdded)
                        {
                            foreach (var processed in matchingY.ToList())
                                textAnnotations.Remove(processed);
                        }
                        else
                        {
                            index++;
                        }

                    }
                    else
                    {
                        index++;
                    }
                }

            }

            return elements;
        }
    }
}
