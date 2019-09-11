using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class RecieptMetaData
    {
        public String RecieptName { get; set; }

        public List<LabelFormMap> Attributes { get; set; }

        public RecieptMetaData()
        {
            

        }
    }

    public class LabelFormMap
    {
        public String FormLabel { get; set; }

        public RecieptLabel RecieptLabel { get; set; }

    }

    public class RecieptLabel
    {
        public String RowLabel { get; set; }
        public String ColumnLabel { get; set; }
        public String DataType { get; set; }
        public String RegularExpression { get; set; }
    }
}
