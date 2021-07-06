using System;
using System.IO;
using System.Threading.Tasks;
using Api_Macoratti.Repository;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;

namespace Api.Domain.GraphQl
{
    public class TesteGraphQLMiddleware
    {
        // instancia para processar o request http
        private readonly RequestDelegate _next;

        // instancia do UnitOfWork
        private readonly IUnitOfWork _context;

        public TesteGraphQLMiddleware(RequestDelegate next, IUnitOfWork contexto)
        {
            _next = next;
            _context = contexto;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            //verifica se o caminhodo request é /graphql
            if(httpContext.Request.Path.StartsWithSegments("/graphql"))
            {
                // temta ler o corpo do request usando um StreamReader
                using(var stream = new StreamReader(httpContext.Request.Body))
                {
                    var query = await stream.ReadToEndAsync();

                    if(!String.IsNullOrWhiteSpace(query))
                    {
                        // um objeto schema é criado com a prop Query definida com uma instancia do nosso contexto (repositorio)
                        var schema = new Schema 
                        {
                            Query = new CategoriaQuery(_context)
                        };
                        // cria um DocumentExecuter que executa a aculta contra o schema e o resultado é escrito no response como JSON via Write Result
                        var result = await new DocumentExecuter().ExecuteAsync(options => 
                        {
                            options.Schema = schema;
                            options.Query = query;
                        });
                        await WriteResult(httpContext, result);
                    }
                }
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task WriteResult(HttpContext httpContext, ExecutionResult result)
        {
            var json = new DocumentWriter(indent: true).Write(result);
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }
    }
}