namespace LostControl.Model
{
    /// <summary>
    /// The one way the off-folder differs from the other folders in a round of Find the off-folder.
    /// Always about the icon's shape, never its colour (D49).
    /// </summary>
    public enum FolderDifference
    {
        Glyph = 0,
        TabSide = 1,
        Rotation = 3,
        Size = 4,
    }
}
