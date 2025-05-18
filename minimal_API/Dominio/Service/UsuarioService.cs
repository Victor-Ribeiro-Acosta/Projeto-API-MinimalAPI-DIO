using minimal_API.Dominio.Entities;
using minimal_API.InfraEstrutura.Context;
using minimal_API.Dominio.Interface;

namespace minimal_API.Dominio.Service;

public class ServicoUsuario: IUsuario
{
    private readonly InfraEstruturaContext _context;

    public ServicoUsuario(InfraEstruturaContext context)
    {
        _context = context;
    }

    public int GerarIdUsuario()
    {
        return _context.usuarios?.Count() ?? 0;
    }

    public Usuario InserirNovoUsuario(Usuario usuario)
    {
        _context.usuarios.Add(usuario);
        _context.SaveChanges();
        return usuario;
    }

    public Usuario LogarUsuario(string email, string senha)
    {
        var usuarioCadastrado = _context.usuarios.FirstOrDefault(u => u.email == email && u.senha == senha);
        if (usuarioCadastrado == null)
        {
            return null;
        }
        return usuarioCadastrado;
    }
}