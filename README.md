# 🛡️ Projeto SafeScribe API (.NET)

Este repositório contém a implementação da **SafeScribe API**, desenvolvida em **.NET 8**, com autenticação JWT e arquitetura em camadas organizada conforme boas práticas de **Clean Architecture**.  
O projeto foi criado como parte de uma entrega prática com foco em **segurança, autenticação e controle de acesso**.

---

## 🎯 Objetivos do Projeto
- Implementar uma **API REST** segura com autenticação **JWT (JSON Web Token)**.  
- Aplicar conceitos de **Clean Architecture e SOLID**.  
- Utilizar **roles (Admin, Editor, User)** para controle de acesso aos endpoints.  
- Estruturar camadas **Domain, Application, Infrastructure e API**.  
- Facilitar o teste e o consumo via **Swagger**.  

---

## 🛠️ Estrutura do Projeto

SafeScribe/
│

├── SafeScribe/ # Camada de apresentação (Controllers e Program.cs)

├── SafeScribe.Application/ # Casos de uso, interfaces e lógica de negócio

├── SafeScribe.Infrastructure/ # Serviços, contexto e integração de dados (InMemory)

├── SafeScribe.Domain/ # Entidades e regras de domínio

│

├── SafeScribe.sln # Arquivo da solução .NET

└── appsettings.json # Configurações (JWT, banco etc.)

---

## ▶️ Como Rodar Localmente

### 📌 Pré-requisitos
- [.NET SDK 8+](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- Nenhum banco de dados externo é necessário (usa **InMemory**).

---

### 📥 Clonar o repositório
```bash
git clone <url-do-repo>
cd SafeScribe
```
---

### ⚙️ Restaurar dependências
```
dotnet restore

```
---

### ⚙️ Configurar o JWT
Abra o arquivo appsettings.json e insira sua chave secreta:
```
"Jwt": {
  "Key": "sua_chave_super_secreta_aqui",
  "Issuer": "SafeScribeIssuer",
  "Audience": "SafeScribeAudience",
  "ExpireMinutes": 60
}

```
---
### ▶️ Executar a API
```
dotnet run --project SafeScribe
```
---

### A API ficará disponível em:
https://localhost:5001
http://localhost:5000

---

### ✅ Testar a Aplicação
Acesse o Swagger UI:

```bash
https://localhost:5001/swagger

```
## No Swagger:

Clique no botão Authorize (canto superior direito).

Cole o token JWT no formato:
```
Bearer {seu_token_aqui}
```
Teste os endpoints autenticados.

----
# 📦 Exemplos de Requisição
### 🔑 POST /api/auth/login
```json
{
  "username": "victor",
  "password": "123456"
}
```
Resposta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5..."
}
```

-----

### 🗒️ POST /api/notes
```json
{
  "title": "Minha primeira nota",
  "content": "Esta é uma nota protegida por JWT."
}
```
Requer role: Editor ou Admin

----

### 📃 GET /api/notes
Retorna todas as notas do usuário autenticado.
Requer role: User, Editor ou Admin

---

### ❌ DELETE /api/notes/{id}
Remove uma nota específica.
Requer role: Admin

--- 
#### 🧩 Exemplo de Token JWT (decodificado)
```json
{
  "name": "victor",
  "role": "Admin",
  "iss": "SafeScribeIssuer",
  "aud": "SafeScribeAudience",
  "exp": 1761007730
}
```

# 👥 Integrantes
(coloca seu nome e seu RM Aqui)
---
### 📌 Observações
O projeto utiliza Entity Framework InMemory para simplificar testes locais.

O Swagger já possui botão “Authorize” para envio de token JWT.

Estrutura compatível com Clean Architecture e boas práticas de segurança em .NET.
