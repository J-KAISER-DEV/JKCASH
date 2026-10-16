using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;

namespace JKCASH.Menus
{
    public class MenuCategorias
    {
        private readonly ICategoriaRepositorio _repositorio;

        public MenuCategorias(ICategoriaRepositorio repositorio)
        {
            ArgumentNullException.ThrowIfNull(repositorio);
            _repositorio = repositorio;
        }

        public void Executar()
        {
            NavegacaoConsole.Executar("Categorias", "Voltar",
                ("Cadastrar categoria", CadastrarCategoria),
                ("Listar categorias", ListarCategorias));
        }

        public void CadastrarCategoria()
        {
            string nome = LeituraConsole.LerTexto("Nome da categoria: ");
            Categoria categoria = new(nome);
            _repositorio.Adicionar(categoria);

            Console.WriteLine($"Categoria cadastrada: {categoria.Nome}");
        }

        public void ListarCategorias()
        {
            List<Categoria> lista = _repositorio.ListarTodas();

            if (lista.Count == 0)
            {
                Console.WriteLine("Nenhuma categoria cadastrada.");
                return;
            }

            foreach (Categoria categoria in lista)
            {
                Console.WriteLine($"{categoria.Nome} | Id: {categoria.Id}");
            }
        }

        public static Categoria SelecionarCategoria(List<Categoria> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {lista[i].Nome}");
            }

            int numero = LeituraConsole.LerNumeroDaLista("Escolha o número da categoria: ", lista.Count);
            return lista[numero - 1];
        }
    }
}
