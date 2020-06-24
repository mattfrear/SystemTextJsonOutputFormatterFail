using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Models;
using System;
using System.Buffers;
using System.IO;

namespace SystemTextJsonOutputFormatterFail
{
    internal static class Program
    {
        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        private static void Main()
        {
            var person = new Person { Title = Title.Mr, FirstName = "Matt", Age = 99, Income = 1234 };

            // Format using XmlDataContractSerializerOutputFormatter - works
            var xmlFormatter = new XmlDataContractSerializerOutputFormatter();
            var result = Serialize(person, xmlFormatter, ApplicationXml);
            Console.WriteLine("Xml: " + result);

            // Format using NewtonsoftJsonOutputFormatter - also works
            var newtonsoftFormatter = new NewtonsoftJsonOutputFormatter(new Newtonsoft.Json.JsonSerializerSettings(), ArrayPool<char>.Shared, new MvcOptions());
            result = Serialize(person, newtonsoftFormatter, ApplicationJson);
            Console.WriteLine("Newtonsoft Json: " + result);

            // Format using SystemTextJsonOutputFormatter - returns empty string :-(
            var systemTextJsonFormatter = new SystemTextJsonOutputFormatter(new System.Text.Json.JsonSerializerOptions());
            result = Serialize(person, systemTextJsonFormatter, ApplicationJson);
            Console.WriteLine("System.Text.Json: " + result);

            Console.ReadLine();
        }

        private static string Serialize<T>(T value, IOutputFormatter formatter, MediaTypeHeaderValue contentType)
        {
            if (value == null)
            {
                return string.Empty;
            }

            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(
                    stringWriter,
                    value,
                    value.GetType(),
                    contentType);

                formatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                stringWriter.FlushAsync().GetAwaiter().GetResult();
                return stringWriter.ToString();
            }
        }

        private static OutputFormatterWriteContext GetOutputFormatterContext(
            TextWriter writer,
            object outputValue,
            Type outputType,
            MediaTypeHeaderValue contentType)
        {
            return new OutputFormatterWriteContext(
                GetHttpContext(contentType),
                (stream, encoding) => writer,
                outputType,
                outputValue);
        }

        private static HttpContext GetHttpContext(MediaTypeHeaderValue contentType)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.AcceptCharset] = contentType.Charset.ToString();
            httpContext.Request.Headers[HeaderNames.Accept] = contentType.MediaType.Value;
            httpContext.Request.ContentType = contentType.MediaType.Value;

            httpContext.Response.Body = new MemoryStream();
            httpContext.RequestServices =
                new ServiceCollection()
                    .AddSingleton(Options.Create(new MvcOptions()))
                    .BuildServiceProvider();

            return httpContext;
        }
    }
}
