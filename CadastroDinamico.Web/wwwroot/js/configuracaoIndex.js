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

function exibirMensagem(message) {
    $('#modalMessageText')[0].textContent = message;
    $('#showModalMessage').click();
}

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
    bloquearTela();
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/Configuracao/SelecionarDatabases",
        success: function (data) {
            successConsultaDatabase(data);
        },
        error: function (err) {
            desbloquearTela();
            exibirMensagem("Houve um erro ao consultar as databases.");
        }
    });
}

// Schemas
function consultarSchemas() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/Configuracao/SelecionarSchemas?database=" + $("#selDatabases")[0].value,
        success: function (data) {
            successConsultaSchema(data);
        },
        error: function (err) {
            desbloquearTela();
            exibirMensagem("Houve um erro ao consultar os schemas.");
        }
    });
}

// Tabelas
function consultarTabelas() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/Configuracao/SelecionarTabelas?database=" + $("#selDatabases")[0].value +
            "&schema=" + $("#selSchemas")[0].value,
        success: function (data) {
            successConsultaTabela(data);
        },
        error: function (err) {
            desbloquearTela();
            exibirMensagem("Houve um erro ao consultar as tabelas.");
        }
    });
}

// Colunas
function consultarColunas() {
    if ($("#selTabelas")[0].value !== '') {
        $.ajax({
            method: "GET",
            dataType: "JSON",
            url: "/Configuracao/SelecionarColunas?database=" + $("#selDatabases")[0].value +
                "&schema=" + $("#selSchemas")[0].value +
                "&tabela=" + $("#selTabelas")[0].value,
            success: function (data) {
                successConsultarColunas(data);
            },
            error: function (err) {
                desbloquearTela();
                exibirMensagem("Houve um erro ao consultar as colunas.");
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
            url: "/Configuracao/SelecionarColunasChaveEstrangeira?database=" + $("#selDatabases")[0].value +
                "&schema=" + $("#selSchemas")[0].value +
                "&tabela=" + $("#selTabelas")[0].value,
            error: function (err) {
                $("#divFk").attr('hidden', true);
                exibirMensagem("Houve um erro ao consultar as colunas de chave estrangeira.");
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
    // $.blockUI({ message: '<h1><img src="../images/load.gif" /> Carregando...</h1>' });
    document.getElementById('modalLoading').style.display = 'block';
}

function desbloquearTela()
{
    // $.unblockUI();
    document.getElementById('modalLoading').style.display = 'none';
}

function gravar(abrirCadDinamico = false) {
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
        exibirMensagem('Selecione ao menos uma coluna que deseja exibir!');
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
            url: "/Configuracao/GravarConfiguracoesTabela",
            success: function (data) {
                if (data.result) {
                    if (abrirCadDinamico) {
                        let database = document.getElementById("selDatabases").value;
                        let schema = document.getElementById("selSchemas").value;
                        let tabela = document.getElementById("selTabelas").value;

                        window.location.href = `/CadDinamico/Index?database=${database}&schema=${schema}&tabela=${tabela}`;
                    }
                    else {
                        exibirMensagem("Configurações salvas com sucesso!");
                    }
                }
                else {
                    exibirMensagem("Erro ao salvar as configurações: " + data.message);
                }
            },
            error: function (err) {
                exibirMensagem("Erro ao salvar as configurações: " + err.message);
            }
        });
    }
}

function cadDinamico() {
    gravar(true);
}