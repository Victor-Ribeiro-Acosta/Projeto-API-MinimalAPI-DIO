

using minimal_API.Dominio.Entities;

namespace minimal_API.Dominio.Views;

public class ViewUsuario
{
    public string nome { get; set; }
    public string senha { get; set; }
    public string token { get; set; }

    public ViewUsuario(Usuario usuario, String autenticate)
    {
        nome = usuario.nome;
        senha = usuario.senha;
        token = autenticate;

    }
}