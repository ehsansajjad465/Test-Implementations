using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TestConsole;
using TestConsole.Factories;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            Parking.ParkingPhysicalCashLogic();

            var test = DateTime.ParseExact("16.07.19 04:31", "dd.MM.yy HH:mm", CultureInfo.InvariantCulture);

            var atmRecieptMetaData = new ATMRecieptMetaDataFactory().CreateMetaData();
            var ocrRequest = OCRHelper.BuildRequest(null);
            var temp = OCRHelper.GetOCRResult(ocrRequest).Result;

            var textAnnotations = temp.responses.FirstOrDefault()?.textAnnotations;
            var resultText = ATMReciepts.ExtractATMReciept(atmRecieptMetaData, textAnnotations);
            //var resultText = ATMReciepts.ExtractATMReciept(textAnnotations);

            Parking.ParkingLogic();

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
    }



    public class TextElement
    {
        public LabelFormMap Label { get; set; }
        //public eLabels? Label { get; set; }
        public eTypes? Type { get; set; }

        public String Value { get; set; }

        public TextAnnotation ExtractedValue { get; set; }

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



    public class CostsChargesBasePage
    {

    }

    public class FundCostPage : CostsChargesBasePage
    {

    }
    public class FundCostPageViewModel : CostsChargesViewModel<FundCostPage>
    {

    }
    public interface ICostsChargesViewModel<out T> where T : CostsChargesBasePage

    {

    }

    public abstract class CostsChargesViewModel<T> : ICostsChargesViewModel<T> where T : CostsChargesBasePage
    {
    }

    class Base
    {
        protected int Data { get; set; }
    }
    class SubClasss1 : Base
    {
        public SubClasss1(int Data)
        {
            this.Data = Data;
        }
    }
    class SubClasss2 : Base
    {
        public SubClasss1 MyFunction()
        {
            SubClasss1 copy = new SubClasss1(this.Data);

            return copy;
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
