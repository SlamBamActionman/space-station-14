using Content.Shared.Access.Systems;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Access.Components;

/// <summary>
/// Provides functionality related to ID cards, such as name, job and job icon information.
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(true)]
[Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWrite)]
public sealed partial class IdCardComponent : Component
{
    /// <summary>
    /// The name stored on the ID card.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    // FIXME Friends
    public string? FullName;

    /// <summary>
    /// Job title LocId of the job prototype stored on the card.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    [Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWrite)]
    public LocId? JobTitle;

    [DataField]
    [AutoNetworkedField]
    private string? _jobTitle;

    /// <summary>
    /// Base string of the job title, either from the job prototype or set manually on the card.
    /// </summary>
    [Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWriteExecute)]
    public string? LocalizedJobTitle { set => _jobTitle = value; get => _jobTitle ?? (JobTitle != null ? Loc.GetString(JobTitle) : string.Empty); }

    /// <summary>
    /// The state of the job icon rsi.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public ProtoId<JobIconPrototype> JobIcon = "JobIconUnknown";

    /// <summary>
    /// Holds the job prototype when the ID card has no associated station record.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public ProtoId<JobPrototype>? JobPrototype;

    /// <summary>
    /// The proto IDs of the departments associated with the job.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public List<ProtoId<DepartmentPrototype>> JobDepartments = new();

    /// <summary>
    /// Determines if accesses from this card should be logged by <see cref="AccessReaderComponent"/>.
    /// </summary>
    [DataField]
    public bool BypassLogging;

    /// <summary>
    /// The name of the entity, should it only have a job assigned and no name.
    /// </summary>
    [DataField]
    public LocId NameLocId = "access-id-card-component-owner-name-job-title-text";

    /// <summary>
    /// The name of the entity, if it has both a name and job assigned.
    /// </summary>
    [DataField]
    public LocId FullNameLocId = "access-id-card-component-owner-full-name-job-title-text";

    /// <summary>
    /// Whether the card can be microwaved for additional accesses.
    /// </summary>
    [DataField]
    public bool CanMicrowave = true;
}
