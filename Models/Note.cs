using System;
using System.Collections.Generic;

namespace note_backend.Models
{
    // Modelo para representar uma nota
    public class Note
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
        public List<string> Tags { get; set; } = new List<string>();
        public string Categoria { get; set; } = string.Empty;
    }
}
