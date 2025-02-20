﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace PnP.Core.Services
{
    internal struct ApiCallResponse
    {
        internal ApiCallResponse(ApiCall apiCall, string json, HttpStatusCode statusCode,
            Guid batchRequestId, Dictionary<string, string> headers, Stream binaryContent = null)
        {
            ApiCall = apiCall;
            Json = json;
            StatusCode = statusCode;
            BatchRequestId = batchRequestId;
            Headers = headers;
            BinaryContent = binaryContent;
        }

        /// <summary>
        /// The API call that issued the current response
        /// </summary>
        internal ApiCall ApiCall { get; private set; }

        /// <summary>
        /// The Id of the batch used to get the current response
        /// </summary>
        internal Guid BatchRequestId { get; private set; }

        /// <summary>
        /// Contains the json response of the request (if any)
        /// </summary>
        internal string Json { get; private set; }

        /// <summary>
        /// Contains the request http status code
        /// </summary>
        internal HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Contains additional response headers (if any)
        /// </summary>
        internal Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Stream containing binary content of the response
        /// </summary>
        internal Stream BinaryContent { get; private set; }
    }
}
