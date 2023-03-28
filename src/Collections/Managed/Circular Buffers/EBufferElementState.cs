namespace HereticalSolutions.Collections.Managed
{
     /// <summary>
     /// Indicates the status of the circular buffer element
     /// </summary>
     public enum EBufferElementState
     {
          /// <summary>
          /// Empty, available for writing
          /// </summary>
          VACANT,
          
          /// <summary>
          /// Allocated for writing, being filled in
          /// </summary>
          ALLOCATED_FOR_PRODUCER,
          
          /// <summary>
          /// Written, available for reading
          /// </summary>
          FILLED,
          
          /// <summary>
          /// Allocated for reading, being read
          /// </summary>
          ALLOCATED_FOR_CONSUMER
     }
}