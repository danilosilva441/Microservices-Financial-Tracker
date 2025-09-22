# Microservices Financial Tracker (Full-Stack)

![Status](https://img.shields.io/badge/status-MVP%20Concluído-brightgreen)
![Backend](https://img.shields.io/badge/Backend-ASP.NET%20%7C%20Node.js-blueviolet)
![Frontend](https://img.shields.io/badge/Frontend-Vue.js-green)
![Infra](https://img.shields.io/badge/Infra-Docker%20%7C%20Nginx-blue)
![Database](https://img.shields.io/badge/Database-PostgreSQL-darkblue)

Este é um projeto de portfólio full-stack que implementa uma aplicação para gerenciamento de operações financeiras. A arquitetura é baseada em microserviços híbridos no backend e uma Single Page Application (SPA) reativa no frontend. O objetivo é demonstrar competências em todo o ciclo de vida do desenvolvimento de software, desde o design da arquitetura e a implementação do backend até a criação de uma interface de usuário interativa e a orquestração da infraestrutura com Docker.

---

## 🏛️ Visão Geral da Arquitetura

O sistema é composto por múltiplos serviços independentes que se comunicam através de um API Gateway, servindo dados para uma aplicação frontend moderna.

**Fluxo da Requisição:**
`Cliente (Vue.js App no Navegador)` ➔ `Nginx (API Gateway)` ➔ `Serviço de Backend Apropriado` ➔ `PostgreSQL`

* **Frontend (Vue.js SPA):** A interface do usuário com a qual o usuário interage. É responsável por gerenciar o estado da aplicação, lidar com a autenticação do usuário (JWT) e consumir os dados da API.
* **API Gateway (Nginx):** Roteia as requisições do frontend para o microserviço de backend apropriado.
* **AuthService (ASP.NET Core):** Gerencia Usuários, Perfis (Roles) e todo o fluxo de autenticação, gerando tokens JWT seguros.
* **BillingService (ASP.NET Core):** Gerencia toda a lógica de negócio principal: Operações (contratos), Metas Mensais e Faturamentos (ganhos).
* **AnalysisService (Node.js):** Motor de cálculo proativo que analisa o histórico de faturamentos e gera projeções de performance, rodando tarefas agendadas.
* **Banco de Dados (PostgreSQL):** Armazena os dados de forma relacional e persistente.

---

## 🖼️ Telas da Aplicação

| Tela de Login | Painel de Operações | Detalhes da Operação |
| :---: | :---: | :---: |
| ![Tela de Login](https://ibb.co/TxtZQw5B) | ![Painel de Operações](https://ibb.co/rfmpZMv2) | ![Detalhes da Operação](https://ibb.co/dJmqc1z2) |

---

## 🚀 Tecnologias Utilizadas

### **Frontend**
* **Vue.js 3:** Framework progressivo com a Composition API para uma interface reativa e organizada.
* **Vue Router:** Para o roteamento do lado do cliente e criação da Single Page Application.
* **Pinia:** Para o gerenciamento de estado centralizado, de forma simples e poderosa.
* **TailwindCSS:** Framework de CSS utility-first para um design rápido, moderno e customizável.
* **Axios:** Para fazer as requisições HTTP para o backend.

### **Backend**
* **C# / ASP.NET Core:** Para os serviços críticos (`Auth`, `Billing`) que exigem robustez e performance.
* **Node.js / Express:** Para o serviço de análise (`AnalysisService`), que se beneficia da agilidade do JavaScript.
* **Entity Framework Core:** ORM para o mapeamento objeto-relacional nos serviços .NET.

### **Infraestrutura & Banco de Dados**
* **Docker & Docker Compose:** Containeriza cada serviço, garantindo um ambiente de desenvolvimento e produção consistente e orquestrado.
* **Nginx:** Atua como API Gateway e Reverse Proxy.
* **PostgreSQL:** Banco de dados relacional.

---

## ✨ Funcionalidades

* **Fluxo de Autenticação Completo:** Interface de Login que se comunica com o backend, armazena o token JWT no Local Storage e o utiliza para requisições autenticadas.
* **Roteamento Protegido:** Uso de Guardas de Rota no Vue Router para impedir o acesso a páginas internas por usuários não autenticados.
* **CRUD de Operações:** O usuário pode visualizar, criar e (no futuro) editar suas operações financeiras diretamente pelo frontend.
* **Visualização de Dados Relacionados:** A página de detalhes exibe uma operação e todos os seus faturamentos associados.
* **Criação de Faturamentos:** Formulário para adicionar novos ganhos a uma operação, com a interface sendo atualizada em tempo real.
* **Design Responsivo (Base):** O uso de TailwindCSS permite que a aplicação seja facilmente adaptável a diferentes tamanhos de tela.
* *...e todas as funcionalidades do backend (Roles, filtros, projeção automática, etc.).*

---

## 🛠️ Como Executar o Projeto

Siga os passos abaixo para executar a aplicação completa localmente.

### Pré-requisitos
* [Git](https://git-scm.com/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* [Node.js (versão LTS)](https://nodejs.org/)

### Passos para Instalação

A aplicação é dividida em duas partes que precisam ser executadas em paralelo.

#### **1. Executando o Backend (Docker)**
1.  Na **raiz do projeto**, execute o comando para subir todos os contêineres do backend e do banco de dados:
    ```bash
    docker-compose up --build -d
    ```
2.  Isso subirá o API Gateway na porta `8080`.

#### **2. Executando o Frontend (Vite)**
1.  Abra um **novo terminal**.
2.  Navegue até a pasta do frontend:
    ```bash
    cd frontend
    ```
3.  Instale as dependências (apenas na primeira vez):
    ```bash
    npm install
    ```
4.  Inicie o servidor de desenvolvimento do frontend:
    ```bash
    npm run dev
    ```
5.  Abra seu navegador e acesse a URL que o terminal indicar (geralmente **`http://localhost:5173`**).

---

## 🔮 Próximos Passos

* [ ] Implementar a funcionalidade de **Edição** para Operações e Faturamentos.
* [ ] Construir as páginas de **Metas** e **Relatórios** no frontend.
* [ ] Adicionar **gráficos** ao Dashboard para uma visualização de dados mais rica.
* [ ] Implementar um pipeline de **CI/CD** com GitHub Actions.
* [ ] Fazer o **deploy** da aplicação completa em um serviço de nuvem (Railway, Render ou Azure).

---

## 👨‍💻 Autor

Feito por **Danilo Silva**.

[![LinkedIn](https://img.shields.io/badge/linkedin-%230077B5.svg?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/danilo-d-9b04a6140/)
[![GitHub](https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white)](https://github.com/danilosilva441)