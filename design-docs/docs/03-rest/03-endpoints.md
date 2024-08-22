# Endpoints & Variations

# Endpoints.
In REST we work with representations of resources. For that reason let's start adding `Models`

1. In our project, add a class named `ProductModel` that is a 1-to-1 representation of the `Product` class.
2. Let's return our new class troughout our ProductController 

### Exercise.
Modify each action to use the Model class.


## Benefit
The main reason we should work with DTOs or Models across the project and now with the Domain objects is that allows us to:
- modify when we want
- don't cascade chages. ie. don't change the db 
- constantly keep an eye on what we design and what we output.
- preventing overfetching and underfetching