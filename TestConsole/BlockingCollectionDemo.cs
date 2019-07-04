using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class BlockingCollectionDemo
    {
        public BlockingCollection<WorkItem> ventilatorQueue;
        public BlockingCollection<WorkItem> sinkQueue;
        public void Initialize()
        {
            ventilatorQueue = new BlockingCollection<WorkItem>(10);
            sinkQueue = new BlockingCollection<WorkItem>(100);
        }

        public void Execute()
        {

        }

    }



    public class WorkItem
    {
        public string Text { get; set; }
    }
}
