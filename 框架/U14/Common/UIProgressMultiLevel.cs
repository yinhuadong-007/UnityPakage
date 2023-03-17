namespace U14.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using U14.Event;

    public class UIProgressMultiLevel
    {
        public static class EProgressEventType
        {
            public const string LevelUp = "LevelUp";
        }

        [SerializeField, ChineseLabel("进度条")] private Image barBaseLevel;
        [SerializeField, ChineseLabel("进度显示文本")] private Text textBaseLevel;
        [SerializeField, ChineseLabel("进度条速度")] private float speedBaseLevel = 1;
        [SerializeField, ChineseLabel("等级max图标")] private CanvasGroup levelMax;
        [SerializeField, ChineseLabel("等级经验列表")] private List<int> LevelExpList;
        [SerializeField, ChineseLabel("初始等级")] private int curLevelIndex = 0;

        private float curExpProgress = 0;
        private float targetExpProgress = 0;
        private float subExpProgress = 0;

        private int targetBaseLevel;
        float curExpValue = 0;
        public UIProgressMultiLevel(Image barBaseLevel, Text textBaseLevel, float speedBaseLevel, CanvasGroup levelMax, List<int> LevelExpList, int curLevelIndex = 0)
        {
            this.barBaseLevel = barBaseLevel;
            this.textBaseLevel = textBaseLevel;
            this.speedBaseLevel = speedBaseLevel;
            this.levelMax = levelMax;
            this.LevelExpList = LevelExpList;
            this.curLevelIndex = curLevelIndex;

            if (levelMax != null) levelMax.alpha = 0;
        }

        public void ResetData()
        {
            curExpProgress = 0;
            targetExpProgress = 0;
            subExpProgress = 0;
        }

        void Update()
        {
            if (curExpProgress != targetExpProgress && subExpProgress != 0)
            {
                UpdateExpProgress();
            }
        }

        public void SetCurrentExpValue(float val)
        {
            curExpValue = val;
            if ((curExpProgress == targetExpProgress || subExpProgress == 0) && LevelExpList.Count > curLevelIndex)
            {
                SetExpProgress();
            }
        }

        private void SetCurExpProgress(float curValue, float totalValue)
        {
            float percent = curValue / totalValue;
            if (percent < 0) return;
            if (percent > 1) percent = 1;

            targetExpProgress = percent;

            subExpProgress = targetExpProgress - curExpProgress == 0 ? 0 : (targetExpProgress - curExpProgress > 0 ? speedBaseLevel : -speedBaseLevel);
            UpdateExpProgress();
        }

        private void UpdateExpProgress()
        {
            curExpProgress += Time.deltaTime * speedBaseLevel;
            if ((targetExpProgress - curExpProgress) * subExpProgress <= 0) curExpProgress = targetExpProgress;

            // float val = Mathf.FloorToInt(curExpProgress * 10000) / 100;
            // val = val > 100 ? 100 : val;
            // textBaseLevel.text = $"{val}%";

            if (barBaseLevel != null)
            {
                barBaseLevel.fillAmount = curExpProgress;
            }
            if (curExpProgress >= 1)
            {
                curLevelIndex++;
                if (LevelExpList.Count > curLevelIndex)
                {
                    ResetData();
                    textBaseLevel.text = "" + (curLevelIndex + 1);
                    SetExpProgress();
                }
                else
                {
                    if (textBaseLevel != null) textBaseLevel.transform.parent.GetComponent<CanvasGroup>().alpha = 0;
                    if (levelMax != null) levelMax.alpha = 1;
                }
                //Todo:升级
                Debug.Log("UIProgressMultiLevel Level Up");
                EventUtil.DispatchEvent(EProgressEventType.LevelUp, curLevelIndex);
            }
        }

        private void SetExpProgress()
        {
            int nextLevelIndex = curLevelIndex + 1;
            int curTargetExp = LevelExpList[curLevelIndex];
            int nextTargetExp = LevelExpList[nextLevelIndex];
            SetCurExpProgress(curExpValue - curTargetExp, nextTargetExp - curTargetExp);
        }
    }
}
