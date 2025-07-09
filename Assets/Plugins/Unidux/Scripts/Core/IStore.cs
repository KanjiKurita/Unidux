using System;
using R3;

namespace Unidux
{
    public interface IStore<TState> where TState : StateBase
    {
        TState State { get; set; }
        Subject<TState> Subject { get; }
        
        object Dispatch(object action);
        void Update();
    }

    public interface IStoreObject
    {
        object ObjectState { get; set; }
        Observable<object> ObjectSubject { get; }
        Type StateType { get; }
    }
}
