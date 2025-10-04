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
- **AnalysisService (Node.js):** Motor de cálculo que roda em background, se autentica de forma segura no `AuthService` e consome dados do `BillingService` para gerar projeções de faturamento.
- **Banco de Dados (PostgreSQL):** Cada serviço possui seu próprio banco de dados (`auth_db`, `billing_db`), garantindo a autonomia.

---

## ✨ Funcionalidades Principais

* **[✅] Autenticação Segura com Perfis (User/Admin):** Sistema de login completo com tokens JWT, protegendo o acesso aos dados.
* **[✅] Gerenciamento de Operações:**
    * **API:** CRUD completo para criar, ler, atualizar e deletar operações.
    * **Frontend:** Interface para listar, visualizar detalhes, **criar** e **deletar** operações (ações de criação/deleção restritas a Admins).
* **[✅] Rastreamento de Faturamentos:**
    * **API:** CRUD completo para adicionar, ler, atualizar e deletar faturamentos diários.
    * **Frontend:** Interface para listar faturamentos nos detalhes da operação, **adicionar** e **deletar** registros.
* **[🚧 EM ANDAMENTO] Dashboard de Performance:**
    * **Frontend:** Tela de dashboard com cards de KPIs (Key Performance Indicators) e gráficos (usando `Chart.js`) para visualização de dados.
    * **Dependência:** Aguardando a finalização do `AnalysisService` para popular os dados de projeção.
* **[🚧 EM ANDAMENTO] Motor de Análise Proativo:**
    * O `AnalysisService` é responsável por calcular as projeções de faturamento que alimentam o dashboard. **Esta é a funcionalidade atualmente em desenvolvimento.**

---

## 🛠️ Como Executar o Projeto

Siga os passos abaixo para executar a aplicação completa em seu ambiente local.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Passos
1.  **Clone o repositório e navegue até a pasta.**
2.  **Configure as Variáveis de Ambiente:** Verifique o arquivo `docker-compose.yml`, principalmente as credenciais do `SYSTEM_EMAIL` e `SYSTEM_PASSWORD` para o `analysis_service`.
3.  **Inicie os Contêineres:**
    ```bash
    docker-compose up --build -d
    ```
4.  **Acesse a Aplicação:**
    * A API estará disponível em `http://localhost:8080`.
    * O frontend de desenvolvimento (se estiver rodando localmente) estará em `http://localhost:5173`.

---

## 📡 Documentação da API (Endpoints Principais)
A API possui mais endpoints para funcionalidades avançadas (como `Empresas` e `Solicitações`), mas estes são os essenciais para o escopo atual do frontend.

| Método | Endpoint | Autorização | Descrição |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/users` | Nenhuma | Registra um novo usuário. |
| `POST` | `/api/token` | Nenhuma | Autentica um usuário e retorna um token JWT. |
| `GET` | `/api/operacoes` | Bearer Token | Lista operações do usuário (ou todas se for serviço interno). |
| `POST`| `/api/operacoes` | **Admin** | Cria uma nova operação. |
| `DELETE`|`/api/operacoes/{id}` | **Admin** | Exclui uma operação. |
| `POST`| `/api/operacoes/{id}/faturamentos` | Bearer Token | Adiciona um novo faturamento a uma operação. |
| `DELETE`|`/api/operacoes/{opId}/faturamentos/{fatId}`| **Admin** | Exclui um faturamento. |

---

## 🔮 Roadmap Futuro

- **[🎯] Finalizar e Estabilizar o `AnalysisService`:** Corrigir a inicialização do serviço para garantir que ele se autentique e busque os dados do `BillingService`, populando o campo `projecaoFaturamento`.
- **[ ] Polimento do Dashboard:** Adicionar mais gráficos e filtros de período.
- **[ ] Implementar Edição:** Adicionar a funcionalidade de editar Operações e Faturamentos no frontend.
- **[ ] Adicionar Módulos Avançados:** Construir as interfaces de frontend para as funcionalidades de `Empresas`, `Mensalistas` e `Solicitações de Ajuste` que já existem no backend.



---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)