# Allocations

## Problem 

Imagine that you are making an object pool or some other kind of collection. Sooner or later, the number of elements that you create in the collection constructor will no longer be sufficient and you will need to make the collection resizable - if the bullets from the pool suddenly start to be reused before they are pushed back, then your machine guns will quickly turn into shotguns. 

At first everything is fine; you just add one more element to the collection every time you lack capacity. Then you realize that the resize operation is happening too often and causes garbage collector spikes. You decide not to waste time and add new elements like List in System.Collections does - doubling the capacity each time. 

Then you realize that this strategy begins to lose effectiveness with each new power of two. In addition, you begin to realize that you want to control the process completely - to be able to specify how many elements are created first and added each time the collection is depleted. 

## Solution 

* Collections contain both the logic and the delegate, calling which changes their size 
* The rules for changing and the amount of elements are set by the structure - the allocation descriptor 
* Allocation descriptors are used as arguments for factories to create collections - one for initial allocation, and another for additional when resizing 
* Allocation descriptors are encapsulated along with allocation delegates responsible for creating new instances in the Command pattern 
* Factories use allocation commands to perform initial allocations and add additional allocation commands to the collections where necessary to achieve the resizeability 

### Responsibilities 

* The factory is responsible for creating instances of collections and elements 
* The descriptor is responsible for determining the allocation rules and the allocation amount 
* The delegate in the allocation command is responsible for the allocation algorithm, the factory is responsible for choosing the method for the delegate 
* The collection itself is responsible for the resize call 

### Classes 

* `EAllocationAmountRule` 

Enum. Specifies the rule by which the amount of elements in the collection is calculated during initialization or resizing. ZERO - 0 elements are allocated, ADD_ONE - 1 new element is added, DOUBLE_AMOUNT - the capacity is doubled, ADD_PREDEFINED_AMOUNT - the specified amount of elements is added 

* `AllocationCommandDescriptor` 

Serializable structure. Encapsulates the allocation rule and the amount of allocated elements 

* `AllocationCommandDescriptor` 

Command pattern. Encapsulates a descriptor and a delegate that produces a new instance of the desired type 