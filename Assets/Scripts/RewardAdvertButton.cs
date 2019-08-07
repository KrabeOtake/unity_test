using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Button))]
    public abstract class RewardAdverbButton : MonoBehaviour
    {
        [SerializeField] private Button button;

		public static string UNIQUE_USER_ID = "demoUserUnity";
		public static string APP_KEY = "9c530d5d";
		public static String REWARDED_INSTANCE_ID = "0";

		float AdTimeOut = 3.0f;

        private float _time;
        private bool _lock;
        private bool _isShowned;

        public UnityAction<bool> OnShownAdd;

		private GameObject ShowText;

		[Header("Behaviour events")]
        [SerializeField] private UnityEvent OnTimeOut;
        [SerializeField] private UnityEvent BeforeShowAd;
        [SerializeField] private UnityEvent OnAdFound;
        [SerializeField] private UnityEvent AfterShowAd;

        public Button Btn => button;

        void Start()
		{
			Debug.Log("unity-script: RewardedVideoButton Start called");

			//Dynamic config example
			IronSourceConfig.Instance.setClientSideCallbacks(true);
			string id = IronSource.Agent.getAdvertiserId();
			Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);
			IronSource.Agent.validateIntegration();
			Debug.Log("unity-script: unity version" + IronSource.unityVersion());
			IronSource.Agent.setUserId(UNIQUE_USER_ID);
			IronSource.Agent.init(APP_KEY, IronSourceAdUnits.REWARDED_VIDEO);
			IronSource.Agent.validateIntegration();

			// SDK init
			IronSource.Agent.init(APP_KEY);

			//Add Rewarded Video Events
			IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
			IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
			IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
			IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
			IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
		}

        void Awake()
        {
			ShowText = GameObject.Find("Text");
            if (button == null)
                button = GetComponent<Button>();
            button.onClick.AddListener(ShowAd);
            OnShownAdd += _GetReward;
			ShowText.GetComponent<UnityEngine.UI.Text>().color = UnityEngine.Color.red;
			OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        public bool reward;

        private void Update()
        {
            if (!reward) return;
            try
            {
                GetReward(_isShowned);
                AfterShowAd?.Invoke();
                reward = false;

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            _lock = false;

        }


        public virtual void ShowAd()
        {

            if (!_lock)
            {
                _lock = true;
                Debug.Log("SHOW AD");
                StartCoroutine(WaitTimer(IronSource.Agent.isRewardedVideoAvailable));
            }
            else
            {
                Debug.Log("ЗАЛОЧЕНО БЛЯТЬ");
            }
        }


        IEnumerator WaitTimer(Func<bool> isReady)
        {
            bool isInvoked = false;
            bool ready = isReady();
            _time = 0.0f;
            while (!ready && _time <= AdTimeOut)
            {
                if (!isInvoked)
                {
                    BeforeShowAd.Invoke();
                    isInvoked = true;
                }

                _time += 0.5f;
                yield return new WaitForSecondsRealtime(0.5f);
                ready = isReady();
            }


            _lock = false;

            if (ready)
            {
                OnAdFound.Invoke();
                Debug.Log("WaitTimer " + Thread.CurrentThread.ManagedThreadId);
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                OnTimeOut.Invoke();
            }
        }

        // wrapper over GetReward, to add some default behaviour
        public void _GetReward(bool isShowned)
        {
            _lock = false;
            _isShowned = isShowned;
            reward = true;
            //MainThreadDispatcher.Enqueue(() =>
            //{
            //    try
            //    {
            //        Debug.Log("_GetReward " + Thread.CurrentThread.ManagedThreadId);
            //        Debug.Log("Ad unlock");
            //        if (UIStatisticsTest.AdEnabled)
            //        {
            //            GetReward(isShowned);
            //        }
            //        else
            //        {

            //            Test(isShowned);
            //        }

            //        AfterShowAd?.Invoke();

            //    }
            //    catch (Exception e)
            //    {
            //        Debug.Log(e);

            //    }
            //});


        }

        public void Test(bool isShsowned)
        {
            Time.timeScale = 1.0f;
        }

        protected virtual void GetReward(bool isShowned) { }

		/************* RewardedVideo API *************/
		public void ShowRewardedVideoButtonClicked()
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

			// DemandOnly
			// ShowDemandOnlyRewardedVideo ();
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
		void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
		{
			Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
			if (canShowAd)
			{
				ShowText.GetComponent<UnityEngine.UI.Text>().color = UnityEngine.Color.blue;
			}
			else
			{
				ShowText.GetComponent<UnityEngine.UI.Text>().color = UnityEngine.Color.red;
			}
		}

		void RewardedVideoAdOpenedEvent()
		{
			Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
		}

		void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
		{
			Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
		}

		void RewardedVideoAdClosedEvent()
		{
			Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
		}

		void RewardedVideoAdStartedEvent()
		{
			Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
		}

		void RewardedVideoAdEndedEvent()
		{
			Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
		}

		void RewardedVideoAdShowFailedEvent(IronSourceError error)
		{
			Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
		}

		void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
		{
			Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
		}

		void OnApplicationPause(bool isPaused)
		{
			Debug.Log("unity-script: OnApplicationPause = " + isPaused);
			IronSource.Agent.onApplicationPause(isPaused);
		}

#if UNITY_EDITOR
        private void Reset()
        {
            if (button == null)
                button = GetComponent<Button>();
        }
#endif


	}
}