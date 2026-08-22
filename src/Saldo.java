public class Saldo {
    private double saldo;
    private double credito;
    private double chequeEspecial;

//----------------------Construtctor---------------------
    public Saldo(double saldo, double credito, double chequeEspecial) {
        this.saldo = saldo;
        this.credito = credito;
        this.chequeEspecial = chequeEspecial;
    }

    public Saldo(double saldo) {
    }

    //--------------GET E SET SALDO--------------------
    public double getSaldo() {
        return saldo;
    }
    public void setSaldo(double saldo) {
        this.saldo = saldo;
    }
//--------------------GET E SET CREDITO--------------------
    public double getCredito() {
        return credito;
    }
    public void setCredito(double credito) {
        this.credito = credito;
    }
//--------------------GET E SET CHEQUEESPECIAL--------------------------
    public double getChequeEspecial() {
        return chequeEspecial;
    }
    public void setChequeEspecial(double chequeEspecial) {
        this.chequeEspecial = chequeEspecial;
    }
//---------------------------------------------------



}
