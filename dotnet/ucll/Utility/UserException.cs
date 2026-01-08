/// <summary>
/// An exception throw to indicate a handled user error like passing an invalid path.
/// </summary>
internal class UserException(string? message) : Exception(message);