# unity-event-system

A simple and intuitive event broadcast and listener system with zero GC. 


### Requirement
- UniTask (UniTask don't mean much right now, but they will be important in a later example of implementing a Dispatcher for network packets.)


# How to Use?
[Example Folder](https://github.com/shlifedev/unity-event-system/tree/main/GameEvent/Example)

## Define Your Own Message.

Messages are always declared as struct to avoid GC allocation. 

```cs
public struct YourMessage : IMessage{
     public int Value{get; set;}
}
```


## Broadcast Message

```cs
EventBus.Broadcast(new YourMessage() { Value = 100 });
```


## Inherit Listener, And Regist/UnRegist.
```cs
public class YourGameObject :
MonoBehaviour, 
IEventListener<YourMessage>
{
void OnEnable(){
   EventBus.Register(this);
}

void OnDisable(){
   EventBus.UnRegister(this);
}


public void OnEventReceived(YourMessage message){
     Debug.Log(message.Value + " Received. ");
}
}
```

 


## Smart Use Tip.

Don't bother registering listeners on OnEnable and OnDisable. 
Create a base mono behaviour. so that OnEnable/OnDisable is always called.

```cs
public abstract class BaseMonoBehaviour : MonoBehaviour{

   protected virtual void OnEnable() => EventBus.Register(this);
   protected virtual void OnDisable() => EventBus.UnRegister(this); 
   protected virtual void OnDestroy() => EventBus.UnRegister(this); 
}
```


## ... And

Non-moNno classes can also use EventBus to manage listeners,

but it is the programmer's responsibility to manage the lifecycle of the object by unregistering it when it is no longer needed, such as through the Dispose pattern. 
