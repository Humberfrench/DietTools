namespace Dietcode.Core.Lib
{
    public class PhoneValidatorData
    {
        public PhoneValidatorData()
        {
            Valid = true;
            Message = "OK";
        }
        public bool Valid { get; set; }
        public string Message { get; set; }
    }
}
