using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceInstance
{
	private static IronSourceInstance _Instance;

    private static string UNIQUE_USER_ID = "demoUserUnity";
    private static string APP_KEY = "9c530d5d";
    private static string REWARDED_INSTANCE_ID = "0";

	public static IronSource Instance()
	{
		if (_Instance == null)
		{
            _Instance = new IronSourceInstance();
            GameObject myRoadInstance = GameObject.Instantiate(Resources.Load("IronSourceEventsPrefab")) as GameObject;
            //Dynamic config example
            IronSourceConfig.Instance.setClientSideCallbacks(true);
			string id = IronSource.Agent.getAdvertiserId();
			IronSource.Agent.validateIntegration();
			IronSource.Agent.setUserId(UNIQUE_USER_ID);
			IronSource.Agent.init(APP_KEY, IronSourceAdUnits.REWARDED_VIDEO);
			IronSource.Agent.validateIntegration();

			// SDK init
			IronSource.Agent.init(APP_KEY);

			//Add Rewarded Video Events
			IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
			IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
			IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
			IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
			IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
		}

		return IronSource.Agent;

	}

	private void OnDestroy()
	{
		if (_Instance == this)
		{
			_Instance = null;
		}
	}

    void OnApplicationPause(bool isPaused)
    {
        Debug.Log("unity-script: OnApplicationPause = " + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }

    /************* RewardedVideo API *************/
    void ShowRewardedVideoButtonClicked()
	{
		Debug.Log("unity-script: ShowRewardedVideoButtonClicked");
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
		}
		else
		{
			Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
		}
	}

	void LoadDemandOnlyRewardedVideo()
	{
		Debug.Log("unity-script: LoadDemandOnlyRewardedVideoButtonClicked");
		IronSource.Agent.loadISDemandOnlyRewardedVideo(REWARDED_INSTANCE_ID);
	}

	void ShowDemandOnlyRewardedVideo()
	{
		Debug.Log("unity-script: ShowDemandOnlyRewardedVideoButtonClicked");
		if (IronSource.Agent.isISDemandOnlyRewardedVideoAvailable(REWARDED_INSTANCE_ID))
		{
			IronSource.Agent.showISDemandOnlyRewardedVideo(REWARDED_INSTANCE_ID);
		}
		else
		{
			Debug.Log("unity-script: IronSource.Agent.isISDemandOnlyRewardedVideoAvailable - False");
		}
	}

    /************* RewardedVideo Delegates *************/
	static void RewardedVideoAdOpenedEvent()
	{
		Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
	}

    static void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
	{
		Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
	}

    static void RewardedVideoAdClosedEvent()
	{
		Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
	}

    static void RewardedVideoAdStartedEvent()
	{
		Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
	}

    static void RewardedVideoAdEndedEvent()
	{
		Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
	}

    static void RewardedVideoAdShowFailedEvent(IronSourceError error)
	{
		Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
	}

    static void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
	{
		Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
	}
}
