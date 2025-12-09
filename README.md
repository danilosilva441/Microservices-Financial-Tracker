# DSS Systech Platform

**Plataforma de Gestão Operacional & Financeira Multi-Tenant (SaaS)**  

**Status:** 🟢 **Estável v2.1 (Enterprise Grade)**  
**Foco Atual:** **Fase 4: Qualidade & Testes Unitários**

---

## 📖 Visão Geral

O **DSS Systech** é uma plataforma **SaaS (Software as a Service) B2B** desenvolvida para gestão financeira e operacional de múltiplas unidades de negócio.

Diferente de um CRUD simples, este projeto implementa uma **arquitetura de microsserviços multi-tenant robusta**, focada em **Isolamento de Dados (Security-by-Default)**, hierarquia de permissões complexa e inteligência de dados em tempo real.

O sistema resolve o problema da gestão descentralizada (planilhas e papel), oferecendo um fluxo digital onde **líderes operacionais** submetem fechamentos, **supervisores** auditam, e **gerentes** visualizam a lucratividade real.

---

## 🧱 Arquitetura & Stack Tecnológica

A solução é orquestrada via **Docker Compose**, composta por **3 microsserviços principais** e um **Kernel compartilhado**.

### 🛠️ Backend & Infraestrutura

- **AuthService (.NET 8)**  
  Gestão de Identidade, Tokens JWT v2.0, Hierarquia e Provisionamento de Tenants.

- **BillingService (.NET 8)**  
  Core Business (Unidades, Despesas, Fluxo de Aprovação). Implementa **Global Query Filters** para segurança.

- **AnalysisService (Node.js)**  
  Motor de inteligência que agrega dados e calcula **lucratividade (Receita - Despesa)** em tempo real.

- **SharedKernel**  
  Biblioteca de domínios compartilhados e contratos de segurança (`ITenantEntity`).

- **Banco de Dados:** PostgreSQL (Schemas isolados por serviço).
- **API Gateway:** Nginx (Reverse Proxy para roteamento seguro `/api/*`).

### 🧪 Qualidade & Testes

- **xUnit:** Framework de testes.
- **Moq:** Simulação de dependências e repositórios.
- **FluentAssertions:** Asserções legíveis e expressivas.
- **SQLite (In-Memory):** Para testes de integração de banco de dados e transações.

---

## 🚀 Roadmap de Desenvolvimento

### ✅ **Fase 1: A Grande Refatoração (v1.0 → v2.0)**
- [x] Migração de Monólito para Microsserviços.
- [x] Implementação do padrão Repository Pattern e Injeção de Dependência.
- [x] Containerização total (Docker) com Healthchecks.
- [x] Configuração do Nginx Gateway.

### ✅ **Fase 2: Funcionalidades de Negócio (v2.0)**
- [x] Módulo de **Despesas**: CRUD e importação em lote via Excel (MiniExcel).
- [x] **Fluxo de Aprovação**: Workflow de estado (`Pendente → Aprovado/Rejeitado`) para fechamentos de caixa.
- [x] **Dashboard de Lucro**: Cálculo automático de lucratividade consumindo dados de múltiplos serviços.
- [x] **Hierarquia**: Gerentes podem criar a sua própria equipa (Supervisores, Líderes) via API.

### ✅ **Fase 3: Segurança & Isolamento (v2.1)**
- [x] **Isolamento de Tenant**: Implementação de **Global Query Filters** no EF Core. O sistema aplica `WHERE TenantId = X` automaticamente em todas as consultas, impedindo vazamento de dados.
- [x] **Autenticação Robusta**: Proteção global com `[Authorize]` e validação de Claims no JWT.
- [x] **Admin Global**: Lógica "Admin-Aware" que permite ao sistema (Analysis) ver dados globais, enquanto restringe usuários comuns.

### 🔄 **Fase 4: Qualidade & Blindagem (EM ANDAMENTO)**
- [x] **BillingService Tests**: Validação de cálculos financeiros e testes de segurança de isolamento de dados.
- [x] **AuthService Tests**: Cobertura de 100% das regras de hierarquia (ex: "Gerente não pode criar outro Gerente") e validações de cadastro (**60 testes passando**).
- [ ] **AnalysisService Tests**: Testes unitários em Jest para a lógica matemática.

### 🔜 **Fases Futuras**
- **Fase 5:** Frontend (Vue.js + Pinia + TailwindCSS).
- **Fase 6:** Funcionalidades Enterprise (Audit Logs, Observabilidade).
- **Fase 7:** Inovação (OCR com Tesseract.js para leitura de comprovantes).

---

## 📡 Principais Endpoints (API Reference)

### 🔐 **AuthService**

| Método | Endpoint | Acesso | Descrição |
|--------|----------|---------|-----------|
| POST | `/api/tenant/provision` | Público | Cria uma nova Empresa e o seu Gerente. |
| POST | `/api/token` | Público | Login (Retorna JWT com `tenantId`). |
| POST | `/api/users/tenant-user` | Gerente | Cria funcionários (Supervisor, Líder) para a empresa. |

### 💰 **BillingService**

| Método | Endpoint | Acesso | Descrição |
|--------|----------|---------|-----------|
| GET | `/api/unidades` | Autenticado | Lista as unidades do Tenant (Segurança Automática). |
| POST | `/api/unidades/{id}/fechamentos` | Líder+ | Submete um fechamento de caixa diário. |
| PUT | `/api/unidades/.../fechamentos/{id}` | Supervisor+ | Aprova/Rejeita um fechamento. |
| POST | `/api/expenses/upload` | Gerente | Upload de planilha de despesas (`.xlsx`). |

### 📊 **AnalysisService**

| Método | Endpoint | Acesso | Descrição |
|--------|----------|---------|-----------|
| GET | `/api/analysis/dashboard-data` | Gerente | Retorna KPIs de Lucro, Receita e Despesa em tempo real. |

---

## 🐳 Como Executar

**Pré-requisitos:** Docker Desktop e Git.

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/danilosilva441/Microservices-Financial-Tracker.git
   ```

2. **Suba o ambiente:**
   ```bash
   docker-compose up --build -d
   ```

3. **Execute os Testes (Opcional):**
   ```bash
   dotnet test backend/AuthService.Tests
   dotnet test backend/BillingService.Tests
   ```

---

## 👨‍💻 Autor

**Danilo Silva**  
Desenvolvedor Full Stack | DevOps & DataOps Enthusiast  
📧 danilosilva441@gmail.com  
🌐 github.com/danilosilva441

---

*© 2024 DSS Systech Platform. Todos os direitos reservados.*