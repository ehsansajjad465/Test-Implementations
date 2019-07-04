using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class TestDisposable : DisposableBase
    {
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                // dispose child class resources
            }
        }
    }
}
