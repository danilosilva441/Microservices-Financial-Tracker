# Microservices Financial Tracker

![Status](https://img.shields.io/badge/status-MVP%20Funcional-brightgreen)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Frontend](https://img.shields.io/badge/Frontend-Vue.js-green)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)

## 📄 Sobre o Projeto
O **Microservices Financial Tracker** é um sistema full-stack para controle e visualização de performance de operações financeiras. O foco principal é fornecer um dashboard centralizado com gráficos e relatórios que comparam metas com faturamentos, alimentado por um backend robusto construído em uma arquitetura de microserviços.

O projeto demonstra a criação de uma aplicação completa, desde a infraestrutura com Docker e um API Gateway, passando por um backend seguro com regras de negócio, até uma interface reativa no frontend.

---

## 🏛️ Arquitetura
O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único e seguro.

- **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado.
- **AuthService (ASP.NET Core):** Gerencia Usuários, Perfis (Roles) e todo o fluxo de autenticação com JWT.
- **BillingService (ASP.NET Core):** O coração do sistema. Gerencia Operações e Faturamentos, aplicando todas as regras de negócio.
- **AnalysisService (Node.js):** Motor de cálculo que roda em background, se autentica de forma segura e consome dados do `BillingService` para gerar projeções de faturamento.
- **Banco de Dados (PostgreSQL):** Cada serviço possui seu próprio banco de dados (`auth_db`, `billing_db`), garantindo a autonomia.

---

## ✨ Funcionalidades Implementadas

* **[✅] Autenticação Segura com Perfis (User/Admin):** Sistema de login completo com tokens JWT, protegendo o acesso aos dados e controlando a visibilidade de funcionalidades no frontend.
* **[✅] Gerenciamento de Operações:**
    * **API:** CRUD completo para criar, ler, atualizar e deletar operações.
    * **Frontend:** Interface para listar, visualizar detalhes, **criar** e **deletar** operações (ações restritas a Admins).
* **[✅] Rastreamento de Faturamentos:**
    * **API:** CRUD completo para adicionar, ler, atualizar e deletar faturamentos diários.
    * **Frontend:** Interface para listar faturamentos nos detalhes da operação, **adicionar** e **deletar** registros.
* **[✅] Dashboard de Performance:**
    * **Frontend:** Tela de dashboard com cards de KPIs e gráficos (`Chart.js`) para visualização de dados em tempo real.
* **[✅] Motor de Análise Proativo:**
    * O `AnalysisService` roda em background, se autentica de forma segura e calcula as projeções de faturamento que alimentam o dashboard.

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
    Verifique o arquivo `docker-compose.yml` e ajuste, se necessário, as variáveis `SYSTEM_EMAIL` e `SYSTEM_PASSWORD` para o `analysis_service`.

3.  **Inicie os Contêineres:**
    ```bash
    docker-compose up --build -d
    ```
4.  **Acesse a Aplicação:**
    * A aplicação completa (Frontend + Backend) estará disponível em **`http://localhost:8080`**.
    * O frontend é servido pelo Nginx na rota principal, e as chamadas de API (`/api/*`) são automaticamente redirecionadas para os serviços de backend corretos.

---

## 📡 Documentação da API (Endpoints Principais)
A API do backend possui mais funcionalidades do que as atualmente expostas no frontend.

| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/users` | Nenhuma | Registra um novo usuário. |
| `POST` | `/api/token` | Nenhuma | Autentica um usuário e retorna um token JWT. |
| `GET` | `/api/operacoes` | Bearer Token | Lista operações (filtradas por vínculo do usuário). |
| `POST`| `/api/operacoes` | **Admin** | Cria uma nova operação. |
| `DELETE`|`/api/operacoes/{id}` | **Admin** | Exclui uma operação. |
| `POST`| `/api/operacoes/{id}/faturamentos` | Bearer Token | Adiciona um novo faturamento a uma operação. |
| `DELETE`|`/api/operacoes/{opId}/faturamentos/{fatId}`| **Admin** | Exclui um faturamento. |
| `GET` | `/api/analysis/dashboard-data` | Nenhuma | **(Uso Interno)** Endpoint para o `AnalysisService` buscar dados. |

---

## 🔮 Roadmap Futuro
Com o MVP funcional, os próximos passos se concentram em expandir as funcionalidades e preparar para produção.

- **[🎯] Implementar Edição:** Adicionar a funcionalidade de editar Operações e Faturamentos no frontend.
- **[🎯] Construir Telas de Admin:** Criar as interfaces para gerenciar `Empresas` e `Mensalistas`.
- **[🎯] Polimento da UX:** Substituir `alert()` por um sistema de notificações (toasts) e refinar a responsividade.
- **[🚀] Testes Automatizados:** Adicionar testes unitários (Vitest) e E2E (Cypress/Playwright).
- **[🚀] Deploy:** Publicar a aplicação completa em uma plataforma de nuvem como Railway, Heroku ou Azure.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)