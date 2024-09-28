
# Dynamic Object API

## Overview

This project is a **Dynamic Object API** built using **.NET Core** and **Entity Framework**, which allows users to create, read, update, and delete dynamic objects such as `Products`, `Customers`, `Orders`, etc. All the object types share a single central table, and the API supports transaction management for master objects and their related sub-objects.

## Features

### 1. Dynamic Object Creation
- Create any type of object (e.g., `Product`, `Customer`, `Order`) with flexible fields.
- Objects are stored in a dynamic format, allowing for future expansion without the need for additional database tables.

### 2. CRUD Operations
- **Create**: Dynamically add new objects and define their structure.
- **Read**: Retrieve objects based on type and ID, with optional filters.
- **Update**: Modify existing objects dynamically.
- **Delete**: Remove an object along with its sub-objects in a single operation.

### 3. Transaction Management
- Transactions ensure that all related objects (master and sub-objects) are created, updated, or deleted together.
- If any part of a transaction fails, the entire operation is rolled back.

### 4. Dynamic Field Validation
- Each object type (e.g., `Product`, `Customer`) requires certain fields (`ProductId`, `Name`, etc.).
- Validation ensures that required fields are present before processing.

## API Endpoints


# Create a New Product

POST /api/DynamicCrud

Body:
{
  "objectType": "product",
  "fields": {
    "ProductName": "Sample Product",
    "ProductDescription": "This is a sample product.",
    "Price": 99.99
  }
}

# Create a New Customer

POST /api/DynamicCrud

Body:
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

# List All Products

GET /api/DynamicCrud/product

# Retrieve a Specific Product

GET /api/DynamicCrud/product/1

# Filter Products by Name

GET /api/DynamicCrud/product?ProductName=Sample%20Product

# Update Product Price

PUT /api/DynamicCrud/product/1

Body:
{
  "Price": 79.99
}

# Delete a Product

DELETE /api/DynamicCrud/product/1

# Create a New Order

POST /api/DynamicCrud

Body:
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
          "Price": 99.99
        }
      }
    ]
  }
}

# Retrieve Orders of a Specific Customer

GET /api/DynamicCrud/order?CustomerId=1


## Technology Stack
- **ASP.NET Core** for building the API.
- **Entity Framework Core** for data access.
- **SQL Server** for the database.
- **Newtonsoft.Json** for serializing dynamic fields.

## License
This project is licensed under the MIT License.
