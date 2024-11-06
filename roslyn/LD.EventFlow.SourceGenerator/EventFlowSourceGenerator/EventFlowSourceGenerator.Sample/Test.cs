 
  
using System;
using LD.EventFlow;
using LD.EventFlow.Attributes; 

namespace LD.EventFlow
{ 
    [Construct]  
    public interface IEventMessage
    {


    } 
    
    [Construct]  
    public interface WW : IEventMessage
    {
        
    }
} 

[Construct]  
public interface IEventQQ : WW
{
    
}
namespace QQ
{  
    

    public class Vector3
    {
        public float x;
    }
}

namespace Game
{


     public partial struct A : IEventQQ
    {
        A(int a, int b, QQ.Vector3 p)
        {

        } 
    }
    public partial  struct B: IEventMessage
    {
        B(int a, int b, QQ.Vector3 p)
        {
        }

    }
 
}


public partial  struct B: IEventMessage
{
    B(int a, int b, QQ.Vector3 p)
    {
    }

}

public class A
{
    void T()
    {
         
    }
}

[AttributeUsage(AttributeTargets.Interface, Inherited = true)]
class QWE : Attribute
{
    
}