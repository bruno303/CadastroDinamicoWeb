using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CadastroDinamico.Core
{
    public class DownloadArquivoCore
    {
        public async Task<byte[]> RetornarArquivoDownload(string tabela, string schema, string database, string html, string filename)
        {
            try
            {
                var path = Path.Combine("DownloadFiles", DateTime.Now.ToString("yyyy-MM-dd_HHmmss")) + Path.DirectorySeparatorChar;
                var pathZips = Path.Combine("DownloadFiles", "Zips", DateTime.Now.ToString("yyyy-MM-dd_HHmmss")) + Path.DirectorySeparatorChar;

                Directory.CreateDirectory(path);
                Directory.CreateDirectory(pathZips);

                CopiarArquivosEstaticos(path);
                html = TratarPathsHtml(html);

                await new Utils.Arquivo().EscreverEmArquivoAsync(path + filename + ".html", html, false);
                ZipFile.CreateFromDirectory(path, pathZips + filename + ".zip");

                var bytes = await File.ReadAllBytesAsync(pathZips + filename + ".zip");

                ExcluirArquivosDiretorio(path);
                ExcluirArquivosDiretorio(pathZips);
                Directory.Delete(path, true);
                Directory.Delete(pathZips, true);

                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ExcluirArquivosDiretorio(string path)
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private void CopiarArquivosEstaticos(string newDirectory)
        {
            if (newDirectory.EndsWith(Path.DirectorySeparatorChar))
            {
                newDirectory = newDirectory.Substring(0, newDirectory.Length - 1);
            }
            
            var files = new List<string>
            {
                Path.Combine("wwwroot", "bundles", "js", "site.min.js"),
                Path.Combine("wwwroot", "bundles", "js", "useDatatable.min.js"),
                Path.Combine("wwwroot", "bundles", "js", "cadDinamicoAlteracao.min.js"),
                Path.Combine("wwwroot", "bundles", "css", "site.min.css"),
                Path.Combine("wwwroot", "lib", "datatables", "datatables.min.css"),
                Path.Combine("wwwroot", "images", "icon.ico")
            };

            Directory.CreateDirectory(Path.Combine(newDirectory, "bundles", "js"));
            Directory.CreateDirectory(Path.Combine(newDirectory, "bundles", "css"));
            Directory.CreateDirectory(Path.Combine(newDirectory, "lib", "datatables"));
            Directory.CreateDirectory(Path.Combine(newDirectory, "images"));

            foreach (var file in files)
            {
                File.Copy(file, file.Replace("wwwroot", newDirectory));
            }
        }

        private string TratarPathsHtml(string html)
        {
            var paths = new List<string>
            {
                "/bundles/css/",
                "/images/",
                "/lib/",
                "/bundles/js/",
            };

            foreach (var path in paths)
            {
                html = html.Replace(path, path.Substring(1, path.Length - 1));
            }

            return html;
        }
    }
}
