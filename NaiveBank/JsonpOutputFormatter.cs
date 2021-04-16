using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace NaiveBank
{
    /// <summary>
    /// Handles JsonP requests when requests are fired with text/javascript
    /// </summary>
    public class JsonpOutputFormatter : TextOutputFormatter
    {
        private static string ApplicationJavascriptMediaType = "application/javascript";
        private static string TextJavascriptMediaType = "text/javascript";
        private static string ApplicationXJavascriptMediaType = "application/x-javascript";
        private static string JsonpParameterName = "callback";
        
        private SystemTextJsonOutputFormatter jsonFormatter;

        /// <summary>
        /// Initializes a new <see cref="JsonpOutputFormatter"/> instance.
        /// </summary>
        /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
        public JsonpOutputFormatter(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonFormatter = new SystemTextJsonOutputFormatter(jsonSerializerOptions);

            foreach (var supportedEncoding in this.jsonFormatter.SupportedEncodings)
            {
                base.SupportedEncodings.Add(supportedEncoding);
            }

            foreach (var supportedMediaTypes in this.jsonFormatter.SupportedMediaTypes)
            {
                base.SupportedMediaTypes.Add(supportedMediaTypes);
            }

            SupportedMediaTypes.Add(ApplicationJavascriptMediaType);
            SupportedMediaTypes.Add(TextJavascriptMediaType);
            SupportedMediaTypes.Add(ApplicationXJavascriptMediaType);
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var httpContext = context.HttpContext;
            string jsonpCallbackFunctionName = GetJsonCallbackFunctionName(httpContext.Request);

            if (string.IsNullOrEmpty(jsonpCallbackFunctionName))
            {
                await this.jsonFormatter.WriteResponseBodyAsync(context, selectedEncoding);
                return;
            }

            Stream responseStream = httpContext.Response.Body;

            // Write the padding
            using StreamWriter writer = new StreamWriter(responseStream, selectedEncoding);
            await writer.WriteAsync(jsonpCallbackFunctionName + "(");
            await writer.FlushAsync();

            // Write the data
            await this.jsonFormatter.WriteResponseBodyAsync(context, selectedEncoding);

            // Write the ending
            await writer.WriteAsync(");");
            await writer.FlushAsync();
        }

        /// <inheritdoc />
        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            var httpContext = context.HttpContext;
            string jsonpCallbackFunctionName = GetJsonCallbackFunctionName(httpContext.Request);
            if (!string.IsNullOrEmpty(jsonpCallbackFunctionName))
            {
                context.ContentType = ApplicationJavascriptMediaType;
            }

            return base.CanWriteResult(context);
        }

        /// <summary>
        /// Retrieves the Jsonp Callback function from the query string.
        /// </summary>
        /// <returns>The callback function name.</returns>
        private static string GetJsonCallbackFunctionName(HttpRequest request)
        {
            if (request.Method != HttpMethods.Get)
            {
                return null;
            }

            var queryVal = request.Query[JsonpParameterName];

            if (string.IsNullOrEmpty(queryVal))
            {
                return null;
            }

            return queryVal;
        }
    }
}
