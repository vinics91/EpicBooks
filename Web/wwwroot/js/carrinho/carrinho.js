﻿$(document).ready(function () {
    if ($('#chkDoisCartoes').prop('checked')) {
        $('#pagarDoisCartoes').removeClass('d-none');
        $('.valorCartao').removeClass('d-none');
    }

    $('#chkDoisCartoes').on('change', function () {
        if ($('#chkDoisCartoes').prop('checked')) {
            $('#pagarDoisCartoes').removeClass('d-none');
            $('.valorCartao').removeClass('d-none');
        }
        else {
            $('.valorCartao input[type="text"]').val('');
            $('#pagarDoisCartoes input[type="text"]').val('');
            $('#pagarDoisCartoes option').prop('selected', false);
            $('#pagarDoisCartoes').addClass('d-none');
            $('.valorCartao').addClass('d-none');
        }
    });

    $('#itensPedido').on('click', '.btnAumentarQtde', function () {
        $.ajax({
            type: "get",
            url: "/Loja/Carrinho/AumentarQtde/",
            data: { id: $(this).val() },
            dataType: "html",
            success: function (response) {
                $("#itensPedido").html(response);
                $(document).ready();
            }
        });
        CalcularFrete($('#selectEndereco').val());
    });

    $('#itensPedido').on('click', '.btnDiminuirQtde', function () {
        $.ajax({
            type: "get",
            url: "/Loja/Carrinho/DiminuirQtde/",
            data: { id: $(this).val() },
            dataType: "html",
            success: function (response) {
                $("#itensPedido").html(response);
            }
        });
        CalcularFrete($('#selectEndereco').val());
    });

    $('#itensPedido').on('click', '.btnRemoverItem', function () {
        $.ajax({
            type: "get",
            url: "/Loja/Carrinho/RemoverItem/",
            data: { id: $(this).val() },
            dataType: "html",
            success: function (response) {
                $("#itensPedido").html(response);
            }
        });
        CalcularFrete();
    });

    $('#btnCalcFrete').on('click', function () {
        if ($('#inputCep').val() !== undefined && $('#inputCep').val().trim() !== '') {
            CalcularFrete();
        }
    });

    $('#selectEndereco').on('change', function () {
        CalcularFrete($(this).val());
    });
});

function CalcularFrete(cep) {
    if (cep === undefined) {
        $.ajax({
            type: "get",
            url: "/Loja/Carrinho/CalcularFrete/",
            data: { cep: $('#inputCep').val() },
            dataType: "json",
            success: function (data) {
                var retorno = JSON.parse(data);
                $("#valorFrete").html('R$ ' + retorno.valor);
            }
        });
    }
    else{
        $.ajax({
            type: "get",
            url: "/Loja/Carrinho/CalcularFrete/",
            data: { cep: cep },
            dataType: "json",
            success: function (data) {
                var retorno = JSON.parse(data);
                $("#valorFrete").html('R$ ' + retorno.valor);
            }
        });
    }
}