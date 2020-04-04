﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Application;
using Core.Impl.Business;
using Core.Impl.Control;
using Domain.Negocio;
using Domain.Produto;
using Microsoft.AspNetCore.Mvc;
using Web.Util;

namespace Web.Areas.Loja.Controllers
{
    public class CarrinhoController : Controller
    {
        private Result resultado;

        [Area("Loja")]
        public IActionResult Index()
        {
            Carrinho carrinho = new Carrinho();

            if (HttpContext.Session.Keys.Count() > 0)
                carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");//Se o carrinho existir, carrega
            else
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);//Senao, cria

            foreach (var item in carrinho.ItensPedido)
            {
                carrinho.QtdeTotalItens += item.Qtde;
            }
            if (!string.IsNullOrEmpty(carrinho.Cep) && !string.IsNullOrWhiteSpace(carrinho.Cep))
                carrinho.Frete = CalculoFrete.Calcular(carrinho.Cep, carrinho.ItensPedido.Count());

            return View(carrinho);
        }

        [Area("Loja")]
        public IActionResult AdicionarItem(int id)
        {
            List<Livro> livros;
            Carrinho carrinho;
            ItemPedido itemPedido = new ItemPedido();
            Livro livro = new Livro
            {
                Id = id
            };

            resultado = new Facade().Consultar(livro);
            if (!string.IsNullOrEmpty(resultado.Msg))
            {
                TempData["MsgErro"] = resultado.Msg;
                return PartialView("_produtos");
            }
            else
            {
                livros = new List<Livro>();
                foreach (var item in resultado.Entidades)
                {
                    livros.Add((Livro)item);
                }
                itemPedido.Qtde = 1;
                itemPedido.HoraInclusao = DateTime.Now;
                itemPedido.Produto = livros.FirstOrDefault();

                if (HttpContext.Session.Keys.Count() > 0)
                    carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");//Se o carrinho existir, carrega
                else
                    SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", new Carrinho());//Senao, cria

                carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");

                ItemPedido itemPed = carrinho.ItensPedido.Find(x => x.Produto.Id == id);//Verifica se o produto ja esta no carrinho

                if (itemPed != null)
                    carrinho.ItensPedido[carrinho.ItensPedido.IndexOf(itemPed)].Qtde += 1;//Se sim, incrementa qtde em 1
                else
                    carrinho.ItensPedido.Add(itemPedido);//Senao, adiciona o novo item ao carrinho

                carrinho.QtdeTotalItens += 1;
                carrinho.HoraUltimaInclusao = DateTime.Now;
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);

                return RedirectToAction("index");
            }
        }

        [Area("Loja")]
        public PartialViewResult AumentarQtde(int id)
        {
            Carrinho carrinho;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");

            ItemPedido itemPed = carrinho.ItensPedido.Find(x => x.Produto.Id == id);//Verifica se o produto ja esta no carrinho
            carrinho.ItensPedido[carrinho.ItensPedido.IndexOf(itemPed)].Qtde += 1;//Se sim, incrementa qtde em 1
            carrinho.QtdeTotalItens += 1;

            SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);

            return PartialView("_itensPedido", carrinho.ItensPedido);
        }

        [Area("Loja")]
        public PartialViewResult DiminuirQtde(int id)
        {
            Carrinho carrinho;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");

            ItemPedido itemPed = carrinho.ItensPedido.Find(x => x.Produto.Id == id);//Verifica se o produto ja esta no carrinho
            if (carrinho.ItensPedido[carrinho.ItensPedido.IndexOf(itemPed)].Qtde > 1)
            {
                carrinho.ItensPedido[carrinho.ItensPedido.IndexOf(itemPed)].Qtde -= 1;//Decrementa qtde em 1 se a qtde for maior que 1
                carrinho.QtdeTotalItens -= 1;
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);
            }

            return PartialView("_itensPedido", carrinho.ItensPedido);
        }

        [Area("Loja")]
        public PartialViewResult RemoverItem(int id)
        {
            Carrinho carrinho;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");

            ItemPedido itemPed = carrinho.ItensPedido.Find(x => x.Produto.Id == id);//Verifica se o produto ja esta no carrinho
            if (itemPed != null)
            {
                carrinho.QtdeTotalItens -= itemPed.Qtde;
                carrinho.ItensPedido.Remove(itemPed);//Decrementa qtde em 1 se a qtde for maior que 1
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);
            }

            return PartialView("_itensPedido", carrinho.ItensPedido);
        }
        public class Teste
        {
            public string Valor { get; set; }
        }
        [Area("Loja")]
        public JsonResult CalcularFrete(string cep)
        {
            Carrinho carrinho;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");
            carrinho.Frete = CalculoFrete.Calcular(cep, carrinho.QtdeTotalItens);
            SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);

            return Json("{\"valor\":" + "\"" + carrinho.Frete + "\"}");
        }

        [Area("Loja")]
        public JsonResult AdicionarCupomTroca(string codCupom)
        {
            Carrinho carrinho;
            Cupom cupom;
            //List<Cupom> cupons;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");
            cupom = carrinho.CuponsTroca.Find(x => x.CodigoCupom == codCupom);//Verifica se o cupom ja foi adicionado
            if (cupom == null)
            {
                //consultar cupom no banco
                //cupom.CodigoCupom = codCupom;
                //resultado = new Facade().Consultar(cupom);
                //if (!string.IsNullOrEmpty(resultado.Msg))
                //{
                //    TempData["MsgErro"] = resultado.Msg;
                //    return Json("{ }");
                //}
                //else
                //{
                //    cupons = new List<Cupom>();
                //    foreach (var item in resultado.Entidades)
                //    {
                //        cupons.Add((Cupom)item);
                //    }
                //    cupom = cupons.FirstOrDefault();
                //}
                cupom = new Cupom //teste temporario
                {
                    Id = 23,
                    CodigoCupom = codCupom,
                    Status = 1,
                    TipoCupom = 'T',
                    ValorCupom = 25.00,
                    UsuarioId = 1
                };
                carrinho.CuponsTroca.Add(cupom);//Se nao foi, adiciona
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);
                return Json("{\"adicionado\":\"1\"}");
            }
            else
                return Json("{\"adicionado\":\"0\"}");
        }

        [Area("Loja")]
        public JsonResult AdicionarCupomPromo(string codCupom)
        {
            Carrinho carrinho;
            Cupom cupom;
            //List<Cupom> cupons;

            carrinho = SessionHelper.Get<Carrinho>(HttpContext.Session, "carrinho");
            if (carrinho.CupomPromocional.CodigoCupom != codCupom)
            {
                //consultar cupom no banco
                //cupom.CodigoCupom = codCupom;
                //resultado = new Facade().Consultar(cupom);
                //if (!string.IsNullOrEmpty(resultado.Msg))
                //{
                //    TempData["MsgErro"] = resultado.Msg;
                //    return Json("{ }");
                //}
                //else
                //{
                //    cupons = new List<Cupom>();
                //    foreach (var item in resultado.Entidades)
                //    {
                //        cupons.Add((Cupom)item);
                //    }
                //    cupom = cupons.FirstOrDefault();
                //}
                cupom = new Cupom //teste temporario
                {
                    Id = 23,
                    CodigoCupom = codCupom,
                    Status = 1,
                    TipoCupom = 'P',
                    ValorCupom = 25.00
                };
                carrinho.CupomPromocional = cupom;
                SessionHelper.Set<Carrinho>(HttpContext.Session, "carrinho", carrinho);
                return Json("{\"adicionado\":\"1\"}");
            }

            return Json("{\"adicionado\":\"0\"}");
        }
        //[Area("Loja")]
        //public PartialViewResult AdicionarCartaoCredito(int idCartao, double valor)
        //{
        //}
    }
}