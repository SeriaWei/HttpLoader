using System;

namespace HttpLoader.HttpParser
{
    public class ParseHttpRequestException : Exception
    {
        public ParseHttpRequestException(string message, string step, string component) : base($"{message} Method: {step}() Data: {component}")
        {
        }
    }
}
