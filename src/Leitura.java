import java.util.Scanner;

public class Leitura {
    private Scanner scan = new Scanner(System.in);

    private int lerInt() {
        String entrada = scan.nextLine();
        return Integer.parseInt(entrada);
    }

    public int entradaValor(){
        int inputUser = lerInt();
        return inputUser;
    }

    public String entradaTexto(){
        String entrada = scan.nextLine();
        return entrada;

    }
}
