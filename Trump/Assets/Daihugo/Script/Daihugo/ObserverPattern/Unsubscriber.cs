using System;
using System.Collections.Generic;

class Unsubscriber : IDisposable
{
    //発行先リスト
    private List<IDaihugoObserver> observers;
    //DisposeされたときにRemoveするIObserver<int>
    private IDaihugoObserver observer;

    public Unsubscriber(List<IDaihugoObserver> observers, IDaihugoObserver observer)
    {
        this.observers = observers;
        this.observer = observer;
    }

    public void Dispose()
    {
        //Disposeされたら発行先リストから対象の発行先を削除する
        observers.Remove(observer);
    }
}