using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SystemTextJsonOutputFormatterFail
{
    static class Program
    {
        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        static void Main(string[] args)
        {
            var person = new Person { Title = Title.Mr, FirstName = "Matt", Age = 99, Income = 1234 };

            var xmlResult = Serialize(person, ApplicationXml);

            Console.WriteLine("Xml:" + xmlResult);
        }

        private static string Serialize(Person value, MediaTypeHeaderValue contentType)
        {
            using (var stringWriter = new StringWriter())
            {
                var outputFormatterContext = GetOutputFormatterContext(
                    stringWriter,
                    value,
                    value.GetType(),
                    contentType);

                //var formatter = OutputFormatterSelector.SelectFormatter(
                //    outputFormatterContext,
                //    new List<IOutputFormatter>(),
                //    new MediaTypeCollection());

                //if (formatter == null)
                //{
                //    throw new FormatterNotFoundException(contentType);
                //}

                //formatter.WriteAsync(outputFormatterContext).GetAwaiter().GetResult();
                //stringWriter.FlushAsync().GetAwaiter().GetResult();
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

    internal class FormatterNotFoundException : Exception
    {
        public FormatterNotFoundException(MediaTypeHeaderValue contentType)
            : base($"OutputFormatter not found for '{contentType}'")
        { }
    }
}
