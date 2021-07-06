using Api_Macoratti.Repository;
using GraphQL;
using GraphQL.Types;

namespace Api.Domain.GraphQl
{
    public class CategoriaQuery : ObjectGraphType
    {
        public CategoriaQuery(IUnitOfWork _context)
        {
            // retorna um objeto Categoria
            Field<CategoriaType>("categoria", arguments: new QueryArguments( 
                new QueryArgument<IntGraphType>() { Name = "id" }),
                resolve: context => 
                {
                    var id = context.GetArgument<int>("id");
                    return _context.CategoriaRepository.GetById(c => c.CategoriaId == id);
                });
            
            // retorna uma lista de objetos categoria
            Field<ListGraphType<CategoriaType>>("categorias",
                resolve: context => 
                {
                    return _context.CategoriaRepository.Get();
                });
        }
    }
}