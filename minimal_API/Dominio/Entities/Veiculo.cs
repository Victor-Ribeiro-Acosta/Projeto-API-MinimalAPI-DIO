

using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_API.Dominio.Entities;

public class Veiculo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    public string marca { get; set; }
    public string modelo { get; set; }
    public int ano { get; set; }
}