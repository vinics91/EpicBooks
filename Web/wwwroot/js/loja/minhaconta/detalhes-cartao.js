﻿$(document).ready(function () {
    if ($('#textoModal').text() !== undefined && $('#textoModal').text().trim() !== '')
        $('#modalMensagem').modal('show');

    $('#btnOkModal').on('click', function () {
        if ($('#textoModal').text() === 'Cartão excluído com sucesso!' || $('#textoModal').text() === 'Cartão cadastrado com sucesso!' ||
            $('#textoModal').text() === 'Alterações salvas com sucesso!')
            location.href = '/Loja/MinhaConta/DadosPagamento/';
    });
     
    $('#btnExcluirCartao').on('click', function () {
        $('#textoModalConfirmacao').text('Deseja realmente excluir esse cartão?');
        $('#modalConfirmacao').modal('show');
    });

    $('#btnSimModal').on('click', function () {
        $.ajax({
            type: "get",
            url: "/Loja/MinhaConta/ExcluirCartao/",
            data: { CartaoId: $('#btnExcluirCartao').val() },
            dataType: "json",
            success: function (response) {
                var text = JSON.parse(response);
                if (text.Mensagem !== undefined && text.Mensagem !== '') {
                    $('#textoModal').text(text.Mensagem);
                    $('#modalMensagem').modal('show');
                }
            }
        });
    });
});