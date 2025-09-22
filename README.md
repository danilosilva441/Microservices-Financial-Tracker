# Microservices Financial Tracker (Full-Stack)

![Status](https://img.shields.io/badge/status-backend%20em%20evolução-yellow)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL-darkblue)

Projeto de portfólio full-stack que implementa uma aplicação para gerenciamento de operações financeiras. A arquitetura é baseada em microserviços híbridos no backend e uma Single Page Application (SPA) reativa no frontend. O objetivo é demonstrar competências em todo o ciclo de vida do desenvolvimento de software, desde o design da arquitetura e a implementação de regras de negócio complexas até a orquestração da infraestrutura com Docker.

**⚠️ Atenção: O backend está atualmente em uma fase de evolução para implementar regras de negócio avançadas. O frontend está temporariamente desativado.**

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único, seguro e gerenciável.

* **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado.
* **AuthService (ASP.NET Core):** Gerencia Usuários e Perfis (Roles), processa registro/login e gera tokens JWT seguros.
* **BillingService (ASP.NET Core):** O coração do sistema. Gerencia Operações, Faturamentos e Metas, aplicando regras de negócio como controle de acesso por vínculo usuário-operação e validações de data.
* **AnalysisService (Node.js):** Motor de cálculo proativo que analisa o histórico e gera projeções de performance.
* **Banco de Dados (PostgreSQL):** Armazena os dados de forma relacional e persistente.

---

## ✨ Funcionalidades do Backend

* **Autenticação & Autorização:**
    * [✅] Registro e Login com senhas criptografadas (`BCrypt`).
    * [✅] Geração de Tokens JWT contendo `claims` de perfil (`Role`).
    * [✅] Estrutura de Perfis (`Roles`) relacional (User/Admin).
    * [🚧] **Em desenvolvimento:** Autorização granular baseada em vínculo Usuário-Operação.
    * [🚧] **Em desenvolvimento:** Proteção de endpoints baseada em Perfis (ex: `[Authorize(Roles = "Admin")]`).

* **Lógica de Negócio (Billing):**
    * [✅] CRUD completo para Operações e Faturamentos.
    * [✅] Modelo de dados realista (Operações com Nome, Descrição, Endereço, Moeda, etc.).
    * [✅] Filtros dinâmicos na API para consultas.
    * [🚧] **Em desenvolvimento:** Regras de data para registro de faturamentos (sem datas futuras, janela de lançamento D+1).

* **Análise e Projeção:**
    * [✅] Endpoint reativo para projeções sob demanda.
    * [✅] Job agendado (`node-cron`) que calcula e salva projeções de forma proativa.

* **Design de API e Arquitetura:**
    * [✅] Arquitetura em camadas (`Controllers`, `Services`, `Repositories`) para separação de responsabilidades.
    * [✅] Uso de DTOs para garantir um contrato de API limpo e seguro.

---

## 🛠️ Como Executar o Backend

Siga os passos abaixo para executar a aplicação completa (exceto frontend).

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos para Instalação

1.  **Clone o repositório e navegue até a pasta.**
2.  **Verifique a senha do banco de dados** no `docker-compose.yml` e nos arquivos `appsettings.json`.
3.  **Suba os contêineres** com Docker Compose:
    ```bash
    docker-compose up --build -d
    ```
4.  Use o **Postman** ou similar para interagir com a API através do Gateway na porta **`http://localhost:8080`**.

---

## 🔮 Roadmap de Evolução

-   [ ] **Finalizar Regras de Negócio do Backend:**
    -   [ ] Implementar Vínculo Usuário-Operação.
    -   [ ] Implementar regras de data para faturamentos.
    -   [ ] Aplicar `[Authorize(Roles = "...")]` nos endpoints.
-   [ ] **Adicionar Testes Unitários e de Integração** para o backend.
-   [ ] **Reativar e Finalizar o Frontend** com todas as novas funcionalidades.
-   [ ] **Deploy** da aplicação completa em um serviço de nuvem.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)