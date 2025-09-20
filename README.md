# Microservices Financial Tracker API

![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)
![Language](https://img.shields.io/badge/C%23-ASP.NET%20Core-blueviolet)
![Language](https://img.shields.io/badge/JavaScript-Node.js-green)
![Infra](https://img.shields.io/badge/Docker-blue)
![Database](https://img.shields.io/badge/PostgreSQL-darkblue)

Projeto de portfólio que implementa uma API para gerenciamento de operações financeiras, construído com uma arquitetura de microserviços híbrida. O objetivo é demonstrar competências em design de sistemas distribuídos, segurança com JWT, orquestração com Docker e a habilidade de escolher a tecnologia certa para cada tarefa.

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único, seguro e gerenciável.

**Fluxo da Requisição:**
`Cliente (Postman/Frontend)` ➔ `Nginx (API Gateway)` ➔ `Serviço Específico` ➔ `PostgreSQL`

* **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado. Ex: `/api/auth/*` vai para o `AuthService`, `/api/billing/*` vai para o `BillingService`.
* **AuthService (ASP.NET Core):** Responsável pelo registro, login e geração de tokens JWT. É o guardião da identidade no sistema.
* **BillingService (ASP.NET Core):** Gerencia toda a lógica de negócio principal: cadastro de operações, definição de metas e controle de faturamento. Acesso protegido por JWT.
* **AnalysisService (Node.js - Planejado):** Serviço leve e ágil para realizar cálculos e projeções sobre as metas financeiras.
* **Banco de Dados (PostgreSQL):** Armazena os dados de todos os serviços em um ambiente containerizado.

---

## 🚀 Tecnologias Utilizadas

### Backend
* **C# / ASP.NET Core:** Utilizado para os serviços críticos (`Auth`, `Billing`) que exigem robustez, performance e um sistema de tipos forte.
* **Node.js / Express (Planejado):** Para serviços auxiliares e leves que se beneficiam da agilidade e do ecossistema do JavaScript.

### Banco de Dados
* **PostgreSQL:** Um banco de dados relacional poderoso e confiável.

### Infraestrutura & Orquestração
* **Docker & Docker Compose:** Containeriza cada serviço, garantindo um ambiente de desenvolvimento e produção consistente e isolado. Orquestra a subida de toda a aplicação com um único comando.
* **Nginx:** Atua como API Gateway e Reverse Proxy, centralizando o acesso aos microserviços.

### Frontend (Planejado)
* **Vue.js:** Framework progressivo para a construção da interface do usuário.
* **TailwindCSS:** Framework de CSS utility-first para um design rápido e moderno.

---

## ✨ Funcionalidades Implementadas

* **Autenticação Segura:** Registro e Login de usuários com senhas criptografadas (BCrypt).
* **Autorização com JWT:** Geração de JSON Web Tokens no login para proteger o acesso aos endpoints sensíveis.
* **Gerenciamento de Operações:** CRUD completo para as operações financeiras do usuário.
* **Gerenciamento de Metas:** Definição e consulta de metas financeiras mensais.
* **Segurança de Dados:** Cada usuário só pode acessar e modificar seus próprios dados, garantido pela validação do `UserId` extraído do token JWT.

---

## 🛠️ Como Executar o Projeto

Siga os passos abaixo para executar a aplicação localmente.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Passos para Instalação

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git](https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git)
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
    Este comando irá construir as imagens dos seus serviços .NET e iniciar todos os contêineres (Nginx, os dois serviços .NET e o PostgreSQL) em segundo plano.

### Testando a API

Após a execução, os serviços estarão disponíveis. Use o Postman ou similar para testar os endpoints:

* **Serviço de Autenticação:**
    * `POST /api/auth/register` - Para criar um novo usuário.
    * `POST /api/auth/login` - Para obter um token JWT.

* **Serviço de Faturamento (requer token JWT):**
    * `GET /api/billing/operacoes` - Lista as operações do usuário logado.
    * `POST /api/billing/operacoes` - Cria uma nova operação.
    * `POST /api/billing/metas` - Define uma meta mensal.

---

## 🔮 Próximos Passos

Este projeto está em desenvolvimento. Os próximos passos planejados são:

-   [ ] Implementar o `AnalysisService` em Node.js para projeções.
-   [ ] Desenvolver o frontend com Vue.js e TailwindCSS.
-   [ ] Configurar um pipeline de CI/CD com GitHub Actions.
-   [ ] Realizar o deploy da aplicação em uma plataforma cloud (Railway, Render ou Azure).

---

## 👨‍💻 Autor

Feito com ❤️ por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)