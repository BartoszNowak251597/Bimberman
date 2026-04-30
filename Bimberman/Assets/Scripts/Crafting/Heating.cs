using TMPro;
using UnityEngine;

namespace Crafting
{
    public class Heating : MonoBehaviour
    {
        [Header("Mixture")]
        [SerializeField] private BottleData currentData;

        [Header("Temperature")]
        [SerializeField] private float roomTemperature = 20f;
        [SerializeField] private float maxTemperature = 120f;
        [SerializeField] private float temperaturePerBlow = 5f;

        [Header("Bellow")]
        [SerializeField] private float blowCooldown = 1f;

        [Header("Target")]
        [SerializeField] private float minCorrectTemperature = 70f;
        [SerializeField] private float maxCorrectTemperature = 80f;

        [Header("Cooling")]
        [SerializeField] private float coolingStartDelay = 1f;
        [SerializeField] private float coolingInterval = 0.25f;
        [SerializeField] private float coolingAmount = 0.5f;

        [Header("Timer")]
        [SerializeField] private float requiredCorrectHeatTime = 5f;
        private float _correctHeatTimer = 0f;

        [Header("Penalty")]
        [SerializeField] private float overheatPenaltyPerSecond = 12f;

        [Header("UI")]
        [SerializeField] private TMP_Text bellowText;
        [SerializeField] private TMP_Text currentTemperatureText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text qualityText;

        [Header("Lamp")]
        [SerializeField] private Renderer temperatureLampRenderer;
        [SerializeField] private Light temperatureLampLight;
        [SerializeField] private float temperaturePerLightStep = 10f;
        [SerializeField] private float lightIntensityPerStep = 0.5f;
        [SerializeField] private float maxLightIntensity = 8f;

        [Header("Colors")]
        [SerializeField] private Color noMixtureColor = Color.black;
        [SerializeField] private Color heatingColor = new Color(0.3f, 0.8f, 1f);
        [SerializeField] private Color correctTemperatureColor = Color.green;
        [SerializeField] private Color burnedColor = Color.red;
        [SerializeField] private Color finishedColor = new Color(0.8f, 0.3f, 1f);

        private float timeSinceLastBlow = 999f;
        private float coolingTimer = 0f;

        private void Awake()
        {
            currentData = null;
            _correctHeatTimer = 0f;
            ResetLampLight();
        }

        private void Update()
        {
            if (currentData == null)
            {
                _correctHeatTimer = 0f;
                UpdateVisuals();
                return;
            }

            if (currentData.stage == BrewStage.Finished)
            {
                UpdateVisuals();
                return;
            }

            UpdateCooling();
            UpdateState();
            UpdateCorrectHeatTimer();
            UpdateOverheatPenalty();
            UpdateVisuals();
        }

        public void SetMixture(BottleData data)
        {
            currentData = data;

            _correctHeatTimer = 0f;
            timeSinceLastBlow = 999f;
            coolingTimer = 0f;

            if (currentData != null)
            {
                currentData.stage = BrewStage.Heating;
                currentData.qualityPercent = 100f;
            }

            UpdateVisuals();
        }
        
        public BottleData GetMixture()
        {
            return currentData;
        }

        public bool HasMixture()
        {
            return currentData != null
                && currentData.usedIngredients != null
                && currentData.usedIngredients.Count > 0;
        }

        public bool CanBlow()
        {
            return HasMixture()
                && currentData.stage != BrewStage.Finished
                && timeSinceLastBlow >= blowCooldown;
        }

        public void Blow()
        {
            if (!CanBlow())
                return;

            currentData.currentTemperature += temperaturePerBlow;

            currentData.currentTemperature = Mathf.Clamp(
                currentData.currentTemperature,
                roomTemperature,
                maxTemperature
            );

            timeSinceLastBlow = 0f;
            coolingTimer = 0f;

            UpdateState();
            UpdateVisuals();
        }

        private void UpdateCooling()
        {
            timeSinceLastBlow += Time.deltaTime;

            if (timeSinceLastBlow < coolingStartDelay)
                return;

            coolingTimer += Time.deltaTime;

            if (coolingTimer < coolingInterval)
                return;

            coolingTimer = 0f;

            currentData.currentTemperature -= coolingAmount;

            currentData.currentTemperature = Mathf.Max(
                currentData.currentTemperature,
                roomTemperature
            );
        }

        private void UpdateState()
        {
            if (currentData == null)
                return;

            if (currentData.stage == BrewStage.Finished)
                return;

            if (currentData.currentTemperature > maxCorrectTemperature)
            {
                currentData.stage = BrewStage.Burned;
            }
            else if (
                currentData.currentTemperature >= minCorrectTemperature &&
                currentData.currentTemperature <= maxCorrectTemperature
            )
            {
                currentData.stage = BrewStage.HeatedCorrectly;
            }
            else
            {
                currentData.stage = BrewStage.Heating;
            }
        }

        private void UpdateCorrectHeatTimer()
        {
            if (currentData == null)
                return;

            if (currentData.stage == BrewStage.HeatedCorrectly)
            {
                _correctHeatTimer += Time.deltaTime;

                if (_correctHeatTimer >= requiredCorrectHeatTime)
                {
                    _correctHeatTimer = requiredCorrectHeatTime;
                    currentData.stage = BrewStage.Finished;
                    currentData.CalculateBottleYield();

                    Debug.Log($"Heating finished. Quality: {currentData.qualityPercent:0}%");
                }
            }
            else if (currentData.stage == BrewStage.Heating)
            {
                _correctHeatTimer -= Time.deltaTime * 0.5f;
                _correctHeatTimer = Mathf.Max(0f, _correctHeatTimer);
            }
        }

        private void UpdateOverheatPenalty()
        {
            if (currentData == null)
                return;

            if (currentData.stage != BrewStage.Burned)
                return;

            currentData.qualityPercent -= overheatPenaltyPerSecond * Time.deltaTime;
            currentData.qualityPercent = Mathf.Clamp(currentData.qualityPercent, 0f, 100f);
        }

        private void UpdateVisuals()
        {
            if (bellowText != null)
            {
                if (currentData == null)
                {
                    bellowText.text = "";
                }
                else if (currentData.stage == BrewStage.Finished)
                {
                    bellowText.text = "Finished";
                }
                else if (CanBlow())
                {
                    bellowText.text = "Press to blow";
                }
                else
                {
                    bellowText.text = "Wait";
                }
            }

            if (currentTemperatureText != null)
            {
                if (currentData != null)
                {
                    float roundedTemperature = Mathf.Round(currentData.currentTemperature * 2f) / 2f;

                    currentTemperatureText.text =
                        $"Temperature: {roundedTemperature:0.#}°C\n" +
                        $"Target: {minCorrectTemperature:0.#}-{maxCorrectTemperature:0.#}°C";
                }
                else
                {
                    currentTemperatureText.text = "";
                }
            }

            if (timerText != null)
            {
                timerText.text = currentData != null
                    ? $"Time: {_correctHeatTimer:0.0}/{requiredCorrectHeatTime:0.0}s"
                    : "";
            }

            if (qualityText != null)
            {
                qualityText.text = currentData != null
                    ? $"Quality: {currentData.qualityPercent:0}%"
                    : "";
            }

            UpdateLampColor();
            UpdateLampLight();
        }

        private void UpdateLampColor()
        {
            if (temperatureLampRenderer == null)
                return;

            if (currentData == null)
            {
                temperatureLampRenderer.material.color = noMixtureColor;
                return;
            }

            if (currentData.stage == BrewStage.Finished)
            {
                temperatureLampRenderer.material.color = finishedColor;
            }
            else if (currentData.stage == BrewStage.HeatedCorrectly)
            {
                temperatureLampRenderer.material.color = correctTemperatureColor;
            }
            else if (currentData.stage == BrewStage.Burned)
            {
                temperatureLampRenderer.material.color = burnedColor;
            }
            else
            {
                temperatureLampRenderer.material.color = heatingColor;
            }
        }

        private void UpdateLampLight()
        {
            if (temperatureLampLight == null)
                return;

            if (currentData == null)
            {
                ResetLampLight();
                return;
            }

            float temperatureAboveRoom = Mathf.Max(
                0f,
                currentData.currentTemperature - roomTemperature
            );

            int steps = Mathf.FloorToInt(temperatureAboveRoom / temperaturePerLightStep);
            float targetIntensity = steps * lightIntensityPerStep;

            temperatureLampLight.intensity = Mathf.Clamp(
                targetIntensity,
                0f,
                maxLightIntensity
            );
        }

        private void ResetLampLight()
        {
            if (temperatureLampLight != null)
            {
                temperatureLampLight.intensity = 0f;
            }
        }
    }
}