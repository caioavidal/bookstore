# BookStore Web API Documentation

Welcome to the BookStore Web API documentation. This API allows you to manage a bookstore's inventory and orders. The application follows the clean architecture pattern, with distinct layers for Application, Domain, Infrastructure, and Security. It also provides unit tests to ensure the robustness and reliability of the codebase.

## Use Cases

### Administrator

- Add books to the inventory
- List out-of-stock books
- Edit book details
- Delete books from the inventory

### Customer

- Get a list of available books
- Create orders for desired books
- Cancel placed orders
- List orders made by the customer

### All Users

- Create a JWT token using email and password for authentication
- Create new user accounts

## Application Architecture

The BookStore Web API is structured using the clean architecture pattern. This promotes separation of concerns and modularity in the codebase. The architecture consists of the following layers:

1. **Application Layer**: This layer contains the business logic and orchestrates the interactions between different components. It defines use cases and interfaces for interacting with the Domain layer.

2. **Domain Layer**: The heart of the application, this layer contains the core business entities, rules, and logic. It's independent of any external concerns and represents the actual domain model.

3. **Infrastructure Layer**: This layer provides implementations for interacting with external resources such as databases, external services, and APIs. It bridges the gap between the application and external systems.

4. **Security Layer**: Responsible for handling authentication and authorization. It ensures that only authorized users can access certain features and resources of the application.

The architecture is designed to be maintainable, scalable, and testable. Each layer is covered by unit tests to ensure code quality and reliability.

## Running the Application

To run the BookStore Web API, follow these steps:

1. Install .NET 7 SDK.

2. Set up a PostgreSQL database.

3. Run the database script to create the required tables and schema.

4. Open a terminal and navigate to the root directory of the application.

5. Build the Docker image using the provided Dockerfile:

   ```bash
   docker build -t bookstore-web-api .
   ```

6. Run the application using Docker Compose:

   ```bash
   docker-compose up
   ```

This will start the BookStore Web API along with its required services.

Remember to configure environment variables, connection strings, and other settings as needed before running the application.

## Contributing

Contributions are welcome! If you'd like to contribute to the BookStore Web API, please follow the contribution guidelines outlined in the repository.

---

Feel free to customize and expand upon this README template to fit the specifics of your application and development environment. Remember that documentation should be clear, concise, and informative to help users understand and use your application effectively.
