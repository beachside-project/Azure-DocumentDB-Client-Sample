using System;
using System.Threading;

namespace DocumentDbDemo.Utils
{
    public static class Gc
    {
        public static void Throw<TObject>(ref TObject obj) where TObject : class, IDisposable => Throw(ref obj, null);

        public static void Throw<TObject>(ref TObject obj, Action<TObject> finalizer) where TObject : class, IDisposable
        {
            var target = Interlocked.Exchange(ref obj, null);
            if (target == null) return;
            try
            {
                finalizer?.Invoke(target);
                target.Dispose();
            }
            catch
            {
                // 無視でいいよね。
            }
        }
    }
}
