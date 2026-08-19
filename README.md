# EventPrimitives
A library of wrapper classes that turn events and values into standalone objects.
This allows them to be passed around directly through a unified API for subscribing and awaiting.

- Event Passing. Allows passing events as arguments to methods or other classes.
- Unification. Provides common types for events and values, avoiding duplicated subscription and awaiting logic across classes.
- Performance. Provides a delegate-based Observer implementation with configurable subscriber storage and minimal additional allocations.
- Asynchrony. Supports await via `UniTask`.

## Installation
UMP (Unity Package Manager)  
`https://github.com/CatCodeGames/EventPrimitives.git?path=Assets/EventPrimitives`

## Events
Events are lightweight wrappers around standard C# events, exposing them through a unified API provided by the library.

### Usage
```csharp
// Initialize with notification only when the value changes
EventValue<int> state = new EventValue<int>(0, NotifyMode.OnChanged);

state.Value = 10;
state.Changed += (value) => Debug.Log(value);

// Subscribe and immediately invoke the handler with the current value
state.AddListener(value => Debug.Log(value), invokeImmediately: true);

// Scoped subscription with automatic unsubscription at the end of the using block
using var handle = state.AddListenerScoped((value) => Debug.Log(value), false);

// Manual subscription lifetime management
IDisposable disposable = state.AddListenerDisposable((value) => Debug.Log(value), true);
disposable.Dispose();


var cts = new CancellationTokenSource();
// Asynchronously wait for a condition, including an optional initial value check
await state.WaitAsync((value) => value > 10, checkInitialState: true, cts.Token);

// Asynchronously wait for the next value change
await state.WaitAsync(cts.Token);

```

### Structure and API

#### EventValue<T>

`EventValue<T>` is a container for a value and its change event.
It can notify subscribers on every write or only when the value actually changes.
The `IReadOnlyEventValue` interface provides read-only access without allowing the value to be changed or the event to be invoked.

#### EventSource

`EventSource` is a container for an event.
The `IReadOnlyEventSource` interface exposes the event for subscription while preventing it from being invoked externally.

#### Extension Methods

Subscription methods optionally allow the handler to be invoked immediately when subscribed.

- `AddListener` / `RemoveListener` — subscribe and unsubscribe directly.
- `AddListenerScoped` — returns a struct that automatically unsubscribes at the end of a using block.
- `AddListenerDisposable` — returns an `IDisposable` for manually managing the subscription lifetime.
- `WaitAsync` — asynchronously waits for an event using `UniTask` and `UniTaskCompletionSourceCore`.


## Observable

Observable provides a lightweight delegate-based implementation of the Observer pattern.
Subscriptions are represented by structs, and subscriber storage can be selected independently. This allows the implementation to be adapted to different Unity scenarios while keeping additional allocations to a minimum.


### Usage

```csharp
// Initialize with notification only when the value changes
var state = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChanged);

state.Value = 10;

// Subscribe
var subscription = state.Subscribe((value) => Debug.Log(value));

// Unsubscribe
subscribtion.Unsubscribe();
// or
subscribtion.Dispose();

```

### Structure and API

#### ObservableValue<T>
`ObservableValue<T>` - a container for a value and its change notifications.
It can notify subscribers on every write or only when the value actually changes.
The `IReadonlyObservableValue` interface provides read-only access without allowing the value to be changed or notifications to be triggered externally.

#### ObservableSource
`ObservableSource`- a container for an event source.
The `IReadonlyObservableSource` interface allows external code to subscribe without allowing it to trigger the event.

#### ISubscriberStorage
Base interface for storing and invoking subscribers, used by `ObservableValue` and `ObservableSource`.

By default, `ArrayBackedLinkedList` is used — an array of slot structs that are reused after removal. This reduces allocations when adding and removing subscribers.

Custom `ISubscriberStorage` implementations can be provided when a different storage strategy is better suited to a particular scenario.

#### Extension Methods

- `WaitAsync` — asynchronously waits for an event using `UniTask` and `UniTaskCompletionSourceCore`.



___


# EventPrimitives
Библиотека классов-оберток для преобразования событий и значений в самостоятельные объекты.  
Это позволяет работать с ними напрямую через единый интерфейс подписки и ожидания.

- Передача событий. Позволяет передавать событие как аргумент в методы или другие классы.
- Унификация. Использование единых типов данных исключает дублирование логики подписки и ожидания для каждого нового класса. 
- Производительность. Реализация паттерна «Наблюдатель» на основе делегатов с возможностью выбора хранилища подписчиков и минимальным количеством дополнительных аллокаций.
- Асинхронность. Поддержка await через UniTask.


## Установка
UMP (Unity Package Manager)  
`https://github.com/CatCodeGames/EventPrimitives.git?path=Assets/EventPrimitives`

## Events
Events — простые обёртки над стандартными событиями C#, позволяющие работать с ними через единый API библиотеки.

### Использование
```csharp
// Инициализация с уведомлением только при изменении значения
EventValue<int> state = new EventValue<int>(0,  NotificationMode.OnChanged);

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
### Структура и API

#### EventValue<T>
`EventValue<T>` - контейнер для значения и события его изменения.  
Позволяет вызывать событие при каждой записи или только при обновлении данных.  
Через интерфейс `IReadOnlyEventValue` ограничивает доступ к изменению значения и вызову события.

#### EventSource
Контейнер для события.  
Через интерфейс `IReadOnlyEventSource` ограничивает доступ к вызову события извне.

#### Методы расширения
Методы подписки позволяют сразу вызвать обработчик при его добавлении.
- `AddListener` / `RemoveListener` — прямая подписка и отписка от события.
- `AddListenerScoped` — возвращает структуру для автоматической отписки при выходе из блока using.
- `AddListenerDisposable` — возвращает `IDisposable` объект для ручного управления временем жизни подписки.
- `WaitAsync` — асинхронное ожидание события через `UniTask` на базе `UniTaskCompletionSourceCore`.



## Observable
Observable — лёгкая реализация паттерна «Наблюдатель» на основе делегатов. Подписки представлены структурами, а хранилище подписчиков можно выбирать отдельно. Это позволяет использовать Observer в Unity с минимальными аллокациями и адаптировать хранение подписчиков под конкретный сценарий.

### Использование

```csharp
// Инициализация с уведомлением только при изменении значения
var state = ObservableValue<int>.CreateDefault(0, NotificationMode.OnChanged);

state.Value = 10;
// Подписка
var subscription = state.Subscribe((value) => Debug.Log(value));

// отписка
subscribtion.Unsubscribe();
// или
subscribtion.Dispose();

```

### Структура и API

#### ObservableValue<T>
`ObservableValue<T>` - контейнер для значения и события его изменения.  
Позволяет вызывать событие при каждой записи или только при обновлении данных.  
Через интерфейс `IReadonlyObservableValue` ограничивает доступ к изменению значения и вызову события.

#### ObservableSource
Контейнер для события.  
Через интерфейс `IReadonlyObservableSource` ограничивает доступ к вызову события извне.

#### ISubscriberStorage
Базовый интерфейс для хранения и вызова подписчиков, используемый ObservableValue и ObservableSource.

По умолчанию используется `ArrayBackedLinkedList` — массив структур-слотов с переиспользованием освобождённых слотов. Это позволяет уменьшить количество аллокаций при работе с подписчиками.
При необходимости можно реализовать собственный `ISubscriberStorage` с другой структурой хранения.

#### Методы расширения

- `WaitAsync` — асинхронное ожидание события через `UniTask` на базе `UniTaskCompletionSourceCore`.
