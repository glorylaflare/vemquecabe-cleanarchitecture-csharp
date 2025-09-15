using VemQueCabe.Domain.Interfaces;

namespace VemQueCabe.Domain.Shared;

/// <summary>
/// Represents a unit of work that encapsulates a set of repositories and provides a mechanism to commit changes.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the repository for managing driver entities.
    /// </summary>
    IDriverRepository Drivers { get; }

    /// <summary>
    /// Gets the repository for managing passenger entities.
    /// </summary>
    IPassengerRepository Passengers { get; }

    /// <summary>
    /// Gets the repository for managing ride request entities.
    /// </summary>
    IRideRequestRepository RideRequests { get; }

    /// <summary>
    /// Gets the repository for managing ride entities.
    /// </summary>
    IRideRepository Rides { get; }

    /// <summary>
    /// Gets the repository for managing user entities.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Commits all changes made in the current unit of work asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the commit was successful.
    /// </returns>
    Task<bool> CommitAsync();
}
