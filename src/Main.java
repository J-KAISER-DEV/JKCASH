import bancodedados.DBContas;
import modelos.Conta;
import modelos.Movimentacao;
import utis.Leitura;

Leitura leitura = new Leitura();
void menu(){

        System.out.println("""
            \n=============MENU=================
            1- CONTA--- CORRENTE/POUPANÇA
            2 - MOVIMENTAÇÃO / ENTRADA E SAIDA
            3 - FATURA DO CREDITO
            """);
        System.out.print("Selecione uma Opção: ");
        switch (leitura.entradaValor()){
            case 1:
                opConta();
                break;
            case 2:
                opMovimentacao();
                break;
            case 0 :
                System.exit(0);
            default:
                System.out.println("Opção Invalida");
                menu();
        }
}
void opMovimentacao(){
    System.out.println("""
            1 - Resgistrar Despesa
            2 - Registrar Entrada
            3 - Registrar Transferencia Interna
            4 - Voltar
            """);
    System.out.print("Selecione uma Opção: ");
    switch (leitura.entradaValor()){
        case 1:
            Movimentacao.registrarSaida();
            menu();
            break;
        case 2:
            Movimentacao.registrarEntrada();
            menu();
            break;
        default:
            System.out.println("Opção Invalida, Voltando No Menu");
            menu();
    }

        }
void opConta() {
    System.out.println("""
             ----------MENU CONTA----------
            1 - CRIAR CONTA
            2 - EDITAR CONTA
            3 - APAGAR CONTA
            4 - CONSULTAR CONTA
            5 - VOLTAR
            """);
    System.out.print("Selecione uma Opção: ");
    switch (leitura.entradaValor()) {
        case 1:
            System.out.println("""
                    ----------CRIAR CONTA----------
                    1 - CORRENTE
                    2 - POUPANÇA
                    3 - VOLTAR MENU CONTA
                    """);
            System.out.print("Selecione A Opção: ");
            switch (leitura.entradaValor()){
                case 1:
                    Conta.criarContaCorrente();
                    menu();
                    break;
                case 2 :
                    Conta.criarContaPoupança();
                    menu();
                    break;
                case 3 :
                    opConta();
                    break;
                default:
                    System.out.println("OPÇÃO INVALIDA");
                    opConta();
                    break;
            }
            break;
        case 2:
            DBContas.editarConta();
            opConta();
            break;
        case 3:
            DBContas.apagarConta();
            opConta();
            break;
        case 5:
            menu();;
            break;


        default:
            System.out.println("Opção Invalida");
    }

}
void main() {
    DBContas.contasTeste();
    menu();
}
