using System;
using System.Collections.Generic;
using System.IO;
using BWModLoader;
using UnityEngine;

namespace ShipCustomizationExtended
{
    [Mod]
    public class ModBaseClass : MonoBehaviour
    {
        // Mod update loop delay
        private const float TickDelay = 5f;

        // Contains processed Transform-string pairs to avoid unnecessary customization application
        private readonly Dictionary<Transform, string> processedNames = new Dictionary<Transform, string>();

        private readonly ModLogger logger = new ModLogger("[ShipCustomizationExtended]",
            $"{ModLoader.LogPath}\\ShipCustomizationExtended.txt");

        private enum LoggingLevel
        {
            Debug = 0,
            Info,
            Warning,
            Error,
            NoLog
        }

        // Modify this variable to change logging level
        private LoggingLevel loggingLevel = LoggingLevel.Info;

        private void Log(string message)
        {
            if (loggingLevel > LoggingLevel.Info) return;
            logger.Log($"[Info] {message}");
        }

        private void LogDebug(string message)
        {
            if (loggingLevel > LoggingLevel.Debug) return;
            logger.Log($"[DEBUG] {message}");
        }

        private void LogWarning(string message)
        {
            if (loggingLevel > LoggingLevel.Warning) return;
            logger.Log($"[WARNING] {message}");
        }

        private void LogError(string message)
        {
            if (loggingLevel > LoggingLevel.Error) return;
            logger.Log($"[ERROR] {message}");
        }

        // Get ship name from ship's transform
        private string GetShipNameFromTransform(Transform shipTransform) =>
            GameMode.Instance.teamNames[GameMode.getParentIndex(shipTransform)];

        private void Start()
        {
            Log(
                "Thanks for using Ship Customization Extended mod. If you encounter any errors please ping @David_548219#5947.");
            Log("Keymaps:");
            Log("Home - force update on next tick");

            InvokeRepeating(nameof(ShipLoop), TickDelay, TickDelay);
            LogDebug($"Initialized update loop with delay {TickDelay}s");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Log("Forced update on all ships");
                processedNames.Clear();
            }
        }

        // Primary mod loop function
        private void ShipLoop()
        {
            LogDebug($"Tick, processing {GameMode.Instance.teamParents.Length} ships");
            try
            {
                // Loop through all ships (those include invisible disabled ships)
                foreach (Transform shipTransform in GameMode.Instance.teamParents)
                {
                    string shipName = GetShipNameFromTransform(shipTransform);

                    // Check if the ship was already customized with the name it has
                    if (processedNames.ContainsKey(shipTransform) && processedNames[shipTransform] == shipName)
                    {
                        LogDebug($"Skipped processing {shipName}");
                        continue;
                    }

                    // We go through the name string, until we encounter character '#', then we parse operators and variables
                    bool encounteredBeginner = false;
                    for (int i = 0; i < shipName.Length; ++i)
                    {
                        if (shipName[i] == '#')
                        {
                            encounteredBeginner = true;
                            continue;
                        }

                        if (!encounteredBeginner) continue;

                        LogDebug($"Processing {shipName}...");
                        char op = shipName[i];
                        char val = i < shipName.Length - 1 ? shipName[i + 1] : ' ';
                        if (ProcessOperator(shipTransform, op, val)) ++i;
                    }

                    processedNames[shipTransform] = shipName;
                }
            }
            catch (Exception e)
            {
                LogError($"{e.Source}: {e.Message} @ {e.StackTrace}");
            }
        }

        /// <summary>
        /// Process operator char
        /// </summary>
        /// <param name="shipTransform">Transform of affected ship</param>
        /// <param name="op">Operator char</param>
        /// <param name="val">Value char</param>
        /// <returns>Returns true if value char was used</returns>
        private bool ProcessOperator(Transform shipTransform, char op, char val)
        {
            try
            {
                if (op == 'S')
                {
                    // Sail color
                    if (ProcessColor(val, out Color sailColor))
                    {
                        ApplySailColor(shipTransform, sailColor);
                    }

                    return true;
                }

                if (op == 'L')
                {
                    // Lighting color
                    if (ProcessColor(val, out Color lightColor))
                    {
                        ApplyLightColor(shipTransform, lightColor);
                    }

                    return true;
                }

                if (op == 'R')
                {
                    // Rigging color
                    if (ProcessColor(val, out Color sailColor))
                    {
                        ApplyRiggingColor(shipTransform, sailColor);
                    }

                    return true;
                }

                if (op == 'F')
                {
                    // Flag texture
                    if (ProcessFlagTexture(val, out Texture2D flagTexture))
                    {
                        ApplyCustomFlag(shipTransform, flagTexture);
                    }

                    return true;
                }

                if (op == 'D')
                {
                    // Sail texture
                    if (ProcessFlagTexture(val, out Texture2D sailTexture))
                    {
                        ApplySailTexture(shipTransform, sailTexture);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to process operator {op}");
                LogError($"{e.Source}: {e.Message} @ {e.StackTrace}");
            }

            LogWarning($"Unknown operator {op}");
            return false;
        }

        /// <summary>
        /// Process color value char
        /// </summary>
        /// <param name="val">Color value char</param>
        /// <param name="color">Selected color destination</param>
        /// <returns>Returns true if color exists</returns>
        private bool ProcessColor(char val, out Color color)
        {
            switch (val)
            {
                case 'b':
                    color = Color.black;
                    return true;
                case 'l':
                    color = Color.blue;
                    return true;
                case 'c':
                    color = Color.cyan;
                    return true;
                case 'a':
                    color = Color.gray;
                    return true;
                case 'g':
                    color = Color.green;
                    return true;
                case 'm':
                    color = Color.magenta;
                    return true;
                case 'r':
                    color = Color.red;
                    return true;
                case 'w':
                    color = Color.white;
                    return true;
                case 'y':
                    color = Color.yellow;
                    return true;
                case 'o':
                    color = new Color(1f, 0.5f, 0f);
                    return true;
                default:
                {
                    LogWarning($"Unknown color {val}");
                    color = Color.white;
                    return false;
                }
            }
        }

        /// <summary>
        /// Process flag texture value char
        /// </summary>
        /// <param name="val">Flag texture value char</param>
        /// <param name="texture">Selected flag texture destination</param>
        /// <returns>Returns true if flag texture exists</returns>
        private bool ProcessFlagTexture(char val, out Texture2D texture)
        {
            const string flagsFolder = "flags";
            const int flagWidth = 768;
            const int flagHeight = 520;

            switch (val)
            {
                case 'f':
                    texture = LoadTexture($"{flagsFolder}\\Frog.png", flagWidth, flagHeight);
                    return true;
                case 'p':
                    texture = LoadTexture($"{flagsFolder}\\PirateFrog.png", flagWidth, flagHeight);
                    return true;
                case 's':
                    texture = LoadTexture($"{flagsFolder}\\Ussr.png", flagWidth, flagHeight);
                    return true;
                case 'g':
                    texture = LoadTexture($"{flagsFolder}\\GreatTortuga.png", flagWidth, flagHeight);
                    return true;
                case 'u':
                    texture = LoadTexture($"{flagsFolder}\\UnityOfComrades.png", flagWidth, flagHeight);
                    return true;
                default:
                {
                    LogWarning($"Unknown color {val}");
                    texture = Texture2D.whiteTexture;
                    return false;
                }
            }
        }

        /// <summary>
        /// Loads texture from file system
        /// </summary>
        /// <param name="textureName">Texture file name</param>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <returns>Returns loaded texture, or Texture2D.whiteTexture if unsuccessful</returns>
        private Texture2D LoadTexture(string textureName, int width, int height)
        {
            string filePath = $"{ModLoader.AssetsPath}\\548219\\{textureName}";

            if (!File.Exists(filePath))
            {
                LogWarning($"Texture not found @ {textureName}, replacing with default");
                return Texture2D.whiteTexture;
            }

            LogDebug($"Loading texture from {filePath}");
            byte[] fileData = File.ReadAllBytes(filePath);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.LoadImage(fileData);
            LogDebug("Texture loaded");
            return texture;
        }

        /// <summary>
        /// Applies custom sail color to the ship
        /// </summary>
        /// <param name="shipTransform">Affected ship transform</param>
        /// <param name="color">New sail color</param>
        private void ApplySailColor(Transform shipTransform, Color color)
        {
            LogDebug($"Applying sail color to {shipTransform.name}");
            
            Renderer[] renderers = shipTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                // Sail related renderers contain "sail" in the name
                LogDebug($"Renderer name: {renderer.name}");
                if (!renderer.name.ToLower().Contains("sail"))
                {
                    LogDebug("Skipping renderer");
                    continue;
                }

                LogDebug($"Materials in the renderer: {renderer.materials.Length}");
                foreach (Material material in renderer.materials)
                {
                    LogDebug($"Material name: {material.name}");
                    material.color = color;
                }
            }

            Log($"Sails repainted for {GetShipNameFromTransform(shipTransform)}");
        }

        /// <summary>
        /// Applies custom lighting color to the ship
        /// </summary>
        /// <param name="shipTransform">Affected ship transform</param>
        /// <param name="color">New lighting color</param>
        private void ApplyLightColor(Transform shipTransform, Color color)
        {
            LogDebug($"Applying light color to {shipTransform.name}");

            Light[] lights = shipTransform.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                LogDebug($"Light name: {light.gameObject.name}");
                light.color = color;
            }

            Log($"Light color changed for {GetShipNameFromTransform(shipTransform)}");
        }

        /// <summary>
        /// Applies custom rigging color to the ship
        /// </summary>
        /// <param name="shipTransform">Affected ship transform</param>
        /// <param name="color">New rigging color</param>
        private void ApplyRiggingColor(Transform shipTransform, Color color)
        {
            LogDebug($"Applying sail color to {shipTransform.name}");

            Renderer[] renderers = shipTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                // Sail related renderers contain "rigging" or "rope" in the name
                LogDebug($"Renderer name: {renderer.name}");
                if (!(renderer.name.ToLower().Contains("rigging") || renderer.name.ToLower().Contains("rope")))
                {
                    LogDebug("Skipping renderer");
                    continue;
                }

                LogDebug($"Materials in the renderer: {renderer.materials.Length}");
                foreach (Material material in renderer.materials)
                {
                    LogDebug($"Material name: {material.name}");
                    material.color = color;
                }
            }

            Log($"Rigging repainted for {GetShipNameFromTransform(shipTransform)}");
        }

        /// <summary>
        /// Applies custom flag texture to the ship
        /// </summary>
        /// <param name="shipTransform">Affected ship transform</param>
        /// <param name="flagTexture">New flag texture</param>
        private void ApplyCustomFlag(Transform shipTransform, Texture2D flagTexture)
        {
            LogDebug($"Applying custom flag to {shipTransform.name}");

            Renderer[] renderers = shipTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                // Sail related renderers contain "flag" in the name
                LogDebug($"Renderer name: {renderer.name}");
                if (!renderer.name.ToLower().Contains("flag"))
                {
                    LogDebug("Skipping renderer");
                    continue;
                }

                LogDebug($"Materials in the renderer: {renderer.materials.Length}");
                foreach (Material material in renderer.materials)
                {
                    LogDebug($"Material name: {material.name}");
                    material.mainTexture = flagTexture;
                }
            }
            
            Log($"Flag retextured for {GetShipNameFromTransform(shipTransform)}");
        }

        /// <summary>
        /// Applies custom sail texture to the ship
        /// </summary>
        /// <param name="shipTransform">Affected ship transform</param>
        /// <param name="sailTexture">New sail texture</param>
        private void ApplySailTexture(Transform shipTransform, Texture2D sailTexture)
        {
            LogDebug($"Applying sail texture to {shipTransform.name}");
            
            Renderer[] renderers = shipTransform.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                // Sail related renderers contain "sail" in the name
                LogDebug($"Renderer name: {renderer.name}");
                if (!renderer.name.ToLower().Contains("sail"))
                {
                    LogDebug("Skipping renderer");
                    continue;
                }

                LogDebug($"Materials in the renderer: {renderer.materials.Length}");
                foreach (Material material in renderer.materials)
                {
                    LogDebug($"Material name: {material.name}");
                    material.mainTexture = sailTexture;
                }
            }

            Log($"Sails retextured for {GetShipNameFromTransform(shipTransform)}");
        }
    }
}