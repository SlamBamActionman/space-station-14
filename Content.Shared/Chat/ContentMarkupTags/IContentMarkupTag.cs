﻿using Robust.Shared.Utility;

namespace Content.Shared.Chat.ContentMarkupTags;

public interface IContentMarkupTag
{
    /// <summary>
    /// The string used as the tags name when writing rich text
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Processes another markup node with this content tag.
    /// Note: Any non-text node in the return list MUST include a closing node as well!
    /// </summary>
    public List<MarkupNode>? MarkupNodeProcessing(MarkupNode node)
    {
        return null;
    }

    /// <summary>
    /// Processes a text node with this content tag.
    /// Note: Any non-text node in the return list MUST include a closing node as well!
    /// </summary>
    public List<MarkupNode>? TextNodeProcessing(MarkupNode node)
    {
        return null;
    }

    /// <summary>
    /// Returns a list of nodes replacing the opening markup node for this tag.
    /// Note: If you include a non-text node in the return list that is not closed, you MUST include a closing tag in CloserProcessing.
    /// </summary>
    public List<MarkupNode>? OpenerProcessing(MarkupNode node)
    {
        return null;
    }

    /// <summary>
    /// Returns a list of nodes replacing the closing markup node for this tag.
    /// Note: Any non-text node in the return list MUST include a closing node as well!
    /// </summary>
    public List<MarkupNode>? CloserProcessing(MarkupNode node)
    {
        return null;
    }
}