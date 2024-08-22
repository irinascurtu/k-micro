# Install Automapper
1. Install Automapper package
2. Add a folder named `Mappings` in the infrastructure folder
3. Add a class named `OrderProfileMapping`  
   
<details>
<summary>OrderProfileMapping </summary>

```csharp
  public class OrderProfileMapping : Profile
  {
      public OrderProfileMapping()
      {
          CreateMap<OrderModel, Order>();
      }
  }

```
</details>

4. Register Automapper in Program.cs
`builder.Services.AddAutoMapper(typeof(OrderProfileMapping).Assembly);`

