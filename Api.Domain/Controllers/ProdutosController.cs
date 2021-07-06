using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api_Macoratti.DTOs;
using Api_Macoratti.Filters;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;
using Api_Macoratti.Repository;
using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Macoratti.Controllers
{
    // [EnableQuery] // OData
    [ApiConventionType(typeof(DefaultApiConventions))] // statuscode do swagger é aplicado para todos os métodos do controlador
    [Produces("application/json")] // muda o método de retorno do swagger para 'application/json'
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }
        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        // [HttpGet("/primeiro")] -> se eu usar a barra "/" , vai ignorar a route api/[controller]
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            // para ver a paginação, digitar a querry string "?pageNumber=1&pageSize=2"
            // https://localhost:5001/api/produtos?pageNumber=1&pageSize=4
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters); // usar o banco de dados justifica o uso de métodos assíncronos
            
            var metadata = new 
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata)); // header
            
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        // [HttpGet("{valor:alpha:length(5)}")] restrição -> aceita somente valores alfanuméricos com tamanho de 5
        [HttpGet("{id:int:min(1)}", Name ="ObterProduto")] // restrição -> o valor mínimo do id será 1
        public async Task<ActionResult<ProdutoDTO>> Get(int id) // com retorno ActionResult<T>
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound();
            }
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return produtoDto;

            // para evitar fazer todo esse código abaixo, usamos o AutoMapper
            // return new ProdutoDTO
            // {
            //     ProdutoId = produto.ProdutoId,
            //     Nome = produto.Nome,
            //     Preco = produto.Preco,
            //     ImagemUrl = produto.ImagemUrl,
            //     Descricao = produto.Descricao,
            //     CategoriaId = produto.CategoriaId
            // };
        }
        // com retorno IActionResult
        // [HttpGet]
        // public IActionResult Get()
        // {
        //     var produto = _uof.Produtos.FirstOrDefault();
        //     if(produto == null)
        //     {
        //         return NotFound();
        //     }
        //     return Ok(produto);
        // }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]ProdutoDTO produtoDto)
        {
            // isso é feito automaticamente devido ao [ApiController]
            // if(!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }
            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTO);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDto)
        {
            if(id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }
            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound();
            }
            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();
            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return produtoDto;
        }
    }
}