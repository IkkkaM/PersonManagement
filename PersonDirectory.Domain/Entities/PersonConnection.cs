using PersonDirectory.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PersonDirectory.Domain.Entities;

public class PersonConnection : BaseEntity
{
    [Required]
    public int PersonId { get; private set; }

    [Required]
    public int ConnectedPersonId { get; private set; }

    [Required]
    public ConnectionType ConnectionType { get; private set; }

    // Navigation properties
    public virtual Person Person { get; private set; }
    public virtual Person ConnectedPerson { get; private set; }

    protected PersonConnection()
    {
        // EF Core constructor
    }

    public PersonConnection(int personId, int connectedPersonId, ConnectionType connectionType) : this()
    {
        if (personId <= 0)
            throw new ArgumentException("PersonId must be greater than 0", nameof(personId));

        if (connectedPersonId <= 0)
            throw new ArgumentException("ConnectedPersonId must be greater than 0", nameof(connectedPersonId));

        if (personId == connectedPersonId)
            throw new ArgumentException("Person cannot be connected to themselves");

        PersonId = personId;
        ConnectedPersonId = connectedPersonId;
        ConnectionType = connectionType;
    }

    public void UpdateConnectionType(ConnectionType connectionType)
    {
        ConnectionType = connectionType;
        SetUpdatedAt();
    }
}