using System.Collections.Generic;

namespace Unibill.Impl
{
	public class AnalyticsReporter
	{
		private BillingPlatform platform;

		private IUnityAnalytics analytics;

		public AnalyticsReporter(BillingPlatform platform, IUnityAnalytics analytics)
		{
			this.platform = platform;
			this.analytics = analytics;
		}

		public void onPurchaseSucceeded(PurchasableItem item)
		{
			string receipt;
			string signature;
			extractReceiptAndSignature(item, out receipt, out signature);
			analytics.Transaction(item.LocalId, item.priceInLocalCurrency, item.isoCurrencySymbol, receipt, signature);
		}

		private void extractReceiptAndSignature(PurchasableItem item, out string receipt, out string signature)
		{
			receipt = null;
			signature = null;
			switch (platform)
			{
			case BillingPlatform.AppleAppStore:
				receipt = item.receipt;
				signature = null;
				break;
			case BillingPlatform.GooglePlay:
			{
				Dictionary<string, object> dictionary = item.receipt.hashtableFromJson();
				if (dictionary != null)
				{
					if (dictionary.ContainsKey("json"))
					{
						receipt = (string)dictionary["json"];
					}
					if (dictionary.ContainsKey("signature"))
					{
						signature = (string)dictionary["signature"];
					}
				}
				break;
			}
			}
		}
	}
}
