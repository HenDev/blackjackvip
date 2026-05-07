using System;
using System.Collections.Generic;
using UnityEngine;
#if MOBILE_INTEGRATIONS_HAS_UNITY_IAP
using UnityEngine.Purchasing;
#endif

internal static class UnityIapProvider
{
    public static void Initialize(IAPManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_UNITY_IAP
        UnityIapListenerBridge bridge = manager.GetComponent<UnityIapListenerBridge>();
        if (bridge == null)
        {
            bridge = manager.gameObject.AddComponent<UnityIapListenerBridge>();
        }

        bridge.Initialize(manager);
#else
        manager.SetInitializationState(false, manager.GetConfiguredProductIds(), "Unity IAP package is not installed.");
#endif
    }

    public static void BuyProduct(IAPManager manager, string productId)
    {
#if MOBILE_INTEGRATIONS_HAS_UNITY_IAP
        UnityIapListenerBridge bridge = manager.GetComponent<UnityIapListenerBridge>();
        if (bridge == null)
        {
            manager.NotifyPurchaseFailed(productId, "Unity IAP bridge is missing.");
            return;
        }

        bridge.BuyProduct(productId);
#else
        manager.NotifyPurchaseFailed(productId, "Unity IAP package is not installed.");
#endif
    }

    public static void RestorePurchases(IAPManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_UNITY_IAP
        UnityIapListenerBridge bridge = manager.GetComponent<UnityIapListenerBridge>();
        if (bridge == null)
        {
            manager.SetInitializationState(false, manager.RegisteredProducts, "Unity IAP bridge is missing.");
            return;
        }

        bridge.RestorePurchases();
#else
        manager.SetInitializationState(false, manager.RegisteredProducts, "Unity IAP package is not installed.");
#endif
    }
}

#if MOBILE_INTEGRATIONS_HAS_UNITY_IAP
internal sealed class UnityIapListenerBridge : MonoBehaviour
{
    private IAPManager _manager;
    private StoreController _storeController;
    private bool _isInitializing;
    private bool _callbacksRegistered;

    public void Initialize(IAPManager manager)
    {
        _manager = manager;
        EnsureStoreController();

        if (HasFetchedProducts())
        {
            _manager.SetInitializationState(true, GetAvailableProductIds(), null);
            return;
        }

        if (_isInitializing)
        {
            return;
        }

        List<ProductDefinition> productDefinitions = BuildProductDefinitions();
        if (productDefinitions.Count == 0)
        {
            _manager.SetInitializationState(false, _manager.GetConfiguredProductIds(), "No product ids configured.");
            return;
        }

        _isInitializing = true;
        InitializeAsync(productDefinitions);
    }

    public void BuyProduct(string productId)
    {
        if (_storeController == null || !HasFetchedProducts())
        {
            _manager.NotifyPurchaseFailed(productId, "Unity IAP is not initialized.");
            return;
        }

        Product product = _storeController.GetProductById(productId);
        if (product == null)
        {
            _manager.NotifyPurchaseFailed(productId, "Product is not registered in the store controller.");
            return;
        }

        if (!product.availableToPurchase)
        {
            _manager.NotifyPurchaseFailed(productId, "Product is currently unavailable to purchase.");
            return;
        }

        _storeController.PurchaseProduct(product);
    }

    public void RestorePurchases()
    {
        if (_storeController == null)
        {
            _manager.SetInitializationState(false, GetAvailableProductIds(), "Unity IAP is not initialized.");
            return;
        }

#if UNITY_IOS || UNITY_STANDALONE_OSX
        _storeController.RestoreTransactions((success, error) =>
        {
            if (!success && !string.IsNullOrWhiteSpace(error))
            {
                Debug.LogWarning("IAPManager: restore purchases failed. " + error);
                return;
            }

            Debug.Log("IAPManager: restore purchases result=" + success);
        });
#else
        Debug.Log("IAPManager: restore purchases is only required on Apple platforms.");
#endif
    }

    private async void InitializeAsync(List<ProductDefinition> productDefinitions)
    {
        try
        {
            await _storeController.Connect();
            _storeController.FetchProducts(productDefinitions);
        }
        catch (Exception exception)
        {
            _isInitializing = false;
            _manager.SetInitializationState(false, _manager.GetConfiguredProductIds(), exception.Message);
        }
    }

    private void EnsureStoreController()
    {
        if (_storeController != null)
        {
            return;
        }

        _storeController = UnityIAPServices.StoreController();
        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        if (_callbacksRegistered)
        {
            return;
        }

        _storeController.OnProductsFetched += OnProductsFetched;
        _storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        _storeController.OnPurchasePending += OnPurchasePending;
        _storeController.OnPurchaseFailed += OnPurchaseFailed;
        _callbacksRegistered = true;
    }

    private List<ProductDefinition> BuildProductDefinitions()
    {
        var productDefinitions = new List<ProductDefinition>();

        foreach (string productId in _manager.GetConfiguredProductIds())
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                continue;
            }

            productDefinitions.Add(new ProductDefinition(
                productId,
                _manager.IsNonConsumable(productId) ? ProductType.NonConsumable : ProductType.Consumable));
        }

        return productDefinitions;
    }

    private void OnProductsFetched(List<Product> products)
    {
        _isInitializing = false;
        _manager.SetInitializationState(true, GetAvailableProductIds(), null);
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        _isInitializing = false;
        _manager.SetInitializationState(false, GetAvailableProductIds(), failure != null ? failure.FailureReason : "Failed to fetch products.");
    }

    private void OnPurchasePending(PendingOrder order)
    {
        string productId = GetProductId(order);
        if (string.IsNullOrWhiteSpace(productId))
        {
            _manager.NotifyPurchaseFailed("unknown", "Purchase completed without a valid product id.");
            return;
        }

        _manager.NotifyPurchaseSucceeded(productId);
        _storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        string productId = GetProductId(failedOrder);
        if (string.IsNullOrWhiteSpace(productId))
        {
            productId = "unknown";
        }

        string reason = failedOrder != null
            ? failedOrder.FailureReason + ". " + failedOrder.Details
            : "Unknown purchase failure";

        _manager.NotifyPurchaseFailed(productId, reason);
    }

    private IEnumerable<string> GetAvailableProductIds()
    {
        if (_storeController == null || !HasFetchedProducts())
        {
            return _manager.GetConfiguredProductIds();
        }

        var productIds = new List<string>();
        foreach (Product product in _storeController.GetProducts())
        {
            if (product != null && product.definition != null && !string.IsNullOrWhiteSpace(product.definition.id) && !productIds.Contains(product.definition.id))
            {
                productIds.Add(product.definition.id);
            }
        }

        return productIds;
    }

    private bool HasFetchedProducts()
    {
        return _storeController != null && _storeController.GetProducts() != null && _storeController.GetProducts().Count > 0;
    }

    private static string GetProductId(Order order)
    {
        if (order == null || order.CartOrdered == null)
        {
            return null;
        }

        IReadOnlyList<CartItem> items = order.CartOrdered.Items();
        if (items == null || items.Count == 0)
        {
            return null;
        }

        Product product = items[0] != null ? items[0].Product : null;
        if (product == null || product.definition == null || string.IsNullOrWhiteSpace(product.definition.id))
        {
            return null;
        }

        return product.definition.id;
    }

    private void OnDestroy()
    {
        if (!_callbacksRegistered || _storeController == null)
        {
            return;
        }

        _storeController.OnProductsFetched -= OnProductsFetched;
        _storeController.OnProductsFetchFailed -= OnProductsFetchFailed;
        _storeController.OnPurchasePending -= OnPurchasePending;
        _storeController.OnPurchaseFailed -= OnPurchaseFailed;
        _callbacksRegistered = false;
    }
}
#endif
