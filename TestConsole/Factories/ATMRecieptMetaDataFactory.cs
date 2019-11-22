using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Factories
{
    public class ATMRecieptMetaDataFactory : ReceiptMetaDataFactory
    {
        public override RecieptMetaData CreateMetaData()
        {
            return new RecieptMetaData()
            {
                RecieptName = "ATMCounter",
                Attributes = new List<LabelFormMap>()
            {
                new LabelFormMap()
                {
                    FormLabel ="MachineID",
                    RecieptLabel = new RecieptLabel() { RowLabel="MACHINE",ColumnLabel = "", DataType="Number",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="CollectionDate",
                    RecieptLabel = new RecieptLabel() { RowLabel="DATE", ColumnLabel = "", DataType="DateTime",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="Time",
                    RecieptLabel = new RecieptLabel() { RowLabel="TIME", ColumnLabel = "", DataType="DateTime",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="Cassette1",
                    RecieptLabel = new RecieptLabel() { RowLabel="+Dispensed", ColumnLabel = "Type1", DataType="Number",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="Cassette2",
                    RecieptLabel = new RecieptLabel() { RowLabel="+Dispensed", ColumnLabel = "Type2", DataType="Number",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="Cassette3",
                    RecieptLabel = new RecieptLabel() { RowLabel="=Total", ColumnLabel = "Type3", DataType="Number",RegularExpression="" }
                },
                new LabelFormMap()
                {
                    FormLabel ="Cassette4",
                    RecieptLabel = new RecieptLabel() { RowLabel="=Total", ColumnLabel = "Type4", DataType="Number",RegularExpression="" }
                }
            }
            };
        }
    }
}
