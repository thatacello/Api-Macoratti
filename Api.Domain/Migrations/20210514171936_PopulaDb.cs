using Microsoft.EntityFrameworkCore.Migrations;

namespace Api_Macoratti.Migrations
{
    public partial class PopulaDb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Bebidas', " + 
                " 'https://cdn.pixabay.com/photo/2017/09/20/18/00/ice-cubes-2769457_960_720.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Lanches', " + 
                " 'https://cdn.pixabay.com/photo/2018/08/29/19/01/fig-3640553_960_720.jpg')");
            mb.Sql("Insert into Categorias(Nome, ImagemUrl) Values('Sobremesas', " + 
                " 'https://cdn.pixabay.com/photo/2016/11/29/09/00/doughnuts-1868573_960_720.jpg')");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values("+
                    "'Coca-Cola', 'Refrigerante de Cola 350 ml', 5.45, 'https://cdn.pixabay.com/photo/2014/09/26/19/51/drink-462776_960_720.jpg', "+
                    " 50, now(), (Select CategoriaId from Categorias where Nome='Bebidas'))");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values("+
                    "'Croissant', 'Recheado de Presunto e Queijo', 4.50, 'https://cdn.pixabay.com/photo/2012/02/29/12/17/bread-18987_960_720.jpg', "+
                    " 10, now(), (Select CategoriaId from Categorias where Nome='Lanches'))");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values("+
                    "'Bolo', 'Recheio de chocolate com doce de leite', 16.90, 'https://cdn.pixabay.com/photo/2017/01/11/11/33/cake-1971552_960_720.jpg', "+
                    " 5, now(), (Select CategoriaId from Categorias where Nome='Sobremesas'))");

            mb.Sql("Insert into Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) Values("+
                    "'Churros', 'Com recheio de doce de leite', 10.00, 'https://cdn.pixabay.com/photo/2017/03/30/15/47/churros-2188871_960_720.jpg', "+
                    " 20, now(), (Select CategoriaId from Categorias where Nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");
        }
    }
}
