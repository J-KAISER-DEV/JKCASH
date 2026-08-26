Leitura leitura = new Leitura();
void menu(){

        System.out.println("""
            1- CONTA--- CORRENTE/POUPANÇA
            2 - MOVIMENTAÇÃO / ENTRADA E SAIDA
            3 - EXTRATO
            4 - POUPANÇA
            """);
        System.out.print("Selecione uma Opção: ");
        switch (leitura.entradaValor()){
            case 1:
                opConta();
                break;
            case 0 :
                System.exit(0);
            default:
                System.out.println("Opção Invalida");
                menu();
        }
}
void opConta() {
    System.out.println("""
             ----------MENU CONTA----------
            1 - CRIAR CONTA
            2 - EDITAR CONTA
            3 - APAGAR CONTA
            4 - VOLTAR
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
                    menu();
                    break;
                default:
                    System.out.println("OPÇÃO INVALIDA");
                    opConta();
                    break;
            }
            break;
        case 2:


        default:
            System.out.println("Opção Invalida");
    }

}
void main() {
    DBContas.contasTeste();
    menu();
}
