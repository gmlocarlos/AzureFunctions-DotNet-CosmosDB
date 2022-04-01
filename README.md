# AzureFunctions-DotNet-CosmosDB
This is a .NET Core project with Azure Functions and CosmosDB database

# Usage

**Step 1.** Install Azure Functions Core Tools v4

**Step 2.** Clone this repository and open in Visual Studio Code

**Step 3.** In the Azure Portal create a database called **DemoDB** and a container called **CreditCards**, the container must have as partition key the value **/cardNumber** 

**Step 4.** Compile the project with the next command:

```gradle
func start
```

**Step 4.(Optional)** if you decide to create another function in the project, execute the next command and select the option **Http Trigger**

```gradle
func new
```

# Deploy to Azure

**Step 1.** In the azure portal, create a functions aplication

**Step 2.** In the Visual Studio Code install the extension **Azure Functions**

**Step 3.** In the azure option in the left side, sign in with your azure account

**Step 4.** In the **Functions** section find your create functions aplication and right click, and press the option **Deploy to function app**