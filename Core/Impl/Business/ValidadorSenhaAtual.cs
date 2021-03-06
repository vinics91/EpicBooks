﻿using Core.Application;
using Core.Impl.Control;
using Core.Interfaces;
using Domain;
using Domain.DadosCliente;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Core.Impl.Business
{
    public class ValidadorSenhaAtual : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade.GetType().Name.Equals("Usuario"))
            {
                Usuario usuario = (Usuario)entidade;
                Result resultado;

                if (!string.IsNullOrEmpty(usuario.Senha) && usuario.DadosAlterados.Equals("SENHA"))
                {
                    resultado = new Facade().Consultar(new Usuario { Id = usuario.Id, Senha = usuario.Senha });
                    if (resultado.Msg != null)
                        return "Erro ao consultar usuário";
                    if(resultado.Entidades.Count > 0)
                    {
                        List<Usuario> usuarios = new List<Usuario>();
                        foreach (var item in resultado.Entidades)
                        {
                            usuarios.Add((Usuario)item); 
                        }
                    }
                    else
                        return "Senha atual incorreta";
                }
            }
            else
            {
                return "Deve ser registrado um usuário";
            }
            return null;
        }
    }
}
