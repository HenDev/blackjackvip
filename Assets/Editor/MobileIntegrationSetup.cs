using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MobileIntegrationSetup : EditorWindow
{
    [MenuItem("Tools/Mobile Integrations/Auto Setup")]
    public static void ShowWindow()
    {
        GetWindow<MobileIntegrationSetup>("Mobile Integrations Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generador de Integraciones Móviles", EditorStyles.boldLabel);

        if (GUILayout.Button("Crear MobileManager en la escena"))
        {
            CreateMobileManager();
        }
    }

    private static void CreateMobileManager()
    {
        GameObject manager = GameObject.Find("MobileManager");
        if (manager == null)
        {
            manager = new GameObject("MobileManager");
            Undo.RegisterCreatedObjectUndo(manager, "Create MobileManager");
        }

        AddComponentIfMissing<AdsManager>(manager);
        AddComponentIfMissing<IAPManager>(manager);
        AddComponentIfMissing<NotificationManager>(manager);
        AddComponentIfMissing<PrivacyManager>(manager);
        AddComponentIfMissing<InternetChecker>(manager);

        Debug.Log("MobileManager creado con todos los componentes necesarios.");
    }

    private static void AddComponentIfMissing<T>(GameObject go) where T : Component
    {
        if (go.GetComponent<T>() == null)
        {
            go.AddComponent<T>();
        }
    }
}

// ------------------ Ads ------------------
public class AdsManager : MonoBehaviour
{
    [Header("LevelPlay Ads")]
    public string appKey;
    public List<string> placements = new List<string>();

    void Awake()
    {
        Debug.Log("Inicializando LevelPlay Ads con AppKey: " + appKey);
        // Aquí iría la inicialización real de IronSource SDK
    }

    public void ShowAd(string placement)
    {
        Debug.Log("Mostrando anuncio en placement: " + placement);
        // IronSource.Agent.showInterstitial(placement);
    }
}

// ------------------ IAP ------------------
public class IAPManager : MonoBehaviour
{
    [Header("In-App Purchases")]
    public List<string> consumableIds = new List<string>();
    public List<string> nonConsumableIds = new List<string>();

    void Awake()
    {
        Debug.Log("Inicializando Unity IAP con productos...");
        foreach (var id in consumableIds) Debug.Log("Consumable: " + id);
        foreach (var id in nonConsumableIds) Debug.Log("Non-Consumable: " + id);
        // Aquí iría la inicialización real de Unity IAP
    }

    public void BuyProduct(string productId)
    {
        Debug.Log("Comprando producto: " + productId);
        // Códigos de compra con Unity IAP
    }
}

// ------------------ Notificaciones ------------------
public class NotificationManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Inicializando sistema de notificaciones...");
        // Integración con Firebase o Unity Mobile Notifications
    }

    public void SendLocalNotification(string title, string message)
    {
        Debug.Log("Notificación local: " + title + " - " + message);
        // Código para notificación local
    }
}

// ------------------ GDPR & ATT ------------------
public class PrivacyManager : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Mostrando diálogos de GDPR y ATT...");
        // Mostrar consent dialog y ATT prompt
    }
}

// ------------------ Internet Checker ------------------
public class InternetChecker : MonoBehaviour
{
    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No hay conexión a internet.");
            // Aquí puedes mostrar un popup en la UI
        }
    }
}