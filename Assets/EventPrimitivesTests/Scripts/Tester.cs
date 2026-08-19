using UnityEngine;
using CatCode.EventPrimitives;
using System.Threading;
using Cysharp.Threading.Tasks;

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
