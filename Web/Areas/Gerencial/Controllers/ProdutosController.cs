﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Application;
using Core.Impl.Control;
using Domain.Produto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Gerencial.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private Result resultado;

        public ProdutosController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        [Area("Gerencial")]
        public IActionResult Index()
        {
            Livro livro = new Livro();
            List<Livro> livros;

            resultado = new Facade().Consultar(livro);
            if (!string.IsNullOrEmpty(resultado.Msg))
            {
                TempData["MsgErro"] = resultado.Msg;
                return View();
            }
            else
            {
                TempData["MsgSucesso"] = resultado.Msg;
                livros = new List<Livro>();
                foreach (var item in resultado.Entidades)
                {
                    livros.Add((Livro)item);
                }
                return View(livros);
            }
        }

        [Area("Gerencial")]
        public IActionResult Cadastrar()
        {
            return View(new Livro());
        }

        [Area("Gerencial")]
        [HttpPost]
        public async Task<IActionResult> CadastrarAsync(Livro livro)
        {
            string pastaDestino = "produtos";
            string nomeArquivo = Path.GetRandomFileName();
            if (livro.Imagem.FileName.Contains("jpg"))
                nomeArquivo += ".jpg";
            else
                return BadRequest();

            string caminho_WebRoot = _appEnvironment.WebRootPath;
            string caminhoCurtoENomeArquivo = "\\img\\uploads\\" + pastaDestino + "\\" + nomeArquivo;
            string caminhoDestinoArquivoCompleto = caminho_WebRoot + caminhoCurtoENomeArquivo;
            using (var stream = new FileStream(caminhoDestinoArquivoCompleto, FileMode.Create))
            {
                await livro.Imagem.CopyToAsync(stream);
            }

            if (!string.IsNullOrEmpty(caminhoCurtoENomeArquivo.Trim()))
                livro.CaminhoImagem = caminhoCurtoENomeArquivo;

            resultado = new Facade().Salvar(livro);
            if (!string.IsNullOrEmpty(resultado.Msg))
            {
                TempData["MsgErro"] = resultado.Msg;
                return View();
            }
            else
            {
                TempData["MsgSucesso"] = resultado.Msg;
                return RedirectToAction("Index", "Produtos", new { area = "Gerencial" });
            }
        }

        [Area("Gerencial")]
        public IActionResult Detalhes(int id)
        {
            Livro livro = new Livro();
            List<Livro> livros;
            livro.Id = id;
            resultado = new Facade().Consultar(livro);
            if (!string.IsNullOrEmpty(resultado.Msg))
            {
                TempData["MsgErro"] = resultado.Msg;
                return View();
            }
            else
            {
                TempData["MsgSucesso"] = resultado.Msg;
                livros = new List<Livro>();
                foreach (var item in resultado.Entidades)
                {
                    livros.Add((Livro)item);
                }
                return View(livros.FirstOrDefault());
            }
        }
    }
}