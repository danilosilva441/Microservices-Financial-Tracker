# Minimundo - Gestão de Faturamento & Previsões (Full-Stack)

![Status](https://img.shields.io/badge/status-Backend%20Concluído-brightgreen)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL-darkblue)

Este é um projeto full-stack para gestão de operações financeiras, com um foco em faturamento diário, gerenciamento de mensalistas e projeções de performance. A arquitetura é baseada em microserviços híbridos no backend e uma Single Page Application (SPA) reativa no frontend.

O objetivo é demonstrar competências em todo o ciclo de vida do desenvolvimento de software, desde o design de uma arquitetura robusta e a implementação de regras de negócio complexas, até a orquestração da infraestrutura com Docker e a criação de uma API segura e profissional.

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único, seguro e gerenciável para o frontend.

* **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado.
* **AuthService (ASP.NET Core):** Gerencia Usuários, Perfis (Roles) e todo o fluxo de autenticação, gerando tokens JWT seguros.
* **BillingService (ASP.NET Core):** O coração do sistema. Gerencia Operações, Faturamentos (avulsos e de mensalistas), Metas, Empresas e Mensalistas, aplicando todas as regras de negócio.
* **AnalysisService (Node.js):** Motor de cálculo proativo que analisa o histórico e gera projeções de performance através de jobs agendados.
* **Banco de Dados (PostgreSQL):** Armazena os dados de cada serviço em bancos de dados separados, garantindo a autonomia dos microserviços.

---

## ✨ Funcionalidades do Backend

* **Autenticação & Autorização:**
    * [✅] Registro e Login com senhas criptografadas (`BCrypt`).
    * [✅] Geração de Tokens JWT contendo `claims` de perfil (`Role`).
    * [✅] Sistema de Perfis (`Roles`) relacional (User/Admin).
    * [✅] Endpoint administrativo para promover usuários a Admin.
    * [✅] Autorização granular: um usuário só pode acessar os dados das operações às quais está vinculado.
    * [✅] Proteção de endpoints baseada em Perfis (ex: `[Authorize(Roles = "Admin")]`).

* **Lógica de Negócio (Billing):**
    * [✅] CRUD completo e seguro para **Operações**, **Empresas**, **Mensalistas** e **Faturamentos**.
    * [✅] Vínculo automático do usuário criador à sua nova operação.
    * [✅] Endpoint de Admin para gerenciar vínculos Usuário-Operação.
    * [✅] Regras de data para registro de faturamentos (impede datas futuras e força a janela de lançamento D+1).
    * [✅] Modelo de faturamento duplo: **Fixo** (mensalistas) e **Variável** (avulsos).

* **Análise e Projeção:**
    * [✅] Endpoint reativo para projeções sob demanda.
    * [✅] Job agendado (`node-cron`) que calcula e salva projeções de forma proativa e segura.

* **Design de API e Arquitetura:**
    * [✅] Arquitetura em camadas (`Controllers`, `Services`, `Repositories`) para separação de responsabilidades.
    * [✅] Arquitetura Multi-Banco: um banco de dados dedicado para cada serviço (`auth_db`, `billing_db`).
    * [✅] Orquestração de startup com `healthchecks` no Docker Compose para garantir uma inicialização estável.
    * [✅] Uso de DTOs para garantir um contrato de API limpo e seguro.

---

## 🛠️ Como Executar o Backend

Siga os passos abaixo para executar a aplicação completa.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos para Instalação

1.  **Clone o repositório e navegue até a pasta.**
2.  **Verifique as configurações** no arquivo `docker-compose.yml` (senhas do banco, chave JWT, etc.).
3.  **Suba os contêineres** com Docker Compose:
    ```bash
    docker-compose up --build -d
    ```
4.  Aguarde até que todos os contêineres estejam com o status `healthy` (verifique com `docker ps`).
5.  Use o **Postman** ou similar para interagir com a API através do Gateway na porta **`http://localhost:8080`**.

---

## 🔮 Roadmap de Evolução

* [🎯] **Desenvolver o Frontend Completo (Vue.js):** Construir as telas de CRUD, painéis de Admin e o Dashboard principal com gráficos.
* [🎯] **Adicionar Testes Unitários e de Integração** para garantir a qualidade contínua do backend.
* [🎯] **Configurar CI/CD (GitHub Actions):** Automatizar o processo de build, teste e deploy.
* [🎯] **Realizar o Deploy em Nuvem:** Publicar a aplicação completa em um serviço como Azure, AWS ou Railway.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)