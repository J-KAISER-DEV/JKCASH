# Melhorias do JKCASH

Lista de necessidades levantadas durante o uso da aplicação. Registrar um item aqui não significa que ele já foi implementado.

## Cronograma atualizado — 26/10/2026

Ordem proposta de trabalho, sem datas de entrega estimadas. O Windows Forms já utiliza SQLite em todas as telas; a importação dos registros antigos do SQL Server continua pendente, se necessária.

| Etapa | Objetivo | Situação |
|---|---|---|
| 1. Consistência financeira | Conferir o estado atual e gravar lançamentos, saldos e limites em conjunto; tratar conflitos | Implementada no código; últimos ajustes dos serviços compilados, com validação prática pendente |
| 2. Organização das dependências | Centralizar a montagem das telas, repositórios e serviços | Concluída no Windows Forms pela FabricaTelas; navegação permanece em FrmPrincipal |
| 3. Redução de duplicação | Compartilhar operações EF comuns entre repositórios SQL Server e SQLite, mantendo diferenças dos provedores | Todos os pares SQL/Local unificados em sete repositórios EF; solução compilada, validação prática pendente; demais repetições podem ser avaliadas separadamente |
| 4. Consultas eficientes | Separar consultas do dashboard da apresentação, limitar o histórico carregado e revisar filtros de período e prévias de exclusão | DashboardConsulta e DTO integrados, com somas no banco e oito movimentações; leitura SQLite vazia conferida; histórico preenchido, medições, extrato e prévias de exclusão pendentes |
| 5. Responsividade e tratamento de falhas | Padronizar carregamento e gravação sem bloquear a interface e evitar cliques repetidos | Parcialmente existente; falta padronizar as telas restantes |
| 6. Melhorias funcionais | Confirmação com resumo antes de registrar e avaliação da necessidade apenas nas saídas | Itens 001 e 002 pendentes |
| 7. API | Reutilizar serviços para expor operações do JKCASH, conectar SQL Server e planejar autenticação | Projeto criado; ainda com o exemplo WeatherForecast |
| 8. Sincronização | Pendências persistentes, envio e recebimento sem duplicação e resolução de conflitos | Adiada; não há sincronização implementada |

Na etapa 2 não foi introduzido um contêiner de injeção: a fábrica faz a composição manual. Cada operação de repositório continua criando e liberando seu contexto. Mudanças nesta ordem serão registradas conforme as prioridades do usuário.

## 001 — Confirmar o registro de uma transação

- **Situação:** a fazer.
- **Prioridade:** a definir.
- **Necessidade:** atualmente, ao clicar em **Registrar**, a transação é registrada sem uma etapa de revisão e confirmação.
- **Comportamento esperado:** exibir um resumo da operação e perguntar se o usuário deseja confirmar, com os botões **Sim** e **Não**.

### Fluxo esperado

1. O usuário preenche os dados e clica em **Registrar**.
2. A aplicação verifica os campos e as regras da operação. Se houver um problema, informa o motivo para correção.
3. Com os dados válidos, apresenta o resumo: conta, tipo de movimentação, valor, data, categoria e descrição. Nas saídas, inclui também justificativa, importância, urgência e planejamento, conforme o item 002.
4. Pergunta: **“Deseja confirmar esta transação?”**
5. **Sim:** tenta registrar a transação e atualizar o saldo. Informa sucesso somente após a gravação ser concluída; se houver falha, informa o erro.
6. **Não** ou fechar a confirmação: retorna ao formulário com os dados preenchidos, sem gravar a transação nem alterar o saldo.

### Critérios de conclusão

- Clicar em **Registrar** não grava nem movimenta o saldo antes da confirmação.
- O resumo corresponde aos dados que serão registrados.
- Cancelar preserva o preenchimento para revisão.
- Durante a gravação, cliques repetidos não geram transações duplicadas.

**Escopo inicial:** registro de transações na interface Windows Forms. A confirmação de outros processos será detalhada em itens próprios, conforme as necessidades forem levantadas.

## 002 — Exigir a avaliação da necessidade somente nas saídas

- **Situação:** a fazer.
- **Prioridade:** a definir.
- **Necessidade:** importância, urgência, planejamento e justificativa foram criados para analisar os gastos. Esses campos não se aplicam às entradas de dinheiro.
- **Comportamento esperado:** solicitar esses quatro campos somente nas transações de saída.

| Campos | Entrada | Saída |
|---|---|---|
| Conta, valor, data, categoria e descrição | Obrigatórios | Obrigatórios |
| Importância, urgência, planejamento e justificativa | Não se aplicam | Obrigatórios |

### Critérios de conclusão

- Ao selecionar **Entrada**, ocultar os quatro campos de avaliação e não exigir seu preenchimento.
- Ao selecionar **Saída**, apresentar e validar os quatro campos.
- Ao registrar uma entrada, salvar esses quatro campos como `null`, sem reaproveitar valores preenchidos anteriormente para uma saída.
- Aplicar a mesma regra na entidade e no serviço, além do formulário, e ajustar o banco para aceitar a ausência desses valores nas entradas.
- Na confirmação e nos detalhes de uma entrada, não apresentar esses campos como dados ausentes ou erros; quando necessário, identificá-los como **não se aplica**.
- Preservar os dados das transações já registradas; qualquer mudança em dados antigos deverá ser tratada explicitamente na implementação.

**Escopo inicial:** transações de entrada e saída de contas. Transferências e compras no cartão não têm suas regras alteradas por este item.

## 003 — Funcionar sem depender do SQL Server

- **Situação:** em implementação; telas locais integradas, importação e validação de execução pendentes.
- **Prioridade:** foco atual do planejamento, por decisão do usuário.
- **Objetivo:** abrir, consultar e registrar dados no computador sem precisar do SQL Server funcionando ou instalado, preservando os registros após fechar a aplicação.
- **Estado atual:** o Windows Forms utiliza SQLite em todas as telas. O Console mantém suas opções anteriores de SQL Server e memória. O modo em memória perde os dados ao encerrar.

### Progresso da implementação

- Pacote SQLite, conexão local, conversor monetário e ajustes de mapeamento preparados com o usuário.
- Migração `CriacaoInicialSqlite` gerada e aplicada pelo usuário no banco local.
- Repositório local de categorias implementado pelo usuário.
- Repositórios locais de contas, cartões, transações, transferências, compras/faturas/pagamentos e exclusões implementados a pedido do usuário. Compilação da Infrastructure concluída sem avisos ou erros.
- Dashboard, Contas, Categorias, Transações, Cartões e Faturas ligados ao SQLite, incluindo serviços de transferências, compras e pagamentos. Inicialização do banco local por migrações antes de abrir a janela principal. Compilação do Windows Forms concluída sem avisos ou erros.
- Dados existentes ainda não foram importados e os fluxos dos novos repositórios e telas ainda não foram testados.

Detalhes: [Repositórios locais](JKCASH.Infrastructure/Persistencia/REPOSITORIOS-LOCAIS.md).

### Escopo atual

Criar armazenamento local persistente para a aplicação. A proposta é usar SQLite, um banco em arquivo acessado pela própria aplicação, sem um serviço de banco separado. A independência é do SQL Server: ainda precisamos guardar os dados permanentemente.

Fonte técnica: [SQLite — funcionamento sem servidor separado](https://www.sqlite.org/serverless.html).

A aplicação fará suas consultas e gravações na base local. Não alternará automaticamente para SQL Server se ele estiver disponível, evitando dois históricos diferentes durante o uso.

### Etapas propostas

| Etapa | Entrega | Condição para concluir |
|---|---|---|
| 1. Preparar o armazenamento | Dependência SQLite compatível com o EF usado, configuração e esquema local | Criar e abrir o arquivo local sem acessar SQL Server |
| 2. Adaptar categorias | Repositório local ligado ao serviço e à tela | Cadastrar, fechar e reabrir preservando categoria e Id |
| 3. Adaptar contas e transações | Persistência local das contas, movimentações, transferências e consultas | Preservar centavos exatos, regras de saldo e os dois lados das transferências |
| 4. Adaptar cartões e faturas | Compras, parcelas, pagamentos, limites e consultas locais | Gravar cada operação e seus efeitos relacionados em conjunto |
| 5. Completar a integração | Dashboard, relatórios, exclusões e inicialização do Console e Windows Forms | Nenhuma funcionalidade do modo local exigir SQL Server |
| 6. Disponibilizar os dados atuais | Importação inicial explícita dos dados existentes, quando necessária | Preservar identificadores, relações e valores sem apagar a origem |
| 7. Disponibilizar para uso | Execução pelo atalho e preservação dos dados ao atualizar | Abrir e reiniciar normalmente sem depender de Visual Studio ou SQL Server |

As etapas são incrementos de desenvolvimento. A aplicação só passa a usar o modo local como padrão quando os fluxos utilizados estiverem adaptados; evitar misturar gravações reais locais e SQL durante a transição.

### Regras de implementação

- Guardar o arquivo de dados em uma pasta própria do usuário, como `%LocalAppData%\JKCASH`, fora de `bin/Debug` e da pasta de instalação. Atualizar ou recompilar não pode substituir esse arquivo.
- Manter Domain e Application responsáveis pelas regras e coordenação; implementar o armazenamento em Infrastructure e selecionar os repositórios na inicialização.
- Reaproveitar interfaces e serviços onde possível, revisando diferenças de mapeamento, consultas, datas, valores monetários e transações entre provedores. Não presumir que basta trocar a conexão.
- Manter precisão financeira e operações atômicas: transferência com débito e crédito; compra com parcelas e limite; pagamento com saída, fatura e limite; exclusão com seus ajustes.
- Preservar os Ids das entidades, inclusive em uma eventual importação.
- A importação depende de acesso à origem naquele momento. Sem importação, registros existentes apenas no SQL Server não aparecem na base local.
- Não sobrescrever uma base local já preenchida nem mesclar históricos automaticamente. A migração precisa identificar origem e destino e conferir o resultado.
- Neste escopo, salvar com sucesso significa confirmar a gravação local. Não haverá fila ou indicador de sincronização.

### Critérios de conclusão planejados

- Abrir, consultar e registrar sem SQL Server disponível.
- Preservar os dados após encerrar e reiniciar.
- Preservar saldos, limites, vínculos e centavos exatos.
- Uma falha durante uma operação conjunta não deixar efeitos parciais gravados.
- Atualizar o aplicativo sem apagar o arquivo de dados.
- Se houver importação, conferir quantidades, relações, saldos e valores antes de passar a usar a base local.

Esses critérios orientam a conclusão da implementação. O progresso efetivo está registrado acima; não confundir classes criadas com funcionalidades já selecionadas pela interface.

## 004 — Sincronizar os dados entre máquinas

- **Situação:** adiado por decisão do usuário.
- **Objetivo futuro:** usar o mesmo histórico atualizado em duas máquinas, cada uma com persistência local.
- **Fora do escopo atual:** servidor central, API, fila de envio, recebimento de alterações, controle de duplicações, propagação de exclusões e resolução de conflitos.
- **Ponto de atenção futuro:** os dados locais de duas máquinas não se atualizam entre si automaticamente. A solução de sincronização será planejada separadamente antes de disponibilizar esse comportamento.
- **Decisão para o futuro:** avaliar como integrar os dados locais acumulados até lá. Não existe promessa de que poderão ser enviados e mesclados sem uma estratégia de migração e conciliação.
