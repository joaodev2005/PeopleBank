# 🏦 PeopleBank

[![.NET CI](https://github.com/joaodev2005/PeopleBank/actions/workflows/ci.yml/badge.svg)](https://github.com/joaodev2005/PeopleBank/actions/workflows/ci.yml)

**Sistema integrado de RH e Conta Digital**  
Projeto pessoal desenvolvido para demonstrar uma arquitetura corporativa completa com **.NET 10**, **Clean Architecture**, **DDD** e processamento assíncrono.

> 💡 O PeopleBank une gestão de pessoas (empresas, funcionários, ponto eletrônico) com serviços financeiros (conta digital, Pix, folha de pagamento e benefícios flexíveis) em uma única plataforma.

---

## ✨ Funcionalidades

- **Onboarding de funcionários** com abertura automática de conta digital
- **Registro de ponto** com cálculo de horas extras
- **Processamento de folha de pagamento** assíncrono (Kafka + Worker Service)
- **Transferências Pix** com idempotência e bloqueio de saldo (Redis)
- **Benefícios flexíveis** (VR, VA) com carteiras virtuais e regras de estabelecimento
- **Extrato bancário** unificado de salário, benefícios e Pix

---

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, com 7 projetos bem definidos:

| Camada             | Responsabilidade                                                                 |
|--------------------|----------------------------------------------------------------------------------|
| **Domain**         | Entidades, Value Objects, Enums, Interfaces de repositório e eventos de domínio |
| **Application**    | Casos de uso, validações (FluentValidation) e orquestração                       |
| **Infrastructure** | Persistência (EF Core, FluentMigrator), Redis, Kafka e repositórios concretos  |
| **API**            | Controllers, middleware global de exceções e configuração                       |
| **Worker**         | Serviço em background que consome eventos do Kafka                              |
| **Communication**  | DTOs de Request e Response                                                       |
| **Exception**      | Exceções customizadas e mensagens de erro padronizadas (RFC 7807)               |

### 🔄 Fluxo de processamento (exemplo: folha de pagamento)

```plaintext
[API] --publica evento--> [Kafka] --consome--> [Worker]
                                                  │
                                                  ├── Calcula horas extras
                                                  ├── Credita salário na conta digital
                                                  ├── Credita benefícios (VR/VA)
                                                  └── Marca folha como Processada
```

---

## 🧰 Stack Tecnológica

- **.NET 10** (ASP.NET Core)
- **Entity Framework Core** (SQL Server)
- **FluentMigrator** (migrações)
- **Apache Kafka** (mensageria assíncrona)
- **Redis** (cache, idempotência e bloqueio de saldo)
- **Polly** (resiliência)
- **Mapster** (mapeamento de objetos)
- **FluentValidation** (validação de requests)
- **Docker** (infraestrutura local)
- **xUnit, Moq, FluentAssertions, Bogus, Testcontainers** (testes)

---

## 📦 Como rodar o projeto localmente

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### 1. Subir a infraestrutura (SQL Server, Redis, Kafka)

```bash
docker-compose up -d
```

### 2. Rodar a API

```bash
cd src/Backend/PeopleBank.Api
dotnet run
```

A API estará disponível em `https://localhost:7130` (Swagger em `/swagger`).

### 3. Rodar o Worker (em outro terminal)

```bash
cd src/Backend/PeopleBank.Worker
dotnet run
```

O Worker consumirá eventos dos tópicos `employee-created`, `payroll-requested` e `pix-requested`.

### 4. Criar o banco de dados (se necessário)

```bash
docker exec -it peoplebank-sqlserver-1 /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "DevPass123!" -C -Q "CREATE DATABASE PeopleBank"
```

As migrações são executadas automaticamente ao iniciar a API.

---

## 🧪 Executando os testes

```bash
dotnet test
```

### Testes de integração

Os testes de integração usam **Testcontainers** (SQL Server real em container).  
Certifique-se de que o Docker Desktop esteja rodando.

```bash
dotnet test tests/PeopleBank.WebApi.Tests
```

---

## 📚 Principais endpoints

| Método | Rota                           | Descrição                                |
|--------|--------------------------------|------------------------------------------|
| POST   | `/api/companies`               | Cadastrar empresa                        |
| POST   | `/api/employees`               | Cadastrar funcionário (abre conta)       |
| POST   | `/api/accounts`                | Abrir conta digital manualmente          |
| GET    | `/api/accounts/{id}/statement` | Consultar extrato                        |
| POST   | `/api/time-entries`            | Registrar ponto (ClockIn/ClockOut)       |
| POST   | `/api/payroll/process`         | Processar folha de pagamento             |
| POST   | `/api/pix`                     | Solicitar transferência Pix              |
| POST   | `/api/benefits/definitions`    | Cadastrar benefício (VR, VA)             |
| POST   | `/api/benefits/spend`          | Gastar saldo de benefício                |

> 🔍 Acesse o Swagger para ver todos os endpoints e schemas: `https://localhost:7130/swagger`

---

## 📁 Estrutura de Pastas

```plaintext
PeopleBank/
├── src/
│   ├── Backend/
│   │   ├── PeopleBank.Domain/
│   │   ├── PeopleBank.Application/
│   │   ├── PeopleBank.Infrastructure/
│   │   ├── PeopleBank.Api/
│   │   └── PeopleBank.Worker/
│   └── Shared/
│       ├── PeopleBank.Communication/
│       └── PeopleBank.Exception/
├── tests/
│   ├── PeopleBank.Domain.Tests/
│   ├── PeopleBank.UseCase.Tests/
│   ├── PeopleBank.Validator.Tests/
│   ├── PeopleBank.WebApi.Tests/
│   └── PeopleBank.CommonTestUtilities/
└── docker-compose.yml
```

---

## 🧠 Decisões de design

- **DDD tático** – Agregados, Value Objects e Eventos de Domínio
- **CQRS simples** – Casos de uso separados por operação
- **Mensageria** – Kafka para desacoplamento entre API e Worker
- **Idempotência** – Redis + chave única para evitar duplicidade
- **Resiliência** – Polly para retry e circuit breaker no Worker
- **Testabilidade** – Suíte completa com testes unitários, de validação e integração

---

## 📜 Licença

Este projeto é apenas para fins educacionais e de portfólio.
