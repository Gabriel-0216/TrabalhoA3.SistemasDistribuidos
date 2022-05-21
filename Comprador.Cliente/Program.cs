using System.Net.Sockets;
using System.Text;

var finalizar = false;
const string LISTAR_PRODUTOS = "LISTAR_PRODUTOS";
const string DAR_LANCE = "DAR_LANCE";

Console.WriteLine("CLIENTE: COMPRADOR");

Console.WriteLine("Digite seu nome.");
var nome = Console.ReadLine();
Console.WriteLine("Digite seu email.");
var email = Console.ReadLine();

var cliente = new Comprador() { Email = email, Nome = nome };

while(finalizar is not true)
{
    var opcao = 0;
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("Digite 1 para requisitar todos os produtos.");
    Console.WriteLine("Digite 2 para dar um lance em um produto.");
    Console.WriteLine("Digite 0 para finalizar o leilão.");
    Console.WriteLine("-----------------------------------------");


    try
    {
        opcao = Convert.ToInt32(Console.ReadLine());
        switch (opcao)
        {
            case 1:
                RequisitarProdutos();
                break;
            case 2:
                DarLance();
                Console.WriteLine("Digite algo para continuar");
                Console.ReadKey();
                Console.Clear();
                break;
            case 0:
                finalizar = true;
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }

    }catch(Exception ex)
    {
        Console.WriteLine("Digite um número válido.");
    }
}




void DarLance()
{
    var client = CriarClienteTcp();
    var continuar = false;
    var idProduto = 0;
    var lance = 0.0M;
    while(continuar is not true)
    {
        try
        {
            Console.WriteLine("Digite o ID do produto a dar o lance.");

            idProduto = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Digite o valor do lance: ");
            lance = Convert.ToDecimal(Console.ReadLine());
            continuar = true;
        }
        catch(Exception ex)
        {
            Console.WriteLine("Valor inválido, continuar");
        }
    }

    var mensagem = $"{DAR_LANCE}, Id_Produto: ${idProduto}$, Lance: %{lance}%, Email_Comprador: &{cliente.Email}&";
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    Console.WriteLine(resposta);
    stream.Close();
    client.Close();

}
void RequisitarProdutos()
{
    var client = CriarClienteTcp();
    var mensagem = LISTAR_PRODUTOS;
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    Console.WriteLine(resposta);
    stream.Close();
    client.Close();
}

TcpClient CriarClienteTcp()
{
    return new TcpClient("127.0.0.1", 1302);
}

public class Comprador
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}