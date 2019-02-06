var table = null;

$(document).ready(() => {
    const form = document.querySelector('form');
    form.onsubmit = newOnSubmit;
    requestData();
});

function requestData() {
    $.ajax({
        url: `/CadDinamico/ConsultarDadosIndex?database=${$('#Database').val()}&schema=${$('#Schema').val()}&tabela=${$('#Tabela').val()}`,
        method: 'get',
        error: function (err) {
            alert('Não foi possível consultar os dados da tabela.');
        },
        success: function (data) {
            mountTable(data);
        }
    });
}

function mountTable(data) {
    let innerHTML = '';
    let url = '';
    const linesCount = data.consultaDados.length;
    const colsCount = data.consultaDados.length > 0 ? data.consultaDados[0].length : 0;

    for (let lin = 0; lin < linesCount; lin++) {
        innerHTML += '<tr>';
        for (let col = 1; col < colsCount; col++) {
            if (data.colunas[col - 1].tipo.toUpperCase() === 'BIT') {
                innerHTML += `<input type="hidden" value="false"></td>`
                innerHTML += `<td><span style="display: none">${data.consultaDados[lin][col].toString().toUpperCase() === 'TRUE' ? 1 : 0}</span><input class="checkbox" onclick="this.checked=!this.checked;" type="checkbox" ${data.consultaDados[lin][col].toString().toUpperCase() === 'TRUE' ? "checked" : ""}><input type="hidden" value="false"></td>`;
            } else {
                innerHTML += `<td>${data.consultaDados[lin][col].toString()}</td>`;
            }
        }
        url = `/CadDinamico/TelaDinamicaAlteracao?database=${data.database}&schema=${data.schema}&tabela=${data.nome}&pk=${data.consultaDados[lin][0].toString()}`;
        innerHTML += `<td><a role="button" class="btn btn-success btn-sm" href="${url}" title="Editar"><i class="fas fa-edit"></i></a><a role="button" class="btn btn-danger btn-sm" href="${url}" title="Excluir"><i class="fas fa-trash-alt"></i></a></td>`;
        innerHTML += '</tr>';
    }
    updateDatatable(innerHTML);
}

function updateDatatable(innerHTML) {
    const tableBody = document.querySelector('#tableDadosFiltro tbody');
    if (table !== null && table !== undefined) {
        table.destroy();
        table = null;
    }
    tableBody.innerHTML = innerHTML;
    table = createDatatable('.datatable');
}

function newOnSubmit(e) {
    e.preventDefault();
    const formData = new FormData();
    let inputs = Array.from(document.querySelectorAll('form input[name]'));
    let selects = Array.from(document.querySelectorAll('form select[name]'));
    selects.forEach(element => inputs.push(element));

    inputs.forEach(value => {
        const inputField = $(`#${value.id}`);

        if (inputField[0].type === 'checkbox') {
            if (inputField[0].checked) {
                formData.append(value.name, true);
            }
        }
        else {
            formData.append(value.name, inputField.val());
        }
    });

    $.ajax({
        url: '/CadDinamico/Filtrar',
        method: 'post',
        data: formData,
        dataType: 'json',
        processData: false,
        contentType: false,
        success: function (data) {
            mountTable(data);
        },
        error: function (err) { }
    });
}