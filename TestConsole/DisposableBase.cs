using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public abstract class DisposableBase : IDisposable
    {
        private static readonly System.Diagnostics.TraceSource trace = new System.Diagnostics.TraceSource("TestDisposable");

        public bool IsDisposed
        {
            get;
            private set;
        }

        public event EventHandler<EventArgs> Disposed;

        protected DisposableBase()
        {
            IsDisposed = false;
        }

        ~DisposableBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
            //GC.SuppressFinalize(this);
            if (this.Disposed != null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

        protected virtual void VerifyNotDisposed()
        {
            if (!IsDisposed)
            {
                return;
            }
            throw new ObjectDisposedException(ToString());
        }

        protected abstract void Dispose(bool disposing);
    }
}
