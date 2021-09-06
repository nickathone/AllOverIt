﻿using System;

namespace AllOverIt.Exceptions
{
    public class ProcessException : Exception
    {
        public ProcessException()
        {
        }

        public ProcessException(string message)
            : base(message)
        {
        }

        public ProcessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}