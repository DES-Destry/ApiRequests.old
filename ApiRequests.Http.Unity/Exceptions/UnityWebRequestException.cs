using System;
using UnityEngine.Networking;

namespace ApiRequests.Http.Unity.Exceptions
{
    public class UnityWebRequestException : Exception
    {
        public UnityWebRequest.Result FailedResult { get; }
        
        public UnityWebRequestException(UnityWebRequest.Result requestFailedResult)
        {
            FailedResult = requestFailedResult;
        }
    }
}