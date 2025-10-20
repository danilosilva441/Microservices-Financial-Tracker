Microservices Financial Tracker
​📄 Sobre o Projeto
​O Microservices Financial Tracker é um sistema full-stack para controlo e visualização de performance de operações financeiras. A versão 1.0 implementa um MVP (Mínimo Produto Viável) robusto, permitindo o registo de operações, o lançamento de faturamentos e a visualização de um dashboard de performance em tempo real, tudo a funcionar num ambiente de produção na nuvem.
​O projeto demonstra a criação de uma aplicação completa, desde a infraestrutura com Docker e um API Gateway, passando por um backend seguro com regras de negócio, até uma interface reativa no frontend.
​🏛️ Arquitetura
​O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único e seguro.
​API Gateway (Nginx): Roteia as requisições para o microserviço apropriado.
​AuthService (ASP.NET Core): Gere Utilizadores, Perfis (Roles) e todo o fluxo de autenticação com JWT.
​BillingService (ASP.NET Core): O coração do sistema. Gere Operações e Faturamentos, aplicando todas as regras de negócio.
​AnalysisService (Node.js): Motor de cálculo que roda em segundo plano, autentica-se de forma segura e consome dados do BillingService para gerar projeções de faturamento.
​Frontend (Vue.js + Nginx): A interface do utilizador, containerizada para produção.
​Base de Dados (PostgreSQL): Cada serviço tem a sua própria base de dados lógica (auth_db, billing_db), garantindo a autonomia.
​✨ Funcionalidades Atuais (v1.0)
​[✅] Autenticação Segura com Perfis (User/Admin): Sistema de login completo com tokens JWT, protegendo o acesso aos dados e controlando a visibilidade de funcionalidades no frontend.
​[✅] Gestão de Operações: Interface para listar, visualizar detalhes, criar e apagar operações (ações restritas a Admins).
​[✅] Rastreamento de Faturamentos: Interface para listar, adicionar e apagar faturamentos diários para cada operação.
​[✅] Dashboard de Performance: Tela principal com cards de KPIs e gráficos (Chart.js) que exibem os dados de metas e projeções em tempo real.
​[✅] Motor de Análise Automático: O AnalysisService calcula as projeções de faturamento e atualiza a base de dados periodicamente.
​[✅] Infraestrutura Completa: Todos os serviços estão containerizados com Docker e prontos para deploy.
​🛠️ Como Executar o Projeto (Localmente)
​Clone o repositório.
​Crie um ficheiro .env na raiz do projeto, baseado no docker-compose.yml, para as suas senhas (POSTGRES_PASSWORD, JWT_KEY, SYSTEM_PASSWORD).
​Inicie os contentores: docker-compose up --build -d.
​Aceda à aplicação através da porta do api_gateway: http://localhost:8080.
​🔮 O Que Vem a Seguir? (Próximas Evoluções)
​Com a base da aplicação estável, o desenvolvimento futuro irá focar-se em transformar o sistema numa ferramenta de gestão ainda mais poderosa. As próximas grandes funcionalidades planeadas para o backend incluem:
​Hierarquia de Perfis Avançada: Introdução de perfis como Gerente e Supervisor, com permissões em cascata.
​Faturação por Turnos: Evolução do sistema de faturação para suportar múltiplos lançamentos parciais por dia.
​Segurança Reforçada: Implementação de Refresh Tokens para um sistema de autenticação mais seguro e duradouro.
​Estas melhorias serão desenvolvidas primeiro no backend, com a interface do frontend a ser atualizada numa fase posterior.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)