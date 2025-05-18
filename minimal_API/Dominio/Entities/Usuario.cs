

using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_API.Dominio.Entities;

public class Usuario
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string nome { get; set; }
    public string email { get; set; }
    public string senha { get; set; }
}