namespace Dietcode.Core.Lib.Passwords
{
    [Flags]
    internal enum PasswordFlags
    {
        None = 0,
        Upper = 1 << 0,
        Lower = 1 << 1,
        Digit = 1 << 2,
        Symbol = 1 << 3,
        Whitespace = 1 << 4, // espaço, tab, newline
        NonAscii = 1 << 5  // acentos / unicode
    }
}
