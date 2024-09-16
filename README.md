# **Doc-Bridge-Service**

The **Doc-Bridge-Service** is a .NET Core application that continuously checks for XML file drops at various network locations, converts these files into JSON, and posts them to an API. The project utilizes common logging and validation behaviors from a shared infrastructure project, **BridgeInfrastructure**, to ensure reusability across multiple services. Unit testing is handled by **xUnit**, and test data generation is performed using **Bogus**.

## **Table of Contents**
1. [Project Structure](#project-structure)
2. [Getting Started](#getting-started)
3. [Installation](#installation)
4. [How It Works](#how-it-works)
5. [Testing](#testing)

## **Project Structure**

### **1. BridgeInfrastructure**
This project contains reusable components, specifically:
- **LoggingBehavior**: Logs the execution details and duration of a request.
- **ValidatorBehavior**: Validates incoming requests using FluentValidation before processing them.
  
The idea behind this project is to keep common behaviors decoupled and reusable across different services.

### **2. Doc-Bridge-Service**
The main project responsible for:
- Watching network locations for XML file drops.
- Converting XML to JSON.
- Posting JSON to an API based on the file's location and name.
  
It leverages the reusable logging and validation behaviors from **BridgeInfrastructure**.

### **3. DocBridgeService.Tests**
The unit testing project:
- Uses **xUnit** for unit testing.
- Uses **Moq** for mocking dependencies like the logging and validation behaviors.
- Uses **Bogus** for generating realistic test data.

---

## **Getting Started**

### **Prerequisites**
Ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (Version 6.0 or higher)
- A code editor like [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/).
- Optionally, [Docker](https://www.docker.com/) for containerizing the application.

### **Cloning the Repository**
Clone the project to your local machine using the following command:

```bash
git clone https://github.com/your-repo/doc-bridge-service.git
cd doc-bridge-service
```

## **Installation**

### **Step 1: Restore NuGet Packages**
Restore the required packages for the solution:

```bash
dotnet restore
```

### **Step 2: Build the Solution**
Build the solution:

```bash
dotnet build
```

### **Step 3: Run the Application**
Run the application (Doc-Bridge-Service):

```bash
dotnet run --project Doc-Bridge-Service
```

## **How It Works**
### **Logging**
The LoggingBehavior logs the details and execution time for each request and writes the logs to Amazon CloudWatch. Serilog is used as the logging library, configured to send logs both to CloudWatch and the local console for development.

### **Validation**
The ValidatorBehavior leverages FluentValidation to ensure that incoming requests are properly validated before processing. If validation fails, an exception is thrown, and the failure is logged.

### **Service Architecture**
The service checks multiple network locations for XML files. When an XML file is found:

1. It is converted to JSON.
2. The JSON is sent as a POST request to the appropriate API endpoint based on the file’s location and name.

## **Testing**
Unit tests are written using xUnit and placed in the DocBridgeService.Tests project. The tests use Moq to mock dependencies like LoggingBehavior and ValidatorBehavior, ensuring isolated unit tests. Bogus is used to generate realistic test data for various models.

### **Run Unit Tests**
To run the tests:
```Bash
dotnet test
```
Example test scenarios include:

* Verifying that logging is triggered when requests are processed.
* Ensuring that invalid requests trigger validation exceptions.
* Generating random test data using Bogus for more robust and dynamic tests.


