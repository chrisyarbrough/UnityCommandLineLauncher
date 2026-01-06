/// <summary>
/// An exception throw to indicate a handled user error like passing an invalid path.
/// </summary>
class UserException(string? message) : Exception(message);