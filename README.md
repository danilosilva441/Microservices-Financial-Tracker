Microservices Financial Tracker
📄 Sobre o Projeto
O Microservices Financial Tracker é um sistema full-stack para controlo e visualização de performance de operações financeiras. A versão 1.0, atualmente no ar, implementa um MVP (Mínimo Produto Viável) robusto, permitindo o registo de operações, o lançamento de faturamentos e a visualização de um dashboard de performance em tempo real.
O projeto está agora a evoluir para a versão 2.0, que irá transformar a aplicação numa ferramenta de gestão empresarial completa, com um sistema de perfis hierárquico e uma lógica de faturação parcial avançada, focada nas necessidades de operações com múltiplos turnos.
🏛️ Arquitetura
O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único e seguro.
 * API Gateway (Nginx): Roteia as requisições para o microserviço apropriado.
 * AuthService (ASP.NET Core): Gere Utilizadores, Perfis (Roles) e todo o fluxo de autenticação com JWT.
 * BillingService (ASP.NET Core): O coração do sistema. Gere Operações e Faturamentos, aplicando todas as regras de negócio.
 * AnalysisService (Node.js): Motor de cálculo que roda em segundo plano, autentica-se de forma segura e consome dados do BillingService para gerar projeções de faturamento.
 * Frontend (Vue.js + Nginx): A interface do utilizador, containerizada para produção.
 * Base de Dados (PostgreSQL): Cada serviço tem a sua própria base de dados lógica (auth_db, billing_db), garantindo a autonomia.
✨ Funcionalidades (Versão 1.0 - Online)
 * [✅] Autenticação Segura com Perfis (User/Admin): Sistema de login completo com tokens JWT, protegendo o acesso aos dados.
 * [✅] Gestão de Operações: Interface para listar, visualizar detalhes, criar e apagar operações (ações restritas a Admins).
 * [✅] Rastreamento de Faturamentos: Interface para listar, adicionar e apagar faturamentos diários para cada operação.
 * [✅] Dashboard de Performance: Tela principal com cards de KPIs e gráficos que exibem os dados de metas e projeções em tempo real.
 * [✅] Motor de Análise Automático: O AnalysisService calcula as projeções de faturamento e atualiza a base de dados periodicamente.
 * [✅] Infraestrutura Completa: Todos os serviços estão containerizados com Docker e são servidos publicamente através do API Gateway na Railway.
🚀 Roadmap Futuro (Versão 2.0 - Foco no Backend)
O desenvolvimento atual está focado em expandir as capacidades do backend para suportar uma estrutura de gestão empresarial complexa. As alterações no frontend serão feitas numa fase posterior.
Fase 1: Fundações - Nova Estrutura de Dados
 * AuthService:
   * [🎯] Perfis Hierárquicos: Substituir os perfis "User/Admin" por uma estrutura granular: Dev, Gerente, Supervisor, Lider, Operador.
   * [🎯] Estrutura de Utilizadores: Alterar a entidade User para incluir um campo de hierarquia (ex: ReportsToUserId) que define a relação chefe-subordinado.
   * [🎯] Refresh Tokens: Implementar uma nova tabela e lógica para gerir tokens de atualização (refresh tokens), aumentando a segurança e a experiência do utilizador.
 * BillingService:
   * [🎯] Faturação Parcial: Transformar a entidade Faturamento em FaturamentoParcial, com campos de HoraInicio e HoraFim.
   * [🎯] Detalhes de Pagamento: Criar novas entidades para que cada FaturamentoParcial possa detalhar os valores por método de pagamento (Dinheiro, Cartão, etc.).
   * [🎯] Faturação Diária: Criar uma nova entidade FaturamentoDiario que consolida automaticamente os faturamentos parciais de um dia.
   * [🎯] Vínculo por Função: Adicionar um campo Role à tabela UsuarioOperacao para definir a função de um utilizador (Lider, Operador) dentro de uma operação específica.
Fase 2: Lógica de Negócio - Faturação Avançada (BillingService)
 * [🎯] Endpoint de Faturação Parcial: Criar o novo endpoint para receber os lançamentos de turno.
 * [🎯] Validação de Horários: Implementar a lógica que garante que o horário de início de um faturamento parcial seja sequencial ao anterior.
 * [🎯] Consolidação Automática: Desenvolver o mecanismo que atualiza o FaturamentoDiario sempre que um FaturamentoParcial é criado ou alterado.
 * [🎯] Lançamentos D+1: Criar a lógica para lidar com turnos que terminam depois da meia-noite, atribuindo-os ao dia operacional correto.
Fase 3: Segurança - Hierarquia e Refresh Tokens (Auth & Billing)
 * [🎯] AuthService:
   * Alterar o endpoint /api/token para retornar tanto o access_token (curta duração) quanto o refresh_token (longa duração).
   * Criar o novo endpoint /api/token/refresh para a renovação de tokens de acesso.
 * [🎯] BillingService:
   * Implementar a lógica de autorização hierárquica nos endpoints. Por exemplo, o GET /api/operacoes deverá retornar dados diferentes com base no perfil (Gerente, Supervisor, etc.) extraído do token.
🛠️ Como Executar o Projeto (Localmente)
 * Clone o repositório.
 * Crie um ficheiro .env na raiz do projeto, baseado no docker-compose.yml, para as suas senhas (POSTGRES_PASSWORD, JWT_KEY, SYSTEM_PASSWORD).
 * Inicie os contentores: docker-compose up --build -d.
 * Crie o utilizador de sistema (necessário para o AnalysisService funcionar): O script de inicialização do analysis_service irá criar e promover o utilizador system@internal.service automaticamente.
 * Aceda à aplicação através da porta do api_gateway: http://localhost:8080.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)