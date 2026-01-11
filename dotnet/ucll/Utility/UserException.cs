/// <summary>
/// An exception thrown to indicate a handled user error like passing an invalid path.
/// </summary>
internal class UserException(string? message) : Exception(message);

internal class UserCancelledException(string? message) : Exception(message);