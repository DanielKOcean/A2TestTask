using A2TestTask.Models.Database;
using System.Collections.Generic;

namespace A2TestTask.Models.GraphQL
{
    public class SearchReportWoodDealResponse
    {
        public SearchReportWoodDeal SearchReportWoodDeal { get; set; }
    }

    public class SearchReportWoodDeal
    {
        public List<Content> Content { get; set; }
    }

    public class Content
    {
        public string DealNumber { get; set; }

        public string DealDate { get; set; }

        public string SellerName { get; set; }

        public string SellerInn { get; set; }

        public string BuyerName { get; set; }

        public string BuyerInn { get; set; }

        public double WoodVolumeBuyer { get; set; }

        public double WoodVolumeSeller { get; set; }

        public WoodDeal ToData() => new WoodDeal
        {
            DealNumber = DealNumber,
            DealDate = DealDate,
            SellerInn = SellerInn,
            SellerName = SellerName,
            BuyerInn = BuyerInn,
            BuyerName = BuyerName,
            WoodVolumeBuyer = WoodVolumeBuyer,
            WoodVolumeSeller = WoodVolumeSeller,
        };
    }
}
