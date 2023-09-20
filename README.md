# GeekShopping
Project to learn and practice microservices concepts

I've created this project to practice microservices concepts. It is made with .Net framework and simulates an ecomerce of geek products. So far I created 3 mricro service 
Products, Cart and Login Apis. For login i used Identity server with [Duende Framework](https://duendesoftware.com/). For frontend I used Aspnet with razor pages.

# GeekShopping

Welcome to the GeekShopping project! This repository has been created as a platform to learn and practice microservices concepts. Built using the .NET framework, GeekShopping simulates an e-commerce platform specifically tailored for geeky products. The project currently consists of three microservices: Products, Cart, and Login APIs. Additionally, Identity Server with the Duend framework has been integrated for secure authentication, and the frontend is developed using ASP.NET with Razor Pages.

## Project Overview

Microservices are a crucial architectural pattern for building scalable and maintainable applications. GeekShopping serves as a practical playground for exploring microservices principles and technologies within the context of an e-commerce platform dedicated to geek culture.

## Microservices

### 1. Products Microservice

The Products microservice is responsible for managing the catalog of geek products. It handles product creation, retrieval, updating, and deletion. You can interact with this microservice to view and manage the available products.

### 2. Cart Microservice

The Cart microservice manages the shopping cart for each user. It enables users to add and remove items from their cart, view their cart's contents, and proceed to checkout.

### 3. Login Microservice

The Login microservice is responsible for authentication and user management. It uses Identity Server with the Duend framework to provide secure login and registration functionality. User authentication is a fundamental aspect of any e-commerce platform, and this microservice ensures user data is handled securely.

## Technologies Used

- **.NET Framework**: The project is built on the .NET framework, which provides a robust and versatile platform for microservices development.

- **Identity Server with Duende**: Secure authentication and user management are handled by Identity Server with the Duend framework, ensuring the safety of user data.

- **ASP.NET with Razor Pages**: The frontend of the application is developed using ASP.NET with Razor Pages, providing a user-friendly interface for interacting with the microservices.

- **MySQL**: I used MySQL as database of the project.

## Getting Started

To explore and run the GeekShopping project, follow these steps:

### Prerequisites

- **.NET SDK**: Ensure you have the .NET SDK installed on your system.

### Installation

1. **Clone the Repository**: Start by cloning this repository to your local machine using the following command:

   ```bash
   git clone https://github.com/your-username/GeekShopping.git
   ```

2. **Navigate to Microservices**: Enter the relevant microservices folders (Products, Cart, and Login) and set yours database connection strings.

3. **Start Exploring**: Once you've set up the microservices, you can start exploring the project, interact with the APIs, and understand how microservices work together to create a seamless e-commerce experience.

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow these guidelines:

1. Fork the repository.
2. Create a new branch for your feature or bug fix: `git checkout -b feature-name`.
3. Make your changes and commit them: `git commit -m 'Description of your changes'`.
4. Push your changes to your forked repository: `git push origin feature-name`.
5. Open a pull request, and describe the changes you've made.

Feel free to reach out if you have any questions or need further assistance. Enjoy exploring and experimenting with microservices concepts in GeekShopping!

## Acknowledgements

This project is based from MicroService Architecture with .NET course teached by Leandro Costa
