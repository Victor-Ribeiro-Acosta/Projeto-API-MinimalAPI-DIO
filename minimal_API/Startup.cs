using minimal_API.Dominio.Entities;
using minimal_API.Dominio.DTO;
using minimal_API.Dominio.Service;
using minimal_API.Dominio.Views;
using minimal_API.InfraEstrutura.Context;
using minimal_API.Dominio.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace minimal_API;
public class Startup
{
    public IConfiguration configuration { get; set; } = default!;
    // buscar string em appsettings
    public string stringJwt;

    public Startup(IConfiguration config)
    {
        configuration = config;
        stringJwt = configuration?.GetSection("JWT").ToString() ?? "";
    }

    public void ConfigureServices(IServiceCollection services)
    {

        // Adicionando escopos para o projeto
        services.AddScoped<IUsuario, ServicoUsuario>();
        services.AddScoped<IVeiculo, ServicoVeiculo>();
        // configurando banco de dados
        services.AddDbContext<InfraEstruturaContext>(options => options.UseInMemoryDatabase("APIdeVeiculos"));

        services.AddEndpointsApiExplorer();

        services.AddAuthorization();

        // Adicionando configurações do swagger
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token jwt:",

            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },

                    new string[]{ }
                }
            });
        }
        );
        // Configurar token jwt

        // Validar string jwt
        if (string.IsNullOrEmpty(stringJwt)) stringJwt = "key-secret-dev";

        // Adicionar configuração para gerar token
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
        ).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(stringJwt))
            };
        });

        services.AddCors(option =>
        {
            option.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(app =>
        {
            #region "Token
            // criar função para gerar token
            string GerarTokenJWT(Usuario usuario)
            {
                // configurar variaveis
                var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(stringJwt));
                var credencials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
                List<Claim> claims = new List<Claim>()
                {
                    new Claim("Usuario", usuario.nome),
                    new Claim("Email", usuario.email),
                    new Claim("Senha", usuario.senha)
                };
                // Criar token
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credencials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            #endregion

            // ENDPOINTS
            #region "Rotas Usuario"
            app.MapPost("/usuarios", (UsuarioDTO dadosUsuario, IUsuario _contextUsuario) =>
            {
                if (dadosUsuario == null)
                {
                    return Results.BadRequest();
                }
                int id = _contextUsuario.GerarIdUsuario() + 1;

                Usuario usuario = new Usuario()
                {
                    id = id,
                    nome = dadosUsuario.nome,
                    email = dadosUsuario.email,
                    senha = dadosUsuario.senha
                };


                _contextUsuario.InserirNovoUsuario(usuario);
                return Results.Ok($"Usuario {usuario.nome} cadastrado com Sucesso");

            }).WithTags("Administrador");


            app.MapPost("/login", (LoginDTO login, IUsuario _contextUsuario) =>
            {

                var usuarioAutenticado = _contextUsuario.LogarUsuario(login.email, login.senha);

                if (usuarioAutenticado == null)
                {
                    return Results.NotFound();
                }

                ViewUsuario vUsuario = new ViewUsuario(usuarioAutenticado, GerarTokenJWT(usuarioAutenticado));

                return Results.Ok(vUsuario);

            }).WithTags("Administrador");
            #endregion

            #region "Rotas veiculo"
            app.MapPost("/veiculos", (VeiculoDTO veiculoCadastro, IVeiculo _contextVeiculo) =>
            {
                if (veiculoCadastro == null)
                {
                    return Results.BadRequest();
                }

                int id = _contextVeiculo.GerarIdVeiculo() + 1;

                var resultado = _contextVeiculo.RegistrarVeiculo(new Veiculo()
                {
                    id = id,
                    marca = veiculoCadastro.marca,
                    modelo = veiculoCadastro.modelo,
                    ano = veiculoCadastro.ano
                });

                if (resultado == null)
                {
                    return Results.BadRequest();
                }
                return Results.Ok(new ViewVeiculo(resultado));

            }).RequireAuthorization().WithTags("Veiculo");


            app.MapGet("/veiculos", (IVeiculo _contextVeiculo) =>
            {
                return Results.Ok(_contextVeiculo.ListarVeiculosCadastrados());
            }).WithTags("Veiculo");


            app.MapPut("/veiculos/{id}", (int id, VeiculoDTO veiculoAtualizado, IVeiculo _contextVeiculo) =>
            {
                var resultado = _contextVeiculo.EditarVeiculo(id, veiculoAtualizado);
                if (resultado == null)
                {
                    return Results.BadRequest();
                }

                return Results.Ok(resultado);

            }).RequireAuthorization().WithTags("Veiculo");


            app.MapDelete("/veiculos", (int id, IVeiculo _contextVeiculo) =>
            {
                var resultado = _contextVeiculo.DeletarVeiculo(id);

                if (!resultado)
                {
                    return Results.BadRequest();
                }
                return Results.NoContent();
            }).RequireAuthorization().WithTags("Veiculo");
            #endregion
        });

    }

}


