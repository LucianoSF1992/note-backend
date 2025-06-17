using Microsoft.AspNetCore.Mvc;
using note_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace note_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        // Armazenamento em memória das notas
        private static readonly List<Note> notas = new List<Note>();

        // Adicionar uma nova nota
        [HttpPost]
        public ActionResult<Note> AdicionarNota([FromBody] Note novaNota)
        {
            novaNota.Id = Guid.NewGuid();
            novaNota.DataCriacao = DateTime.UtcNow;
            novaNota.DataAtualizacao = DateTime.UtcNow;
            notas.Add(novaNota);
            return CreatedAtAction(nameof(ObterNotaPorId), new { id = novaNota.Id }, novaNota);
        }

        // Obter nota por ID
        [HttpGet("{id}")]
        public ActionResult<Note> ObterNotaPorId(Guid id)
        {
            var nota = notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
            {
                return NotFound(new { mensagem = "Nota não encontrada." });
            }
            return Ok(nota);
        }

        // Editar uma nota existente
        [HttpPut("{id}")]
        public ActionResult EditarNota(Guid id, [FromBody] Note notaAtualizada)
        {
            var nota = notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
            {
                return NotFound(new { mensagem = "Nota não encontrada." });
            }
            nota.Titulo = notaAtualizada.Titulo;
            nota.Conteudo = notaAtualizada.Conteudo;
            nota.Tags = notaAtualizada.Tags ?? new List<string>();
            nota.Categoria = notaAtualizada.Categoria;
            nota.DataAtualizacao = DateTime.UtcNow;
            return NoContent();
        }

        // Excluir uma nota
        [HttpDelete("{id}")]
        public ActionResult ExcluirNota(Guid id)
        {
            var nota = notas.FirstOrDefault(n => n.Id == id);
            if (nota == null)
            {
                return NotFound(new { mensagem = "Nota não encontrada." });
            }
            notas.Remove(nota);
            return NoContent();
        }

        // Obter todas as notas com opções de pesquisa, ordenação e filtragem
        [HttpGet]
        public ActionResult<IEnumerable<Note>> ObterNotas(
            [FromQuery] string? pesquisa,
            [FromQuery] string? ordenarPor,
            [FromQuery] string? categoria,
            [FromQuery] string? tag)
        {
            var resultado = notas.AsEnumerable();

            // Filtrar por pesquisa no título ou conteúdo
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                resultado = resultado.Where(n =>
                    n.Titulo.Contains(pesquisa, StringComparison.OrdinalIgnoreCase) ||
                    n.Conteudo.Contains(pesquisa, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por categoria
            if (!string.IsNullOrWhiteSpace(categoria))
            {
                resultado = resultado.Where(n =>
                    n.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
            }

            // Filtrar por tag
            if (!string.IsNullOrWhiteSpace(tag))
            {
                resultado = resultado.Where(n =>
                    n.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
            }

            // Ordenar resultados
            resultado = ordenarPor?.ToLower() switch
            {
                "titulo" => resultado.OrderBy(n => n.Titulo),
                "datacriacao" => resultado.OrderBy(n => n.DataCriacao),
                "dataatualizacao" => resultado.OrderBy(n => n.DataAtualizacao),
                _ => resultado.OrderBy(n => n.DataCriacao)
            };

            return Ok(resultado);
        }
    }
}
