# Collection interfaces

## Public interfaces

### `IResizable`

Indicates that the collection can change its size.

Contains the property `ResizeAllocationCommand` - the command for resizing allocations, and the `Resize()` method that adds new elements to the collection

### `IAppendable`

Indicates that the collection can be appended with a new element on request. This works as an alternative to resizing the collection when it is depleted - for example, if we have an object that we created outside the collection and want to add it (10 soldiers placed on the level in the editor, which can be reused after they are destroyed).

Contains the `AppendAllocationCommand` property - the command for append allocations, and the `Append()` method that allocates a new element

### `ITopUppable`

Indicates that the collection can fill the element with new contents on request. Collections are not responsible for the lifecycle of their elements - collection users may unintentionally destroy them or void their contents (for example, if an element was a transform that was a child of another transform that was destroyed - like a VFX that was attached to a rocket). This interface allows you to fill the element with new contents.

Contains the `TopUp(T value)` method that fills the collection element with new contents

### `IFixedSizeCollection`

Indicates that the collection has a fixed size (like an array). Allows the user to find out the maximum size of the collection (before resizing, if any) and get the element by index

Contains the `Capacity` property, which returns the current maximum size of the collection, and the `ElementAt(int index)` method, which returns the collection element by integer index

### `IIndexable`

Indicates that the collection has a count of currently used elements and can access them by index.

Contains the `Count` property, which returns the amount of currently used elements of the collection, the `this[int index]` indexer property, which returns the collection element by integer index, and the `Get(int index)` method, which does the same, but does not perform index validation (allows, for example, to get an element of the collection that has not yet been used)

## Private interfaces

### `IModifiable`

Indicates that a collection contains another collection and allows access to it

Contains the `Contents` property, which provides access to the nested collection, and the `UpdateContents(TCollection newContents)` method, which allows you to replace the nested collection with another

### `ICountUpdateable`

Indicates that the collection can be resized to the specified size on request.

Contains the `UpdateCount(int newCount)` method, which changes the size of the collection by allocating new or destroying old elements