using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Components;

/// <summary>
/// Enables an item to change the accesses of access-locked entities.
/// </summary>
/// <remarks>Currently used for the Access Configurator item.</remarks>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedAccessOverriderSystem))]
public sealed partial class AccessOverriderComponent : Component
{
    /// <summary>
    /// Slot ID for the privileged ID.
    /// </summary>
    public static string PrivilegedIdCardSlotId = "AccessOverrider-privilegedId";

    /// <summary>
    /// If the Access Overrider UI will show info about the privileged ID.
    /// </summary>
    [DataField]
    public bool ShowPrivilegedId = true;

    /// <summary>
    /// Item slot for the privileged ID to be inserted into.
    /// </summary>
    [DataField]
    public ItemSlot PrivilegedIdSlot = new();

    /// <summary>
    /// Sound to play if the access can't be changed for an entity.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public SoundSpecifier? DenialSound;

    /// <summary>
    /// Access reader having its access adjusted.
    /// </summary>
    public EntityUid TargetAccessReaderId = new();

    /// <summary>
    /// BUI message for updating the access list.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class WriteToTargetAccessReaderIdMessage : BoundUserInterfaceMessage
    {
        public readonly List<ProtoId<AccessLevelPrototype>> AccessList;

        public WriteToTargetAccessReaderIdMessage(List<ProtoId<AccessLevelPrototype>> accessList)
        {
            AccessList = accessList;
        }
    }

    /// <summary>
    /// The access levels that should show up in the UI for selection.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<ProtoId<AccessLevelPrototype>> AccessLevels = new();

    /// <summary>
    /// Doafter length.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float DoAfter;

    [Serializable, NetSerializable]
    public sealed class AccessOverriderBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly string TargetLabel;
        public readonly Color TargetLabelColor;
        public readonly string PrivilegedIdName;
        public readonly bool IsPrivilegedIdPresent;
        public readonly bool IsPrivilegedIdAuthorized;
        public readonly bool ShowPrivilegedIdGrid;
        public readonly ProtoId<AccessLevelPrototype>[]? TargetAccessReaderIdAccessList;
        public readonly ProtoId<AccessLevelPrototype>[]? AllowedModifyAccessList;
        public readonly ProtoId<AccessLevelPrototype>[]? MissingPrivilegesList;

        public AccessOverriderBoundUserInterfaceState(bool isPrivilegedIdPresent,
            bool isPrivilegedIdAuthorized,
            ProtoId<AccessLevelPrototype>[]? targetAccessReaderIdAccessList,
            ProtoId<AccessLevelPrototype>[]? allowedModifyAccessList,
            ProtoId<AccessLevelPrototype>[]? missingPrivilegesList,
            string privilegedIdName,
            string targetLabel,
            Color targetLabelColor,
            bool showPrivilegedIdGrid)
        {
            IsPrivilegedIdPresent = isPrivilegedIdPresent;
            IsPrivilegedIdAuthorized = isPrivilegedIdAuthorized;
            TargetAccessReaderIdAccessList = targetAccessReaderIdAccessList;
            AllowedModifyAccessList = allowedModifyAccessList;
            MissingPrivilegesList = missingPrivilegesList;
            PrivilegedIdName = privilegedIdName;
            TargetLabel = targetLabel;
            TargetLabelColor = targetLabelColor;
            ShowPrivilegedIdGrid = showPrivilegedIdGrid;
        }
    }

    [Serializable, NetSerializable]
    public enum AccessOverriderUiKey : byte
    {
        Key,
    }
}
