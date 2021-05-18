using System;
using System.Collections.Generic;
using System.Linq;
using Api_Macoratti.Context;
using Api_Macoratti.DTOs;
using Api_Macoratti.Models;
using Api_Macoratti.Repository;
using Api_Macoratti.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api_Macoratti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork uof, IConfiguration config, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uof = uof;
            _configuration = config;
            _logger = logger;
            _mapper = mapper;
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
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            _logger.LogInformation("============ GET api/categorias/produtos ==============");
            var categorias = _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
            // return _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
        }
        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                var categorias = _uof.CategoriaRepository.Get().ToList();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
                // return _uof.CategoriaRepository.Get().ToList();
            }
            catch(System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter categorias do banco de dados");
            }
            
        }
        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
                if(categoria == null)
                {
                    return NotFound($"A categoria com id={id} não foi encontrada");
                }

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
                return categoriaDto;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter categorias do banco de dados");
            }
        }
        [HttpPost]
        public ActionResult Post([FromBody] CategoriaDTO categoriaDto)
        {
            try 
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Add(categoria);
                _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar criar uma nova categoria");
            }
        }
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            try
            {
                if(id != categoriaDto.CategoriaId)
                {
                    return BadRequest($"Não foi possível atualizar a categoria com id={id}");
                }
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                _uof.CategoriaRepository.Update(categoria);
                _uof.Commit();

                return Ok($"Categoria com id={id} foi atualizada com sucesso");
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar a categoria com id={id}");
            }
        }
        [HttpDelete("{id}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
                if(categoria == null)
                {
                    return NotFound($"A categoria com id = {id} não foi encontrada");
                }
                _uof.CategoriaRepository.Delete(categoria);
                _uof.Commit();
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDto;
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir a categoria com id={id}");
            }
        }
    }
}