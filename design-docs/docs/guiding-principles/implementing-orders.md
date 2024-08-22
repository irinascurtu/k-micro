# Orders API

## Create a HttpClient that calls
In this section we will create a HTTP client that will call the ProductsAPI to get products, so we can check them when we create an order

-Start by adding a class named 



## Using Polly
Polly is “a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner,” as stated on the official Polly GitHub repository, which can be found at the following link:

https://github.com/App-vNext/Polly#resilience-policies

## Applying policies to HTTP clients

When calling a web service, it’s a good practice to define an HTTP client factory and register it in a dependency services collection.

In this scenario, you will not call the methods that might throw an exception yourself. Instead, you must define a policy and then attach it to a registered HTTP client, so that it automatically follows that policy.

To do so, we will use an extension class named HttpPolicyExtensions to create policies specifically for common HTTP requests and failures, as shown in the following code:

```c#
RetryPolicy policy = Policy
  .Handle<CustomException>().Or<ArithmeticException>()
  .WaitAndRetry(new[]
  {
    TimeSpan.FromSeconds(1), // 1 second between 1st and 2nd try.
    TimeSpan.FromSeconds(2), // 2 seconds between 2nd and 3rd try.
    TimeSpan.FromSeconds(5) // 5 seconds between 3rd and 4th try.
  });

  ///Defining wait intervals between retries

RetryPolicy policy = Policy
  .Handle<CustomException>().Or<ArithmeticException>()
  .WaitAndRetry(3, retryAttempt => 
    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
//  2 ^ 1 = 2 seconds then
//  2 ^ 2 = 4 seconds then
//  2 ^ 3 = 8 seconds then

AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
  // Handle network failures, 408 and 5xx status codes.
  .HandleTransientHttpError()
  // Define the policy using all the same options as before.
  .RetryAsync(3);
```