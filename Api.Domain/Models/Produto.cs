using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api_Macoratti.Validations;

namespace Api_Macoratti.Models
{
    [Table("Produtos")]
    public class Produto : IValidatableObject
    {
        [Key]
        public int ProdutoId { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(80)]
        [PrimeiraLetraMaiuscula]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8,2)")] // oito posições e duas casas decimais
        [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }
        [Required]
        [MaxLength(500)]
        public string ImagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public Categoria Categoria { get; set; } // um produto possui uma categoria
        public int CategoriaId { get; set; } // relacionamento entre Produto e Categoria

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(this.Estoque <= 0)
            {
                yield return new ValidationResult("O estoque deve ser maior que zero", 
                    new[] {
                        nameof(this.Estoque)
                    });
            }
        }
    }
}