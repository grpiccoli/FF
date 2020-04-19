namespace FPFI.Models.ViewModels
{
    public class ModalFooter
    {
        public string SubmitButtonText { get; set; } = "Save";
        public string CancelButtonText { get; set; } = "Cancel";
        public string SubmitButtonID { get; set; } = "btn-submit";
        public string CancelButtonID { get; set; } = "btn-cancel";
        public bool OnlyCancelButton { get; set; }
    }
}