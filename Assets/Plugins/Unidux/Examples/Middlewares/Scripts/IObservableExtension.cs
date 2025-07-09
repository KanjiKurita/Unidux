﻿using System;
using R3;

namespace Unidux.Example.Middlewares
{
    public static class IObservableExtension
    {
        public static IObservable<Unit> AsThunkObservable(this object observer)
        {
            if (observer is IObservable<Unit>)
            {
                return observer as IObservable<Unit>;
            }

            return null;
        }
    }
}