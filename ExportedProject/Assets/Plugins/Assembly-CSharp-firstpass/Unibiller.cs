using System;
using System.Collections.Generic;
using Unibill;
using Unibill.Impl;
using Uniject.Impl;
using UnityEngine;

public class Unibiller
{
	private static Biller biller;

	public static BillingPlatform BillingPlatform
	{
		get
		{
			if (biller != null)
			{
				return biller.InventoryDatabase.CurrentPlatform;
			}
			return BillingPlatform.UnityEditor;
		}
	}

	public static bool Initialised
	{
		get
		{
			if (biller != null)
			{
				return biller.State == BillerState.INITIALISED || biller.State == BillerState.INITIALISED_WITH_ERROR;
			}
			return false;
		}
	}

	public static UnibillError[] Errors
	{
		get
		{
			if (biller != null)
			{
				return biller.Errors.ToArray();
			}
			return new UnibillError[0];
		}
	}

	public static PurchasableItem[] AllPurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllPurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllNonConsumablePurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllNonConsumablePurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllConsumablePurchasableItems
	{
		get
		{
			return biller.InventoryDatabase.AllConsumablePurchasableItems.ToArray();
		}
	}

	public static PurchasableItem[] AllSubscriptions
	{
		get
		{
			return biller.InventoryDatabase.AllSubscriptions.ToArray();
		}
	}

	public static string[] AllCurrencies
	{
		get
		{
			return biller.CurrencyIdentifiers;
		}
	}

	public static event Action<UnibillState> onBillerReady;

	public static event Action<PurchaseEvent> onPurchaseCompleteEvent;

	public static event Action<PurchasableItem> onPurchaseComplete;

	public static event Action<PurchaseFailedEvent> onPurchaseFailed;

	public static event Action<PurchasableItem> onPurchaseDeferred;

	public static event Action<PurchasableItem> onPurchaseRefunded;

	public static event Action<bool> onTransactionsRestored;

	public static void Initialise(List<ProductDefinition> runtimeProducts = null)
	{
		if (biller == null)
		{
			RemoteConfigManager remoteConfigManager = new RemoteConfigManager(new UnityResourceLoader(), new UnityPlayerPrefsStorage(), new UnityLogger(), Application.platform, runtimeProducts);
			UnibillConfiguration config = remoteConfigManager.Config;
			GameObject gameObject = new GameObject();
			gameObject.name = "Unibill";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			UnibillUnityUtil util = gameObject.AddComponent<UnibillUnityUtil>();
			BillerFactory billerFactory = new BillerFactory(new UnityResourceLoader(), new UnityLogger(), new UnityPlayerPrefsStorage(), new RawBillingPlatformProvider(config, gameObject), config, util, new UnityAnalytics());
			biller = billerFactory.instantiate();
			_internal_hook_events(biller, billerFactory);
		}
		biller.Initialise();
	}

	public static PurchasableItem GetPurchasableItemById(string unibillPurchasableId)
	{
		if (biller != null)
		{
			return biller.InventoryDatabase.getItemById(unibillPurchasableId);
		}
		return null;
	}

	public static void initiatePurchase(PurchasableItem purchasable, string developerPayload = "")
	{
		if (biller != null)
		{
			biller.purchase(purchasable, developerPayload);
		}
	}

	public static void initiatePurchase(string purchasableId, string developerPayload = "")
	{
		if (biller != null)
		{
			biller.purchase(purchasableId, developerPayload);
		}
	}

	public static int GetPurchaseCount(PurchasableItem item)
	{
		if (biller != null)
		{
			return biller.getPurchaseHistory(item);
		}
		return 0;
	}

	public static int GetPurchaseCount(string purchasableId)
	{
		if (biller != null)
		{
			return biller.getPurchaseHistory(purchasableId);
		}
		return 0;
	}

	public static decimal GetCurrencyBalance(string currencyIdentifier)
	{
		if (biller != null)
		{
			return biller.getCurrencyBalance(currencyIdentifier);
		}
		return 0m;
	}

	public static void CreditBalance(string currencyIdentifier, decimal amount)
	{
		if (biller != null)
		{
			biller.creditCurrencyBalance(currencyIdentifier, amount);
		}
	}

	public static bool DebitBalance(string currencyIdentifier, decimal amount)
	{
		if (biller != null)
		{
			return biller.debitCurrencyBalance(currencyIdentifier, amount);
		}
		return false;
	}

	public static void restoreTransactions()
	{
		if (biller != null)
		{
			biller.restoreTransactions();
		}
	}

	public static void clearTransactions()
	{
		if (biller != null)
		{
			biller.ClearPurchases();
		}
	}

	public static IAppleExtensions getAppleExtensions()
	{
		return biller.getAppleExtensions();
	}

	public static void _internal_hook_events(Biller biller, BillerFactory factory)
	{
		biller.onBillerReady += delegate(bool success)
		{
			if (Unibiller.onBillerReady != null)
			{
				if (success)
				{
					Unibiller.onBillerReady((biller.State != BillerState.INITIALISED) ? UnibillState.SUCCESS_WITH_ERRORS : UnibillState.SUCCESS);
				}
				else
				{
					Unibiller.onBillerReady(UnibillState.CRITICAL_ERROR);
				}
			}
		};
		biller.onPurchaseComplete += _onPurchaseComplete;
		biller.onPurchaseFailed += _onPurchaseFailed;
		biller.onPurchaseDeferred += _onPurchaseDeferred;
		biller.onPurchaseRefunded += _onPurchaseRefunded;
		biller.onTransactionsRestored += _onTransactionsRestored;
	}

	private static void _onPurchaseComplete(PurchaseEvent e)
	{
		if (Unibiller.onPurchaseComplete != null)
		{
			Unibiller.onPurchaseComplete(e.PurchasedItem);
		}
		if (Unibiller.onPurchaseCompleteEvent != null)
		{
			Unibiller.onPurchaseCompleteEvent(e);
		}
	}

	private static void _onPurchaseFailed(PurchaseFailedEvent e)
	{
		if (Unibiller.onPurchaseFailed != null)
		{
			Unibiller.onPurchaseFailed(e);
		}
	}

	private static void _onPurchaseDeferred(PurchasableItem item)
	{
		if (Unibiller.onPurchaseDeferred != null)
		{
			Unibiller.onPurchaseDeferred(item);
		}
	}

	private static void _onPurchaseRefunded(PurchasableItem item)
	{
		if (Unibiller.onPurchaseRefunded != null)
		{
			Unibiller.onPurchaseRefunded(item);
		}
	}

	private static void _onTransactionsRestored(bool success)
	{
		if (Unibiller.onTransactionsRestored != null)
		{
			Unibiller.onTransactionsRestored(success);
		}
	}
}
