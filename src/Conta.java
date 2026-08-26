
public class Conta {
   private String tipoConta;
    private String banco;
    private String titular;
    private Saldo saldo;

    public Conta(String tipoConta, String banco, String titular, Saldo saldo) {
        this.tipoConta = tipoConta;
        this.banco = banco;
        this.titular = titular;
        this.saldo = saldo;
        }

    public static void criarContaCorrente(){
        Leitura leitura = new Leitura();
        String tipoConta = "Conta Corrente";
        //-------------------------------------
        System.out.print("Banco da Conta: ");
        String banco = leitura.entradaTexto();
        //-----------------------------------
        System.out.print("Nome do titular: ");
        String titular = leitura.entradaTexto();
        //-----------------------------------------
        System.out.print("Valor Atual da conta R$: ");
        double saldo = leitura.entradaValor();
        //---------------------------------------------
        System.out.print("Digite seu Limite de Credito R$: ");
        double credito = leitura.entradaValor();
        //---------------------------------------------
        System.out.print("Digite seu Limite de Cheque Especial R$: ");
        double chequeEspecial = leitura.entradaValor();
        //--------------------------------------------
        System.out.print("Credito Usado Total R$: ");
        double creditoUsado = leitura.entradaValor();
        credito -= creditoUsado;
        //-------------------------------------------
        if(saldo<0){
            double chequeEspecialUsado = saldo;
            chequeEspecial -= chequeEspecialUsado;

        }

        Saldo saldoUser = new Saldo(saldo, credito, chequeEspecial);
        Conta contaUser = new Conta(tipoConta,banco, titular, saldoUser);
        DBContas.salvar(contaUser);
        System.out.println("Conta Corrente " + contaUser.getBanco()+ " Criada com sucesso!");
    }
    public static void criarContaPoupança(){
        Leitura leitura = new Leitura();
        String tipoConta = "Conta Poupança";
        //-------------------------------------
        System.out.print("Banco da Conta: ");
        String banco = leitura.entradaTexto();
        //-----------------------------------
        System.out.print("Nome do titular: ");
        String titular = leitura.entradaTexto();
        //-----------------------------------------
        System.out.print("Valor Atual da conta R$: ");
        double saldo = leitura.entradaValor();
        //---------------------------------------------


        Saldo saldoUser = new Saldo(saldo);
        Conta contaUser = new Conta(tipoConta,banco, titular, saldoUser);
        DBContas.salvar(contaUser);
        System.out.println("Conta Poupança " + contaUser.getTipoConta()+ "Criada com sucesso!");
    }

    public static void menuConta(){

    }


    public String getTipoConta() {
        return tipoConta;
    }

    public void setTipoConta(String tipoConta) {
        this.tipoConta = tipoConta;
    }

    public String getBanco() {
        return banco;
    }

    public void setBanco(String banco) {
        this.banco = banco;
    }

    public String getTitular() {
        return titular;
    }

    public void setTitular(String titular) {
        this.titular = titular;
    }

    public Saldo getSaldo() {
        return saldo;
    }

    public void setSaldo(Saldo saldo) {
        this.saldo = saldo;
    }
}

