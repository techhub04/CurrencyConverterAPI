# 1. Introduction
  The API is designed and implemented to convert and fetch the latest and historical exchange rates of currency.   
# 2. Functional overview 
   2.1 Retrieval of latest exchange rates for a specific base currency  
   2.2 Convert the amounts between different currencies
   2.3 Retrieve historical exchange rates for a given period with pagination (e.g., 2020-01-01 to 2020-01-31, base EUR)

# 3. Fetures
    - **Authentication & Authorization**: Custom JWT authentication with Role-Based Access Control (RBAC).  
    - **Caching**: In-memory caching for performance optimization.  
    - **Dependency Injection**: Follows best practices for modularity and maintainability.  
    - **Factory Pattern**: Dynamically instantiates new service instances.  
    - **API Throttling**: Protects endpoints from abuse.  
    - **Logging & Monitoring**: Structured logging via Serilog & Seq, distributed tracing with OpenTelemetry & Jaeger.  
    - **Code Coverage**: more than 90% test coverage to ensure reliability.  
    - **Multi-Environment Deployment**: Supports Dev, Test, and Prod environments.  
    - **API Versioning**: Future-proofing with version control for backward compatibility. 
       
# 4. Setup Instructions for CurrencyConverterAPI
  1. Install Dependencies
      1.1 Install Seq from datalust.co and access the dashboard at http://localhost:5341 to view logs.
      1.2 Install Docker Desktop and run Jaeger for monitoring:
      docker run --name jaeger -p 13133:13133 -p 16686:16686 -p 4317:4317 -d --restart=unless-stopped jaegertracing/opentelemetry-all-in-one
      View traces at http://localhost:16686/trace.
  2. Setup & Run the API
      2.1 Clone the repository via Git Bash or Visual Studio:
       https://github.com/techhub04/CurrencyConverterAPI.git
      2.2 Build the solution to restore all NuGet packages.
      2.3 Run the authentication endpoint to obtain a JWT token (valid for 60 minutes):
          https://localhost:7022/api/v1/Identity/GetToken/adminUser
      2.4 Use the generated token in the Currency Converter API. Currently only the Admin role is required for authorization but can be extended.

  3. API Testing
      3.1 Import the Postman collection (CurrencyConvertor.postman_collection.json) from the solution and test endpoints.
  4. Currency Providers
      4.1 The system supports two providers:
           FrankFurter: Uses FrankFurterAPI for real exchange rates. XChangeNow: A mock implementation using JSON-based exchange rates.
      4.2 XChangeFuture is reserved for future provider integrations.
      4.3 Factory Pattern is used, allowing easy addition of new currency providers with minimal changes.
  5. Testing & Code Coverage
      5.1 Unit & Integration tests are available in the CurrencyConverterTests project.
      5.2 Code coverage report is available under https://github.com/techhub04/CurrencyConverterAPI/tree/main/CoverageReport. Open Index.htm to view.

  # 4. Technical Implementation Details
     4.1 Backend logic - C# 8.0
     4.2 Logging Framework or tools :  Seq, Serilog
     4.3 OpenTelemetry - jaeger

  # 5. Possible future enhancements
    5.1 Replace In-Memory Caching with Redis for distributed caching and multi-instance deployment support.
    5.2 Migrate JWT Authentication to Azure Active Directory or Keycloak for seamless external token management.
    5.3 Implement CI/CD Pipeline with Docker/Containers using Azure DevOps for automated deployment.
  
  
     

  
