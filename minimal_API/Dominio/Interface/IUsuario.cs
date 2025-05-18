using minimal_API.Dominio.Entities;

namespace minimal_API.Dominio.Interface;

public interface IUsuario
{
   Usuario InserirNovoUsuario(Usuario usuario);
   Usuario LogarUsuario(string email, string senha);
   int GerarIdUsuario();
}