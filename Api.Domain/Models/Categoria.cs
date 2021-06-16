using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Macoratti.Models
{
    [Table("Categorias")] // não é necessário
    public class Categoria
    {
        [Key] // não é necessário
        public int CategoriaId { get; set; }
        [Required]
        [MaxLength(80)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string ImagemUrl { get; set; }
        public ICollection<Produto> Produtos { get; set; } // uma categoria possui muitos produtos (por isso é ICollection)
        public Categoria() // é responsabilidade da Categoria criar um novo produto
        {
            Produtos = new Collection<Produto>();
        }
    }
}