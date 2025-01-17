using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

//Source: https://docs.unity3d.com/Packages/com.unity.ads@3.3/manual/MonetizationBasicIntegrationUnity.html
[RequireComponent(typeof(Button))]
public class RewardedAdsButton : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    private string gameId = "4875722";
#elif UNITY_ANDROID
    private string gameId = "4875723";
#endif

    private Game _data;

    Button myButton;
    public string myPlacementId = "Get_Keys";


    private void Awake()
    {
        _data = GameObject.Find("GameData").GetComponent<Game>();
    }

    void Start()
    {
        myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement�s status:
        myButton.interactable = Advertisement.IsReady(myPlacementId);

        // Map the ShowRewardedVideo function to the button�s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            _data.KeyIncrease();
            this.transform.parent.parent.gameObject.SetActive(false);
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }
    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

}
