﻿using System;

 namespace AspCoreCardGameEngine.Domain.Exceptions
{
    // Inspiration from https://dusted.codes/advanced-tips-and-tricks-for-aspnet-core-applications
    public class DomainException : Exception
    {
        public readonly DomainErrorCode ErrorCode;

        public DomainException(DomainErrorCode code, string message) : base(message)
        {
            ErrorCode = code;
        }

        public DomainException(DomainErrorCode code, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = code;
        }
    }
}
