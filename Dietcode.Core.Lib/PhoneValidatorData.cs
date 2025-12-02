namespace Dietcode.Core.Lib
{
    public record class PhoneValidatorData
    {
        public bool Valid { get; set; } = true;
        public string Message { get; set; } = "OK";
    }
}
