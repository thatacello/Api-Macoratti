using System;
using Api_Macoratti.Models;
using GraphQL.Types;

namespace Api.Domain.GraphQl
{
    // Dfine qual entidade será mapeada para o Type
    public class CategoriaType : ObjectGraphType<Categoria>
    {
        public CategoriaType()
        {
            // campos do type
            Field(x => x.CategoriaId).Description("Id da Categoria");
            Field(x => x.Nome);
            Field(x => x.ImagemUrl);

            Field<ListGraphType<CategoriaType>>("categorias");
        }
    }
}