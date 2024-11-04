# unity-event-system

A simple and intuitive event broadcast and listener system with zero GC. 
GC없이 게임 이벤트를 브로드캐스트하고, 리스너를 등록해서 핸들링할 수 있는 이벤트 시스템입니다.


### TodoList

[Ok] - Zero GC Event Listener, Register, Custom Message  

[Todo] - Class Message Support (선언된 변수 토큰에 직접 메세지를 할당하여 Pooling된 클래스가 잘못된 포인터를 참조하게 되는 이슈를 피하기 위해 로즐린이 필요합니다.)  

[Todo] - Code Generator With Roslyn (로즐린을 사용하면 EventBus.Register의 메세지 등록이 굉장히 빨라집니다. 현재도 빠른 상태입니다.)  



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
