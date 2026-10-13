//Variaveis e comportamentos 

Console.Write("Digite o nome do produto: ");
string nomeProduto =  Console.ReadLine()!;
Console.Write("Digite o valor do produto:");
bool precoValido = decimal.TryParse(Console.ReadLine(), out decimal valorProduto);
Console.Write("Digite a quantidade: ");
bool entradaValida = int.TryParse(Console.ReadLine()!, out int quantidade);
 
   if(entradaValida && precoValido)
{
    decimal total = valorProduto * quantidade;
    if(total >= 100)
    {
        decimal desconto = total * 0.10m;
        Console.WriteLine($"Produto: {nomeProduto}\n" +
    $"valor R$:{valorProduto}\n" +
    $"Total R$:{total}" +
    $"Desconto: R${desconto}" +
    $"Total real: R${total - desconto}");
    }
    else
    {
        Console.WriteLine($"Produto: {nomeProduto}\n" +
    $"valor R$:{valorProduto}\n" +
    $"Total R$:{total}");
  
    }

}
else
{
    Console.WriteLine("Entrada invalida");
}
