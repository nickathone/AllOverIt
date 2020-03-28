using System;

namespace AllOverIt.Exceptions
{
  public interface IExceptionHandler
  {
    void Handle(Exception exception);
  }
}