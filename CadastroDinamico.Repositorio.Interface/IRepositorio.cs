using CadastroDinamico.Dominio;
using System.Collections.Generic;

namespace CadastroDinamico.Repositorio.Interface
{
    public interface IRepositorio
    {
        void CriarDatabaseAplicacao();
        string RetornarConnectionString();
        List<Database> RetornarDatabases();
        List<Schema> RetornarSchemas(string database);
        List<Tabela> RetornarTabelas(string database, string schema);
        List<Coluna> RetornarColunas(string database, string schema, string tabela);
    }
}
