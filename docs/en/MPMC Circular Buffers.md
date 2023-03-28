# MPMC Circular Buffer

## Problem

Let's say you want to make a multi-threaded application. One of the hardest problems on your way is how to make sure that classes from different threads do not read and write values to the same memory location (variable) at the same time. There are many solutions to this problem - from synchronization primitives (lock, mutex, semaphore, etc.), abundantly added to the code of classes that can be accessed from different threads to stateless classes, lock-free algorithms, etc.

One of the simple yet effective solutions used in distributed systems based on the Actor system is the principle of messaging: let the target class be processed by a single thread and let the rest interact with it by sending and receiving messages. Minimum locks, minimum work to transfer a single-threaded application to a multi-threaded one. However, a new problem arises: an efficient system for sending and receiving messages that works with multiple threads for both writers and readers.

## Solution

In the world of embedded development, a solution has long been invented that satisfies this requirement, since it often happens there that the modules exchanging messages work at different speeds (for example, the CPU works faster than most sensors) and, accordingly, one must wait while the second one finishes writing the state to work with - circular buffers.

Multiple Producers Multiple Consumers (MPMC) circular buffer has long been used in AAA game engines to achieve interoperability between application systems running on different threads and for thread-safe messaging.

### Classes

* `EBufferElementState`

Enum. Indicates the status of the circular buffer element. VACANT - empty, available for wtiting, ALLOCATED_FOR_PRODUCER - allocated for writing, being filled in, FILLED - written, available for reading, ALLOCATED_FOR_CONSUMER - allocated for reading, being read.

* `ConcurrentGenericCircularBuffer`

MPMC Circular Buffer. Provides methods `GetState(int index)`, which returns the state of the buffer element by index, `TryProduce(TValue value)`, which attempts to write to the buffer, and `TryConsume(out TValue value)`, which attempts to read the value from the buffer