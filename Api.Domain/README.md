## Code First

- Cria o banco de dados e as tabelas a partir das entidades e da classe de contexto

## Database First

- Acessa o banco de dados existente a partir das entidades e da classe de contexto

## Postman

- Simula um cliente usando minha aplicação

## query string

- inicia no ponto de interrogação

## filtros

Os filtros são **atributos** anexados às classes ou métodos dos controladores que injetam lógica extra ao processamento da requisição e permitem a implementação de funcionalidades relacionadas a **autorização, exception, log e cache** de forma simples e elegante.
Eles permitem executar um código personalizado antes ou depois de executar um método Action.
Permitem também realizar **tarefas repetitivas** comuns a métodos Actions e são chamados em certos estágios do pipeline.

## tipos de filtros

- Authorization -> determina se o usuário está autorizado no request atual. São executados primeiro.

- Resource -> podem executar código antes e depois do resto do filtro ser executado. Tratam do request após a autorização e executam antes do model binding ocorrer.

- Action -> executam o código imediatamente antes e depois do método Action do controlador ser chamado.

- Exception -> são usados para manipular exceções ocorridas antes de qualquer coisa ser escrita no corpo da resposta.

- Result -> executam o código antes ou depois da execução dos resultados das Actions individuais do controlador.