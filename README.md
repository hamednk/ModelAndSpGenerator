# Store Procedure and Model Generator

This project is a tool designed to generate C# models and SQL stored procedures based on your database tables. By connecting to your database, you can select tables and generate corresponding C# models or SQL stored procedures with ease.

## Features

- **Database Connection**: Connect to your SQL Server database using integrated security.
- **Model Generation**: Generate C# model classes based on selected database tables.
- **Stored Procedure Generation**: Generate SQL stored procedures for selected tables.

## How to Use

1. **Connect to Database**:
   - Enter your database connection details.
   - Example: `Data Source=YourServer;Initial Catalog=YourDatabase;Integrated Security=true`

2. **Generate Models**:
   - Select the tables you want to generate models for.
   - Click on `Model Generate` to produce C# model classes.

3. **Generate Stored Procedures**:
   - Select the tables for which you need stored procedures.
   - Click on `StoreProcedure Generate` to create the corresponding SQL stored procedures.

## Example

### Generating a C# Model

```csharp
public class YourTableName
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Other properties based on your table columns
}
```

### Generating a Stored Procedure

```sql
CREATE PROCEDURE GetYourTableName
AS
BEGIN
    SELECT * FROM YourTableName;
END
```

## Prerequisites

- .NET Framework 4.7.2 or higher
- SQL Server (tested on 2016+)
- Read/Write access to the target database.


## Installation

1. Clone this repository:
   ```bash
   git clone https://github.com/your-username/sp-and-model-generator.git
   ```
2. Open the solution in Visual Studio.
3. Build the project to restore NuGet packages.



## Contributing
Feel free to fork the project and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.
