// Databases
function consultarDatabases() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarDatabases",
        sucess: function (data) {
            $("#selDatabases > option").remove();
            fillDropDown("#selDatabases", data);
            consultarSchemas();
        },
        error: function (data) {
            alert("Houve um erro ao consultar as databases.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selDatabases > option").remove();
                fillDropDown("#selDatabases", jqXHR.responseJSON);
                consultarSchemas();
            }
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
        sucess: function (data) {
            $("#selTabelas > option").remove();
            fillDropDown("#selTabelas", data);
            consultarColunas();
        },
        error: function (data) {
            alert("Houve um erro ao consultar as tabelas.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selTabelas > option").remove();
                fillDropDown("#selTabelas", jqXHR.responseJSON);
                consultarColunas();
            }
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
            sucess: function (data) {
                $("#tblColunas > tbody tr").remove();
                $.each(data, function (i, coluna) {
                    $("#tblColunas > tbody").append('<tr><td>' + coluna.name + '</td><td><input type="checkbox" checked="' + coluna.visivel + '" /></td></tr>');
                });
                consultarColunasChaveEstrangeira();
                awaitLoad(false);
            },
            error: function (data) {
                awaitLoad(false);
                alert("Houve um erro ao consultar as colunas.");
            },
            complete: (jqXHR) => {
                if (jqXHR.readyState === 4) {
                    $("#tblColunas > tbody tr").remove();
                    $.each(jqXHR.responseJSON, function (i, coluna) {
                        $("#tblColunas > tbody").append('<tr><td>' + coluna.name + '</td><td><input type="checkbox" ' +
                            (coluna.visivel ? 'checked="checked"' : "") + ' /></td></tr>');
                    });
                    consultarColunasChaveEstrangeira();
                }
                awaitLoad(false);
            },
            beforeSend: () => {
                awaitLoad(true);
            }
        })
    }
    else {
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
            error: function (data) {
                $("#tblChavesEstrangeiras").attr('hidden', true);
                awaitLoad(false);
                alert("Houve um erro ao consultar as colunas de chave estrangeira.");
            },
            complete: (jqXHR) => {
                if (jqXHR.readyState === 4) {
                    if (jqXHR.responseJSON.length > 0) {
                        $("#tblChavesEstrangeiras").attr('hidden', false);
                    }
                    $("#tblChavesEstrangeiras > tbody tr").remove();
                    $.each(jqXHR.responseJSON, function (i, coluna) {
                        let opcoes = '';
                        $.each(coluna.colunasTabelaReferenciada, (i, col) => {
                            opcoes += '<option value="' + col + '">' + col + '</option>'
                        });
                        $("#tblChavesEstrangeiras > tbody").append('<tr><td>' + coluna.nome + '</td><td>' + coluna.tabelaReferenciada +
                            '</td><td>' + coluna.colunaReferenciada + '</td><td><select class="custom-select" id="' + 'select-' + i +
                            '">' + opcoes + '</select></tr>');
                        $('#select-' + i)[0].selectedIndex = coluna.indiceColTabelaReferenciada;
                    });
                }
                awaitLoad(false);
            },
            sucess: function(data) {
                $("#tblChavesEstrangeiras > tbody tr").remove();
                if (data.length > 0) {
                    $("#tblChavesEstrangeiras").attr('hidden', false);
                }
                $.each(data, function (i, coluna) {
                    let opcoes = '';
                    $.each(coluna.colunasTabelaReferenciada, (i, col) => {
                        opcoes += '<option value="' + col + '">' + col + '</option>'
                    });
                    $("#tblChavesEstrangeiras > tbody").append('<tr><td>' + coluna.nome + '</td><td>' + coluna.tabelaReferenciada +
                        '</td><td>' + coluna.colunaReferenciada + '</td><td><select class="custom-select">' + opcoes +
                        '</select></tr>');
                });
                awaitLoad(false);
            },
            beforeSend: () => {
                $("#tblChavesEstrangeiras").attr('hidden', true);
                awaitLoad(true);
            }
        })
    }
    else {
        $("#tblColunas > tbody tr").remove();
    }
}

function awaitLoad(enabled) {
    $("#selDatabases")[0].disabled = enabled;
    $("#selSchemas")[0].disabled = enabled;
    $("#selTabelas")[0].disabled = enabled;
}

// Schemas
function consultarSchemas() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarSchemas?database=" + $("#selDatabases")[0].value,
        sucess: function (data) {
            $("#selSchemas > option").remove();
            fillDropDown("#selSchemas", data);
        },
        error: function (data) {
            alert("Houve um erro ao consultar os schemas.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selSchemas > option").remove();
                fillDropDown("#selSchemas", jqXHR.responseJSON);
                consultarTabelas();
            }
        }
    })
}

//Geral
$(document).ready(() => {
    consultarDatabases();

    $("#selDatabases").change(() => {
        $("#tblChavesEstrangeiras").attr('hidden', false);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        $("#selSchemas > option").remove();
        consultarSchemas();
    });

    $("#selSchemas").change(() => {
        $("#tblChavesEstrangeiras").attr('hidden', false);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        $("#selTabelas > option").remove();
        consultarTabelas();
    });

    $("#selTabelas").change(() => {
        $("#tblChavesEstrangeiras").attr('hidden', false);
        $("#tblChavesEstrangeiras > tbody tr").remove();
        $("#tblColunas > tbody tr").remove();
        consultarColunas();
    });
})

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
        $("#selTabelas")[0].value + "|"
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