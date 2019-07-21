using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DigitalDreams.UI
{
    public class DD_UI_System : MonoBehaviour
    {

        #region Variables
        [Header("Main Properties")]
        public DD_UI_Screen m_StartScreen;

        [Header("System Events")]
        public UnityEvent onSwitchScreen = new UnityEvent();

        [Header("Fader Properties")]
        public Image m_Fader;
        public float m_fadeInDuration = 1f;
        public float m_fadeOutDuration = 1f;

        private Component[] screens = new Component[0];

        private DD_UI_Screen previousScreen;
        public DD_UI_Screen PreviousScreen { get { return previousScreen; } }

        private DD_UI_Screen currentScreen;
        public DD_UI_Screen CurrentScreen { get { return currentScreen; } }
        #endregion



        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            print("UI_System Starting...");
            screens = GetComponentsInChildren<DD_UI_Screen>(true);

            if(m_StartScreen)
            {
                SwitchScreen(m_StartScreen);
            }

            if (m_Fader)
            {
                m_Fader.gameObject.SetActive(true);
            }
            FadeIn();
        }
        #endregion



        #region Helper Methods
        public void SwitchScreen(DD_UI_Screen aScreen)
        {
            if(aScreen)
            {
                if(currentScreen)
                {
                    currentScreen.CloseScreen();
                    previousScreen = currentScreen;
                }

                currentScreen = aScreen;
                currentScreen.gameObject.SetActive(true);
                currentScreen.StartScreen();

                if (onSwitchScreen != null)
                {
                    onSwitchScreen.Invoke();
                }
            }
        }

        public void FadeIn()
        {
            if(m_Fader)
            {
                m_Fader.CrossFadeAlpha(0f, m_fadeInDuration, false);
            }
        }

        public void FadeOut()
        {
            if (m_Fader)
            {
                m_Fader.CrossFadeAlpha(1f, m_fadeOutDuration, false);
            }
        }

        public void GoToPreviousScreen()
        {
            if(previousScreen)
            {
                SwitchScreen(previousScreen);
            }
        }

        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(WaitToLoadScene(sceneIndex));
        }

        IEnumerator WaitToLoadScene(int sceneIndex)
        {
            yield return null;
        }
        #endregion
    }
}
