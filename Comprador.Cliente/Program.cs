using System.Net.Sockets;
using System.Text;

var finalizar = false;
const string IP_SERVIDOR = "";
const string LISTAR_PRODUTOS = "LISTAR_PRODUTOS";
const string DAR_LANCE = "DAR_LANCE";
const string CONSULTAR_ARREMATADOS = "CONSULTAR_ARREMATADOS";


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
    Console.WriteLine("Digite 3 para verificar seus arremates.");
    Console.WriteLine("Digite 0 para finalizar o leilão.");
    Console.WriteLine("-----------------------------------------");


    try
    {
        opcao = Convert.ToInt32(Console.ReadLine());
        switch (opcao)
        {
            case 1:
                var b = RequisitarProdutos();
                break;
            case 2:
                DarLance();
                Console.WriteLine("Digite algo para continuar");
                Console.ReadKey();
                Console.Clear();
                break;
            case 3:
                VerificarArrematados();
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


void VerificarArrematados()
{

    var client = CriarClienteTcp();
    var mensagem = $"CONSULTAR_ARREMATADOS: %{cliente.Email}%";
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    var resposta = streamReader.ReadLine();
    if(resposta is not null)
    {
        Console.WriteLine("SEUS ARREMATES:");
        Console.WriteLine(resposta);
        Console.WriteLine("---------------------------");
    }
    stream.Close();
    client.Close();
}

void DarLance()
{
    if (!RequisitarProdutos())
    {
        return;
    }
    else
    {
        var client = CriarClienteTcp();
        var continuar = false;
        var idProduto = 0;
        var lance = 0;
        while (continuar is not true)
        {
            try
            {
                Console.WriteLine("Digite o ID do produto a dar o lance.");

                idProduto = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Digite o valor do lance: ");
                lance = Convert.ToInt32(Console.ReadLine());
                continuar = true;
            }
            catch (Exception ex)
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
  
}
bool RequisitarProdutos()
{
    var operacaoSucesso = false;
    var client = CriarClienteTcp();
    var mensagem = LISTAR_PRODUTOS;
    var contarBytes = Encoding.ASCII.GetByteCount(mensagem + 1);
    var envioDados = Encoding.ASCII.GetBytes(mensagem);
    var stream = client.GetStream();
    stream.Write(envioDados, 0, envioDados.Length);
    Console.WriteLine("Enviando ao servidor.");
    var streamReader = new StreamReader(stream);
    if(streamReader is not null)
    {
        var resposta = streamReader.ReadLine();
        if (resposta is not null)
        {
            Console.WriteLine(resposta);
            if(resposta.Contains("Nenhum produto em leilão"))
            {
                operacaoSucesso = false;
            }
            else
            {
                operacaoSucesso = true;
            }
        }
        else
        {
            operacaoSucesso = false;
        }
    }    
    else
    {
        Console.WriteLine("Falha ao receber resposta do servidor.");
        operacaoSucesso = false;
    }
    stream.Close();
    client.Close();
    return operacaoSucesso;
}

TcpClient CriarClienteTcp()
{
    return new TcpClient(IP_SERVIDOR, 1302);
}

public class Comprador
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}