public interface IPoolable <T>
{
    /// <summary>
    /// Lease an object from the pool.
    /// Until freed, the leased path is occupied and cannot be used anymore.
    /// </summary>
    /// <returns>An object from the pool.</returns>
    T Lease();

    /// <summary>
    /// Get a copy of an object from the pool.
    /// The object is not taken from the pool so it doesn't need to be freed eventually.
    /// </summary>
    /// <returns>A copy of an object from the pool.</returns>
    T Clone();

    /// <summary>
    /// Return an object back to the pool.
    /// </summary>
    void Free(T obj);
}