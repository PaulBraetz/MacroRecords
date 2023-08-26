namespace RhoMicro.MacroRecords
{
    /// <summary>
    /// Defines field visibility modifiers.
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// The field will be <see langword="public"/>.
        /// </summary>
        Public,
        /// <summary>
        /// The field will be <see langword="private"/>.
        /// </summary>
        Private,
        /// <summary>
        /// The field will be <see langword="protected"/>.
        /// </summary>
        Protected,
        /// <summary>
        /// The field will be <see langword="internal"/>.
        /// </summary>
        Internal,
        /// <summary>
        /// The field will be <see langword="protected"/> <see langword="internal"/>.
        /// </summary>
        ProtectedInternal,
        /// <summary>
        /// The field will be <see langword="private"/> <see langword="protected"/>.
        /// </summary>
        PrivateProtected
    }
}
