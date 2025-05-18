

using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using minimal_API.Dominio.Entities;

namespace minimal_API.Dominio.Views;

public class ViewVeiculo
{
    public string marca { get; set; }
    public string modelo { get; set; }
    public int ano { get; set; }

    public ViewVeiculo(Veiculo veiculo)
    {
        marca = veiculo.marca;
        modelo = veiculo.modelo;
        ano = veiculo.ano;
    }
}