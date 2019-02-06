function createDatatable(selector) {
    return $(selector).DataTable({
        language: {
            "decimal": ",",
            "emptyTable": "Nenhum registro disponível",
            "info": "Exibindo _START_ a _END_ de _TOTAL_ registro(s)",
            "infoEmpty": "",
            "infoFiltered": "(filtrado de _MAX_ registros)",
            "infoPostFix": "",
            "thousands": ".",
            "lengthMenu": "Exibir  _MENU_ registros",
            "loadingRecords": "Carregando...",
            "processing": "Processando...",
            "search": "Buscar:",
            "zeroRecords": "Nenhum registro encontrado",
            "paginate": {
                "first": "Primeira",
                "last": "Última",
                "next": "Próxima",
                "previous": "Anterior"
            },
            "aria": {
                "sortAscending": ": ative para ordenar ascendentemente",
                "sortDescending": ": ative para ordenar descendentemente"
            }
        },
        searching: false
    });
}