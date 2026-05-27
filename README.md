<h1 align="center">EcommerceAPIDemo</h1>

<h3 align="center">A .NET 8 Web API showcase for an e-commerce platform, demonstrating CRUD operations, advanced query filtering, pagination, and complex sales refund workflows with EF Core and SQL Server.</h3>

<div align="center">

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-blue?style=for-the-badge&logoColor=white)
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)

</div>

## Key Features
* RESTful API design with a `Games` and `Sales` domain
* Pagination & query filtering
* Full and partial refund logic with validation
* Entity Framework Core with SQL Server
* Service layer + Repository pattern via DI
* DTO mapping for clean API contracts

## Tech Stack
* .NET 8
* ASP.NET Core
* Entity Framework
* SQL Server

## Database Schema
<img width="844" height="635" alt="EcomDBDiagram" src="https://github.com/user-attachments/assets/97b7b3b5-8ce9-4965-87c0-c789b655ef0e" />

### Type Mapping: C# to SQL Server
EF Core maps C# types in the application layer to the following SQL Server types in the database layer:
| C# Type | SQL Server Type |
|---|---|
| `int` | `int` |
| `string` | `nvarchar(max)` |
| `DateTime` | `datetime2` |
| `double` | `float` |
| `bool` | `bit` |
| `enum` | `int` |

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/games` | List all games with pagination & filtering
| GET | `/api/games/{id}` | Get a specific game by ID
| POST | `/api/games` | Create a new game
| PUT | `/api/games/{id}` | Update an existing game
| GET | `/api/games/categories` | List all categories with pagination
| GET | `/api/games/categories/{id}` | Get a specific category by ID
| POST | `/api/games/categories` | Create a new category
| PUT | `/api/games/categories/{id}` | Update an existing category
| GET | `/api/sales` | List all sales with pagination & filtering
| GET | `/api/sales/{id}` | Get a specific sale by ID
| POST | `/api/sales` | Create a new sale
| PUT | `/api/sales/{id}?refundAmount={amount}` | Process a full or partial refund for a specific sale

### Query Parameters

Pagination is supported on all `GET` list endpoints using the following parameters:

| Parameter | Type | Default | Description |
|---|---|---|---|
| `pageNumber` | `int` | `1` | The page of results to return |
| `pageSize` | `int` | `10` | Number of items per page (max `50`) |

#### Games Filters
Applied to `GET /api/games`:

| Parameter | Type | Description |
|---|---|---|
| `categoryId` | `int?` | Filter by an associated category ID |
| `minPrice` | `double?` | Filter for games priced greater than or equal to value |
| `maxPrice` | `double?` | Filter for games priced less than value |

**Example:**  
`GET /api/games?categoryId=2&minPrice=10.00&maxPrice=59.99&pageNumber=1&pageSize=10`

#### Sales Filters
Applied to `GET /api/sales`:

| Parameter | Type | Description |
|---|---|---|
| `gameProductId` | `int?` | Filter sales containing a specific game |
| `transactionDate` | `date` | Filter by the date of the transaction (`YYYY-MM-DD`) |
| `minTransactionValue` | `double?` | Filter for sales with total value greater than or equal to |
| `maxTransactionValue` | `double?` | Filter for sales with total value less than |

**Example:**  
`GET /api/sales?transactionDate=2026-05-26&pageNumber=1&pageSize=20`

## Sample Requests & Responses

```
// POST /api/sales
// Request Body
{
  "purchasedGameIds": [1, 13],
  "creditCardType": 1,
  "lastFourDigitsOfPaymentCard": 6899,
  "subTotal": 250.9,
  "salesTax": 952.64,
  "total": 923.92
}

// Response: 200 OK
{
  "id": 18,
  "transactionDate": "2026-05-26T22:10:51.2210706-05:00",
  "transactionLastUpdatedDate": "2026-05-26T22:10:51.2211263-05:00",
  "isRefund": false,
  "isPartialRefund": false,
  "gamesPurchasedIds": [1, 13],
  "creditCardType": 1,
  "lastFourDigitsOfPaymentCard": 6899,
  "subTotal": 250.9,
  "salesTax": 952.64,
  "total": 923.92,
  "actualTransactionValue": 923.92
}
```
Handling a partial refund on a sale:

<img width="877" height="770" alt="Handling a partial refund on a sale in Postman" src="https://github.com/user-attachments/assets/b1a91891-a906-44dc-9fac-b41742f371a8" />

Creating a new game in the database:

<img width="876" height="767" alt="Creating a new game in the database using Postman" src="https://github.com/user-attachments/assets/31f4efa9-6cea-4616-ad9f-beba5d9a199b" />

Listing categories:

<img width="874" height="771" alt="List the first page of categories in Postman" src="https://github.com/user-attachments/assets/613a240c-10a9-453e-8ef8-041f5113b513" />

## Running It Locally
Prerequisites:
* .NET 8 SDK
* SQL Server 
* Postman (Optional, for testing the included collection / populating seed data)

1. **Clone the Repository**
```
git clone https://github.com/GoldRino456/EcommerceAPIDemo.git
```

2. **Configure the Database Connection**
Open appsettings.json and verify the DefaultConnection string matches your local SQL Server instance:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=Ecommerce;Integrated Security=true;TrustServerCertificate=True"
}
```

3. **Apply Entity Framework Migrations**
From the project directory, run:
```
dotnet ef database update
```
> If you do not have the EF Core tools installed globally, install them first:
> ```
> dotnet tool install --global dotnet-ef
> ```

This will create the Ecommerce database and apply all existing migrations.

4. **Run The Application**
```
dotnet run
```
The API will start on:

HTTP: http://localhost:8081
HTTPS: https://localhost:8080
By default, the browser will automatically open the Swagger UI at /swagger.

5. **Test The Endpoints**
You can interact with the API directly through:

Swagger UI: https://localhost:8080/swagger
Postman: Import the included collection found in the root of the Repository.

## Architecture
```
EcommerceAPIDemo/
├── Controllers/      # API routing & request/response handling
├── Services/         # Business logic & DTO mapping
├── Data/
│   ├── Models/       # EF Core entities
│   ├── DTOs/         # API contracts
│   └── SalesDbContext.cs
└── Migrations/       # EF Core schema migrations
```

## Author

Developed By: **Ethan H. Eastwood**

* Website: [EthanEastwood.dev](https://ethaneastwood.dev)
* Github: [@GoldRino456](https://github.com/GoldRino456)
* LinkedIn: [@ethan-h-eastwood](https://linkedin.com/in/ethan-h-eastwood)

And a special shoutout to [The C# Academy](https://www.thecsharpacademy.com/)! - I'm learning more and more daily because of y'all! Thank you for being such a great and supportive community!