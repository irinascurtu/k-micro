# Fault tolerance

Fault tolerance is the ability of a system, network, or process to continue operating effectively even when some of its components fail. It is a critical concept in computer science, engineering, and other fields where system reliability is essential. Fault-tolerant systems are designed to anticipate potential failures and to ensure that these failures do not lead to a complete system breakdown.

Key Concepts of Fault Tolerance:
- **Redundancy**: Fault-tolerant systems often incorporate redundant components, such as multiple servers, power supplies, or network connections. If one component fails, the redundant component can take over without interrupting the system’s operation.

-**Error Detection**: Fault-tolerant systems include mechanisms to detect when something has gone wrong, such as error-checking codes or monitoring software.

**- Error Correction**: Once a fault is detected, the system may attempt to correct it automatically. This could involve using data redundancy (e.g., parity bits in data storage) or switching to a backup system.

- **Graceful Degradation:** Instead of completely failing, a fault-tolerant system may degrade in performance when some components fail, but it still continues to operate. For example, a website may remain accessible even if some servers go down, though users might experience slower load times.

-**Failover:** This is a specific mechanism in fault-tolerant systems where, upon detecting a failure, the system automatically switches to a standby component or system without human intervention. This ensures continuity of service.

[## Implementing Orders](implementing-orders.md)

## Retries


## Circuit breakers
