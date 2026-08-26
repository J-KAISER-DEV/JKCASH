import java.util.HashMap;
import java.util.Map;

public class DBContas {
    private static Map<String, Conta> bancoDeDados = new HashMap<>();

    public static void salvar(Conta conta) {
        String chave = conta.getTitular() + "-" + conta.getBanco();

        if (bancoDeDados.containsKey(chave)) {
            System.out.println("Conta ja existente");
        } else {
            bancoDeDados.put(chave, conta);
        }
    }

    public static void editarConta() {
        Leitura leitura = new Leitura();
        System.out.print("Digite o Nome do Titular: ");
        String nome = leitura.entradaTexto();
        System.out.print("Digite o Banco do Titular: ");
        String banco = leitura.entradaTexto();
        String chave = nome + "-" + banco;

        if(bancoDeDados.containsKey(chave)){
            System.out.println("ATENÇÃO!! APENAS O BANCO E O NOME DE TITULAR SAO PERMITIDOS ALTERAR");
            System.out.println("NÃO E POSSIVEL MUDAR O TIPO DA CONTA NEM ALTERAR SALDO");
            System.out.println("""
                    1 - ALTERAR BANCO
                    2 - ALTERAR TITULAR
                    """);
            switch (leitura.entradaValor())
            {
                case 1:
                    break;
            }
        }
    }
    public static void contasTeste(){
        Saldo saldo1 = new Saldo(50.00, 2000, 500);
        Conta nubank = new Conta("Poupança", "nubank", "Ronaldo", saldo1);

        Saldo saldo2 = new Saldo(90.00, 7000, 800);
        Conta itau = new Conta("Poupança", "itau", "PAULO", saldo2);

        Saldo saldo3 = new Saldo(150.00, 200, 5004);
        Conta santander = new Conta("Poupança", "santander", "JOSE", saldo3);

        salvar(nubank);
        salvar(itau);
        salvar(santander);

    }
}
