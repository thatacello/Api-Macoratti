using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Macoratti.Context;
using Api_Macoratti.Filters;
using Api_Macoratti.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Macoratti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }
        // [HttpGet("/primeiro")] -> se eu usar a barra "/" , vai ignorar a route api/[controller]
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
        {
            return await _context.Produtos.AsNoTracking().ToListAsync(); // usar o banco de dados justifica o uso de assíncrono
        }
        // [HttpGet("{valor:alpha:length(5)}")] restrição -> aceita somente valores alfanuméricos com tamanho de 5
        [HttpGet("{id:int:min(1)}", Name ="ObterProduto")] // restrição -> o valor mínimo do id será 1
        public ActionResult<Produto> Get(int id) // com retorno ActionResult<T>
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound();
            }
            return produto;
        }
        // com retorno IActionResult
        // [HttpGet]
        // public IActionResult Get()
        // {
        //     var produto = _context.Produtos.FirstOrDefault();
        //     if(produto == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(produto);
        // }
        [HttpPost]
        public ActionResult Post([FromBody]Produto produto)
        {
            // isso é feito automaticamente devido ao [ApiController]
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }
            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest();
            }
            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound();
            }
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return produto;
        }
    }
}