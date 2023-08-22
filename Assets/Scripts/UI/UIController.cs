using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GW.UI
{
    public class UIController : MonoBehaviour
    {
        #region Variables

        [SerializeField] Slider rotationSlider = null;
        [SerializeField] GameObject surface;
        [SerializeField] GameObject hologram;

        [Space(20)]
        [SerializeField] Button surfaceBtn;
        [SerializeField] Button hologramBtn;

        #endregion

        #region Private Variable

        [HideInInspector]
        public float longitude;

        #endregion

        #region Private Functions

        private void Start()
        {
            rotationSlider.value = 0;
            longitude = rotationSlider.value;
        }

        private void Update()
        {
            longitude = rotationSlider.value;
            this.transform.rotation = Quaternion.Euler(0, longitude, 0);
        }

        #endregion

        #region Public Functions

        public void Hologram()
        {
            hologram.gameObject.SetActive(true);
            surface.gameObject.SetActive(false);

            hologramBtn.gameObject.SetActive(false);
            surfaceBtn.gameObject.SetActive(true);
        }

        public void Surface()
        {
            hologram.gameObject.SetActive(false);
            surface.gameObject.SetActive(true);

            hologramBtn.gameObject.SetActive(true);
            surfaceBtn.gameObject.SetActive(false);
        }

        #endregion
    }

}