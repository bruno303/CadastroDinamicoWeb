﻿//Geral
$(document).ready(() => {
    $("#divColunas").attr('hidden', true);
    $("#divFk").attr('hidden', true);

    consultarDatabases();

    $("#selDatabases").change(() => {
        $("#divFk").attr('hidden', true);
        $("#divColunas").attr('hidden', true);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        $("#selSchemas > option").remove();
        consultarSchemas();
    });

    $("#selSchemas").change(() => {
        $("#divFk").attr('hidden', true);
        $("#divColunas").attr('hidden', true);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        $("#selTabelas > option").remove();
        consultarTabelas();
    });

    $("#selTabelas").change(() => {
        $("#divFk").attr('hidden', true);
        $("#divColunas").attr('hidden', true);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        consultarColunas();
    });
});

function successConsultaDatabase(data) {
    $("#selDatabases > option").remove();
    fillDropDown("#selDatabases", data);
    consultarSchemas();
}

function successConsultaSchema(data) {
    $("#selSchemas > option").remove();
    fillDropDown("#selSchemas", data);
    consultarTabelas();
}

function successConsultaTabela(data) {
    $("#selTabelas > option").remove();
    fillDropDown("#selTabelas", data);
    consultarColunas();
}

function successConsultarColunas(data) {
    if (data.length > 0) {
        $('#divColunas').attr('hidden', false);
    }
    $("#tblColunas > tbody tr").remove();
    $.each(data, function (i, coluna) {
        $("#tblColunas > tbody").append('<tr><td>' + coluna.name + '</td><td><input type="checkbox" checked="' + coluna.visivel + '" /></td></tr>');
    });
    consultarColunasChaveEstrangeira();
}

function successConsultaChavesEstrangeiras(data) {
    if (data.length > 0) {
        $("#divFk").attr('hidden', false);
    }
    $("#tblChavesEstrangeiras > tbody tr").remove();
    $.each(data, function (i, coluna) {
        let opcoes = '';
        $.each(coluna.colunasTabelaReferenciada, (i, col) => {
            opcoes += '<option value="' + col + '">' + col + '</option>'
        });
        $("#tblChavesEstrangeiras > tbody").append('<tr><td>' + coluna.nome + '</td><td>' + coluna.tabelaReferenciada +
            '</td><td>' + coluna.colunaReferenciada + '</td><td><select class="custom-select" id="' + 'select-' + i +
            '">' + opcoes + '</select></tr>');
        $('#select-' + i)[0].selectedIndex = coluna.indiceColTabelaReferenciada;
    });
    awaitLoad(false);
}

// Databases
function consultarDatabases() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarDatabases",
        success: function (data) {
            successConsultaDatabase(data);
            console.log('Success invocado!');
        },
        error: function (err) {
            alert("Houve um erro ao consultar as databases.");
        }
    })
}

// Schemas
function consultarSchemas() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarSchemas?database=" + $("#selDatabases")[0].value,
        success: function (data) {
            successConsultaSchema(data);
        },
        error: function (err) {
            alert("Houve um erro ao consultar os schemas.");
        }
    })
}

// Tabelas
function consultarTabelas() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarTabelas?database=" + $("#selDatabases")[0].value +
            "&schema=" + $("#selSchemas")[0].value,
        success: function (data) {
            successConsultaTabela(data);
        },
        error: function (err) {
            alert("Houve um erro ao consultar as tabelas.");
        }
    })
}

// Colunas
function consultarColunas() {
    if ($("#selTabelas")[0].value !== '') {
        $.ajax({
            method: "GET",
            dataType: "JSON",
            url: "/CadDinamico/SelecionarColunas?database=" + $("#selDatabases")[0].value +
                "&schema=" + $("#selSchemas")[0].value +
                "&tabela=" + $("#selTabelas")[0].value,
            success: function (data) {
                successConsultarColunas(data);
            },
            error: function (err) {
                alert("Houve um erro ao consultar as colunas.");
            },
            complete: (jqXHR) => {
                awaitLoad(false);
            },
            beforeSend: () => {
                awaitLoad(true);
            }
        })
    }
    else {
        $("#divColunas").attr('hidden', true);
        $("#tblColunas > tbody tr").remove();
    }
}

function consultarColunasChaveEstrangeira() {
    if ($("#selTabelas")[0].value !== '') {
        $.ajax({
            method: "GET",
            dataType: "JSON",
            url: "/CadDinamico/SelecionarColunasChaveEstrangeira?database=" + $("#selDatabases")[0].value +
                "&schema=" + $("#selSchemas")[0].value +
                "&tabela=" + $("#selTabelas")[0].value,
            error: function (err) {
                $("#divFk").attr('hidden', true);
                alert("Houve um erro ao consultar as colunas de chave estrangeira.");
            },
            complete: (jqXHR) => {
                awaitLoad(false);
            },
            success: function(data) {
                successConsultaChavesEstrangeiras(data);
            },
            beforeSend: () => {
                $("#divFk").attr('hidden', true);
                awaitLoad(true);
            }
        })
    }
    else {
        $("#divFk").attr('hidden', true);
        $("#tblChavesEstrangeiras > tbody tr").remove();
    }
}

function awaitLoad(enabled) {
    $("#selDatabases")[0].disabled = enabled;
    $("#selSchemas")[0].disabled = enabled;
    $("#selTabelas")[0].disabled = enabled;
}

function fillDropDown(select, data) {
    if (data.length > 0) {
        $.each(data, function (i, dado) {
            $(select).append('<option value="' + dado.name + '">' + dado.name + '</option>');
        })
    }
}

function gravar() {
    let dados = '';
    let aux = $("#selDatabases")[0].value + ";" + $("#selSchemas")[0].value + ";" +
        $("#selTabelas")[0].value + "|";
    let linhas = $("#tblColunas tbody > tr");
    $.each(linhas, (i, registro) => {
        if (registro.getElementsByTagName("input")[0].checked) {
            dados += registro.innerHTML.replace(/<\/td>/g, "").split("<td>")[1] + ';';
        }
    });
    if (dados !== '') {
        dados = aux + dados;
        dados = dados.substr(0, dados.length - 1);
    }

    if (dados === undefined || dados === null || dados === '') {
        alert('Selecione ao menos uma coluna que deseja exibir!');
    }
    else {
        /* Dados chave estrangeira */
        let dadosfk = '';
        linhas = $("#tblChavesEstrangeiras tbody > tr");
        $.each(linhas, (i, registro) => {
            let value = registro.getElementsByTagName("select")[0].value;
            dadosfk += registro.innerHTML.replace(/<\/td>/g, "").split("<td>")[1] + ':' + value + ";";
        });
        if (dadosfk !== '') {
            dadosfk = dadosfk.substr(0, dadosfk.length - 1);
        }
        /* Enviar por ajax */
        $.ajax({
            method: "GET",
            url: "/CadDinamico/GravarConfiguracoesTabela?dados=" + dados + "&dadosfk=" + dadosfk,
            sucess: function (data) {
                alert("Configurações salvas com sucesso!");
            },
            error: function (err) {
                alert("Erro ao salvar as configurações.");
            },
            complete: function (jqXHR) {
                if (jqXHR.readyState === 4) {
                    alert("Configurações salvas com sucesso!");
                }
            }
        });
    }
}