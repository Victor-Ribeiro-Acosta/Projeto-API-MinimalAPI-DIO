
using Microsoft.EntityFrameworkCore;
using minimal_API.Dominio.Entities;
namespace minimal_API.InfraEstrutura.Context;

public class InfraEstruturaContext : DbContext
{
    public InfraEstruturaContext(DbContextOptions options): base(options){}
    public DbSet<Usuario> usuarios { get; set; }
    public DbSet<Veiculo> veiculos { get; set; }
}