using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {

        public static string ExtractLabel(string input)
        {
            if (input.Contains("="))
            {
                var temp = input.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                return temp[0];
            }


            return input;
        }
        public static string ExtractValue(string input)
        {
            string equalSign = "=";
            var extractedValue = input;
            if (input.Contains(equalSign))
            {
                extractedValue = input.Split(new String[] { equalSign }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }

            return extractedValue;
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
            [Description("LAST CLEARED")]
            LASTCLEARED
        }

        public class TextElement
        {
            public eLabels? Label { get; set; }
            public eTypes? Type { get; set; }

            public String Value { get; set; }

            public TextAnnotation ExtractedValue { get; set; }

            //public IEnumerable<TextAnnotation> TextAnnotations { get; set; }
        }
        static void Main(string[] args)
        {

            var test = DateTime.ParseExact("16.07.19 04:31", "dd.MM.yy HH:mm", CultureInfo.InvariantCulture);

            Parking.ParkingLogic();

            Console.ReadKey();

            var possibleKeys = new String[] { "MACHINE NO.","MACHINE ID", "DATE - TIME", "CASSETTE", "REJECTED", "REMAINING", "DISPENSED", "TOTAL", "TYPE 1", "TYPE 2", "TYPE 3", "TYPE 4", "LAST CLEARED" };

            var ocrRequest = OCRHelper.BuildRequest(null);
            var temp = OCRHelper.GetOCRResult(ocrRequest).Result;

            var textAnnotations = temp.responses.FirstOrDefault().textAnnotations;

            #region Date & Time Extraction
            var wholeTextArray = textAnnotations[0].description.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            TextElement dateElement=null;
            TextElement timeElement=null;
            foreach (var textItem in wholeTextArray)
            {
                if(textItem.ToUpper().Contains("DATE"))
                {
                    var splitedDateTime = textItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitedDateTime.Any() && splitedDateTime.Length > 2)
                    {
                        dateElement = new TextElement();
                        dateElement.Label = eLabels.DATE;
                        dateElement.Value = splitedDateTime[splitedDateTime.Length - 2];

                        timeElement = new TextElement();
                        timeElement.Label = eLabels.TIME;
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
                if (String.Equals(textBlock.description, "Type", StringComparison.OrdinalIgnoreCase))
                {
                    index = index + matchingY.Count();
                }

                if (matchingY.Any())
                {
                    var matchingWithIndex = matchingY.Select((ax, i) => new { Index = i, Label = ax.description.TryFromEnumStringValue<eLabels>(), TextAnnotation = ax });

                    #region Date and Time extract

                   

                    /*if(matchingWithIndex.Any(x=> x.Label == eLabels.DATETIME))
                    {
                        string dateTimeLabelValue = String.Join(" ", matchingY.Select(x => x.description));
                    }*/

                    #endregion

                    TextElement element = new TextElement();

                    var itemsWithValue = matchingWithIndex.Where(x => x.Label != eLabels.Unknown);

                    if (itemsWithValue.Any())
                    {


                        var labelItem = itemsWithValue.FirstOrDefault();
                        element.Label = labelItem?.Label;
                        //element.Label = matchingY.FirstOrDefault()?.description?.TryFromEnumStringValue<eLabels>();

                        if (element.Label == eLabels.Unknown)
                        {
                            index = index + matchingY.Count();
                            continue;
                        }
                        if(element.Label == eLabels.DATE)
                        {
                            index = index + matchingY.Count();
                            continue;
                        }
                        

                        int skipIndex = labelItem.Index+ 1;

                        if (matchingY.Count() <= 3)
                        {
                            element.Value = matchingY.LastOrDefault()?.description;
                        }
                        else
                        {
                            //element.Type = eTypes.Type1;
                            element.Value = matchingY.Skip(skipIndex).FirstOrDefault()?.description;
                            //element.TextAnnotations = matchingY;
                        }

                        TextElement element2 = new TextElement();
                        if (matchingY.Count() > 3)
                        {
                            element2.Label = element.Label;
                            //element.Type = eTypes.Type1;
                            element2.Value = matchingY.Skip(skipIndex + 1).FirstOrDefault()?.description;
                        }
                        var currentMatchAdded = false;
                        if (!possibleKeys.Any(x => x.Contains(element.Value.ToUpper())))
                        {
                            elements.Add(element);
                            currentMatchAdded = true;
                        }
                        if (element2?.Value !=null && !possibleKeys.Any(x => x.Contains(element2.Value.ToUpper())))
                        {
                            elements.Add(element2);
                            currentMatchAdded = true;
                        }

                        //index = index + matchingY.Count();
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
                
                /*foreach (var vertex in textBlock.boundingPoly.vertices)


                {
                    var xSame = textAnnotations.GroupBy(x => x.boundingPoly.vertices.Where(v => v.y == vertex.y));
                }


                var groupPolys = textAnnotations
                                    .GroupBy(x => x.boundingPoly.vertices

                                                                                    .Where(y => textBlock.boundingPoly.vertices.Any(vextex => y.x == vextex.x && y.y - vextex.y < 20) ||
                                                                                                textBlock.boundingPoly.vertices.Any(vextex => y.y == vextex.y || y.x - vextex.x < 20)).Count() > 0
                                            );*/
                //var groupPolys = textAnnotations
                //                    .GroupBy(x => x.boundingPoly.vertices
                //                                            .Any(vextex => textBlock.boundingPoly.vertices
                //                                                                    .Where(y => (y.x == vextex.x || y.x - vextex.x < 20) ||
                //                                                                                (y.y == vextex.y || y.y - vextex.y < 20) ).Count() > 0)
                //                            );



                var checking = elements.Distinct();


            }





            string Document_Text_Detection = "ATM RECEIPT\nMACHINE NO. = AAA\nDATE - TIME = 15-May-17\n10:19\nCASSETTE\nREJECTED\nREMAINING\nDISPENSED\nTOTAL\nTYPE 1\n00100\n00000\n00045\n00055\n00100\nTYPE 2\n00200\n00000\n00050\n00150\n00200\nCASSETTE\nREJECTED\nREMAINING\nDISPENSED\nTOTAL\nTYPE 3 TYPE 4\n00300 00400\n00000 00000\n00040 00050\n00260 00350\n0030000400\nLAST CLEARED\n10-May-17\n10:54\n";

            string OCRText = "Document_Text_Detection";
            var temp1 = OCRText.Split(new string[] { "\n" }, StringSplitOptions.None);

            Dictionary<CompositeKey, string> values = new Dictionary<CompositeKey, string>();

            foreach (var item in temp1)
            {
                string label = "";
                if (possibleKeys.Any(x => item.IndexOf(x) >= 0))
                {
                    label = ExtractLabel(item);
                }

                string extractedValue = ExtractValue(item);

                //CompositeKey key = new CompositeKey(label,)

                Console.WriteLine("Original: " + item);



                //Console.WriteLine("Extracted: "+ExtractValue(item));
            }

            Console.ReadKey();

            using (var disposeTest = new TestDisposable())
            {

            }


            ExpressionExtensions.ToPostfixString(x => Math.Sin(1 + 2 * x));

            int n = 1;
            int a = 0;
            String m;
            for (string m2; n > 0;)
            {
                var temp2 = Enumerable.Range(0, ++a);
                foreach (var x in temp2)
                {
                    var temp3 = (m = x + "" + (a - x) + x);
                    var revers = temp3.Reverse();
                }
            }
            if (Enumerable.Range(0, ++a).All(x => !(m = x + "" + (a - x) + x).Reverse().SequenceEqual(m)))
                n--;
            Console.WriteLine(a);
        }


    }

    public class CompositeKey
    {
        public string RowLabel { get; set; }
        public string ColumnLabel { get; set; }

        public CompositeKey(string rowLabel, string columnLabel)
        {
            RowLabel = rowLabel;
            ColumnLabel = columnLabel;
        }

        public override int GetHashCode()
        {
            return RowLabel.GetHashCode() ^ ColumnLabel.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is CompositeKey)
            {
                CompositeKey compositeKey = (CompositeKey)obj;

                return ((this.RowLabel == compositeKey.RowLabel) &&
                        (this.ColumnLabel == compositeKey.ColumnLabel));
            }

            return false;
        }
    }
    public interface IInterface
    {
        void Method();
    }

    public abstract class AbstractImplementation : IInterface
    {
        //public void Method()
        //{
        //    throw new NotImplementedException();
        //}

        public abstract void Method();
        void IInterface.Method()
        {

        }
    }

    abstract class AbstractB : AbstractImplementation
    {
    }

    class ChildImplementation : AbstractImplementation, IInterface
    {


        public override void Method()
        {
            throw new NotImplementedException();
        }
    }

    public static class ExpressionExtensions
    {
        public static string ToPostfixString(this Expression<Func<double, double>> function)
        {
            var visitor = new ToPostFixStringVisitor();

            visitor.Visit(function);

            return visitor.Result;
        }

        public abstract class ToStringVisitor : ExpressionVisitor
        {
            protected readonly StringBuilder resultAccumulator = new StringBuilder();

            public string Result
            {
                get { return resultAccumulator.ToString(); }
            }
        }

        private class ToPostFixStringVisitor : ToStringVisitor
        {
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                // enables reusing the visitor – not absolutely required here as the only 
                // place where an instance can be created is in the ToPostfixString method.
                this.resultAccumulator.Clear();

                foreach (var parameter in node.Parameters)
                {
                    this.Visit(parameter);
                }

                this.resultAccumulator.Append("-> ");

                this.Visit(node.Body);

                return node;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                this.resultAccumulator.Append(node.Name);
                this.resultAccumulator.Append(' ');

                return node;
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                this.Visit(node.Left);
                this.Visit(node.Right);

                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        this.resultAccumulator.Append('+');
                        break;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        this.resultAccumulator.Append('*');
                        break;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        this.resultAccumulator.Append('-');
                        break;
                    case ExpressionType.Divide:
                        this.resultAccumulator.Append('/');
                        break;
                    case ExpressionType.Modulo:
                        this.resultAccumulator.Append('%');
                        break;
                    default:
                        throw new NotSupportedException();
                }

                this.resultAccumulator.Append(' ');

                return node;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                foreach (var arg in node.Arguments)
                {
                    this.Visit(arg);
                }

                this.resultAccumulator.Append(node.Method.Name);
                this.resultAccumulator.Append(' ');

                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                this.resultAccumulator.Append(node.Value);

                this.resultAccumulator.Append(' ');

                return node;
            }
        }
    }
}
