package bancodedados;

import modelos.Conta;
import modelos.Saldo;
import utis.Leitura;
import java.util.HashMap;
import java.util.Map;

public class DBContas {
    private static Map<String, Conta> bancoDeDados = new HashMap<>();


    public static Conta verificaDBContas(String tipoConta, String banco){
    String chave = (tipoConta + "-" + banco).toUpperCase().trim();
   return bancoDeDados.get(chave);

}
    public static void salvar(Conta conta) {
        String chave = conta.getTipoConta() + "-" + conta.getBanco();
            bancoDeDados.put(chave, conta);
            System.out.println("modelos.Conta: " + conta.getTipoConta() + " " + conta.getBanco() + " Criada com sucesso!");

    }
public static void apagarConta(){
    Leitura leitura = new Leitura();
    System.out.println("=======APAGAR CONTA =======");
    System.out.print("Digite o tipo da modelos.Conta: ");
    String tipoConta = leitura.entradaTexto().toUpperCase();
    System.out.print("Digite o Banco: ");
    String banco = leitura.entradaTexto().toUpperCase();
    String chaveConta = (tipoConta + "-" + banco);

    if ((bancoDeDados.containsKey(chaveConta))){
        System.out.println("modelos.Conta Apagada");
        bancoDeDados.remove(chaveConta);
    }else {
        System.out.println("modelos.Conta nao Existe");
    }
}
    public static void editarConta() {
        Leitura leitura = new Leitura();
        System.out.print("Digite o tipo da modelos.Conta: ");
        String tipoContaAntiga = leitura.entradaTexto();
        System.out.print("Digite o Banco do Titular: ");
        String bancoAntigo = leitura.entradaTexto();
        String chaveAntiga = tipoContaAntiga + "-" + bancoAntigo;

        if(bancoDeDados.containsKey(chaveAntiga)){
            System.out.println("ATENÇÃO!! APENAS O BANCO E O TIPO DE CONTA SAO PERMITIDOS ALTERAR");
            System.out.println("NÃO E POSSIVEL MUDARNEM ALTERAR SALDO");
            System.out.println("""
                    1 - ALTERAR BANCO
                    2 - ALTERAR TITULAR
                    """);
            switch (leitura.entradaValor())
            {
                case 1:
                    System.out.print("Novo Banco: ");
                    String novoBanco = leitura.entradaTexto();
                    String keyNova = novoBanco.toLowerCase();
                    Conta contaEditada = bancoDeDados.get(chaveAntiga);
                    bancoDeDados.remove(chaveAntiga);
                    contaEditada.setBanco(novoBanco);
                    bancoDeDados.put(keyNova, contaEditada);
                    System.out.println("Banco Alterado para" + novoBanco);
                    break;
                case 2:
                    System.out.print("Digite o novo tipo de modelos.Conta: ");
                    String novoTipo = leitura.entradaTexto();

                    Conta contaMudaTipo = bancoDeDados.get(chaveAntiga);
                    contaMudaTipo.setTipoConta(novoTipo);
                    System.out.println("Tipo de modelos.Conta Alterada!");
                    break;
                default:
                    System.out.println("Opção Invalida");
            }
        }else {
            System.out.println("modelos.Conta Nao Encontrada!");
        }
    }
    public static void contasTeste(){
        Saldo saldo1 = new Saldo(5000.00, 2000, 500);
        Conta nubank = new Conta("POUPANÇA", "NUBANK", saldo1);

        Saldo saldo2 = new Saldo(90.00, 7000, 800);
        Conta itau = new Conta("corrente", "itau", saldo2);

        Saldo saldo3 = new Saldo(150.00, 200, 5004);
        Conta santander = new Conta("Poupança", "santander", saldo3);

        salvar(nubank);
        salvar(itau);
        salvar(santander);

    }

    public static Conta buscarConta(String bancoDigitado){
        for(Conta c : bancoDeDados.values()){
            if (c.getBanco().equalsIgnoreCase(bancoDigitado)){
                return c;
            }
        }
        return null;
    }
}
