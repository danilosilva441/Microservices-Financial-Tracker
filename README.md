# Microservices Financial Tracker API

![Status](https://img.shields.io/badge/status-backend%20concluído-brightgreen)
![Language](https://img.shields.io/badge/C%23-ASP.NET%20Core-blueviolet)
![Language](https://img.shields.io/badge/JavaScript-Node.js-green)
![Infra](https://img.shields.io/badge/Docker-blue)
![Database](https://img.shields.io/badge/PostgreSQL-darkblue)

Projeto de portfólio que implementa uma API para gerenciamento de operações financeiras, construído com uma arquitetura de microserviços híbrida. O objetivo é demonstrar competências em design de sistemas distribuídos, segurança com autenticação e autorização (JWT com Roles), orquestração com Docker e a habilidade de escolher a tecnologia certa para cada tarefa.

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único, seguro e gerenciável.

**Fluxo da Requisição:**
`Cliente (Postman/Frontend)` ➔ `Nginx (API Gateway)` ➔ `Serviço Específico` ➔ `PostgreSQL`

* **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado baseado na URL.
* **AuthService (ASP.NET Core):** Responsável por gerenciar Usuários e Perfis (Roles), processar registro/login e gerar tokens JWT seguros que incluem as `claims` de perfil do usuário.
* **BillingService (ASP.NET Core):** Gerencia toda a lógica de negócio principal: cadastro de Operações (contratos), suas Metas Mensais e o registro de Faturamentos (ganhos) individuais. O acesso é protegido por JWT.
* **AnalysisService (Node.js):** Serviço leve e stateless que atua como um motor de cálculo, recebendo um histórico de faturamentos para projetar a performance futura em relação a uma meta.
* **Banco de Dados (PostgreSQL):** Armazena os dados de todos os serviços em um ambiente containerizado e relacional.

---

## 🚀 Tecnologias Utilizadas

### Backend
* **C# / ASP.NET Core:** Utilizado para os serviços críticos (`Auth`, `Billing`) que exigem robustez, performance e um sistema de tipos forte.
* **Node.js / Express:** Utilizado para o serviço de análise, que se beneficia da agilidade e da facilidade em manipular JSON.

### Banco de Dados
* **PostgreSQL:** Um banco de dados relacional poderoso e confiável.
* **Entity Framework Core:** ORM para o mapeamento objeto-relacional nos serviços .NET.

### Infraestrutura & Orquestração
* **Docker & Docker Compose:** Containeriza cada serviço, garantindo um ambiente de desenvolvimento consistente e isolado. Orquestra a subida de toda a aplicação com um único comando.
* **Nginx:** Atua como API Gateway e Reverse Proxy, centralizando o acesso aos microserviervços.

---

## ✨ Funcionalidades

* **Autenticação & Autorização:**
    * Registro e Login com senhas criptografadas (`BCrypt`).
    * Geração de Tokens JWT contendo `claims` de perfil (`Role`).
    * Estrutura de Perfis (`Roles`) relacional no banco de dados (`User`, `Admin`), permitindo controle de acesso granular.

* **Gerenciamento Financeiro:**
    * CRUD para **Operações** (vistas como Contratos ou Projetos).
    * Associação de uma **Meta Mensal** a cada Operação.
    * Endpoints para registrar **Faturamentos** (ganhos) individuais para cada Operação, criando um histórico de performance.

* **Consultas Avançadas:**
    * API com filtros dinâmicos para buscar operações por **ano**, **mês** e **status** (`isAtiva`).

* **Análise Preditiva:**
    * Serviço de análise que recebe um histórico de faturamentos e projeta a performance em relação à meta.

* **Design de API RESTful:**
    * Controladores organizados pelo Princípio da Responsabilidade Única (`UsersController`, `TokenController`, `OperacoesController`, etc.).
    * Uso de DTOs (Data Transfer Objects) para desacoplar a API do modelo de dados do banco.

---

## 🛠️ Como Executar o Projeto

Siga os passos abaixo para executar a aplicação completa localmente.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos para Instalação

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git](https://github.com/danilosilva441/Microservices-Financial-Tracker.git)
    ```

2.  **Navegue até a pasta do projeto:**
    ```bash
    cd SEU-REPOSITORIO
    ```

3.  **Configuração de Senha:**
    Certifique-se de que a senha do PostgreSQL no arquivo `docker-compose.yml` (`POSTGRES_PASSWORD`) é a mesma utilizada nos arquivos `appsettings.json` dos serviços `AuthService` e `BillingService`.

4.  **Suba os contêineres com Docker Compose:**
    ```bash
    docker-compose up -d --build
    ```
    Este comando irá construir as imagens de cada serviço e iniciar todos os contêineres em segundo plano. Para verificar se todos subiram, use `docker ps`.

### Endpoints da API

Todas as requisições devem ser feitas para a porta do API Gateway (ex: `http://localhost:8080`).

* **AuthService**
    * `POST /api/users`: Registra um novo usuário.
    * `POST /api/token`: Realiza o login e obtém um token JWT.

* **BillingService** (requer Bearer Token de autorização)
    * `GET /api/operacoes`: Lista operações (aceita filtros `?ano=`, `?mes=`, `?isAtiva=`).
    * `POST /api/operacoes`: Cria uma nova operação/contrato com sua meta mensal.
    * `PATCH /api/operacoes/{id}/desativar`: Desativa uma operação.
    * `POST /api/operacoes/{operacaoId}/faturamentos`: Registra um novo faturamento para uma operação.
    * `POST /api/metas`: Define uma meta geral para o usuário em um mês/ano.
    * `GET /api/metas`: Busca a meta geral do usuário.

* **AnalysisService**
    * `POST /api/analysis/projetar`: Realiza a projeção de performance.

---

## 🔮 Próximos Passos

-   [ ] Desenvolver o frontend com Vue.js e TailwindCSS.
-   [ ] Implementar endpoints protegidos por Perfil (ex: `[Authorize(Roles = "Admin")]`).
-   [ ] Configurar um pipeline de CI/CD com GitHub Actions.
-   [ ] Realizar o deploy da aplicação em uma plataforma cloud (Railway, Render ou Azure).

---

## 👨‍💻 Autor

Feito com ❤️ por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)