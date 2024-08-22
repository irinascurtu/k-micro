# Adding Models

This guide will help you create and add three model classes (`OrderItemModel`, `OrderModel`, and `CustomerModel`) to a folder named `Models` in your project.

## Step 1: Create the `Models` Folder

1. Open your project in your preferred IDE or text editor.
2. In the project root directory, create a new folder named `Models`.

## Step 2: Add `OrderItemModel` Class

1. Inside the `Models` folder, create a new file named `OrderItemModel.cs`.
2. Open `OrderItemModel.cs` and add the following code:

    ```csharp
    namespace YourNamespace.Models
    {
        public class OrderItemModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
    ```

3. Save the `OrderItemModel.cs` file.

## Step 3: Add `OrderModel` Class

1. Inside the `Models` folder, create a new file named `OrderModel.cs`.
2. Open `OrderModel.cs` and add the following code:

    ```csharp
    namespace YourNamespace.Models
    {
        public class OrderModel
        {
            public int Id { get; set; }
            public DateTime OrderDate { get; set; }
            public ICollection<OrderItemModel> OrderItems { get; set; }
        }
    }
    ```

3. Save the `OrderModel.cs` file.

## Step 4: Add `CustomerModel` Class

1. Inside the `Models` folder, create a new file named `CustomerModel.cs`.
2. Open `CustomerModel.cs` and add the following code:

    ```csharp
    namespace YourNamespace.Models
    {
        public class CustomerModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }

            public ICollection<OrderModel> Orders { get; set; }
        }
    }
    ```

3. Save the `CustomerModel.cs` file.

## Step 5: Review and Update Namespaces

- Ensure that the `namespace YourNamespace.Models` in each file reflects your project's actual namespace.
- Adjust the namespace if necessary.

## Step 6: Build the Project

1. Save all the files.
2. Build your project to ensure that there are no syntax errors.

## Conclusion

You have successfully added the `OrderItemModel`, `OrderModel`, and `CustomerModel` classes to your project. These models are now ready to be used throughout your application.
