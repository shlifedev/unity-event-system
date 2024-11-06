 
  
using QQ;

public interface IEventMessage
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


     public partial struct A : IEventMessage
    {
        A(int a, int b, QQ.Vector3 p)
        {

        } 
    }
 
}


public partial  struct B: IEventMessage
{
`    B(int a, int b, QQ.Vector3 p)
    {
    }

}

public class A
{
    void T()
    {
         
    }
}