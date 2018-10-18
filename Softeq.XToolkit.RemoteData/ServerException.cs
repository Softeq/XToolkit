// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Softeq.XToolkit.Common.Extensions;

namespace Softeq.XToolkit.RemoteData
{
    public class ServerException : Exception
    {
        public ServerException(string message, IList<ErrorDescription> errors = null) : base(message)
        {
            Errors = errors;
        }

        public IList<ErrorDescription> Errors { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorCode => Errors?.FirstOrDefault()?.Code;
        public string ErrorDescription => Errors?.FirstOrDefault()?.Description;
        public string ErrorDetailedCode => Errors?.FirstOrDefault()?.DetailedErrorCode;

        public override string Message
        {
            get
            {
                var result = string.Empty;
                foreach (var e in Errors.EmptyIfNull())
                {
                    result += $"server error - code: {e.Code}, message: {e.Description}\n";
                }

                return result;
            }
        }

        public override string StackTrace => string.Empty;
    }
}