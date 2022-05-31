using System.Net.Sockets;
using System.Text;

var finalizar = false;
var listaProdutosDoVendedor = new List<Produto>();
const string IP_SERVIDOR = "";
const string INICIAR_LEILAO = "INICIAR_LEILAO";
const string FINALIZAR_LEILAO = "FINALIZAR_LEILAO";
Console.WriteLine("CLIENTE: VENDEDOR");

Console.WriteLine("Digite o seu nome:");
var nome = Console.ReadLine();
Console.WriteLine("Digite o seu email:");
var email = Console.ReadLine();

var vendedor = new Vendedor() { Email = email, Nome = nome };


while(finalizar is not true)
{
    Console.WriteLine("----------------------------");
    Console.WriteLine("Digite 1 para cadastrar um novo produto.");
    Console.WriteLine("Digite 2 para finalizar um leilão.");
    Console.WriteLine("Digite 3 para listar meus produtos em leilão");
    Console.WriteLine("Digite 0 para finalizar a aplicação.");
    Console.WriteLine("-----------------------------------------");

    try
    {
        var opcao = Convert.ToInt32(Console.ReadLine());
        switch (opcao)
        {
            case 1:
                CadastrarNovoProduto();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 2:
                FinalizarLeilao();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 3:
                ListarProdutosEmLeilao();
                Console.WriteLine("Clicar em algo para continuar.");
                Console.ReadKey();
                Console.Clear();
                break;
            case 0:
                finalizar = true;
                break;
            default:
                Console.WriteLine("Opção inexistente.");
                break;
        }

    }
    catch(Exception ex)
    {
        Console.WriteLine("Digite um número para continuar.");
    }
}


void ListarProdutosEmLeilao()
{
    foreach(var item in listaProdutosDoVendedor)
    {
        Console.WriteLine(item.RetornarProduto());
    }
}


void CadastrarNovoProduto()
{
    var continuar = false;
    var valor = 0.0M;
    Console.WriteLine("Digite o nome do produto.");
    var nome = Console.ReadLine();
    Console.WriteLine("Digite o lance mínimo do produto.");
    while(continuar is not true)
    {
        try
        {
            valor = Convert.ToDecimal(Console.ReadLine());
            continuar = true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Digite um valor válido.");
        }
    }
    var produto = new Produto() { NomeProduto = nome, EmailVendedor = vendedor.Email, LanceMinimo = valor };

    var client = CriarClienteTcp();
    var mensagem = INICIAR_LEILAO + "\n" + produto.ToString();
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();

    int inicioIndex = 0, ultimoIndex = 0;

    inicioIndex = resposta.IndexOf("$");
    ultimoIndex = resposta.LastIndexOf("$");
    var idCadastrado = resposta.Substring(inicioIndex + 1, ultimoIndex - inicioIndex - 1);
    produto.Id = Convert.ToInt32(idCadastrado);
    stream.Close();
    client.Close();
    listaProdutosDoVendedor.Add(produto);

}
void FinalizarLeilao()
{
    var continuar = false;
    var idProduto = 0;
    var client = CriarClienteTcp();
    ListarProdutosEmLeilao();
    Console.WriteLine("Digite o ID do produto a ser finalizado o leilão?");
    while(continuar is not true)
    {
        try
        {
            idProduto = Convert.ToInt32(Console.ReadLine());
            var produto = listaProdutosDoVendedor.FirstOrDefault(p => p.Id == idProduto);
            if (produto is null) Console.WriteLine("Esse produto não é valido, tente novamente.");

            continuar = true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Digite um número válido!");
        }
    }
    var mensagem = @$"{FINALIZAR_LEILAO} \n Id: ${idProduto}$, EMAIL_COMPRADOR: &{vendedor.Email}&";
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    Console.WriteLine(resposta);
}



TcpClient CriarClienteTcp()
{
    return new TcpClient(IP_SERVIDOR, 1302);
}

public class Vendedor
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
public class Produto
{
    public int Id { get; set; } = 0;
    public string NomeProduto { get; set; } = string.Empty;
    public string EmailVendedor { get; set; } = string.Empty;
    public decimal LanceMinimo { get; set; }

    public string RetornarProduto()
    {
        return $"Id do produto: {Id}; Nome do produto: {NomeProduto}";
    }
    public override string ToString()
    {
        return $@"NOME_PRODUTO: ${NomeProduto}$
                  EMAIL_VENDEDOR: #{EmailVendedor}#
                  LANCE_MINIMO: &{LanceMinimo}&";
    }

}