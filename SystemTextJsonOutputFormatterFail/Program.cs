using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Models;
using System;
using System.Buffers;

namespace SystemTextJsonOutputFormatterFail
{
    class Program
    {
        private static readonly MediaTypeHeaderValue ApplicationXml = MediaTypeHeaderValue.Parse("application/xml; charset=utf-8");
        private static readonly MediaTypeHeaderValue ApplicationJson = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

        static void Main(string[] args)
        {
            var person = new Person { Title = Title.Mr, FirstName = "Matt", Age = 99, Income = 1234 };

            var mvcOutputFormatter = new MvcOutputFormatter();

            var xmlFormatter = new XmlDataContractSerializerOutputFormatter();
            var xmlResult = mvcOutputFormatter.Serialize(person, xmlFormatter, ApplicationXml);
            Console.WriteLine("Xml: " + xmlResult);

            var newtonsoftFormatter = new NewtonsoftJsonOutputFormatter(
                new Newtonsoft.Json.JsonSerializerSettings(),
                ArrayPool<char>.Shared,
                new MvcOptions());

            var newtonSoftJsonResult = mvcOutputFormatter.Serialize(person, newtonsoftFormatter, ApplicationJson);
            Console.WriteLine("Newtonsoft Json: " + newtonSoftJsonResult);

            var systemTextJsonFormatter = new SystemTextJsonOutputFormatter(new System.Text.Json.JsonSerializerOptions());
            var systemTextJsonResult = mvcOutputFormatter.Serialize(person, systemTextJsonFormatter, ApplicationJson);

            Console.WriteLine("System.Text.Json: " + systemTextJsonResult);

            Console.ReadLine();
        }
    }
}
