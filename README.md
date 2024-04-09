
# Task Management Project

The Task Management project is a comprehensive solution designed for managing tasks with ease and efficiency. Users can add, delete, and update tasks across different categories, making it an ideal tool for personal and professional use.

## Prerequisites

Before you begin, ensure that you have the following installed on your machine:

- **.NET SDK** (Version 6.0 or later)
- **Node.js** (Version 20.11.0 or later)
- **Angular CLI** (Version 17.3.3)
- **MySQL Server** (Version 8.0.33 or later)
- **Package Manager**: npm (Version 10.2.4)
- **OS**: Windows (win32 x64)

## Git Commands

To clone the project to your local machine, use the following command:

```
git clone <repository-url>
```

## Installation and Setup

### Clone the Repository

First, clone the GitHub repository to your local machine.

### Database  Setup

Import the sql dump file : db_dump.sql (Found in project folder:task management project) into MySQL server database.
You can manually import it or use a software like workbench.

### Server Setup

1. Navigate to the backend directory:

```
cd Server
```

2. Update the connection string in `appsettings.json`(file under Task.API) with your SQL Server credentials:

```json
"MyConn": "Server=127.0.0.1;Port=3306;Database=taskdb;Uid=[your username];Pwd=[your password];"
```

3. Open the `.sln` solution with Visual Studio Community, select Task.API as default project and run the solution.

4. You can also use Postman collections to test the API.

5. The API will be accessible at `https://localhost:7169` or another indicated by Swagger.

### Frontend Setup

1. Open a new terminal and navigate to the frontend directory:

```
cd Client
```

2. Install NPM dependencies:

```
npm install --legacy-peer-deps
npm ci --legacy-peer-deps
```

3. If the API address changes, update the apiUrl variable in `environment.ts` and `environment.prod.ts` under the `Client/src/environments` folder.

4. Start the project:

```
npm start
```

## Usage

After starting both the server and the frontend, you can navigate to the UI through your web browser to manage tasks. The API endpoints are also available for integration with other services or for development purposes.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request with your improvements.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Owner
Amine MATTOUS  <amine.matous@gmail.com> 
