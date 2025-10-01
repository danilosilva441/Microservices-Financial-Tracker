# Microservices Financial Tracker

![Status](https://img.shields.io/badge/status-Backend%20Concluído%20%7C%20Frontend%20em%20Desenvolvimento-blue)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Frontend](https://img.shields.io/badge/Frontend-Vue.js-green)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL-darkblue)

## 📄 Sobre o Projeto
O **Microservices Financial Tracker** é um sistema full-stack projetado para gerenciar operações financeiras, rastrear faturamentos diários, gerenciar clientes mensais (mensalistas) e gerar projeções de performance. O projeto foi construído com uma arquitetura de microserviços para garantir escalabilidade, autonomia das equipes e resiliência.

O objetivo é demonstrar competências em todo o ciclo de vida do desenvolvimento de software, desde o design de uma arquitetura robusta e a implementação de regras de negócio complexas, até a orquestração da infraestrutura com Docker e a criação de uma API segura e profissional.

---

## 🏛️ Arquitetura
O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único e seguro.

- **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado.
- **AuthService (ASP.NET Core):** Gerencia Usuários, Perfis (Roles) e todo o fluxo de autenticação, gerando tokens JWT.
- **BillingService (ASP.NET Core):** O coração do sistema. Gerencia Operações, Faturamentos, Empresas, Mensalistas e o fluxo de aprovação de ajustes, aplicando todas as regras de negócio.
- **AnalysisService (Node.js):** Motor de cálculo proativo que, de forma segura e autenticada, consome dados do `BillingService` para gerar projeções de performance.
- **Banco de Dados (PostgreSQL):** Cada serviço possui seu próprio banco de dados (`auth_db`, `billing_db`, `analysis_db`), garantindo a autonomia dos microserviços.

---

## ✨ Funcionalidades Implementadas

### ✅ Backend & Infraestrutura (100% Concluído)
- **Arquitetura Robusta:**
  - [✅] Microserviços independentes com bancos de dados separados.
  - [✅] Orquestração de startup com `healthchecks` no Docker Compose.
  - [✅] API Gateway com Nginx para roteamento centralizado.
  - [✅] Código .NET organizado em camadas (`Controllers`, `Services`, `Repositories`) e com configuração modular.
- **Autenticação & Autorização:**
  - [✅] Sistema de Login/Registro com Tokens JWT.
  - [✅] Sistema de Perfis (`User`, `Admin`) com permissões granulares.
  - [✅] Endpoint administrativo para promover usuários.
  - [✅] Autorização por vínculo: um usuário só pode acessar os dados das operações às quais está vinculado.
- **Lógica de Negócio Completa:**
  - [✅] CRUD completo para `Operacoes`, `Empresas`, `Mensalistas`, `Faturamentos` e `Metas`.
  - [✅] Vínculo automático do criador à sua nova operação.
  - [✅] Validação de regras de negócio (ex: regra D+1 para registro de faturamentos).
  - [✅] **Fluxo de Aprovação:** Sistema completo onde usuários solicitam ajustes (alteração/remoção) de faturamentos e um Admin pode aprovar ou rejeitar, com a alteração sendo aplicada automaticamente no banco.
- **Comunicação Segura entre Serviços:**
  - [✅] O `AnalysisService` se autentica no `AuthService` como um "usuário de sistema" para obter um token JWT e acessar o `BillingService` de forma segura.

### 🟡 Frontend (Parcialmente Implementado)
- **Base Sólida:**
  - [✅] Projeto criado com Vue 3, Vite, Pinia e Vue Router.
  - [✅] Layout principal com menu lateral e navegação funcional.
  - [✅] Sistema de autenticação completo (Login, Logout, armazenamento de token e rotas protegidas).
- **Funcionalidades:**
  - [✅] Listagem e criação de Operações.
  - [✅] Tela de Detalhes da Operação com listagem de faturamentos.
  - [✅] Formulário para adicionar novos faturamentos.
  - [✅] Funcionalidade completa para o **usuário solicitar ajustes** de faturamento.
  - [✅] Tela de Admin para **gerenciar e aprovar/rejeitar** as solicitações.

---

## 🛠️ Como Executar o Projeto

Siga os passos abaixo para executar a aplicação completa em seu ambiente local.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos
1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/danilosilva441/Microservices-Financial-Tracker.git](https://github.com/danilosilva441/Microservices-Financial-Tracker.git)
    cd Microservices-Financial-Tracker
    ```
2.  **Configure as Variáveis de Ambiente:**
    Verifique o arquivo `docker-compose.yml` e ajuste, se necessário, as variáveis de ambiente, como a senha do `SYSTEM_PASSWORD` e a chave `Jwt__Key`.

3.  **Inicie os Contêineres:**
    ```bash
    docker-compose up --build -d
    ```
4.  **Acesse a Aplicação:**
    * A API estará disponível através do Gateway em `http://localhost:8080`.
    * O frontend de desenvolvimento estará rodando em `http://localhost:5173`.

---

## 📡 Documentação da API (Endpoints)

| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| **AuthService** | | | |
| `POST` | `/api/users` | Nenhuma | Registra um novo usuário. |
| `POST` | `/api/token` | Nenhuma | Autentica um usuário e retorna um token JWT. |
| `POST` | `/api/admin/promote-to-admin` | Nenhuma | Promove um usuário existente a Admin. |
| **BillingService** | | | |
| `GET` | `/api/operacoes` | Bearer Token | Lista operações (filtradas por vínculo ou todas se for serviço interno). |
| `GET` | `/api/operacoes/{id}` | Bearer Token | Busca detalhes de uma operação específica. |
| `POST`| `/api/operacoes` | **Admin** | Cria uma nova operação. |
| `PUT` | `/api/operacoes/{id}` | **Admin** | Atualiza uma operação. |
| `DELETE`|`/api/operacoes/{id}` | **Admin** | Exclui permanentemente uma operação. |
| `POST`| `/api/operacoes/{id}/faturamentos` | Bearer Token | Adiciona um novo faturamento a uma operação. |
| `DELETE`|`/api/operacoes/{opId}/faturamentos/{fatId}`| Bearer Token | Exclui permanentemente um faturamento. |
| `POST`| `/api/admin/vincular-usuario-operacao` | **Admin** | Vincula um usuário a uma operação. |
| `POST`| `/api/solicitacoes` | Bearer Token | Cria uma nova solicitação de ajuste para um faturamento. |
| `GET` | `/api/solicitacoes` | **Admin** | Lista todas as solicitações de ajuste. |
| `PATCH`| `/api/solicitacoes/{id}/revisar` | **Admin** | Aprova ou rejeita uma solicitação de ajuste. |
| *... (e outros endpoints de CRUD para Empresas, Mensalistas, etc.)* | | | |

---

## 🔮 Roadmap Futuro (Frontend)

Com o backend finalizado, o foco agora é completar a interface do usuário.

- [🎯] **Completar o CRUD de Operações:** Adicionar a funcionalidade de **Edição**.
- [🎯] **Construir as Telas de Admin:** Criar as views para o CRUD de `Empresas` e `Mensalistas`.
- [🎯] **Dar Vida ao Dashboard Principal:** Popular a `DashboardView.vue` com cards de KPIs e gráficos que consumam os dados das stores.
- [🎯] **Polimento da UX:** Substituir os `alert()` por um sistema de notificações (toasts) mais elegante.


---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)