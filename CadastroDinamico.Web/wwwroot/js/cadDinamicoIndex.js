// Databases
function consultarDatabases() {
    $.ajax({
        method: "GET",
        dataType: "JSON",
        url: "/CadDinamico/SelecionarDatabases",
        sucess: function (data) {
            $("#selDatabases > option").remove();
            fillDropDownDatabase("#selDatabases", data);
            consultarSchemas();
        },
        error: function (data) {
            alert("Houve um erro ao consultar as databases.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selDatabases > option").remove();
                fillDropDownDatabase("#selDatabases", jqXHR.responseJSON);
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
            fillDropDownDatabase("#selTabelas", data);
        },
        error: function (data) {
            alert("Houve um erro ao consultar as tabelas.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selTabelas > option").remove();
                fillDropDownDatabase("#selTabelas", jqXHR.responseJSON);
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
                $("#tblColunas")[0].hidden = false;
                $.each(data, function (i, coluna) {
                    $("#tblColunas > tbody").append('<tr><td>' + coluna.name + '</td><td><input type="checkbox" checked="checked" /></td></tr>');
                });
                awaitLoad(false);
            },
            error: function (data) {
                $("#tblColunas")[0].hidden = true;
                awaitLoad(false);
                alert("Houve um erro ao consultar as colunas.");
            },
            complete: (jqXHR) => {
                if (jqXHR.readyState === 4) {
                    $("#tblColunas > tbody tr").remove();
                    $("#tblColunas")[0].hidden = false;
                    $.each(jqXHR.responseJSON, function (i, coluna) {
                        $("#tblColunas > tbody").append('<tr><td>' + coluna.name + '</td><td><input type="checkbox" checked="checked" /></td></tr>');
                    });
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
            fillDropDownDatabase("#selSchemas", data);
        },
        error: function (data) {
            alert("Houve um erro ao consultar os schemas.");
        },
        complete: (jqXHR) => {
            if (jqXHR.readyState === 4) {
                $("#selSchemas > option").remove();
                fillDropDownDatabase("#selSchemas", jqXHR.responseJSON);
                consultarTabelas();
            }
        }
    })
}

//Geral
$(document).ready(() => {
    consultarDatabases();

    $("#selDatabases").change(() => {
        $("#selSchemas > option").remove();
        consultarSchemas();
    });

    $("#selSchemas").change(() => {
        $("#selTabelas > option").remove();
        consultarTabelas();
    });

    $("#selTabelas").change(() => {
        $("#tblColunas > tbody tr").remove();
        consultarColunas();
    });
})

function fillDropDownDatabase(select, data) {
    $.each(data, function (i, dado) {
        $(select).append('<option value="' + dado.name + '">' + dado.name + '</option>');
    })
}