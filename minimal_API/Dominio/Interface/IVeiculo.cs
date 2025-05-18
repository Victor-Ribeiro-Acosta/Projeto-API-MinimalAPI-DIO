using minimal_API.Dominio.DTO;
using minimal_API.Dominio.Entities;
using minimal_API.Dominio.Views;

namespace minimal_API.Dominio.Interface;

public interface IVeiculo
{
    Veiculo RegistrarVeiculo(Veiculo veiculo);
    List<ViewVeiculo> ListarVeiculosCadastrados();
    Veiculo EditarVeiculo(int id, VeiculoDTO veiculoAtualizado);
    bool DeletarVeiculo(int id);
    int GerarIdVeiculo();
}