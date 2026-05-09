using UnityEngine;
using CatCode.Events;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;

public sealed class Example
{

    
    public async Task EventValueExamples()
    {
        // Инициализация с уведомлением только при изменении значения
        EventValue<int> state = new EventValue<int>(0,  NotifyMode.OnChanged);

        state.Value = 10;

        state.Changed += (value) => Debug.Log(value);

        // Подписка с немедленным вызовом обработчика для текущего значения
        state.AddListener(value => Debug.Log(value), invokeImmediately: true);

        // Подписка с авто-отпиской в конце блока using (структура, без аллокаций)
        using var handle = state.AddListenerScoped((value) => Debug.Log(value), false);

        // Подписка с ручной отпиской (IDisposable класс)
        IDisposable disposable = state.AddListenerDisposable((value) => Debug.Log(value), true);
        disposable.Dispose();

        
        var cts = new CancellationTokenSource();

        // Ожидание выполнения условия (с проверкой текущего значения)
        await state.WaitAsync((value) => value > 10, checkInitialState: true, cts.Token);

        // Ожидание следующего изменения значения
        await state.WaitAsync(cts.Token);
    }


    private EventSignal _signal = new EventSignal();


}

public class Tester : MonoBehaviour
{
    private readonly EventValue<int> _value = new(1);
    private CancellationTokenSource _cts = new();

    public int Value;

    private void Awake()
    {
        _value.AddListener(v => Value = v, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _value.Value++;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            NextTask(_cts.Token).Forget();
            EachTask(_cts.Token).Forget();
        }
    }

    async UniTaskVoid EachTask(CancellationToken token)
    {
        await _value.WaitAsync((v) => v % 2 == 0, false, token);
        Debug.Log("First Each");
        while (!token.IsCancellationRequested)
        {
            await _value.WaitAsync((v) => v % 2 == 0, false, token);
            Debug.Log("Each");
        }
    }

    async UniTask NextTask(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await _value.WaitAsync(token);
            Debug.Log("Next");
        }
    }
}
