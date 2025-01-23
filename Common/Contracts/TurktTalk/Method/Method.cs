using Dawn;
using System;

namespace OLab.Api.TurkTalk.Methods;

public class Method
{
  public string MethodName { get; set; }
  public string CommandChannel { get; set; }

  public Method()
  {

  }

  public Method(string recipientGroupName, string methodName)
  {
    Guard.Argument( recipientGroupName ).NotEmpty( recipientGroupName );
    Guard.Argument( methodName ).NotEmpty( methodName );

    MethodName = methodName;
    CommandChannel = recipientGroupName;
  }

  public virtual string ToJson() { throw new NotImplementedException(); }
}