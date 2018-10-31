# XToolkit

XToolkit is a collection of components for cross-platform mobile development with Xamarin.

## Installation

NuGet:

```
Install-Package Softeq.XToolkit.Common
```

```
Install-Package Softeq.XToolkit.Bindings
```


## Common Library Overview

| Library | Description | Supported platforms |
| ------ | ------ | ------ |
| Softeq.XToolkit.Auth | Auth library for all projects that used our .net backend | Core, Android, iOS |
| Softeq.XToolkit.Common | The most common components without dependencies that can be reused in any project | Core, Android, iOS |
| Softeq.XToolkit.Bindings | Bindings implementation based on INotifyPropertyChanged interface (MVVMLight) | Core, Android, iOS |
| Softeq.XToolkit.RemoteData | Base HttpClient implementation | Core, Android, iOS |
| Softeq.XToolkit.Connectivity | It is a wrapper of Plugin.Connectivity with some improvements | Core, Android, iOS |

### Softeq.XToolkit.Auth

Features:

- **Cache user info in memory and internal storage**
- **Refreshe token**

To initialize AuthService add this code to DI registrations:

```csharp
containerBuilder.RegisterType<AuthService>().WithParameter("authConfig", 
    new AuthConfig([LOGIN_URL], [CLIENT_ID], [CLIENT_SECRET]))
```

### Softeq.XToolkit.Common

The most common components without dependencies.
Almost all projects ~~can~~ should use this library.

#### Common

Collections

- **ObservableRangeCollection** - represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
- **ObservableKeyGroupsCollection** - grouping of items by key into ObservableRange
- **CollectionSorter** - order collection using Comparison
- **BiDictionary** - use this dictionary in case you need fast access to the key by value

Extensions for:

- **Assembly**
- **DateTime**
- **Dictionary**
- **Enumerable**
- **Enum**
- **Task**

Helpers:

- **HashHelper**
- **StringsHelper**
- **TagsHelper**

WeakObjects:

- **WeakAction** - stores an Action without causing a hard reference to be created to the Action's owner. The owner can be garbage collected at any time.
- **WeakFunc** - stores a Func without causing a hard reference to be created to the Func's owner. The owner can be garbage collected at any time.
- **WeakReference** - weak reference for any object
- **WeakEventSubscription** - weak subscription for any event

Other:

- **Timer** crossplatform timer
- **TaskReference** use this class if you want to create a Task but don't want to start it immediately
- **GenericEventArgs**
- **RelayCommand** implementation of ICommand interface

#### Common.iOS

Extensions for:

- **DateTime** DateTime to NSDate and back converter 
- **UIColor** 
- **UIViewController**

#### Common.Droid

- **ContextExtensions**
- **BoolToViewStateConverter**

### Softeq.XToolkit.Bindings

Implementation of Bindings based on **MVVM Light Toolkit** that supports **.NET Standard**.
For more details see [www.mvvmlight.net/doc](http://www.mvvmlight.net/doc)

#### Getting Started

To start using Bindings just add this code to your project

##### iOS

in **AppDelegate.cs**

```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    // init factory for bindings
    BindingExtensions.Initialize(new AppleBindingFactory());

    ...

    return true;
}
```

##### Droid

```csharp
public abstract class MainApplicationBase : Application
{
    ...

    public override void OnCreate()
    {
        base.OnCreate();

        // init factory for bindings
        BindingExtensions.Initialize(new DroidBindingFactory());

        ...
    }

    ...
}
```

### Softeq.XToolkit.RemoteData

REST HttpClient implemenation

### Softeq.XToolkit.Connectivity

Simple cross platform plugin to check connection status of mobile device, gather connection type, bandwidths, and more. It is a wrapper of [Plugin.Connectivity](https://github.com/jamesmontemagno/ConnectivityPlugin). We created ping service to the google.com(customizable) to be sure that the Internet really available. 

```csharp
public void MyMethod
{
    ConnectionManager.StartTracking();
    ConnectionManager.NetworkConnectionChanged += OnNetworkConnectionChanged;
}

private void OnNetworkConnectionChanged(object sender, NetworkConnectionEventArgs e)
{
    // handle result here
}
```

## Contributing

We welcome any contributions.

## License

The XToolkit project is available for free use, as described by the [LICENSE](https://github.com/Softeq/XToolkit/blob/master/LICENSE) (MIT).
