# EventPrimitives
A library of wrapper classes for converting events and values into standalone objects.  
This allows working with them directly through a unified interface for subscription and awaiting.

- Event Passing. Allows passing an event as an argument to methods or other classes.
- Unification. Using unified data types eliminates duplicate subscription and awaiting logic for every new class.
- Asynchrony. await support via UniTask.

# Using
```csharp
// Initialize with notification only on value change
EventValue<int> state = new EventValue<int>(0, NotifyMode.OnChanged);

state.Value = 10;
state.Changed += (value) => Debug.Log(value);

// Subscription with immediate handler invocation for the current value
state.AddListener(value => Debug.Log(value), invokeImmediately: true);

// Subscription with auto-unsubscription at the end of using block (struct, zero-allocation)
using var handle = state.AddListenerScoped((value) => Debug.Log(value), false);

// Subscription with manual unsubscription (IDisposable class)
IDisposable disposable = state.AddListenerDisposable((value) => Debug.Log(value), true);
disposable.Dispose();


var cts = new CancellationTokenSource();
// Asynchronous wait for a condition (including current value check)
await state.WaitAsync((value) => value > 10, checkInitialState: true, cts.Token);

// Asynchronous wait for the next value change
await state.WaitAsync(cts.Token);

```

# Architecture and API

### EventValue<T>
`EventValue<T>` - a container for a value and its change event.  
Allows triggering the event on every write or only on value update.  
Restricts access to value modification and event invocation via the `IReadOnlyEventValue` interface.

### EventSignal
`EventSignal` - a container for an event.  
Restricts access to event invocation from the outside via the `IReadOnlyEventSignal` interface.  

### Extension Methods
Subscription methods allow calling the handler immediately upon addition.
- `AddListener` / `RemoveListener` — direct subscription and unsubscription.
- `AddListenerScoped` — returns a structure for automatic unsubscription within a using block.
- `AddListenerDisposable` — returns an `IDisposable` object for manual subscription lifetime management.
- `WaitAsync` — asynchronous event awaiting via `UniTask` based on `UniTaskCompletionSourceCore`.

___


# EventPrimitives
Библиотека классов-оберток для преобразования событий и значений в самостоятельные объекты.  
Это позволяет работать с ними напрямую через единый интерфейс подписки и ожидания.

- Передача событий. Позволяет передавать событие как аргумент в методы или другие классы.
- Унификация. Использование единых типов данных исключает дублирование логики подписки и ожидания для каждого нового класса. 
- Асинхронность. Поддержка await через UniTask.

# Использование
```csharp
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
// Асинхронное ожидание выполнения условия (с проверкой текущего значения)
await state.WaitAsync((value) => value > 10, checkInitialState: true, cts.Token);

// Асинхронное ожидание следующего изменения значения
await state.WaitAsync(cts.Token);
```

# Architecture and API

### EventValue<T>
`EventValue<T>` - контейнер для значения и события его изменения.  
Позволяет вызывать событие при каждой записи или только при обновлении данных.  
Через интерфейс `IReadOnlyEventValue` ограничивает доступ к изменению значения и вызову события.

### EventSignal
Контейнер для события.  
Через интерфейс `IReadOnlyEventSignal` ограничивает доступ к вызову события извне.

### Методы расширения
Методы подписки позволяют сразу вызвать обработчик при его добавлении.
- `AddListener` / `RemoveListener` — прямая подписка и отписка от события.
- `AddListenerScoped` — возвращает структуру для автоматической отписки при выходе из блока using.
- `AddListenerDisposable` — возвращает `IDisposable` объект для ручного управления временем жизни подписки.
- `WaitAsync` — асинхронное ожидание события через `UniTask` на базе `UniTaskCompletionSourceCore`.
