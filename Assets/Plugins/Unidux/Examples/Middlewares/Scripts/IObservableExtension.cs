using System;
using R3;

namespace Unidux.Example.Middlewares
{
    public static class ObservableExtension
    {
        public static Observable<Unit> AsThunkObservable(this object observer)
        {
            if (observer is Observable<Unit>)
            {
                return observer as Observable<Unit>;
            }

            return null;
        }
    }
}