# 🚖 VemQueCabe - Sistema de Carona Compartilhada

Um sistema completo de carona compartilhada desenvolvido em .NET 8, seguindo princípios de Clean Architecture e Domain-Driven Design (DDD).

## Sobre o Projeto

O **VemQueCabe** é uma API REST para gerenciamento de caronas compartilhadas, conectando motoristas e passageiros de forma eficiente e segura. O sistema permite que usuários se cadastrem como motoristas ou passageiros, criem solicitações de carona, aceitem corridas e gerenciem todo o ciclo de vida de uma viagem.

A ideia do projeto surgiu por através da leitura do [artigo](https://www.geeksforgeeks.org/system-design/domain-driven-design-ddd/) da GeekForGeeks sobre Domain-Driven Design, que inspirou a criação de uma aplicação prática para consolidar os conceitos aprendidos.

## Funcionalidades

### Gestão de Usuários
- Cadastro e autenticação de usuários
- Perfis diferenciados (Motorista/Passageiro)
- Sistema de autenticação JWT
- Criptografia de senhas com BCrypt
- Autorização baseada em roles e políticas

### Gestão de Motoristas
- Cadastro de motoristas com informações do veículo
- Gerenciamento de disponibilidade
- Histórico de corridas realizadas
- Políticas de autorização específicas para motoristas

### Gestão de Passageiros
- Cadastro de passageiros
- Solicitação de caronas
- Histórico de viagens
- Políticas de autorização específicas para passageiros

### Gestão de Corridas
- Criação de solicitações de carona
- Matching entre motoristas e passageiros
- Controle de status das corridas (Pendente, Em Andamento, Concluída, Cancelada)
- Sistema de cache com Redis para performance
- Middleware personalizado para tratamento de erros

## Arquitetura

O projeto segue os princípios da **Clean Architecture** e **Domain-Driven Design**, organizando o código em camadas bem definidas:

### Estrutura do Projeto

```
VemQueCabe/
├── docker-compose.yml          # Configuração dos serviços Docker
├── Dockerfile                  # Build multi-stage da aplicação
├── VemQueCabe.Api/             # Camada de Apresentação
│   ├── Controllers/            # Controllers da API REST
│   │   ├── AuthController.cs
│   │   ├── DriverController.cs
│   │   ├── PassengerController.cs
│   │   ├── RideController.cs
│   │   ├── RideRequestController.cs
│   │   └── UserController.cs
│   ├── Extensions/             # Extensões de configuração
│   │   ├── AuthenticationCollectionExtensions.cs
│   │   ├── AuthorizationCollectionExtensions.cs
│   │   ├── MapperCollectionExtensions.cs
│   │   ├── RedisCollectionExtensions.cs
│   │   ├── ServiceCollectionExtensions.cs
│   │   ├── SwaggerCollectionExtension.cs
│   │   ├── UnitOfWorkCollectionExtensions.cs
│   │   └── Policies/           # Políticas de autorização
│   ├── Middlewares/            # Middlewares personalizados
│   │   └── ErrorHandleMiddleware.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   └── wwwroot/                # Arquivos estáticos
├── VemQueCabe.Application/     # Camada de Aplicação
│   ├── Dtos/                   # Data Transfer Objects
│   ├── Extensions/             # Extensões e utilitários
│   │   ├── CacheKeys.cs
│   │   └── ClaimsPrincipalExtensions.cs
│   ├── Interfaces/             # Contratos de serviços
│   ├── Profiles/               # Mapeamentos AutoMapper
│   ├── Requests/               # Objetos de requisição
│   ├── Responses/              # Objetos de resposta
│   └── Services/               # Implementação dos serviços
├── VemQueCabe.Domain/          # Camada de Domínio
│   ├── Entities/               # Entidades do domínio
│   │   ├── Driver.cs
│   │   ├── Passenger.cs
│   │   ├── Ride.cs
│   │   ├── RideRequest.cs
│   │   └── User.cs
│   ├── Enums/                  # Enumerações
│   ├── Interfaces/             # Contratos de domínio
│   ├── Shared/                 # Elementos compartilhados
│   └── ValueObjects/           # Objetos de valor
├── VemQueCabe.Infra/           # Camada de Infraestrutura
│   ├── Data/                   # Contexto do Entity Framework
│   └── Migrations/             # Migrações do banco de dados
└── VemQueCabe.Tests/           # Projeto de Testes
    ├── Application/            # Testes da camada de aplicação
    │   ├── Fixtures/           # Fixtures para testes
    │   └── Services/           # Testes de serviços
    ├── Domain/                 # Testes da camada de domínio
    │   ├── Entities/           # Testes de entidades
    │   └── ValueObjects/       # Testes de value objects
    └── StrykerOutput/          # Relatórios de mutation testing
```

### Camadas da Arquitetura

#### **API Layer (Presentation)**
- Controllers REST para exposição dos endpoints
- Documentação automática com Swagger
- Validação de entrada e formatação de resposta
- Middlewares para tratamento de erros
- Sistema de autorização com políticas customizadas

#### **Application Layer**
- Serviços de aplicação que orquestram as operações
- Interfaces para inversão de dependência
- Mapeamento entre DTOs e entidades com AutoMapper
- Sistema de cache com Redis
- Extensões para manipulação de claims

#### **Domain Layer**
- Entidades com regras de negócio encapsuladas
- Value Objects para garantir consistência
- Enums para representar estados e tipos
- Interfaces de contratos de domínio

#### **Infrastructure Layer**
- Acesso a dados com Entity Framework Core
- Contexto de banco de dados
- Migrações para controle de versão do schema

### **Arquitetura Docker**

O projeto utiliza uma arquitetura de microsserviços containerizada com Docker Compose:

#### **Serviços**
- **vemquecabe_api** - API principal da aplicação (.NET 8)
- **vemquecabe_sql** - Banco de dados SQL Server 2022
- **vemquecabe_redis** - Cache distribuído Redis 7.2

#### **Rede e Comunicação**
- **Rede personalizada** (`vemquecabe_network`) para comunicação entre serviços
- **Volumes persistentes** para dados do SQL Server
- **Variáveis de ambiente** para configuração de produção

#### **Dockerfile Multi-stage**
1. **Build Stage** - Compila a aplicação com SDK .NET 8
2. **Publish Stage** - Gera os artefatos de produção
3. **Runtime Stage** - Imagem final otimizada com runtime ASP.NET Core

## Tecnologias Utilizadas

### **Backend**
- **.NET 8** - Framework principal
- **C# 12** - Linguagem de programação com recursos modernos
- **Entity Framework Core 8.0** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional

### **Autenticação e Segurança**
- **JWT Bearer** - Autenticação baseada em tokens
- **BCrypt.Net** - Criptografia de senhas
- **ASP.NET Core Identity** - Gestão de usuários e roles
- **Políticas de Autorização** - Controle de acesso granular

### **Performance e Cache**
- **Redis** - Sistema de cache distribuído
- **StackExchange.Redis** - Cliente Redis para .NET

### **Containerização e Deploy**
- **Docker** - Containerização da aplicação
- **Docker Compose** - Orquestração de múltiplos containers
- **Multi-stage Dockerfile** - Build otimizado e imagem de produção enxuta

### **Ferramentas e Bibliotecas**
- **AutoMapper** - Mapeamento automático entre objetos
- **Swashbuckle** - Documentação automática da API com Swagger
- **Scrutor** - Registro de serviços por convenção

### **Testes**
- **xUnit** - Framework de testes unitários
- **Moq** - Biblioteca para criação de mocks
- **Bogus** - Geração de dados fake para testes
- **Coverlet** - Cobertura de código
- **Stryker.NET** - Mutation testing

### **Qualidade de Código**
- **Nullable Reference Types** - Tipos de referência anuláveis habilitados
- **Implicit Usings** - Usings implícitos para código mais limpo
- **Documentation Generation** - Geração automática de documentação XML

## Como Executar

Você pode executar o projeto de duas formas: usando **Docker** (recomendado) ou **localmente** com .NET.

### 🐳 **Execução com Docker (Recomendado)**

O projeto inclui uma configuração completa com Docker Compose que configura automaticamente:
- API do VemQueCabe
- Banco de dados SQL Server
- Cache Redis

#### **Pré-requisitos**
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

#### **Passos para Execução**

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd projeto-secreto
   ```

2. **Execute com Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Aguarde a inicialização** (primeira execução pode demorar alguns minutos)
   ```bash
   docker-compose logs -f vemquecabe_api
   ```

4. **Acesse a API**
   - API: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

#### **Comandos Úteis do Docker**

```bash
# Parar todos os serviços
docker-compose down

# Rebuild e restart
docker-compose down && docker-compose up -d --build

# Ver logs da API
docker-compose logs -f vemquecabe_api

# Ver logs do banco de dados
docker-compose logs -f vemquecabe_sql

# Executar migrações manualmente (se necessário)
docker-compose exec vemquecabe_api dotnet ef database update
```

### 💻 **Execução Local (Desenvolvimento)**

Para desenvolvimento local ou caso prefira não usar Docker.

#### **Pré-requisitos**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) ou SQL Server LocalDB
- [Redis](https://redis.io/download) (opcional, para cache)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

#### **Passos para Execução**

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd projeto-secreto
   ```

2. **Restaure as dependências**
   ```bash
   dotnet restore
   ```

3. **Configure a string de conexão**
   
   Edite o arquivo `VemQueCabe.Api/appsettings.Development.json` e ajuste as configurações:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VemQueCabe;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Redis": {
       "ConnectionString": "localhost:6379"
     }
   }
   ```

4. **Execute as migrações do banco de dados**
   ```bash
   cd VemQueCabe.Api
   dotnet ef database update
   ```

5. **Execute a aplicação**
   ```bash
   dotnet run --project VemQueCabe.Api
   ```

6. **Acesse a API**
   - API: `https://localhost:7xxx` ou `http://localhost:5xxx`
   - Swagger UI: `https://localhost:7xxx/swagger`

## Documentação da API

A API possui documentação automática gerada pelo Swagger. Após executar a aplicação, acesse:

```
https://localhost:7xxx/swagger
```

### **Principais Endpoints**

#### **Autenticação**
- `POST /api/auth/login` - Login de usuário
- `POST /api/auth/register` - Registro de usuário

#### **Usuários**
- `GET /api/user` - Listar usuários
- `GET /api/user/{id}` - Buscar usuário por ID
- `PUT /api/user/{id}` - Atualizar usuário
- `DELETE /api/user/{id}` - Remover usuário

#### **Motoristas**
- `GET /api/driver` - Listar motoristas
- `POST /api/driver` - Cadastrar motorista
- `PUT /api/driver/{id}` - Atualizar motorista

#### **Passageiros**
- `GET /api/passenger` - Listar passageiros
- `POST /api/passenger` - Cadastrar passageiro
- `PUT /api/passenger/{id}` - Atualizar passageiro

#### **Corridas**
- `GET /api/ride` - Listar corridas
- `POST /api/ride` - Criar nova corrida
- `PUT /api/ride/{id}` - Atualizar corrida
- `DELETE /api/ride/{id}` - Cancelar corrida

#### **Solicitações de Carona**
- `GET /api/riderequest` - Listar solicitações
- `POST /api/riderequest` - Criar solicitação
- `PUT /api/riderequest/{id}` - Atualizar solicitação

## Padrões e Técnicas Utilizadas

### **Design Patterns**
- **Repository Pattern** - Abstração do acesso a dados (implícito no Entity Framework)
- **Unit of Work Pattern** - Controle de transações e contexto
- **Factory Pattern** - Criação de objetos sem expor a lógica de instância
- **Singleton Pattern** - Instância única para serviços compartilhados
- **Result Pattern** - Tratamento consistente de erros e resultados
- **Dependency Injection** - Inversão de controle e desacoplamento
- **DTO Pattern** - Transferência de dados entre camadas
- **Middleware Pattern** - Pipeline de processamento de requisições

### **Princípios SOLID**
- **Single Responsibility** - Cada classe possui uma única responsabilidade
- **Open/Closed** - Aberto para extensão, fechado para modificação
- **Liskov Substitution** - Subtipos substituíveis por seus tipos base
- **Interface Segregation** - Interfaces específicas e coesas
- **Dependency Inversion** - Dependência de abstrações, não implementações

### **Domain-Driven Design (DDD)**
- **Entities** - Objetos com identidade única e regras de negócio
- **Value Objects** - Objetos imutáveis sem identidade
- **Aggregates** - Consistência de dados e regras de negócio
- **Domain Services** - Lógica de domínio sem estado

### **Clean Architecture**
- Separação clara de responsabilidades entre camadas
- Independência de frameworks externos
- Testabilidade elevada com inversão de dependências
- Flexibilidade para mudanças e evolução

### **Metodologias de Desenvolvimento**
- **Mutation Testing** - Validação da qualidade dos testes com Stryker
- **Mocking** - Isolamento de dependências em testes

## Testes

O projeto inclui uma suíte completa de testes organizados por camadas:

### **Estrutura de Testes**
- **Testes Unitários** - Validação de lógica de negócio isolada
- **Testes de Domínio** - Validação de entidades e value objects
- **Mutation Testing** - Verificação da qualidade dos testes

### **Executar Testes**

**Todos os testes:**
```bash
dotnet test
```

**Mutation testing (Stryker):**
```bash
dotnet stryker
```

### **Ferramentas de Teste**
- **xUnit** - Framework principal de testes
- **Moq** - Criação de mocks e stubs
- **Bogus** - Geração de dados de teste realistas
- **Stryker.NET** - Mutation testing para validar qualidade dos testes

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## Fontes de Referência

- [Programação Orientada a Objetos: Princípios e Boas Práticas](https://www.rocketseat.com.br/blog/artigos/post/programacao-orientada-a-objetos-principios-e-boas-praticas)
- [DDD: O que é o Domain-Driven Design de um jeito simples](https://pablo-christian.medium.com/ddd-o-que-%C3%A9-o-domain-driven-design-de-um-jeito-simples-93ea0c9a111)
- [Domain Driven Design: do Problema ao Código - Full Cycle](https://www.youtube.com/watch?v=eUf5rhBGLAk&ab_channel=FullCycle)
- [Domain-Driven Design: Conceitos e Implementação](https://avera.com.br/blog/domain-driven-design/)
- [Testes Unitários em .NET com XUnit](https://medium.com/unicoidtech/testes-unit%C3%A1rios-em-net-com-xunit-d7c78c593832)
- [Arrange Act Assert - Padrão AAA para Testes](https://xp123.com/3a-arrange-act-assert/)
- [Tudo sobre Testes: Unitários vs Integração vs E2E](https://medium.com/rpedroni/tudo-sobre-testes-testes-unit%C3%A1rios-vs-testes-de-integra%C3%A7%C3%A3o-vs-testes-e2e-6a7cc955779)
- [Testes de Unidade e Integração com .NET Core e XUnit](https://jozimarback.medium.com/testes-de-unidade-e-integra%C3%A7%C3%A3o-com-net-core-e-xunit-fad7c18a29a1)
- [O que é SOLID: O Guia Completo dos 5 Princípios da POO](https://medium.com/desenvolvendo-com-paixao/o-que-%C3%A9-solid-o-guia-completo-para-voc%C3%AA-entender-os-5-princ%C3%ADpios-da-poo-2b937b3fc530)
- [Complete Guide to Design Patterns in Programming](https://www.geeksforgeeks.org/system-design/complete-guide-to-design-patterns-in-programming/)
- [Desmistificando o Clean Architecture - Full Cycle](https://www.youtube.com/watch?v=3Nc6RMhjH6g&ab_channel=FullCycle)
- [.NET 7 + ASP.NET Core + JWT + Swagger: Implementando a utilização de tokens](https://renatogroffe.medium.com/net-7-asp-net-core-jwt-swagger-implementando-a-utiliza%C3%A7%C3%A3o-de-tokens-2885ab896767)
- [Como usar Redis na sua solução .NET](https://dev.to/lukesilva/como-usar-redis-na-sua-solucao-net-3b0d)
- [The Result Pattern in C#: A Smarter Way to Handle Errors](https://medium.com/@aseem2372005/the-result-pattern-in-c-a-smarter-way-to-handle-errors-c6dee28a0ef0)
- [Simplifique o controle de erros com o Result Pattern no .NET](https://pathbit.medium.com/simplifique-o-controle-de-erros-com-o-result-pattern-no-net-bf35a0cd1b14)
- [ASP.NET Core API DDD SOLID](https://github.com/jeangatto/ASP.NET-Core-API-DDD-SOLID)

---

Se este projeto te ajudou, considere dar uma estrela! ⭐
