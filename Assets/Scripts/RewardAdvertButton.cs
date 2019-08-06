﻿using System;
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

        float AdTimeOut = 3.0f;

        private float _time;
        private bool _lock;
        private bool _isShowned;

        public UnityAction<bool> OnShownAdd;

        [Header("Behaviour events")]
        [SerializeField] private UnityEvent OnTimeOut;
        [SerializeField] private UnityEvent BeforeShowAd;
        [SerializeField] private UnityEvent OnAdFound;
        [SerializeField] private UnityEvent AfterShowAd;

        public Button Btn => button;


        void Awake()
        {
            IronSource.Agent.init("9c530d5d");
            IronSource.Agent.setUserId("1");
            IronSource.Agent.init("9c530d5d", IronSourceAdUnits.REWARDED_VIDEO);
            IronSource.Agent.validateIntegration();
            if (button == null)
                button = GetComponent<Button>();
            button.onClick.AddListener(ShowAd);
            OnShownAdd += _GetReward;
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


#if UNITY_EDITOR
        private void Reset()
        {
            if (button == null)
                button = GetComponent<Button>();
        }
#endif


    }
}