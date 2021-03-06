﻿using Core.Interfaces;
using Domain;
using Domain.Produto;
using System.Collections.Generic;
using System.Linq;

namespace Core.Impl.Business
{
    public class ValidadorDadosObrigatoriosProduto : IStrategy
    {
        public string Processar(EntidadeDominio entidade)
        {
            if (entidade.GetType().Name.Equals("Livro"))
            {
                Livro livro = (Livro)entidade;

                if (string.IsNullOrEmpty(livro.Nome) || string.IsNullOrWhiteSpace(livro.Nome) || 
                    string.IsNullOrEmpty(livro.Descricao) || string.IsNullOrWhiteSpace(livro.Descricao) ||
                    livro.Generos.Count() == 0 || livro.Autores.Count() == 0 ||
                    livro.Editora == null || livro.Editora == 0 || livro.GrupoPrecificacao == 0 ||
                    string.IsNullOrEmpty(livro.AnoLancamento) || string.IsNullOrWhiteSpace(livro.AnoLancamento)  || 
                    livro.Edicao == null || livro.Editora == 0 ||
                    string.IsNullOrEmpty(livro.CaminhoImagem) || string.IsNullOrWhiteSpace(livro.CaminhoImagem) ||
                    string.IsNullOrEmpty(livro.Isbn) || string.IsNullOrWhiteSpace(livro.Isbn) ||
                    livro.QtdePaginas == null || livro.QtdePaginas == 0 || livro.TipoCapa == 0)
                    return "Os campos com * são de preenchimento obrigatório.";
            }
            else
            {
                return "Deve ser registrado um livro.";
            }
            return null;
        }
    }
}
