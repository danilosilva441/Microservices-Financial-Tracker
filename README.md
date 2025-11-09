Status “em produção – v2.0 estável”

Destaque da Grande Refatoração concluída

Roadmap real da Fase 2 (v2.1)

Ênfase na engenharia assistida por IA

Tecnologias atualizadas (.NET 8, Vue 3, Node.js, TailwindCSS, PostgreSQL, Docker, Nginx)



---

🧾 README.md — Microservices Financial Tracker (v2.0)

# 💰 Microservices Financial Tracker — Plataforma Financeira Multi-Tenant

**Status:** 🟢 Em Produção (v2.0 Estável)  
**Backend:** .NET 8 (C#), Node.js  
**Frontend:** Vue.js 3 + TailwindCSS + Chart.js  
**Infraestrutura:** Docker Compose + Nginx + PostgreSQL  
**Arquitetura:** Microsserviços + Multi-Tenancy + JWT Security  

---

## 📖 Visão Geral

O **Microservices Financial Tracker** é uma plataforma de **gestão e análise financeira multi-tenant**, desenvolvida para centralizar o controle de operações, faturamentos e despesas de múltiplas unidades e empresas.

O sistema foi projetado com foco em **segurança, escalabilidade e automação inteligente**, unindo backend modular em microserviços, frontend reativo em Vue.js e um motor de análise em Node.js.  
A infraestrutura é totalmente conteinerizada e orquestrada com Docker + Nginx, pronta para ambientes produtivos.

---

## 🧱 Arquitetura do Sistema

┌──────────────────────────────┐ │          Frontend            │ │  Vue.js + Tailwind + Chart.js│ └──────────────┬───────────────┘ │ API Gateway (Nginx) │ ┌──────────────┼──────────────┐ │ AuthService  │ BillingService│ │ (.NET 8)     │ (.NET 8)      │ └──────────────┼──────────────┘ │ AnalysisService (Node.js - Inteligência) │ PostgreSQL (x3)

Cada microserviço possui seu próprio banco (`auth_db`, `billing_db`, `analysis_db`), com isolamento total de dados.  
A arquitetura **multi-tenant** garante que cada empresa (Tenant) possua dados segregados e segurança contextual via `TenantId` nos tokens JWT.

---

## ⚙️ Tecnologias-Chave

**Backend**
- .NET 8 + C#
- Entity Framework Core
- Repository Pattern
- JWT Authentication com Tenant Claims
- PostgreSQL
- MiniExcel (Upload de Planilhas)
- Node.js (AnalysisService)

**Frontend**
- Vue.js 3 (Composition API + Pinia)
- TailwindCSS
- Chart.js
- Axios
- Modo Mobile-First

**Infraestrutura**
- Docker Compose
- Nginx (API Gateway + Reverse Proxy)
- Multi-Database (auth_db, billing_db)
- Volume Persistence + Healthchecks

---

## ✅ Fase 1 — A Grande Refatoração (v1.0 → v2.0)

**Status:** ✅ Concluída com Sucesso  

A Fase 1 foi o marco principal do projeto, consolidando a migração de um protótipo v1.0 para uma **plataforma robusta multi-tenant**.  
Principais entregas:

### 🧩 Banco de Dados
- Migração completa para **multi-tenancy** via `TenantId`.
- Criação da migração única `V2_Schema_Inicial` nos bancos `auth_db` e `billing_db`.
- Sincronia total entre código e schema.

### 🛠️ Infraestrutura
- Reescrita completa do `docker-compose.yml` e dos `Dockerfiles`.
- Build estável e contêineres **Healthy** para todos os serviços.
- API Gateway (Nginx) roteando corretamente `/api/*`.

### 🔐 AuthService (.NET 8)
- Implementação de **Repository Pattern**.
- Endpoint `/api/tenant/provision` para criação de novos Tenants (empresas).
- JWT com claims de `tenantId` e controle granular de roles.

### 💼 BillingService (.NET 8)
- Refatoração completa da lógica de negócio.
- Novo módulo **Despesas (Expenses)**:
  - `POST /expenses/categories`
  - `POST /expenses`
  - `POST /expenses/upload` (upload via planilha Excel)
- Fluxo de **Fechamento de Caixa**:
  - Líder submete fechamento (`POST /unidades/.../fechamentos`)
  - Supervisor aprova (`PUT /unidades/.../fechamentos/{id}`)

### 📊 AnalysisService (Node.js)
- Substituição de cron job por API sob demanda.
- Autenticação segura com token `Admin` (TenantId NULL).
- Integração com BillingService para cálculos de projeção de faturamento.

### 🧪 Validação
- Testes Postman ponta a ponta cobrindo todo o fluxo:
  - Provisionamento de Tenant → Login → Criação de Unidade → Registro de Faturamento → Fechamento e Aprovação.

---

## 🚀 Fase 2 — Funcionalidades Finais (v2.1)

**Status:** 🔄 Em Desenvolvimento  

Agora que a base estável foi alcançada, o foco é entregar as **funcionalidades finais do produto**.

### 🔧 Backend
- [ ] Depurar `GET /api/analysis/dashboard-data`  
  - Corrigir credenciais do `system@...` (Admin, TenantId NULL)  
  - Validar cálculo de lucro `(Receita - Despesa)`  

### 🖥️ Frontend (Vue.js + TailwindCSS)
- [ ] **Auth v2.0** — Atualizar `auth.store.ts` (Pinia) para armazenar `tenantId` globalmente.  
- [ ] **Dashboard de Lucro** — Tela principal do gerente com dados do AnalysisService.  
- [ ] **Módulo de Despesas** — Listagem e upload de planilhas (`/expenses` + `/expenses/upload`).  
- [ ] **Fluxo Mobile (Líder)** — Formulário para submissão de fechamentos diários.  
- [ ] **Painel do Supervisor** — Tela desktop para aprovar fechamentos pendentes.  
- [ ] **OCR (Prova de Conceito)** — Testes com `Tesseract.js` para leitura de comprovantes físicos.

---

## 🧠 AI-Driven Development

Este projeto foi desenvolvido integralmente com apoio de **ferramentas de Inteligência Artificial**, adotando práticas de **AI-Augmented Engineering**:

| Ferramenta | Utilização |
|-------------|-------------|
| **ChatGPT (OpenAI)** | Arquitetura, design de APIs e otimizações de código. |
| **Gemini (Google)** | Organização de roadmap e etapas de desenvolvimento. |
| **DeepSeek** | Refino de performance e análise de bugs. |

Essa metodologia garantiu um ciclo de desenvolvimento **rápido, iterativo e com alta coerência técnica**, resultando em uma base estável e escalável.

---

## 🐳 Como Executar Localmente

### **Pré-requisitos**
- Docker Desktop  
- Git  

### **Passos**
```bash
# Clone o repositório
git clone https://github.com/danilosilva441/Microservices-Financial-Tracker.git
cd Microservices-Financial-Tracker

# Ajuste variáveis de ambiente no docker-compose.yml
# SYSTEM_EMAIL, SYSTEM_PASSWORD

# Suba os contêineres
docker-compose up --build -d

Acesse:
👉 http://localhost:8080


---

📡 Endpoints Principais

Método	Endpoint	Serviço	Descrição

POST	/api/tenant/provision	AuthService	Cria um novo Tenant (Empresa)
POST	/api/token	AuthService	Gera token JWT
GET	/api/operacoes	BillingService	Lista operações por Tenant
POST	/api/expenses	BillingService	Cadastra despesa
GET	/api/analysis/dashboard-data	AnalysisService	Retorna dados de lucro (receita - despesa)



---

👨‍💻 Autor

Danilo Silva
Desenvolvedor Full Stack | DevOps & DataOps Enthusiast
📧 danilosilva441@gmail.com
🌐 github.com/danilosilva441


---

> “O código é meu, mas a jornada foi construída com IA.” 🧠
— AI-Augmented Development em ação. 