void menu(){
    try {
        System.out.println("""
            1- CONTA--- CORRENTE/POUPANÇA
            2 - MOVIMENTAÇÃO / ENTRADA E SAIDA
            3 - EXTRATO
            4 - POUPANÇA
            """);
        Leitura leitura = new Leitura();
        System.out.print("Selecione uma Opção: ");
        switch (leitura.entradaValor()){
            case 1:
                System.out.println("Menu Conta");
                System.out.println("""
                    1 - Criar Conta Corrente
                    2 - Criar Conta Poupança
                    3 - Voltar Menu
                    """);
                opConta();
                break;
            case 0 :
                System.exit(0);
            default:
                System.out.println("Opção Invalida");
                menu();
        }
    } catch (Exception e) {
        throw new RuntimeException(e);
    }


    }
void opConta() {
Leitura leitura = new Leitura();
    System.out.print("Selecione uma Opção: ");
    switch (leitura.entradaValor()) {
        case 1:
            Conta.criarContaCorrente();
            menu();
            break;
        case 2:
            Conta.criarContaPoupança();
            menu();
            break;
        default:
            System.out.println("Opção Invalida");
            opConta();
    }

}
void main() {
    menu();
}
