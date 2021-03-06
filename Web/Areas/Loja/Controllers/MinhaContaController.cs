﻿using System.Collections.Generic;
using System.Linq;
using Core.Application;
using Core.Impl.Control;
using Domain.DadosCliente;
using Domain.Negocio;
using Domain.Produto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Util;

namespace Web.Areas.Loja.Controllers
{
    public class MinhaContaController : Controller
    {
        private Result resultado;

        [Area("Loja")]
        public IActionResult Index()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                List<Notificacao> notificacoes = new List<Notificacao>();

                resultado = new Facade().Consultar(new Notificacao { UsuarioId = HttpContext.Session.Get<int>("idUsuario"), Visualizada = 0 });
                if(resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(notificacoes);
                }
                if (resultado.Entidades.Count > 0)
                {
                    foreach (var item in resultado.Entidades)
                    {
                        notificacoes.Add((Notificacao)item);
                    }
                }
                return View(notificacoes);
            }
            else
                return RedirectToAction("Login", "Conta");
        }

        [Area("Loja")]
        public IActionResult MeusPedidos()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Pedido { UsuarioId = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View("meusPedidos", new List<Pedido>());
                }
                if (resultado.Entidades.Count == 0)
                {
                    ViewBag.Mensagem = "No momento você não possui pedidos.";
                    return View("meusPedidos", new List<Pedido>());
                }
                List<Pedido> pedidos = new List<Pedido>();
                foreach (var item in resultado.Entidades)
                {
                    pedidos.Add((Pedido)item);
                }

                return View("meusPedidos", pedidos);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult Detalhes(int id)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                List<Pedido> pedidos;
                List<Livro> produtos = new List<Livro>();

                resultado = new Facade().Consultar(new Pedido { Id = id });
                if (!string.IsNullOrEmpty(resultado.Msg))
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View("detalhes", new Pedido());
                }

                pedidos = new List<Pedido>();
                foreach (var item in resultado.Entidades)
                {
                    pedidos.Add((Pedido)item);
                }

                foreach (var item in pedidos.FirstOrDefault().ItensPedido)
                {
                    resultado = new Facade().Consultar(new Livro { Id = item.Id });
                    if (!string.IsNullOrEmpty(resultado.Msg))
                    {
                        ViewBag.Mensagem = resultado.Msg;
                        return View("detalhes", new Pedido());
                    }
                    foreach (var livro in resultado.Entidades)
                    {
                        produtos.Add((Livro)livro);
                    }
                }

                foreach (var item in pedidos.FirstOrDefault().ItensPedido)
                {
                    foreach (var prod in produtos)
                    {
                        if (prod.Id == item.Id)
                            item.Produto.Nome = prod.Nome;
                    }
                }

                return View("detalhes", pedidos.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public JsonResult EnviarSolicitacaoTroca(int itemId, int qtde, int pedidoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                string msg;

                resultado = new Facade().Consultar(new Troca { ItemId = itemId, PedidoId = pedidoId });
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                else if (resultado.Entidades.Count > 0)
                {
                    msg = "Já existe uma solicitação de troca em andamento para este item.";
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }

                Troca troca = new Troca
                {
                    ItemId = itemId,
                    PedidoId = pedidoId,
                    Qtde = qtde,
                    Status = 'P'
                };
                resultado = new Facade().Salvar(troca);
                if (resultado.Msg != null)
                    msg = resultado.Msg;
                else
                    msg = "Troca solicitada com sucesso!";

                return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }

        [Area("Loja")]
        public IActionResult MinhasTrocas()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Troca { UsuarioId = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View("minhasTrocas", new List<Troca>());
                }
                if (resultado.Entidades.Count == 0)
                {
                    ViewBag.Mensagem = "No momento você não possui solicitações de troca.";
                    return View("minhasTrocas", new List<Troca>());
                }
                List<Troca> trocas = new List<Troca>();
                foreach (var item in resultado.Entidades)
                {
                    trocas.Add((Troca)item);
                }

                return View("minhasTrocas", trocas);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult Cupons()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Cupom { UsuarioId = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View("minhasTrocas", new List<Cupom>());
                }
                if (resultado.Entidades.Count == 0)
                {
                    ViewBag.Mensagem = "No momento você não possui solicitações de troca.";
                    return View("minhasTrocas", new List<Cupom>());
                }
                List<Cupom> cupons = new List<Cupom>();
                foreach (var item in resultado.Entidades)
                {
                    cupons.Add((Cupom)item);
                }

                return View(cupons);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult OcultarNotificacao(int notificacaoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Alterar(new Notificacao { Id = notificacaoId, Visualizada = 1 });
                if (resultado.Msg != null)
                    ViewBag.Mensagem = resultado.Msg;
                return RedirectToAction("Index", "MinhaConta");
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public JsonResult CancelarPedido(int pedidoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Pedido { Id = pedidoId });
                List<Pedido> pedidos = new List<Pedido>();
                string msg;

                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                else
                {
                    foreach (var item in resultado.Entidades)
                    {
                        pedidos.Add((Pedido)item);
                    }
                }
                resultado = new Facade().Alterar(new Pedido { Id = pedidoId, Status = 'C', ItensPedido = pedidos.FirstOrDefault().ItensPedido});
                if (resultado.Msg != null)
                    msg = resultado.Msg;
                else
                    msg = "Pedido " + pedidoId + " cancelado com sucesso!";
                return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }

        [Area("Loja")]
        public IActionResult DadosPessoais()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }
                return View(usuarios.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }
        [Area("Loja")]
        public IActionResult EditarDadosPessoais()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }
                return View(usuarios.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }
        [Area("Loja")]
        [HttpPost]
        public IActionResult EditarDadosPessoais(Usuario usuario)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                usuario.Id = HttpContext.Session.Get<int>("idUsuario");
                usuario.DadosAlterados = "PESSOAIS";
                resultado = new Facade().Alterar(usuario);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(usuario);
                }
                ViewBag.Mensagem = "Dados pessoais alterados com sucesso!";
                return View(usuario);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult AlterarSenha()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }
                return View(usuarios.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        [HttpPost]
        public IActionResult AlterarSenha(Usuario usuario)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                usuario.Id = HttpContext.Session.Get<int>("idUsuario");
                usuario.DadosAlterados = "SENHA";
                resultado = new Facade().Alterar(usuario);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(usuario);
                }
                ViewBag.Mensagem = "Senha alterada com sucesso!";
                return View(usuario);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult DadosPagamento()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new CartaoDeCredito { UsuarioId = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<CartaoDeCredito> cartoes = new List<CartaoDeCredito>();
                foreach (var item in resultado.Entidades)
                {
                    cartoes.Add((CartaoDeCredito)item);
                }

                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }

                ViewBag.CartaoPreferencial = usuarios.FirstOrDefault().CartaoPreferencial;
                return View(cartoes);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult DetalhesCartao(int id)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new CartaoDeCredito { Id = id });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<CartaoDeCredito> cartoes = new List<CartaoDeCredito>();
                foreach (var item in resultado.Entidades)
                {
                    cartoes.Add((CartaoDeCredito)item);
                }
                return View(cartoes.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult EditarCartao(int CartaoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new CartaoDeCredito { Id = CartaoId });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<CartaoDeCredito> cartoes = new List<CartaoDeCredito>();
                foreach (var item in resultado.Entidades)
                {
                    cartoes.Add((CartaoDeCredito)item);
                }
                return View(cartoes.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        [HttpPost]
        public IActionResult EditarCartao(CartaoDeCredito cartao)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Alterar(cartao);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(cartao);
                }
                ViewBag.Mensagem = "Alterações salvas com sucesso!";
                return View(cartao);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult ExcluirCartao(int CartaoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                string msg;
                resultado = new Facade().Consultar(new CartaoDeCredito { Id = CartaoId });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<CartaoDeCredito> cartoes = new List<CartaoDeCredito>();
                foreach (var item in resultado.Entidades)
                {
                    cartoes.Add((CartaoDeCredito)item);
                }
                cartoes.FirstOrDefault().Ativo = 0;

                resultado = new Facade().Alterar(cartoes.FirstOrDefault());
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                msg = "Cartão excluído com sucesso!";
                return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }

        [Area("Loja")]
        public IActionResult AlterarCartaoPreferencial(int CartaoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                string msg;

                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario") });
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }
                usuarios.FirstOrDefault().CartaoPreferencial = CartaoId;
                usuarios.FirstOrDefault().DadosAlterados = "CARTAO";
                resultado = new Facade().Alterar(usuarios.FirstOrDefault());
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                msg = "Cartão preferencial alterado com sucesso!";
                return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }

        [Area("Loja")]
        public IActionResult SalvarCartao()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                return View(new CartaoDeCredito());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        [HttpPost]
        public IActionResult SalvarCartao(CartaoDeCredito cartao)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                cartao.UsuarioId = HttpContext.Session.Get<int>("idUsuario");
                resultado = new Facade().Salvar(cartao);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(cartao);
                }
                ViewBag.Mensagem = "Cartão cadastrado com sucesso!";
                return View(cartao);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult Enderecos()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Endereco { UsuarioId = HttpContext.Session.Get<int>("idUsuario"), Ativo = 1 });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Endereco> enderecos = new List<Endereco>();
                foreach (var item in resultado.Entidades)
                {
                    enderecos.Add((Endereco)item);
                }
                return View(enderecos);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult DetalhesEndereco(int id)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Endereco { Id = id });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Endereco> enderecos = new List<Endereco>();
                foreach (var item in resultado.Entidades)
                {
                    enderecos.Add((Endereco)item);
                }
                return View(enderecos.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult EditarEndereco(int EnderecoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Consultar(new Endereco { Id = EnderecoId });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Endereco> enderecos = new List<Endereco>();
                foreach (var item in resultado.Entidades)
                {
                    enderecos.Add((Endereco)item);
                }
                return View(enderecos.FirstOrDefault());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        [HttpPost]
        public IActionResult EditarEndereco(Endereco endereco)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                resultado = new Facade().Alterar(endereco);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(endereco);
                }
                ViewBag.Mensagem = "Alterações salvas com sucesso!";
                return View(endereco);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult ExcluirEndereco(int EnderecoId)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                string msg;
                resultado = new Facade().Consultar(new Endereco { Id = EnderecoId });
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View();
                }
                List<Endereco> enderecos = new List<Endereco>();
                foreach (var item in resultado.Entidades)
                {
                    enderecos.Add((Endereco)item);
                }
                enderecos.FirstOrDefault().Ativo = 0;

                resultado = new Facade().Alterar(enderecos.FirstOrDefault());
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                msg = "Endereço excluído com sucesso!";
                return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }

        [Area("Loja")]
        public IActionResult SalvarEndereco()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                return View(new Endereco());
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        [HttpPost]
        public IActionResult SalvarEndereco(Endereco endereco)
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                endereco.UsuarioId = HttpContext.Session.Get<int>("idUsuario");
                resultado = new Facade().Salvar(endereco);
                if (resultado.Msg != null)
                {
                    ViewBag.Mensagem = resultado.Msg;
                    return View(endereco);
                }
                ViewBag.Mensagem = "Endereço cadastrado com sucesso!";
                return View(endereco);
            }
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public IActionResult InativarConta()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
                return View();
            else
                return RedirectToAction("Logar", "Conta");
        }

        [Area("Loja")]
        public JsonResult Inativar()
        {
            if (HttpContext.Session.Get<int>("idUsuario") > 0)
            {
                ViewBag.NomeUsuario = HttpContext.Session.GetString("nomeUsuario");
                string msg;
                resultado = new Facade().Consultar(new Usuario { Id = HttpContext.Session.Get<int>("idUsuario")});
                if (resultado.Msg != null)
                {
                    msg = resultado.Msg;
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                List<Usuario> usuarios = new List<Usuario>();
                foreach (var item in resultado.Entidades)
                {
                    usuarios.Add((Usuario)item);
                }
                if (usuarios.FirstOrDefault().Ativo > 0)
                {
                    usuarios.FirstOrDefault().Ativo = 0;
                    resultado = new Facade().Alterar(usuarios.FirstOrDefault());
                    if (resultado.Msg != null)
                    {
                        msg = resultado.Msg;
                        return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                    }
                    msg = "Conta inativada com sucesso!";
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
                else
                {
                    msg = "A conta ja está inativada";
                    return Json("{\"Mensagem\":" + "\"" + msg.Replace("\n", " ") + "\"}");
                }
            }
            else
                return Json("{\"Mensagem\":" + "\"" + "Erro: O usuário não está logado" + "\"}");
        }
    }
}
