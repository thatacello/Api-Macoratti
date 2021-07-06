using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api_Macoratti.DTOs;
using Api_Macoratti.Models;
using Api_Macoratti.Pagination;
using Api_Macoratti.Repository;
using Api_Macoratti.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api_Macoratti.Controllers
{
    [Produces("application/json")]
    // [Authorize(AuthenticationSchemes = "Bearer")] // define esquema de autenticação -> retorna 401 se o usuário nao estiver autenticado
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("PermitirApiRequest")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork uof, IMapper mapper) // removidos ->  IConfiguration config, ILogger<CategoriasController> logger, 
        {
            _uof = uof;
            // _configuration = config;
            // _logger = logger;
            _mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet("teste")]
        public string GetTeste()
        {
            return $"CategoriasController - {DateTime.Now.ToLongDateString().ToString()}";
        }
        [HttpGet("autora")]
        public string GetAutora()
        {
            var autora = _configuration["autora"];
            var conexao = _configuration["ConnectionStrings:DefaultConnection"];
            return $"Autora: { autora } Conexao: { conexao }";
        }
        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }
        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            // _logger.LogInformation("============ GET api/categorias/produtos ==============");
            var categorias = await _uof.CategoriaRepository.GetCategoriasProdutos();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
            // return _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new 
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata)); // inclui no header

                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
                // return _uof.CategoriaRepository.Get().ToList();
            }
            catch(System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter categorias do banco de dados");
            }
            
        }
        
        /// <summary>
        /// Retorna uma coleção de objetos Categoria
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.Get().AsQueryable().ToList();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
                // throw new Exception(); -> usei no teste 
                return categoriasDto;
            }
            catch (Exception)
            {
                return BadRequest();
            }           
        }

        // /// <summary>
        // /// Obtem um categoria pelo seu Id
        // /// </summary>
        // /// <param name="id"></param>
        // /// <returns>Objetos Categoria</returns>
        // [HttpGet("{id}", Name = "ObterCategoria")]
        // [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)] // swagger -> padrão
        // [ProducesResponseType(StatusCodes.Status404NotFound)] // exibe NotFound no swagger
        // public ActionResult<CategoriaDTO> Get(int? id)
        // {
        //     // try
        //     // {
        //         var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

        //         if(categoria == null)
        //         {
        //             // return NotFound($"A categoria com id = {id} não foi encontrada");
        //             return NotFound();
        //         }

        //         var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
        //         return categoriaDto;
        //     // }
        //     // catch(Exception)
        //     // {
        //     //     return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter categorias do banco de dados");
        //     // }
        // }

        // exemplo de requisição no remarks

        /// <summary>
        /// Obtem um categoria pelo seu Id
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        /// POST api/categorias
        /// {
        ///     "categoriaId" : 1,
        ///     "nome" : "categoria1",
        ///     "imagemURL" : "http:/teste.net/1.jpg"
        /// }
        /// </remarks>
        /// <param name="categoriaDto"></param>
        /// <returns>Objetos Categoria</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDto)
        {
            try 
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Add(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar uma nova categoria");
            }
        }
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))] // é uma alternativa ao ProducesResponseType acima, pois não é necessário especificar cada tipo de statuscode
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                if(id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"Não foi possível atualizar a categoria com id={id}");
                }
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Update(categoria);
                await _uof.Commit();

                return Ok($"Categoria com id={id} foi atualizada com sucesso");
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar a categoria com id={id}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int? id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
                if(categoria == null)
                {
                    return NotFound($"A categoria com id = {id} não foi encontrada");
                }
                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDto;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir a categoria com id={id}");
            }

            // var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            // if (categoria == null)
            // {
            //     return NotFound();
            // }
            // _uof.CategoriaRepository.Delete(categoria);
            // _uof.Commit();

            // var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            // return categoriaDto;
        }
    }
}