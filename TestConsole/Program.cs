using System;
using System.Collections.Generic;
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
            if(input.Contains("="))
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
        static void Main(string[] args)
        {



            var ocrRequest = OCRHelper.BuildRequest(null);
            var temp = OCRHelper.GetOCRResult(ocrRequest).Result;







            var possibleKeys = new String[] { "MACHINE NO.", "DATE - TIME", "CASSETTE", "REJECTED", "REMAINING", "DISPENSED", "TOTAL", "TYPE 1", "TYPE 2", "TYPE 3", "TYPE 4", "LAST CLEARED" };

            string Document_Text_Detection = "ATM RECEIPT\nMACHINE NO. = AAA\nDATE - TIME = 15-May-17\n10:19\nCASSETTE\nREJECTED\nREMAINING\nDISPENSED\nTOTAL\nTYPE 1\n00100\n00000\n00045\n00055\n00100\nTYPE 2\n00200\n00000\n00050\n00150\n00200\nCASSETTE\nREJECTED\nREMAINING\nDISPENSED\nTOTAL\nTYPE 3 TYPE 4\n00300 00400\n00000 00000\n00040 00050\n00260 00350\n0030000400\nLAST CLEARED\n10-May-17\n10:54\n";

            string OCRText = Document_Text_Detection;
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
                foreach(var x in temp2)
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
