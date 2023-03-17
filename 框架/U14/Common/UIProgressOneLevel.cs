namespace U14.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using U14.Event;

    public class UIProgressOneLevel
    {
        public static class EProgressEventType
        {
            public const string Complete = "Complete";
        }

        [SerializeField, ChineseLabel("进度条")] private Image progressBar;
        [SerializeField, ChineseLabel("进度显示文本")] private Text progressText;
        [SerializeField, ChineseLabel("进度条速度")] private float speedBaseLevel = 1;

        private float curExpProgress = 0;
        private float targetExpProgress = 0;
        private float subExpProgress = 0;

        Coroutine m_update;

        public UIProgressOneLevel(Image progressBar, Text progressText = null, float speedBaseLevel = 1, float curValue = 0, float totalValue = 1)
        {
            this.progressBar = progressBar;
            this.progressText = progressText;
            this.speedBaseLevel = speedBaseLevel;
            SetProgress(0, 1);
            curExpProgress = targetExpProgress;
            UpdateExpProgress();
            m_update = this.progressBar.StartCoroutine(this.Update());
        }

        ~UIProgressOneLevel()
        {
            this.progressBar.StopCoroutine(m_update);
        }

        IEnumerator Update()
        {
            while (true)
            {
                if (curExpProgress != targetExpProgress && subExpProgress != 0)
                {
                    curExpProgress += Time.deltaTime * speedBaseLevel;
                    UpdateExpProgress();
                }
                yield return 0;
            }
        }

        public void SetProgress(float curValue, float totalValue)
        {
            float percent = curValue / totalValue;
            if (percent == targetExpProgress) return;
            if (percent < 0) return;
            if (percent > 1) percent = 1;

            targetExpProgress = percent;

            subExpProgress = targetExpProgress - curExpProgress == 0 ? 0 : (targetExpProgress - curExpProgress > 0 ? speedBaseLevel : -speedBaseLevel);
            UpdateExpProgress();
        }

        private void UpdateExpProgress()
        {
            if ((targetExpProgress - curExpProgress) * subExpProgress <= 0) curExpProgress = targetExpProgress;
            if (progressBar != null)
            {
                progressBar.fillAmount = curExpProgress;
            }
            if (progressText != null)
            {
                float val = Mathf.FloorToInt(curExpProgress * 10000) / 100;
                val = val > 100 ? 100 : val;
                progressText.text = $"{val}%";
            }
            if (curExpProgress >= 1)
            {
                //Todo:完成
                Debug.Log("UIProgressOneLevel Complete");
                EventUtil.DispatchEvent(EProgressEventType.Complete);
            }
        }
    }
}
