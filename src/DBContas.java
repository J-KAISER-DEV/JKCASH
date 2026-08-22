import java.util.HashMap;
import java.util.Map;

public class DBContas {
    private static Map<String,Conta> bancoDeDados = new HashMap<>();
    public static void salvar (Conta conta){
        String chave = conta.getTitular();

        if(bancoDeDados.containsKey(chave)){
            System.out.println("Conta ja Existente!");
        }else{
            bancoDeDados.put(chave,conta);
        }
    }
}
