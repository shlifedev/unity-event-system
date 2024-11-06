# unity-event-system

A very easy to use zero-gc game event sending/listen system. 

- This is useful for decoupling UI code from game code.

- You can listen to any event that happens in your game where it inherits from IEventListener<>.

- Sending/receiving events does not incur any unnecessary GC. (struct only)



## Requirement
- UniTask  


## [Example](https://github.com/shlifedev/event-flow/tree/main/src/Assets/Example)
[Movie_001.webm](https://github.com/user-attachments/assets/19ef0dd3-7288-49fa-b3c3-87b2195be071)

 

## How to use

### [Declare Your Game Event](https://github.com/shlifedev/event-flow/tree/main/src/Assets/Example/Scripts/Messages/OnEntityDamagedMessage.cs)
Just inherit the IEventMessage to the structure.

```
 public struct YourEvent : IEventMessage{ // your event fields.. }
```

### [Inherit IEventListenr<T> And Regist](https://github.com/shlifedev/event-flow/tree/main/src/Assets/Example/Scripts/HealthBarUI.cs)
```cs
public class YourClass : MonoBehaviour, IEventListener<YourMessage>{
    void OnEnable(){
         EventBus.Register(this);
    }
    void OnDestroy(){
         EventBus.UnRegister(this);
    }

    public void OnEventRaised(YourMessage eventMessageArgs){
         Debug.Log("Received!");
    }
}
```

### [Broadcast message](https://github.com/shlifedev/unity-event-system/blob/main/GameEvent/Example/Scripts/GameEntity.cs)
```cs
     EventBus.Broadcast(new YourMessage(){});
```  
