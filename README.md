# Minimundo - Gestão de Faturamento & Previsões (Full-Stack)

![Status](https://img.shields.io/badge/status-backend%20em%20evolução-yellow)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL-darkblue)

Este é um projeto full-stack para gestão de operações financeiras, com um foco em faturamento diário, gerenciamento de mensalistas e projeções de performance. A arquitetura é baseada em microserviços híbridos no backend e uma Single Page Application (SPA) reativa no frontend.

**⚠️ Atenção: O backend está atualmente em uma fase de evolução para implementar regras de negócio avançadas (validação de datas e permissões).**

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, garantindo um ponto de entrada único e seguro.

* **API Gateway (Nginx):** Roteia as requisições para o microserviço apropriado.
* **AuthService (ASP.NET Core):** Gerencia Usuários, Perfis (Roles) e todo o fluxo de autenticação com JWT.
* **BillingService (ASP.NET Core):** O coração do sistema. Gerencia Operações, Faturamentos (avulsos e de mensalistas), Metas, Empresas e Mensalistas.
* **AnalysisService (Node.js):** Motor de cálculo proativo que analisa o histórico e gera projeções de performance.
* **Banco de Dados (PostgreSQL):** Armazena os dados de forma relacional e persistente.
* **Frontend (Vue.js):** Interface de usuário interativa para consumir e gerenciar os dados.

---

## ✨ Funcionalidades do Backend

* **Autenticação & Autorização:**
    * [✅] Registro e Login com senhas criptografadas (`BCrypt`).
    * [✅] Geração de Tokens JWT contendo `claims` de perfil (`Role`).
    * [✅] Estrutura de Perfis (`Roles`) relacional (User/Admin).
    * [✅] Endpoint para promover usuários a Admin.
    * [🚧] **Em desenvolvimento:** Autorização granular baseada em vínculo Usuário-Operação.
    * [🚧] **Em desenvolvimento:** Proteção de endpoints baseada em Perfis (ex: `[Authorize(Roles = "Admin")]`).

* **Lógica de Negócio (Billing):**
    * [✅] CRUD completo para **Operações**.
    * [✅] CRUD completo para **Empresas** (B2B).
    * [✅] CRUD completo para **Mensalistas**.
    * [✅] CRUD completo para **Faturamentos**.
    * [✅] Modelo de faturamento duplo: **Fixo** (mensalistas) e **Variável** (avulsos).
    * [🚧] **Em desenvolvimento:** Regras de data para registro de faturamentos (sem datas futuras, janela de lançamento D+1).

* **Análise e Projeção:**
    * [✅] Endpoint reativo para projeções sob demanda.
    * [✅] Job agendado (`node-cron`) que calcula e salva projeções de forma proativa.

* **Design de API e Arquitetura:**
    * [✅] Arquitetura em camadas (`Controllers`, `Services`, `Repositories`).
    * [✅] Uso de DTOs para garantir um contrato de API limpo e seguro.

---

## Endpoints da API

Todas as requisições devem ser feitas para a porta do API Gateway (ex: `http://localhost:8080`).

* **AuthService**
    * `POST /api/users`: Registra um novo usuário.
    * `POST /api/token`: Realiza o login.
    * `POST /api/admin/promote-to-admin`: (Admin) Promove um usuário a Admin.

* **BillingService**
    * `GET/POST/PUT /api/operacoes`: Gerenciamento completo de Operações.
    * `PATCH /api/operacoes/{id}/desativar`: Desativa uma operação.
    * `GET/POST/PUT /api/empresas`: Gerenciamento completo de Empresas.
    * `GET/POST/PUT /api/operacoes/{id}/mensalistas`: Gerenciamento de Mensalistas de uma operação.
    * `PATCH /api/operacoes/{opId}/mensalistas/{mensId}/desativar`: Desativa um mensalista.
    * `POST /api/operacoes/{id}/faturamentos`: Registra um novo faturamento.
    * `GET/POST /api/metas`: Gerenciamento de metas globais do usuário.

* **AnalysisService**
    * `POST /api/analysis/projetar`: Realiza uma projeção sob demanda.

---

## 🔮 Roadmap de Evolução

-   [ ] **Finalizar Regras de Negócio do Backend:**
    -   [ ] Implementar Vínculo Usuário-Operação em todas as consultas.
    -   [ ] Implementar regras de data para faturamentos.
    -   [ ] Aplicar `[Authorize(Roles = "Admin")]` nos endpoints necessários.
-   [ ] **Adicionar Testes Unitários** para o backend.
-   [ ] **Finalizar o Frontend** com todas as novas funcionalidades (CRUD de Empresas, Mensalistas, etc.).
-   [ ] **Deploy** da aplicação completa em um serviço de nuvem.

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)