using minimal_API.Dominio.DTO;
using minimal_API.Dominio.Entities;
using minimal_API.Dominio.Interface;
using minimal_API.Dominio.Views;
using minimal_API.InfraEstrutura.Context;

namespace minimal_API.Dominio.Service;

public class ServicoVeiculo : IVeiculo
{
    private readonly InfraEstruturaContext _context;

    public ServicoVeiculo(InfraEstruturaContext context)
    {
        _context = context;
    }
    public bool DeletarVeiculo(int id)
    {
        var veiculo = _context.veiculos.FirstOrDefault(v => v.id == id);

        if (veiculo == null)
        {
            return false;
        }

        _context.veiculos.Remove(veiculo);
        _context.SaveChanges();
        return true;
    }

    public Veiculo EditarVeiculo(int id, VeiculoDTO veiculoAtualisado)
    {
        var veiculo = _context.veiculos.FirstOrDefault(v => v.id == id);
        if (veiculo == null)
        {
            return null;
        }

        if (!String.IsNullOrWhiteSpace(veiculoAtualisado.marca))
        {
            veiculo.marca = veiculoAtualisado.marca;
        }

        if (!String.IsNullOrWhiteSpace(veiculoAtualisado.modelo))
        {
            veiculo.modelo = veiculoAtualisado.modelo;
        }

        if (veiculoAtualisado.ano >= 1970)
        {
            veiculo.ano = veiculoAtualisado.ano;
        }

        _context.SaveChanges();
        return veiculo;
    }

    public List<ViewVeiculo> ListarVeiculosCadastrados()
    {
        var veiculos = _context.veiculos.ToList();
        List<ViewVeiculo> viewVeiculos = new List<ViewVeiculo>();

        foreach (Veiculo veiculo in veiculos)
        {
            viewVeiculos.Add(new ViewVeiculo(veiculo));
        }

        return viewVeiculos;
    }

    public Veiculo RegistrarVeiculo(Veiculo veiculo)
    {
        if (veiculo == null)
        {
            return null;
        }

        if (String.IsNullOrWhiteSpace(veiculo.marca) || String.IsNullOrWhiteSpace(veiculo.modelo) || veiculo.ano < 1970)
        {
            return null;
        }

        _context.veiculos.Add(veiculo);
        _context.SaveChanges();
        return veiculo;
    }

    public int GerarIdVeiculo()
    {
        return _context.veiculos?.ToList().Count() ?? 0;
    }
}