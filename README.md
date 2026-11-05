# JKCASH

## Cadastro de gastos

O menu **Gastos** reúne o cadastro reutilizável e as categorias auxiliares. Em **Transações > Gasto (débito ou crédito)**, selecione um gasto salvo ou crie um novo, com opção de salvá-lo para reutilizar. O motivo é obrigatório quando a necessidade ou a importância for baixa. Frequência: Eventual ou Mensal. Veja [como experimentar a alteração e os detalhes de compatibilidade](docs/cadastro-gastos.md).


## Abrir no Visual Studio

Depois de clonar o repositório, abra `JKCASH.sln`, na raiz, usando **Arquivo > Abrir > Projeto/Solução**. Essa solução reúne os projetos do JKCASH. Abrir apenas a pasta do repositório deixa o Visual Studio no modo de pastas.

Use o Visual Studio com suporte ao .NET 10 e o SDK do .NET 10 instalado. Para iniciar a aplicação, defina o projeto `JKCASH` como projeto de inicialização.

## Compilar

```powershell
dotnet build JKCASH.sln
```

O projeto de estudos `C#Aprendizado` tem uma solução própria em `C#Aprendizado/C#Aprendizado.slnx`.

## Interface Windows Forms

As consultas e gravações das telas executam em segundo plano, com bloqueio de cliques repetidos. Contas, Categorias, Transações e Cartões oferecem atualização dos dados após falhas. Quando um registro foi salvo e apenas a atualização da tela falha, a mensagem informa essa diferença. Ao sair de uma tela, uma gravação já iniciada pode concluir; consulte os dados ao retornar. Detalhes da entrega: [Etapa 5 — Responsividade](docs/etapa-5-responsividade.md).

O carregamento dos indicadores é organizado por `IDashboardConsulta`/`DashboardConsulta` e devolve um `DashboardResumo`. As somas são feitas no banco, as faturas são filtradas por conjunto e apenas oito movimentações recentes são carregadas para a tabela. A tela cuida da apresentação. Veja [consulta do dashboard e limites da verificação](docs/dashboard-consulta.md).

Defina **JKCASH.WinForms** como projeto de inicialização e execute com F5. Todas as telas usam o banco local SQLite em `%LocalAppData%\JKCASH\jkcash.db`, sem depender do serviço SQL Server. Na abertura, a aplicação cria a pasta de dados e aplica as migrações pendentes antes de mostrar a janela principal. Se a preparação falhar, informa o erro e não abre as telas com um banco incompleto. O arquivo fica fora da pasta do executável para preservar os dados nas atualizações.

Os registros existentes apenas no SQL Server não são importados automaticamente. O Console mantém os modos SQL e em memória descritos abaixo. A integração local do Windows Forms foi compilada sem erros; a validação dos fluxos em execução permanece pendente.

O dashboard segue a referência visual aprovada: menu azul escuro, fundo claro, botões verdes, cartões de indicadores, barras por categoria e tabela de movimentações. Contas e Categorias permitem consultar e cadastrar localmente; Transações permite registrar movimentações e consultar o extrato. Cartões permite cadastrar cartões, registrar compras e consultar o histórico e as parcelas. Faturas permite consultar os ciclos e registrar pagamentos totais ou parciais.

O **dashboard** consulta dados reais do SQLite local. O filtro inclui todas as contas (inclusive inativas) ou uma conta específica, e permite escolher o mês. Saldo e limites são atuais; receitas, despesas, saídas por categoria e últimas movimentações correspondem ao mês selecionado. Transferências internas não contam como receitas ou despesas. Faturas pendentes somam o saldo ainda não pago dos ciclos com vencimento no mês, dos cartões vinculados às contas selecionadas. Categorias adicionais são agrupadas em Outras categorias no gráfico. Use Atualizar ou volte ao dashboard para consultar novamente.

Em **Cartões**, a primeira aba cadastra cartões e mostra limites. Em **Registrar compra**, selecione cartão, categoria, importância, planejamento, urgência, data, valor total e parcelas (1 para à vista, até 120), além de descrição e justificativa. O serviço existente valida a compra e grava compra, parcelas, faturas e limite em conjunto. **Compras do cartão** mostra o histórico e os detalhes de cada compra, incluindo os vencimentos das parcelas. Compras futuras ou de ciclos encerrados são recusadas. O saldo da conta não muda ao comprar no cartão.

Em **Faturas**, selecione um cartão e uma fatura para ver parcelas e pagamentos confirmados. Escolha uma conta ativa (pode ser de outro banco), a categoria da saída e o valor já pago. Preencher valor pendente prepara o valor; somente **Confirmar pagamento** grava. A data é a de hoje. O serviço aceita pagamentos parciais, recusa valores acima do pendente e confere saldo e cheque especial. Histórico, saída da conta, saldo e limite são gravados juntos. A indicação de atraso não calcula juros nem multas. Identificadores técnicos ficam internos; as telas financeiras permanecem dentro da janela principal.

### Extrato e detalhes na interface gráfica

Em **Contas > Transferir entre contas**, selecione duas contas ativas, informe valor, categoria, importância, planejamento, urgência, descrição e justificativa. A origem é retirada das opções de destino. A tela mostra o saldo atual e o disponível com cheque especial, mas o serviço consulta novamente as contas ao confirmar. A data é a de hoje. **Confirmar transferência** grava os dois saldos e uma saída/entrada vinculadas na mesma operação. Ao concluir, os saldos da tela de Contas são atualizados; o extrato e o dashboard consultam os dados ao abrir ou atualizar suas telas. Transferências internas não contam como receitas ou despesas. **Atualizar contas** consulta os saldos e as categorias novamente.

Em **Transações > Extrato**, escolha as datas inicial e final. O período padrão é do início do mês até hoje, incluindo os dois dias inteiros. **Todas as contas** inclui também contas inativas, preservando a consulta ao histórico. Para consultar uma ou várias contas, desmarque essa opção e marque as contas desejadas na lista. Use **Consultar extrato** para aplicar os filtros; retornar à aba também atualiza o resultado.

A tabela mostra data, nome da conta, descrição, categoria, tipo e valor, sem identificadores técnicos. Cada página contém até 50 movimentações; use Anterior e Próxima para navegar. Os totais abrangem todo o período e todas as páginas, separando receitas e despesas das transferências internas. O resultado do período não é o saldo atual da conta. Alterar datas ou contas limpa a consulta anterior; clique em Consultar extrato para aplicar os novos filtros.

Selecione uma linha e clique em **Ver detalhes**, ou dê dois cliques na linha. Um painel na própria tela mostra conta, banco, data, tipo, valor, categoria, importância, planejamento, urgência, descrição e justificativa. Selecionar outra linha atualiza esse painel; **Fechar detalhes** amplia novamente a tabela. Nenhuma janela adicional é aberta.

Datas invertidas, seleção vazia, ausência de movimentações e falhas de conexão são informadas na própria aba. As consultas executam em segundo plano para manter a navegação responsiva e não gravam no banco. `UcTransacoes.Designer.cs` guarda a estrutura dos controles; `UcTransacoes.Extrato.cs` organiza filtros, paginação e detalhes. A leitura usa `IExtratoConsulta`, implementada por `ExtratoConsulta`, e recebe um `ExtratoPagina` com os registros e os totais já calculados. O serviço de registro continua usando `ITransacaoRepositorio`. Detalhes: [Extrato paginado](docs/extrato-paginado.md).

`FrmPrincipal.Designer.cs` mantém a estrutura dos painéis da janela. `FrmPrincipal.Visual.cs` configura o menu e a navegação em execução; abra esse arquivo como código. `Telas/UcDashboard.cs` organiza o conteúdo com `TableLayoutPanel`, `Panel`, `Label`, `ComboBox`, `Button` e `DataGridView`. `Visual/EstiloVisual.cs` centraliza a paleta, botões, ícones e desenho das bordas, usando somente Windows Forms e System.Drawing. Para conferir o acabamento completo, execute o projeto; o designer da janela mantém apenas a estrutura básica.

A referência e uma captura renderizada da interface ficam em `docs/design/`.

## Executar sem banco de dados

No Visual Studio, selecione o perfil **JKCASH (Memoria)** ao lado do botão de iniciar e pressione F5. Todos os cadastros e movimentações ficam em memória durante a execução; os dados são perdidos ao fechar o programa.

Pelo terminal, na raiz do repositório:

```powershell
dotnet run --project JKCASH/JKCASH.csproj --no-launch-profile -- --memoria
```

## Executar com SQL

No Visual Studio, selecione o perfil **JKCASH (SQL)**. Esse modo precisa de acesso ao banco configurado e das tabelas já criadas. Contas, categorias, transações, cartões, compras, parcelas, faturas e pagamentos são salvos no SQL.

```powershell
dotnet run --project JKCASH/JKCASH.csproj --no-launch-profile
```

Sem o argumento `--memoria`, a aplicação mantém o comportamento de usar SQL. Os perfis do Visual Studio também podem ser usados no terminal com `--launch-profile "JKCASH (Memoria)"` ou `--launch-profile "JKCASH (SQL)"`.

Para atualizar um banco existente com o suporte a transferências, execute `BancoDeDados/Scripts/001-transferencias.sql` no SQL Server. O script pode ser executado novamente: adiciona somente a coluna opcional `TransferenciaId` e seu índice, preservando os registros existentes. Essa atualização já foi aplicada no banco local usado no desenvolvimento.

## Navegação por submenus

O menu principal mostra somente os grupos. Dentro de cada submenu, **0 - Voltar** retorna ao principal; no principal, **0 - Sair** encerra o programa.

| Menu principal | Operações |
|---|---|
| 1 - Contas | Cadastrar, listar, excluir e transferir entre contas |
| 2 - Transações | Registrar, consultar, extrato e resumos por categoria, planejamento, importância e urgência |
| 3 - Cartões | Cadastrar e listar cartões, registrar e consultar compras |
| 4 - Faturas | Listar, consultar parcelas e pagamentos, registrar pagamento |
| 5 - Categorias | Cadastrar e listar categorias |
| 6 - Excluir registros | Excluir contas, categorias, transações, transferências, cartões, compras, faturas, pagamentos e parcelas |

Cada classe de menu organiza suas operações. `NavegacaoConsole` reutiliza a exibição das opções, o retorno e o tratamento de erros. `Program.cs` cria os repositórios e serviços conforme o modo escolhido. As interfaces de compras e faturas recebem a mesma instância de repositório.

Em **Contas > Excluir conta**, selecione uma conta: a tela informa quantas transações serão apagadas junto com ela. Digite **EXCLUIR** para confirmar a exclusão definitiva da conta e de suas transações; qualquer outra resposta cancela. As categorias são preservadas. Contas com cartões ou pagamentos de fatura vinculados continuam bloqueadas, inclusive quando pagam a fatura de um cartão de outra conta. No SQL, um único `SaveChanges` remove as transações e a conta em uma transação do banco: se uma exclusão falhar, tudo é desfeito. O serviço mantém a opção de excluir sem histórico bloqueada por padrão; a remoção conjunta exige a opção explícita `excluirTransacoes: true`.

## Transferências entre contas

Em **Contas > 4 - Transferir entre contas**, escolha a origem, informe o valor e selecione o destino. O destino deve ser outra conta ativa. O valor deve ser positivo, ter no máximo duas casas decimais e caber no saldo e cheque especial da origem. O menu verifica o valor antes de pedir os demais dados. Selecione a categoria, importância, planejamento, urgência, descrição e justificativa. A data e o horário são os do registro da transferência.

`TransferenciaServico` coordena o débito e o crédito. A entidade `Transferencia` cria dois registros `Transacao` ligados pelo mesmo `TransferenciaId`: saída na origem e entrada no destino. O repositório SQL grava os dois saldos e os dois registros na mesma transação; uma falha desfaz toda a operação. Leituras e atualizações da transferência usam isolamento serializável, e saldos desatualizados são recusados. Em memória, os registros são preparados em uma cópia e o serviço restaura os saldos se o armazenamento falhar.

O extrato identifica as transferências e apresenta os totais enviados e recebidos separadamente das entradas e saídas comuns. Os resumos de gastos por categoria, planejamento, importância e urgência não incluem transferências internas, mesmo quando apenas uma das contas é selecionada. Transferências não modificam os saldos iniciais e não representam compras no cartão nem pagamentos de fatura.

Contas que participam de transferências exigem a exclusão prévia das transferências em **Excluir registros > Transferências**. A exclusão da transferência remove os dois registros juntos e reverte os dois saldos. Selecionar uma das transações de uma transferência nesse submenu também remove o par completo.

## Exclusão de registros no Console

Abra **Menu principal > 6 - Excluir registros**. Selecione o tipo e o registro: o programa apresenta os registros afetados e as mudanças de saldo e limite antes da confirmação. Digite **EXCLUIR** para confirmar; qualquer outra resposta cancela. A exclusão é definitiva e não gera backup.

- **Cartão:** remove suas compras, todas as parcelas, faturas, pagamentos e as saídas associadas aos pagamentos. O saldo das contas pagadoras é recomposto. As contas e movimentações sem vínculo com o cartão permanecem.
- **Transação:** desfaz seu efeito no saldo. Quando vinculada a pagamento, também reverte esse pagamento; quando vinculada a transferência, remove os dois lados.
- **Compra ou parcela:** remove a compra inteira e todas as suas parcelas, inclusive de outros meses, e libera o limite correspondente. Faturas que ficarem vazias são removidas. Havendo pagamentos nas faturas afetadas, remova esses pagamentos primeiro.
- **Fatura:** remove as compras completas relacionadas e suas parcelas de todos os meses. Pagamentos nas faturas afetadas precisam ser excluídos primeiro. A prévia mostra a abrangência da remoção.
- **Pagamento:** remove o pagamento e sua saída na conta, devolve o dinheiro à conta e recompõe o valor utilizado no cartão. A operação é recusada se ultrapassar o limite atual do cartão.
- **Conta:** remove a conta e suas transações comuns. Cartões, pagamentos e transferências vinculados precisam ser removidos primeiro.
- **Categoria:** somente pode ser removida se não estiver em uso por transações ou compras.

A exclusão não pode deixar uma conta abaixo do cheque especial nem o cartão fora de seu limite. A aplicação verifica se os dados mudaram desde a prévia. No SQL, exclusões e ajustes são confirmados em uma única transação serializável; uma falha desfaz a operação inteira. Em memória, as coleções são preparadas antes da substituição. Essa opção está disponível no Console nos dois modos de execução.

## Cartões de crédito

Uma conta pode ter nenhum, um ou vários cartões. Cada cartão pertence a uma conta, e o banco do cartão é o banco dessa conta.

Cadastre uma conta em **Contas > Cadastrar conta**. Em **Cartões > Cadastrar cartão**, selecione a conta e informe os dados do cartão. **Cartões > Listar cartões e limites** mostra os cartões com a conta e o banco correspondentes. Cadastrar ou comprar no cartão não debita automaticamente o saldo da conta.

Cartões, compras, parcelas, faturas e pagamentos usam memória no perfil **JKCASH (Memoria)** e persistência no banco no perfil **JKCASH (SQL)**. Pagamentos de faturas estão disponíveis nos dois modos. No SQL, compras, parcelas, faturas e limite são gravados juntos; cada pagamento grava seu histórico, a saída da conta, o saldo e o limite na mesma transação do banco.

## Compras, parcelas e faturas

| Caminho | Operação |
|---|---|
| Cartões > Registrar compra à vista ou parcelada | Criar compra e suas parcelas |
| Cartões > Listar compras de um cartão | Consultar compras e parcelas |
| Cartões > Buscar compra por Id | Localizar uma compra |
| Faturas > Listar faturas de um cartão | Consultar totais por ciclo |
| Faturas > Consultar parcelas e pagamentos | Consultar o histórico da fatura |
| Faturas > Registrar pagamento total ou parcial | Registrar um pagamento confirmado |

Ao cadastrar cada cartão, escolha se compras feitas no próprio dia de fechamento entram na fatura atual ou na próxima. Quando a escolha é a fatura atual, ela permanece aberta durante esse dia e encerra no dia seguinte. Na outra opção, o ciclo encerra no início do dia de fechamento.

Uma compra compromete o valor total no limite imediatamente. Uma compra de R$ 300 em 3 vezes gera três parcelas de R$ 100 em faturas consecutivas. Uma compra à vista gera uma parcela. Nesta versão, são aceitas de 1 a 120 parcelas, cada uma de pelo menos um centavo. Diferenças de arredondamento ficam na última parcela: R$ 100 em 3 vezes vira R$ 33,33 + R$ 33,33 + R$ 33,34.

Parcelas do mesmo cartão e ciclo são agrupadas em uma fatura. As datas usam o dia configurado ou o último dia do mês, se necessário; em março, por exemplo, o dia 31 volta a ser usado depois de fevereiro. O vencimento é a primeira ocorrência do dia configurado após o fechamento. Não há ajuste por feriados ou dias úteis.

O fechamento acontece automaticamente conforme a data, sem precisar deixar o programa aberto. Uma fatura fechada não recebe novas compras. O cadastro aceita compras até hoje cujo primeiro ciclo ainda esteja aberto; importação de compras históricas de ciclos já fechados não faz parte desta versão.

Uma fatura aberta pode receber pagamento antecipado e continua aberta para novas compras até o fechamento. A tela mostra separadamente a situação do ciclo (aberta, fechada ou paga), o total pago e o saldo pendente. Após o vencimento, o saldo restante é sinalizado como atrasado. Sem juros, multas, rotativo ou parcelamento do saldo da fatura. O saldo pendente permanece na fatura original.

## Pagamentos confirmados

Transações, transferências e pagamentos conferem o saldo e as condições atuais da conta durante a gravação no SQLite e no SQL Server. Pagamentos também conferem a fatura e o limite do cartão. Se outra operação alterar esses dados, a movimentação é recusada e as telas atualizam os valores para revisão antes de uma nova confirmação. Uma falha na gravação desfaz toda a operação; não há repetição automática. O botão de registro fica bloqueado durante o processamento para evitar cliques repetidos.

Em **Faturas > Registrar pagamento total ou parcial**, registre um pagamento já confirmado, na data de hoje. Selecione a fatura, a conta pagadora (que pode ser de outro banco), a categoria da saída e o valor. Valores precisam ser positivos, ter até duas casas decimais e não exceder o saldo pendente. A conta deve estar ativa e ter saldo ou cheque especial disponível.

Cada pagamento gera uma saída na conta e um registro no histórico da fatura, ligados pelo Id da transação. O limite do cartão é liberado somente pelo valor pago. É possível pagar R$ 200, depois R$ 100 e depois R$ 200 para quitar uma fatura de R$ 500. As parcelas de outros meses permanecem comprometendo limite até seus próprios pagamentos.

O serviço desfaz a movimentação do saldo e do limite se o armazenamento falhar. No repositório em memória, compras e suas faturas são preparadas antes da confirmação; pagamentos e saídas também respeitam o contrato de salvar juntos. As compras não geram saídas na conta: a saída é registrada no pagamento, evitando duplicar a movimentação do saldo.

### Roteiro para experimentar

1. Escolha o perfil de execução e cadastre uma conta com saldo em **Contas > Cadastrar conta**.
2. Cadastre uma categoria em **Categorias** e um cartão em **Cartões**.
3. Em **Cartões > Registrar compra à vista ou parcelada**, registre uma compra de hoje, por exemplo R$ 300 em 3 vezes.
4. Confira o limite e as compras em **Cartões** e os três ciclos em **Faturas > Listar faturas de um cartão**.
5. Em **Faturas > Registrar pagamento total ou parcial**, pague R$ 40 da primeira fatura. Ela ficará com R$ 60 pendentes e o limite disponível aumentará R$ 40.
6. Consulte o histórico em **Faturas > Consultar parcelas e pagamentos** e a saída em **Transações > Listar todas as transações**.
7. Pague os R$ 60 restantes e confira que os R$ 200 das parcelas futuras continuam comprometendo o limite.
8. No perfil SQL, feche e abra novamente para confirmar a permanência dos dados. No perfil em memória, os dados são descartados ao encerrar.
