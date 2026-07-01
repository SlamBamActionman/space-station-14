using Content.Shared.Access.Systems;

namespace Content.Shared.Access.Components;

/// <summary>
/// This component allows an entity wearing/holding an ID card to be examined to get the card details.
/// </summary>
[RegisterComponent, Access(typeof(IdExaminableSystem))]
public sealed partial class IdExaminableComponent : Component;
