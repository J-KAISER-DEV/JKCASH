import java.util.ArrayList;
import java.util.List;

public class DBDespesas {
    private static List<Despesa>bancoDespesas = new ArrayList<>();
    public static void salvarDespesa(Despesa novaDespesa){
        bancoDespesas.add(novaDespesa);
    }
}
