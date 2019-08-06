using UnityEngine;

namespace Assets.Scripts
{
    public class CustomRewardAdverbButton : RewardAdverbButton
    {
        protected override void GetReward(bool isShowned)
        {
            Debug.Log(isShowned ? "Ad is shown correctly" : "Th player skipped ad");
        }


    }
}