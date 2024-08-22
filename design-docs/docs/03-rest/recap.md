# REST Review and Recap

REST is an architectural style based on HTTP
REST APIs include hypermedia links and actions in their responses, and use hypermedia to inform clients about what operations and resources are available given the current state of the application.
We can use various features of C# and .NET, such as anonymous types and dynamic objects, to create hypermedia resources within our ASP.NET API actions.
Examples of REST and Hypermedia APIs
Some examples of real-world organisations using hypermedia in their APIs.

The GitHub API uses an HTTP Link header in many responses for pagination.

[https://docs.github.com/en/rest/guides/traversing-with-pagination](https://docs.github.com/en/rest/guides/traversing-with-pagination)

Spotify’s API uses hypermedia extensively - for example, check out the structure of the tracks property that’s included when you retrieve a playlist resource:

[https://developer.spotify.com/documentation/web-api/reference/#/operations/get-playlist](https://developer.spotify.com/documentation/web-api/reference/#/operations/get-playlist)

The Amazon API Gateway service exposes a management API based on REST that uses HAL+JSON extensively. (Yes, that’s a REST API you use to configure REST APIs… it gets a bit meta). There’s a good example of this in the documentation for the method-response endpoint:

[https://docs.aws.amazon.com/apigateway/api-reference/resource/method-response/](https://docs.aws.amazon.com/apigateway/api-reference/resource/method-response/)

### Resources and Further Reading

REST is a far more complex topic than we have time to cover in this workshop; this section provides an overview and introduces the idea of using hypermedia to manage application state, but doesn’t go into areas like API versioning, content negotiation, custom media types, and some of the more unusual parts of the HTTP protocol that come into play when we’re designing hypermedia APIs.