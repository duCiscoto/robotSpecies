using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWFSpecies.Model;

namespace WWFSpecies
{
    class Program
    {
        static public HtmlNode _doc { get; set; }
        static public HtmlWeb _web = new HtmlWeb();
        static public List<Specie> MyProperty = new List<Specie>();

        //// Propriedade do tipo List que contém o nó inicial carregado (tebela)

        static void Main(string[] args)
        {
            NavegaPagina("https://www.worldwildlife.org/species/directory?sort=name&direction=");
            getSpecies(_doc);

            foreach (var item in MyProperty)
            {
                Console.WriteLine(MyProperty.IndexOf(item) + " -  " + item.commonName + " / " + item.scientificName + " / " + item.conservationStatus);
            }

        }

        /// <summary>
        /// Função que navega para uma página através de uma URL
        /// </summary>
        /// <param name="url">Url do site</param>
        private static void NavegaPagina(string url)
        {
            _doc = _web.Load(url).DocumentNode;
        }

        //// função responsável por percorrer as páginas carregando suas respectivas tabelas de species
        private static void getSpecies(HtmlNode docNode)
        {
            var especiesPagina = parseSpecies(docNode);

            MyProperty.AddRange(especiesPagina);

            var paginacao = docNode.SelectSingleNode("//li[@class='page active']/following-sibling::li[1]/a");

            ////var att = paginacao.Attributes["href"];

            if (paginacao != null)
            {
                NavegaPagina("https://www.worldwildlife.org" + paginacao.GetAttributeValue("href", string.Empty));

                getSpecies(_doc);
            }
        }

        //// carrega a tabela (relação de species) da página
        private static List<Specie> parseSpecies(HtmlNode docNode)
        {
            var linhas = docNode.SelectNodes("//tbody/tr");

            List<Specie> species = new List<Specie>();

            foreach (var linha in linhas)
            {
                Specie specie = CreateSpecies(linha);

                species.Add(specie);
            }

            return species;
        }

        //// cria a lista de objetos "specie" a partir do html carregado da página
        private static Specie CreateSpecies(HtmlNode linha)
        {
            var commonName = linha.SelectSingleNode("./td[1]/a");

            var scientificName = linha.SelectSingleNode("./td[2]/em");

            var conservationStatus = linha.SelectSingleNode("./td[3]");

            if (commonName == null || scientificName == null || conservationStatus == null)
            {
                throw new Exception("Algum dos XPaths retornou \"null\"!");
            }

            Specie specie = new Specie
            {
                commonName = commonName.InnerText,
                scientificName = scientificName.InnerText,
                conservationStatus = conservationStatus.InnerText,
            };
            
            return specie;
        }
    }
}
