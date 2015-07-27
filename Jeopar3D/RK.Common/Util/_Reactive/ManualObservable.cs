using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common.Util
{
    public class ManualObservable<T> : IObservable<T>
    {
        private IObserver<T> m_observer;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            m_observer = observer;
            return new DummyDisposable(() => m_observer = null);
        }

        public void NotifyNext(T nextItem)
        {
            if (m_observer != null) { m_observer.OnNext(nextItem); }
        }

        public void NotifyError(Exception ex)
        {
            if (m_observer != null) { m_observer.OnError(ex); }
        }

        public void NotifyCompleted()
        {
            if (m_observer != null) { m_observer.OnCompleted(); }
        }
    }
}
