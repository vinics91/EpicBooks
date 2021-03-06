﻿using Core.Application;
using Core.Impl.Business;
using Core.Impl.DAO.DadosCliente;
using Core.Impl.DAO.Negocio;
using Core.Impl.DAO.Produto;
using Core.Interfaces;
using Domain;
using Domain.DadosCliente;
using Domain.Negocio;
using Domain.Produto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Impl.Control
{
    public class Facade : IFacade
    {
        private readonly Dictionary<string, IDAO> daos;
        private readonly Dictionary<string, Dictionary<string, List<IStrategy>>> rns;
        private Result resultado;

        public Facade()
        {
            daos = new Dictionary<string, IDAO>();
            rns = new Dictionary<string, Dictionary<string, List<IStrategy>>>();

            #region Produto 
            ProdutoDAO produtoDAO = new ProdutoDAO();

            daos.Add(nameof(Livro), produtoDAO);

            //Regras de salvar
            ValidadorDadosObrigatoriosProduto validadorDadosObrgProd = new ValidadorDadosObrigatoriosProduto();
            InclusaoDataCadastro inclusaoDataCadastro = new InclusaoDataCadastro();
            //Regra genérica
            ValidadorIsbn validadorIsbn = new ValidadorIsbn();
            //Regras de alterar
            ValidadorDadosObrigatoriosProdutoEdicao validadorDadosObrgProdEdicao = new ValidadorDadosObrigatoriosProdutoEdicao();

            List<IStrategy> rnsSalvarProduto = new List<IStrategy>();
            List<IStrategy> rnsAlterarProduto = new List<IStrategy>();

            rnsSalvarProduto.Add(validadorDadosObrgProd);
            rnsSalvarProduto.Add(inclusaoDataCadastro);

            rnsAlterarProduto.Add(validadorDadosObrgProdEdicao);
            rnsAlterarProduto.Add(validadorIsbn);

            Dictionary<string, List<IStrategy>> rnsProduto = new Dictionary<string, List<IStrategy>>();

            rnsProduto.Add("SALVAR", rnsSalvarProduto);
            rnsProduto.Add("ALTERAR", rnsAlterarProduto);

            rns.Add(nameof(Livro), rnsProduto);
            #endregion

            #region Pedido 
            PedidoDAO pedidoDAO = new PedidoDAO();

            daos.Add(nameof(Pedido), pedidoDAO);

            //Regras de salvar
            CalculoValorTotalPedido calculoValorTotalPedido = new CalculoValorTotalPedido();
            ValidadorDadosObrigatoriosPedido validadorDadosObrigatoriosPedido = new ValidadorDadosObrigatoriosPedido();
            ValidadorCartoesDiferentes validadorCartoesDiferentes = new ValidadorCartoesDiferentes();
            ValidadorNecessidadePgtoCartao validadorNecessidadePgtoCartao = new ValidadorNecessidadePgtoCartao();
            ValidadorValorMinimoCartao validadorValorMinimoCartao = new ValidadorValorMinimoCartao();
            ValidadorCupons validadorCupons = new ValidadorCupons();
            ValidadorDivisaoValorCartoes validadorDivisaoValorCartoes = new ValidadorDivisaoValorCartoes();
            VerificadorNecessidadeGeracaoCupomTroca verificadorNecessidadeGeracaoCupomTroca = new VerificadorNecessidadeGeracaoCupomTroca();

            //Regras de alterar
            ValidadorRetornoOperadora validadorRetornoOperadora = new ValidadorRetornoOperadora();

            List<IStrategy> rnsSalvarPedido = new List<IStrategy>();
            List<IStrategy> rnsAlterarPedido = new List<IStrategy>();

            rnsSalvarPedido.Add(calculoValorTotalPedido);
            rnsSalvarPedido.Add(validadorDadosObrigatoriosPedido);
            rnsSalvarPedido.Add(validadorCartoesDiferentes);
            rnsSalvarPedido.Add(validadorNecessidadePgtoCartao);
            rnsSalvarPedido.Add(validadorValorMinimoCartao);
            rnsSalvarPedido.Add(validadorCupons);
            rnsSalvarPedido.Add(validadorDivisaoValorCartoes);
            rnsSalvarPedido.Add(verificadorNecessidadeGeracaoCupomTroca);
            rnsSalvarPedido.Add(inclusaoDataCadastro);

            rnsAlterarPedido.Add(validadorRetornoOperadora);

            Dictionary<string, List<IStrategy>> rnsPedido = new Dictionary<string, List<IStrategy>>();

            rnsPedido.Add("SALVAR", rnsSalvarPedido);
            rnsPedido.Add("ALTERAR", rnsAlterarPedido);

            rns.Add(nameof(Pedido), rnsPedido);
            #endregion

            #region Troca 
            TrocaDAO trocaDAO = new TrocaDAO();

            daos.Add(nameof(Troca), trocaDAO);

            //Regras de salvar
            ValidadorDadosObrigatoriosTroca validadorDadosObrigatoriosTroca = new ValidadorDadosObrigatoriosTroca();
            ValidadorQtdeTrocaCompativelComPedido validadorQtdeTrocaCompativelComTroca = new ValidadorQtdeTrocaCompativelComPedido();

            //Regras de alterar
            ValidadorGeracaoCupomTrocaRecebida validadorGeracaoCupomTrocaRecebida = new ValidadorGeracaoCupomTrocaRecebida();

            List<IStrategy> rnsSalvarTroca = new List<IStrategy>();
            List<IStrategy> rnsAlterarTroca = new List<IStrategy>();

            rnsSalvarTroca.Add(inclusaoDataCadastro);
            rnsSalvarTroca.Add(validadorDadosObrigatoriosTroca);
            rnsSalvarTroca.Add(validadorQtdeTrocaCompativelComTroca);

            rnsAlterarTroca.Add(validadorGeracaoCupomTrocaRecebida);

            Dictionary<string, List<IStrategy>> rnsTroca = new Dictionary<string, List<IStrategy>>();

            rnsTroca.Add("SALVAR", rnsSalvarTroca);
            rnsTroca.Add("ALTERAR", rnsAlterarTroca);

            rns.Add(nameof(Troca), rnsTroca);
            #endregion

            #region ItemPedido
            ItemPedidoDAO itemPedidoDAO = new ItemPedidoDAO();

            daos.Add(nameof(ItemPedido), itemPedidoDAO);

            rns.Add(nameof(ItemPedido), null);
            #endregion

            #region ItemBloqueado
            ItemBoqueadoDAO itemBoqueadoDAO = new ItemBoqueadoDAO();

            daos.Add(nameof(ItemBloqueado), itemBoqueadoDAO);

            rns.Add(nameof(ItemBloqueado), null);
            #endregion

            #region Cupom
            CupomDAO cupomDAO = new CupomDAO();

            daos.Add(nameof(Cupom), cupomDAO);

            rns.Add(nameof(Cupom), null);
            #endregion

            #region Estoque
            EstoqueDAO estoqueDAO = new EstoqueDAO();

            daos.Add(nameof(Estoque), estoqueDAO);

            rns.Add(nameof(Estoque), null);
            #endregion

            #region EntradaEstoque
            EntradaEstoqueDAO entradaEstoqueDAO = new EntradaEstoqueDAO();

            daos.Add(nameof(EntradaEstoque), entradaEstoqueDAO);

            //Regras de salvar
            ValidadorDadosObrigatoriosEntradaEstoque validadorDadosObrigatoriosEntradaEstoque = new ValidadorDadosObrigatoriosEntradaEstoque();
            ValidadorDataEntradaEstoque validadorDataEntradaEstoque = new ValidadorDataEntradaEstoque();

            List<IStrategy> rnsSalvarEntradaEstoque = new List<IStrategy>();

            rnsSalvarEntradaEstoque.Add(validadorDadosObrigatoriosEntradaEstoque);
            rnsSalvarEntradaEstoque.Add(validadorDataEntradaEstoque);

            Dictionary<string, List<IStrategy>> rnsEntradaEstoque = new Dictionary<string, List<IStrategy>>();

            rnsEntradaEstoque.Add("SALVAR", rnsSalvarEntradaEstoque);

            rns.Add(nameof(EntradaEstoque), rnsEntradaEstoque);
            #endregion

            #region Venda
            VendaDAO vendaDAO = new VendaDAO();

            daos.Add(nameof(Venda), vendaDAO);

            //Regras de consultar
            ValidadorPeriodoVenda validadorPeriodoVenda = new ValidadorPeriodoVenda();

            List<IStrategy> rnsConsultarVenda = new List<IStrategy>();

            rnsConsultarVenda.Add(validadorPeriodoVenda);

            Dictionary<string, List<IStrategy>> rnsVenda = new Dictionary<string, List<IStrategy>>();

            rnsVenda.Add("CONSULTAR", rnsConsultarVenda);

            rns.Add(nameof(Venda), rnsVenda);
            #endregion

            #region Faturamento
            FaturamentoDAO faturamentoDAO = new FaturamentoDAO();

            daos.Add(nameof(Faturamento), faturamentoDAO);

            //Regras de consultar
            ValidadorPeriodoFaturamento validadorPeriodoFaturamento = new ValidadorPeriodoFaturamento();

            List<IStrategy> rnsConsultarFaturamento = new List<IStrategy>();

            rnsConsultarFaturamento.Add(validadorPeriodoFaturamento);

            Dictionary<string, List<IStrategy>> rnsFaturamento = new Dictionary<string, List<IStrategy>>();

            rnsFaturamento.Add("CONSULTAR", rnsConsultarFaturamento);

            rns.Add(nameof(Faturamento), rnsFaturamento);
            #endregion

            #region Lucro
            LucroDAO lucroDAO = new LucroDAO();

            daos.Add(nameof(Lucro), lucroDAO);

            //Regras de consultar
            ValidadorPeriodoLucro validadorPeriodoLucro = new ValidadorPeriodoLucro();

            List<IStrategy> rnsConsultarLucro = new List<IStrategy>();

            rnsConsultarLucro.Add(validadorPeriodoLucro);

            Dictionary<string, List<IStrategy>> rnsLucro = new Dictionary<string, List<IStrategy>>();

            rnsLucro.Add("CONSULTAR", rnsConsultarLucro);

            rns.Add(nameof(Lucro), rnsLucro);
            #endregion

            #region Usuario 
            UsuarioDAO usuarioDAO = new UsuarioDAO();

            daos.Add(nameof(Usuario), usuarioDAO);

            //Regras de salvar

            inclusaoDataCadastro = new InclusaoDataCadastro();
            ValidadorDadosObrigatoriosUsuario validadorDadosObrigatoriosUsuario = new ValidadorDadosObrigatoriosUsuario();
            ValidadorDataNascimento validadorDataNascimento = new ValidadorDataNascimento();
            ValidadorCpf validadorCpf = new ValidadorCpf();
            ValidadorSenha validadorSenha = new ValidadorSenha();
            ValidadorDadosCartao validadorDadosCartao = new ValidadorDadosCartao();
            ValidadorDadosEndereco validadorDadosEndereco = new ValidadorDadosEndereco();
            ValidadorEmail validadorEmail = new ValidadorEmail();
          
            List<IStrategy> rnsSalvarUsuario = new List<IStrategy>();

            rnsSalvarUsuario.Add(inclusaoDataCadastro);
            rnsSalvarUsuario.Add(validadorDadosObrigatoriosUsuario);
            rnsSalvarUsuario.Add(validadorDataNascimento);
            rnsSalvarUsuario.Add(validadorCpf);
            rnsSalvarUsuario.Add(validadorSenha);
            rnsSalvarUsuario.Add(validadorDadosCartao);
            rnsSalvarUsuario.Add(validadorEmail);

            //Regras de alterar

            ValidadorDadosObrigatoriosTrocaSenha validadorDadosObrigatoriosTrocaSenha = new ValidadorDadosObrigatoriosTrocaSenha();
            ValidadorSenhaAtual validadorSenhaAtual = new ValidadorSenhaAtual();
            ValidadorTrocaSenha validadorTrocaSenha = new ValidadorTrocaSenha();
            ValidadorDadosObrigatoriosUsuarioEdicao validadorDadosObrigatoriosUsuarioEdicao = new ValidadorDadosObrigatoriosUsuarioEdicao();

            List<IStrategy> rnsAlterarUsuario = new List<IStrategy>();

            rnsAlterarUsuario.Add(validadorDadosObrigatoriosTrocaSenha);
            rnsAlterarUsuario.Add(validadorSenhaAtual);
            rnsAlterarUsuario.Add(validadorTrocaSenha);
            rnsAlterarUsuario.Add(validadorDadosObrigatoriosUsuarioEdicao);

            Dictionary<string, List<IStrategy>> rnsUsuario = new Dictionary<string, List<IStrategy>>();

            rnsUsuario.Add("SALVAR", rnsSalvarUsuario);
            rnsUsuario.Add("ALTERAR", rnsAlterarUsuario);

            rns.Add(nameof(Usuario), rnsUsuario);
            #endregion

            #region Endereco
            EnderecoDAO enderecoDAO = new EnderecoDAO();

            daos.Add(nameof(Endereco), enderecoDAO);

            //Regras de salvar

            inclusaoDataCadastro = new InclusaoDataCadastro();
            ValidadorDadosObrigatoriosEndereco validadorDadosObrigatoriosEndereco = new ValidadorDadosObrigatoriosEndereco();
            validadorDadosEndereco = new ValidadorDadosEndereco();

            List<IStrategy> rnsSalvarEndereco = new List<IStrategy>();

            rnsSalvarEndereco.Add(inclusaoDataCadastro);
            rnsSalvarEndereco.Add(validadorDadosObrigatoriosEndereco);
            rnsSalvarEndereco.Add(validadorDadosEndereco);

            //Regras de alterar

            List<IStrategy> rnsAlterarEndereco = new List<IStrategy>();

            rnsAlterarEndereco.Add(validadorDadosObrigatoriosEndereco);
            rnsAlterarEndereco.Add(validadorDadosEndereco);

            Dictionary<string, List<IStrategy>> rnsEndereco = new Dictionary<string, List<IStrategy>>();

            rnsEndereco.Add("SALVAR", rnsSalvarEndereco);
            rnsEndereco.Add("ALTERAR", rnsAlterarEndereco);

            rns.Add(nameof(Endereco), rnsEndereco);
            #endregion

            #region Cartao 
            CartaoDeCreditoDAO cartaoDeCreditoDAO = new CartaoDeCreditoDAO();

            daos.Add(nameof(CartaoDeCredito), cartaoDeCreditoDAO);

            //Regras de salvar

            inclusaoDataCadastro = new InclusaoDataCadastro();
            ValidadorDadosObrigatoriosCartao validadorDadosObrigatoriosCartao = new ValidadorDadosObrigatoriosCartao();
            validadorDadosCartao = new ValidadorDadosCartao();

            List<IStrategy> rnsSalvarCartaoDeCredito = new List<IStrategy>();

            rnsSalvarCartaoDeCredito.Add(inclusaoDataCadastro);
            rnsSalvarCartaoDeCredito.Add(validadorDadosObrigatoriosCartao);
            rnsSalvarCartaoDeCredito.Add(validadorDadosCartao);

            //Regras de alterar

            List<IStrategy> rnsAlterarCartaoDeCredito = new List<IStrategy>();

            rnsAlterarCartaoDeCredito.Add(validadorDadosObrigatoriosCartao);
            rnsAlterarCartaoDeCredito.Add(validadorDadosCartao);

            Dictionary<string, List<IStrategy>> rnsCartaoDeCredito = new Dictionary<string, List<IStrategy>>();

            rnsCartaoDeCredito.Add("SALVAR", rnsSalvarCartaoDeCredito);
            rnsCartaoDeCredito.Add("ALTERAR", rnsAlterarCartaoDeCredito);

            rns.Add(nameof(CartaoDeCredito), rnsCartaoDeCredito);
            #endregion

            #region Notificacao
            NotificacaoDAO notificacaoDAO = new NotificacaoDAO();

            daos.Add(nameof(Notificacao), notificacaoDAO);
            Dictionary<string, List<IStrategy>> rnsNotificacao = new Dictionary<string, List<IStrategy>>();

            rnsNotificacao.Add("CONSULTAR", null);

            rns.Add(nameof(Notificacao), rnsNotificacao);
            #endregion
        }
        public Result Alterar(EntidadeDominio entidade)
        {
            resultado = new Result();
            string nmClasse = entidade.GetType().Name;
            string msg = ExecutarRegras(entidade, "ALTERAR");

            if (msg == null)
            {
                IDAO dao;
                daos.TryGetValue(nmClasse, out dao);
                try
                {
                    dao.Alterar(entidade);
                    List<EntidadeDominio> entidades = new List<EntidadeDominio>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades;
                }
                catch (Exception e)
                {
                    resultado.Msg = e.Message;
                }
            }
            else
            {
                resultado.Msg = msg;
            }
            return resultado;
        }

        public Result Consultar(EntidadeDominio entidade)
        {
            resultado = new Result();
            string nmClasse = entidade.GetType().Name;
            string msg = ExecutarRegras(entidade, "CONSULTAR");

            if (msg == null)
            {
                IDAO dao;
                daos.TryGetValue(nmClasse, out dao);
                try
                {
                    resultado.Entidades = dao.Consultar(entidade);
                }
                catch (Exception e)
                {
                    resultado.Msg = e.Message;
                }
            }
            else
            {
                resultado.Msg = msg;
            }
            return resultado;
        }

        public Result Excluir(EntidadeDominio entidade)
        {
            resultado = new Result();
            string nmClasse = entidade.GetType().Name;
            string msg = ExecutarRegras(entidade, "EXCLUIR");

            if (msg == null)
            {
                IDAO dao;
                daos.TryGetValue(nmClasse, out dao);
                try
                {
                    dao.Excluir(entidade);
                    List<EntidadeDominio> entidades = new List<EntidadeDominio>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades;
                }
                catch (Exception e)
                {
                    resultado.Msg = e.Message;
                }
            }
            else
            {
                resultado.Msg = msg;
            }
            return resultado;
        }

        public Result Salvar(EntidadeDominio entidade)
        {
            resultado = new Result();
            string nmClasse = entidade.GetType().Name;
            string msg = ExecutarRegras(entidade, "SALVAR");

            if (msg == null)
            {
                IDAO dao;
                daos.TryGetValue(nmClasse, out dao);
                try
                {
                    dao.Salvar(entidade);
                    List<EntidadeDominio> entidades = new List<EntidadeDominio>();
                    entidades.Add(entidade);
                    resultado.Entidades = entidades;
                }
                catch (Exception e)
                {
                    resultado.Msg = e.Message;
                }
            }
            else
            {
                resultado.Msg = msg;
            }
            return resultado;
        }

        private string ExecutarRegras(EntidadeDominio entidade, string operacao)
        {
            string nmClasse = entidade.GetType().Name;
            StringBuilder msg = new StringBuilder();

            Dictionary<string, List<IStrategy>> regrasOperacao;
            rns.TryGetValue(nmClasse, out regrasOperacao);

            if (regrasOperacao != null)
            {
                List<IStrategy> regras;
                regrasOperacao.TryGetValue(operacao, out regras);

                if (regras != null)
                {
                    foreach (var s in regras)
                    {
                        string m = s.Processar(entidade);

                        if (m != null)
                        {
                            msg.Append(m);
                            msg.Append("\n");
                        }
                    }
                }
            }

            if (msg.Length > 0)
                return msg.ToString();
            else
                return null;
        }
    }
}
