using System;

namespace Standalone.Core
{
    public class ExceptionData
    {
        public string Message { get; set; }

        public ExceptionData(Exception exception) =>
            Message = exception?.Message;

        public static explicit operator ExceptionData(Exception exception) =>
            new ExceptionData(exception);
    }
}