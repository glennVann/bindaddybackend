# BinDaddy Backend API

A production-ready ASP.NET Core 8 backend API for the BinDaddy e-commerce platform. This API provides complete functionality for managing products, orders, users, and shopping carts.

## Features

- **Product Management**: Create, read, update, and delete bin rental products
- **Order Management**: Process and track customer orders
- **User Management**: Manage customer profiles and information
- **Shopping Cart**: Full cart functionality with add, update, and remove operations
- **Database**: MySQL integration with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI documentation
- **Error Handling**: Comprehensive error handling middleware
- **CORS**: Cross-origin resource sharing enabled for frontend integration
- **Logging**: Built-in logging for debugging and monitoring

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: MySQL 8.0
- **Payment**: Stripe integration ready
- **Containerization**: Docker support
- **Deployment**: Railway, Docker, or any cloud platform

## Project Structure

```
bindaddy-backend/
├── Controllers/           # API endpoints
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── UsersController.cs
│   └── CartController.cs
├── Models/               # Database entities
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Category.cs
│   ├── Cart.cs
│   └── CartItem.cs
├── Services/             # Business logic
│   ├── IProductService.cs
│   ├── IOrderService.cs
│   ├── IUserService.cs
│   └── ICartService.cs
├── DTOs/                 # Data transfer objects
│   ├── ProductDto.cs
│   ├── OrderDto.cs
│   ├── UserDto.cs
│   └── CartDto.cs
├── Data/                 # Database context
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── Middleware/           # Custom middleware
│   └── ErrorHandlingMiddleware.cs
├── Program.cs            # Application startup
├── appsettings.json      # Configuration
└── BinDaddy.Backend.csproj  # Project file
```

## Prerequisites

- .NET 8.0 SDK or later
- MySQL 8.0 or later
- Docker (optional, for containerization)

## Local Development Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd bindaddy-backend
```

### 2. Configure Database Connection

Edit `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=bindaddy;User=root;Password=your_password;"
  }
}
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Create Database and Run Migrations

```bash
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at `http://localhost:5000`

Swagger documentation: `http://localhost:5000/swagger`

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/category/{categoryId}` - Get products by category
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/{id}` - Get order by ID
- `GET /api/orders/user/{userId}` - Get orders by user
- `POST /api/orders` - Create new order
- `PUT /api/orders/{id}/status` - Update order status
- `DELETE /api/orders/{id}` - Delete order

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/email/{email}` - Get user by email
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Cart
- `GET /api/cart/user/{userId}` - Get user's cart
- `POST /api/cart/user/{userId}/items` - Add item to cart
- `PUT /api/cart/user/{userId}/items/{cartItemId}` - Update cart item
- `DELETE /api/cart/user/{userId}/items/{cartItemId}` - Remove item from cart
- `DELETE /api/cart/user/{userId}` - Clear cart

## Deployment

### Deploy to Railway

1. **Create Railway Account**
   - Go to https://railway.app
   - Sign up with GitHub

2. **Connect Repository**
   - Create new project
   - Select "Deploy from GitHub"
   - Choose your repository

3. **Configure Environment Variables**
   - Add `DATABASE_URL`: MySQL connection string
   - Add `STRIPE_SECRET_KEY`: Your Stripe secret key
   - Add `JWT_SECRET`: Your JWT secret

4. **Deploy**
   - Railway will automatically build and deploy

### Deploy with Docker

1. **Build Docker Image**
   ```bash
   docker build -t bindaddy-backend .
   ```

2. **Run Container**
   ```bash
   docker run -p 5000:5000 \
     -e ConnectionStrings__DefaultConnection="Server=mysql;Port=3306;Database=bindaddy;User=root;Password=password;" \
     bindaddy-backend
   ```

### Deploy to Azure

1. Create an Azure App Service
2. Configure MySQL database
3. Set environment variables
4. Deploy using Visual Studio or Azure CLI

## Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `DATABASE_URL` | MySQL connection string | `Server=localhost;Port=3306;Database=bindaddy;User=root;Password=password;` |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | `Production` |
| `STRIPE_SECRET_KEY` | Stripe secret API key | `sk_test_...` |
| `STRIPE_PUBLISHABLE_KEY` | Stripe publishable key | `pk_test_...` |
| `JWT_SECRET` | JWT signing secret | `your-secret-key` |
| `PORT` | Server port | `5000` |

## Database Schema

### Users Table
- Id (Primary Key)
- Email (Unique)
- FirstName
- LastName
- Phone
- Address
- CreatedAt
- UpdatedAt

### Products Table
- Id (Primary Key)
- Name
- Description
- Price
- Stock
- CategoryId (Foreign Key)
- ImageUrl
- CreatedAt
- UpdatedAt

### Orders Table
- Id (Primary Key)
- UserId (Foreign Key)
- TotalAmount
- Status
- OrderDate
- DeliveryDate
- Notes

### OrderItems Table
- Id (Primary Key)
- OrderId (Foreign Key)
- ProductId (Foreign Key)
- Quantity
- Price

### Cart Table
- Id (Primary Key)
- UserId (Foreign Key, Unique)
- CreatedAt
- UpdatedAt

### CartItems Table
- Id (Primary Key)
- CartId (Foreign Key)
- ProductId (Foreign Key)
- Quantity
- Price
- AddedAt

### Categories Table
- Id (Primary Key)
- Name
- Description
- CreatedAt

## Stripe Integration

### Setup Stripe

1. Create a Stripe account at https://stripe.com
2. Get your API keys from the dashboard
3. Add keys to environment variables:
   - `STRIPE_SECRET_KEY`
   - `STRIPE_PUBLISHABLE_KEY`

### Payment Processing

Payment processing is ready to be implemented in the checkout flow. The Stripe NuGet package is already installed.

## Logging

The application uses built-in .NET logging. Logs are output to the console and can be configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
```

## Error Handling

The application includes a global error handling middleware that catches all exceptions and returns appropriate HTTP status codes:

- `400 Bad Request` - Invalid input or operation
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Unexpected server error

## CORS Configuration

CORS is enabled to allow requests from any origin. This can be restricted in `Program.cs`:

```csharp
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

## Testing

### Test with cURL

```bash
# Get all products
curl http://localhost:5000/api/products

# Create a user
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phone": "555-1234",
    "address": "123 Main St"
  }'

# Get user by ID
curl http://localhost:5000/api/users/1
```

### Test with Postman

1. Import the API endpoints into Postman
2. Create requests for each endpoint
3. Test CRUD operations

## Troubleshooting

### Database Connection Error
- Verify MySQL is running
- Check connection string in `appsettings.json`
- Ensure database exists

### Port Already in Use
- Change port in `Program.cs` or environment variable
- Kill process using port 5000

### Migration Errors
- Delete existing migrations if needed
- Run `dotnet ef database drop` to reset
- Run `dotnet ef database update` again

## Contributing

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request

## License

This project is part of the BinDaddy e-commerce platform.

## Support

For issues and questions, please contact the development team or create an issue in the repository.

## Next Steps

1. Deploy to Railway or your chosen platform
2. Configure environment variables
3. Set up Stripe payment processing
4. Connect frontend application
5. Test end-to-end functionality
"# bindaddybackend" 
