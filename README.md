<p align="center">
  <img src="https://img.shields.io/badge/.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Core">
  <img src="https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?style=for-the-badge&logo=.net&logoColor=white" alt="Entity Framework Core">
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Newtonsoft.Json-000000?style=for-the-badge&logo=json&logoColor=white" alt="Newtonsoft.Json">
</p>

# Dynamic Object API

## Overview

This project is a **Dynamic Object API** built using **.NET Core** and **Entity Framework**, which allows users to create, read, update, and delete dynamic objects such as `Products`, `Customers`, `Orders`, `OrderProduct`, etc. All object types are dynamically handled, supporting flexible data structures and transaction management for master objects and their related sub-objects.

## Features

### 1. Dynamic Object Creation
- Create any type of object (e.g., `Product`, `Customer`, `Order`, `OrderProduct`) with flexible fields.
- Objects are managed dynamically within defined tables, ensuring data consistency and schema validation.

### 2. CRUD Operations
- **Create**: Dynamically add new objects and define their structure.
- **Read**: Retrieve objects based on type and ID, with optional filters for specific queries, such as retrieving all orders by a specific customer.
- **Update**: Modify existing objects dynamically based on their defined structure.
- **Delete**: Remove an object along with its sub-objects (if applicable) in a single operation.

### 3. Transaction Management
- Transactions ensure that all related objects (master and sub-objects) are created, updated, or deleted together.
- If any part of a transaction fails, the entire operation is rolled back to maintain data integrity.

### 4. Dynamic Field Validation
- Each object type (e.g., `Product`, `Customer`) requires specific fields (`ProductId`, `Name`, etc.).
- Validation logic dynamically checks that required fields are present before processing the request.

## API Endpoints

### Create a New Product
**POST /api/DynamicCrud**

**Body:**
```json
{
  "objectType": "product",
  "fields": {
    "ProductName": "Sample Product",
    "ProductDescription": "This is a sample product.",
    "Price": 99.99,
    "Status": "Available"
  }
}
```

### Create a New Customer
**POST /api/DynamicCrud**

**Body:**
```json
{
  "objectType": "customer",
  "fields": {
    "FirstName": "Ahmet",
    "LastName": "Yılmaz",
    "Email": "ahmet.yilmaz@example.com",
    "Phone": "555-1234",
    "Address": "Istanbul, Turkey"
  }
}
```

### List All Products
**GET /api/DynamicCrud/product**

### Retrieve a Specific Product
**GET /api/DynamicCrud/product/1**

### Filter Products by Name
**GET /api/DynamicCrud/product?ProductName=Sample%20Product**

### Update Product Price
**PUT /api/DynamicCrud/product/1**

**Body:**
```json
{
  "Price": 79.99
}
```

### Delete a Product
**DELETE /api/DynamicCrud/product/1**

### Create a New Order
**POST /api/DynamicCrud**

**Body:**
```json
{
  "objectType": "order",
  "fields": {
    "CustomerId": 1,
    "OrderDate": "2023-10-02T14:00:00",
    "TotalAmount": 299.97,
    "Status": "Processing",
    "SubObjects": [
      {
        "objectType": "orderProduct",
        "fields": {
          "ProductId": 1,
          "Quantity": 3,
          "Price": 79.99
        }
      }
    ]
  }
}
```

### Retrieve Orders of a Specific Customer
**GET /api/DynamicCrud/order?CustomerId=1**

## Technology Stack
- **ASP.NET Core** for building the API.
- **Entity Framework Core** for data access and transaction management.
- **SQL Server** as the database.
- **Newtonsoft.Json** for handling dynamic JSON fields.

## License
This project is licensed under the MIT License.
