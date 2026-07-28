# 🏦 PeopleBank - RH Integrado com Conta Digital

> **⚠️ Status do Projeto: Em Desenvolvimento Ativo (Fase 1 - Modelagem e Arquitetura)**  
> *A estrutura completa do projeto, camadas, injeção de dependência e modelagem de domínio (DDD) já estão definidas. A implementação dos casos de uso e integrações (Kafka/Redis) está em andamento. Este repositório serve como meu laboratório prático para arquitetura de sistemas financeiros distribuídos.*

---

## 📋 Visão Geral

O **PeopleBank** é um sistema de gestão de recursos humanos (RH) totalmente integrado a uma conta digital. O objetivo é automatizar desde a admissão do funcionário até o pagamento de salários e benefícios, utilizando conceitos de **Domain-Driven Design (DDD)**, **Clean Architecture** e processamento assíncrono.

O fluxo principal do sistema cobre três grandes jornadas:

1. **Onboarding do Funcionário**  
   - Empresa se cadastra → RH cadastra funcionário (com CPF e chave Pix) → Sistema abre conta digital automaticamente (saldo R$ 0,00).

2. **Folha de Pagamento (Payroll)**  
   - Funcionário bate ponto (entrada/saída) → Sistema acumula horas → RH dispara processamento mensal → **Worker assíncrono (Kafka)** calcula salários, descontos, extras e benefícios → Valor líquido é depositado via Pix interno na conta do funcionário.

3. **Benefícios Flexíveis**  
   - Empresa define categorias (VR, VA, Mobilidade) → Funcionário recebe saldo separado mensalmente → Utiliza saldo via Pix em estabelecimentos credenciados (com validação de categoria).

---

## 🏗️ Arquitetura do Projeto

O projeto foi estruturado seguindo os princípios da **Clean Architecture** combinados com **DDD** e **SOLID**. A comunicação entre camadas é feita exclusivamente via **Injeção de Dependência** (sem MediatR, utilizando interfaces de Use Case diretamente).

### As 7 Camadas do Sistema

| Camada | Responsabilidade |
| :--- | :--- |
| **Domain** | Entidades, Value Objects (CPF, Email, PixKey), Enums, Interfaces de Repositórios/Serviços e Eventos de Domínio. |
| **Application** | Casos de Uso (Use Cases), Validadores (FluentValidation) e Orquestração de fluxos. |
| **Infrastructure** | Persistência (EF Core + SQL Server), Mensageria (Kafka), Cache (Redis), Repositórios concretos e Migrations. |
| **API** | Controllers, Middleware Global de Exceções e configuração de DI. |
| **Worker** | Background Service dedicado ao processamento assíncrono da folha de pagamento. |
| **Communication** | DTOs isolados (Requests/Responses) para contratos limpos entre API e cliente. |
| **Exception** | Exceções customizadas (DomainException, NotFoundException, etc.) e padronização de erros. |

---

## ⚙️ Stack Tecnológica (Utilizada e Planejada)

| Categoria | Tecnologia |
| :--- | :--- |
| **Linguagem & Framework** | .NET 10, ASP.NET Core, C# |
| **Persistência** | SQL Server + Entity Framework Core |
| **Migrações** | FluentMigrator |
| **Cache Distribuído** | Redis |
| **Mensageria** | Apache Kafka (processamento assíncrono da folha) |
| **Resiliência** | Polly (Retry/Circuit Breaker no Worker) |
| **Validação** | FluentValidation |
| **Testes** | xUnit, Moq, Bogus, FluentAssertions, Testcontainers |
| **Infraestrutura** | Docker, Docker Compose |
| **Documentação** | Swagger/OpenAPI (ambiente Dev) |
