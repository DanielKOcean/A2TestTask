using System.Text.RegularExpressions;

namespace A2TestTask.Models.Database
{
    public class WoodDeal : ValidatedDbEntity
    {
        public string DealNumber { get; set; }

        public string DealDate { get; set; }

        public string SellerName { get; set; }

        public string SellerInn { get; set; }

        public string BuyerName { get; set; }

        public string BuyerInn { get; set; }

        public double WoodVolumeSeller { get; set; }

        public double WoodVolumeBuyer { get; set; }

        public override string ToString() => $"#{DealNumber}";

        #region Validation
        public override bool Validate()
        {
            DealNumber = DealNumber ?? string.Empty; // Suppose we treat null as a 0 string
            DealDate = DealDate ?? "0001-01-01"; // Suppose we treat null as default date
            SellerInn = SellerInn ?? string.Empty; // Suppose we treat null as a 0 string
            BuyerInn = BuyerInn ?? string.Empty; // Suppose we treat null as a 0 string
            SellerName = SellerName ?? string.Empty; // Suppose we treat null as a 0 string
            BuyerName = BuyerName ?? string.Empty; // Suppose we treat null as a 0 string



            ValidateDealNumber();

            ValidateDealDate();

            ValidateSellerInn();

            ValidateBuyerInn();

            ValidateSellerName();

            ValidateBuyerName();

            return true;
        }

        private void ValidateDealNumber()
        {
            if (Regex.IsMatch(DealNumber, @"^\d{28,29}$")) return;

            throw new System.FormatException("DealNumber must be  a 28 or 29 digit number.");
        }

        private void ValidateDealDate()
        {
            if (Regex.IsMatch(DealDate, @"^\d{4}\-(0?[1-9]|1[012])\-(0?[1-9]|[12][0-9]|3[01])$")) return;

            throw new System.FormatException("DealDate must have 'yyyy-mm-dd' format.");
        }

        private static bool ValidateInn(string inn) =>
#if DEBUG
            Regex.IsMatch(inn, @"^\d{0,12}$"); // Ignore numerous INN length errors for process time
#else
            Regex.IsMatch(inn, @"^(?:\d{10}|\d{12})$");
#endif


        private void ValidateSellerInn()
        {
            if (ValidateInn(SellerInn)) return;

            throw new System.FormatException("SellerInn must be a 10 or 12 digit number.");
        }

        private void ValidateBuyerInn()
        {
            if (ValidateInn(BuyerInn)) return;

            throw new System.FormatException("BuyerInn must be a 10 or 12 digit number.");
        }

        private void ValidateSellerName()
        {
            if (SellerName.Length <= 512) return;

            throw new System.FormatException("SellerName must be less or equal than 500.");
        }

        private void ValidateBuyerName()
        {
            if (BuyerName.Length <= 512) return;

            throw new System.FormatException("BuyerName must be less or equal than 500.");
        }
        #endregion
    }
}
