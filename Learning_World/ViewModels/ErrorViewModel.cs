namespace Learning_World.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // New properties to capture error details
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

}
