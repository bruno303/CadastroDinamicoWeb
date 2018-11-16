//Geral
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
        let desabilitado = ((!coluna.podeOcultar) ? ' disabled ' : '');

        $("#tblColunas > tbody").append(
            '<tr><td>' + coluna.name + '</td>' + 
                '<td class="text-center"><input type="checkbox" ' + desabilitado + (coluna.visivel ? 'checked="checked" ' : '') + '/></td>' +
                '<td class="text-center"><input type="checkbox" ' + (coluna.filtro ? 'checked="checked" ' : '') + '/></td>' +
            '</tr>');
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
            '.' + coluna.colunaReferenciada + '</td><td><select class="custom-select" id="' + 'select-' + i +
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
        },
        error: function (err) {
            alert("Houve um erro ao consultar as databases.");
        }
    });
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
    });
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
    });
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
                bloquearTela();
                awaitLoad(true);
            }
        });
    }
    else {
        desbloquearTela();
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
                desbloquearTela();
                awaitLoad(false);
            },
            success: function(data) {
                successConsultaChavesEstrangeiras(data);
            },
            beforeSend: () => {
                $("#divFk").attr('hidden', true);
                awaitLoad(true);
            }
        });
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
        });
    }
}

function bloquearTela()
{
    $.blockUI({ message: '<h1><img src="../images/load.gif" /> Carregando...</h1>' });
}

function desbloquearTela()
{
    $.unblockUI();
}

function gravar() {
    let vdados = '';
    let aux = $("#selDatabases")[0].value + ";" + $("#selSchemas")[0].value + ";" +
        $("#selTabelas")[0].value + "|";
    let linhas = $("#tblColunas tbody > tr");
    $.each(linhas, (i, registro) => {
        if (registro.getElementsByTagName("input")[0].checked) {
            vdados += registro.innerHTML.replace(/<\/td>/g, "").replace('<td class="text-center">', '<td>').split('<td>')[1] + ';';
        }
    });
    if (vdados !== '') {
        vdados = aux + vdados;
        vdados = vdados.substr(0, vdados.length - 1);
    }

    if (vdados === undefined || vdados === null || vdados === '') {
        alert('Selecione ao menos uma coluna que deseja exibir!');
    }
    else {
        /* Dados chave estrangeira */
        let vdadosfk = '';
        linhas = $("#tblChavesEstrangeiras tbody > tr");
        $.each(linhas, (i, registro) => {
            let value = registro.getElementsByTagName("select")[0].value;
            vdadosfk += registro.innerHTML.replace(/<\/td>/g, "").split("<td>")[1] + ':' + value + ";";
        });
        if (vdadosfk !== '') {
            vdadosfk = vdadosfk.substr(0, vdadosfk.length - 1);
        }

        /* Dados colunas de filtro */
        let vdadosfiltro = '';
        linhas = $("#tblColunas tbody > tr");
        $.each(linhas, (i, registro) => {
            if (registro.getElementsByTagName("input")[1].checked) {
                vdadosfiltro += registro.innerHTML.replace(/<\/td>/g, "").replace('<td class="text-center">', '<td>').split('<td>')[1] + ';';
            }
        });
        if (vdadosfiltro !== '') {
            vdadosfiltro = vdadosfiltro.substr(0, vdadosfiltro.length - 1);
        }

        /* Enviar por ajax */
        $.ajax({
            method: "post",
            contentType: "application/x-www-form-urlencoded",
            data: { "dados": vdados, "dadosfk": vdadosfk, "dadosfiltro": vdadosfiltro },
            dataType: "json",
            url: "/CadDinamico/GravarConfiguracoesTabela",
            success: function (data) {
                if (data.result) {
                    alert("Configurações salvas com sucesso!");
                }
                else {
                    alert("Erro ao salvar as configurações: " + data.message);
                }
            },
            error: function (err) {
                alert("Erro ao salvar as configurações: " + err.message);
            }
        });
    }
}