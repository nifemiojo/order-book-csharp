# Order Book and Matching Engine

This repository contains an implementation of an **Order Book** and **Order Matching Engine** in C#. It is designed to simulate the core functionality of a trading system, matching buy and sell orders based on price and time priority. This project is ideal for anyone looking to understand or build trading systems and is especially relevant for aspiring Quant Developers.

---

## Features

- **Order Book Management**:
  - Efficient storage and management of buy and sell orders.
  - Grouping orders by price levels.

- **Order Matching Engine**:
  - Matches orders based on price and time priority.
  - Supports partial and full order fills.

- **Supported Order Types**:
  - Market Orders
  - Limit Orders

- **Concurrency Support**:
  - Orders can be safely and fairly placed concurrently

- **Performance Optimization**:
  - Designed with scalability in mind, suitable for high-frequency trading scenarios.

---

## Getting Started

### Prerequisites

To run this project, you will need:
- **C#** (version 9.0 or later)
- **.NET Core SDK** (version 8.0 or later)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/order-book-matching-engine.git
   ```
2. Navigate to the project directory:
   ```bash
   cd order-book-matching-engine
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the project:
   ```bash
   dotnet run
   ```

---

## Usage

The project exposes a command-line interface for testing the order book and matching engine. Example usage:

- **Place a Buy Order**:
  ```bash
  place buy 100 50  # Buy 100 units at $50
  ```

- **Place a Sell Order**:
  ```bash
  place sell 50 55  # Sell 50 units at $55
  ```

- **View Order Book**:
  ```bash
  show orderbook
  ```

---

## Project Structure

- **OrderBook**: Contains logic for managing buy and sell orders.
- **MatchingEngine**: Handles matching of orders based on price and time priority.
- **Models**: Includes classes representing orders and related data structures.
- **Tests**: Unit tests to ensure the correctness of the order book and matching logic.

---

## Contributing

Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add feature description"
   ```
4. Push the branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Contact

For questions or suggestions, please contact:
- **Name**: [Your Name]
- **Email**: your.email@example.com
- **LinkedIn**: [Your LinkedIn Profile](https://linkedin.com/in/yourusername)

---

Happy coding! 🚀

