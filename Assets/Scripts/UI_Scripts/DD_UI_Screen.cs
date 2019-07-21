using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DigitalDreams.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class DD_UI_Screen : MonoBehaviour
    {
        #region Variables
        [Header("Main Properties")]
        public Selectable m_StartSelectable;

        [Header("Screen Events")]
        public UnityEvent onScreenStart = new UnityEvent();
        public UnityEvent onScreenClose = new UnityEvent();

        private Animator animator;
        #endregion



        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            //animator = GetComponent<Animator>();

            if(m_StartSelectable)
            {
                EventSystem.current.SetSelectedGameObject(m_StartSelectable.gameObject);
            }
        }
        #endregion


        #region Helper Methods
        public virtual void StartScreen()
        {
            if(onScreenStart != null)
            {
                onScreenStart.Invoke();
            }

            //HandleAnimator("show");
            this.GetComponent<CanvasGroup>().alpha = 1;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.GetComponent<CanvasGroup>().interactable = true;
        }

        public virtual void CloseScreen()
        {
            if (onScreenClose != null)
            {
                onScreenClose.Invoke();
            }

            //HandleAnimator("hide");
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            this.gameObject.GetComponent<CanvasGroup>().interactable = false;
        }

        void HandleAnimator(string aTrigger)
        {
            if (animator)
            {
                animator.SetTrigger(aTrigger);
            }
        }
        #endregion
    }
}