using System;
using System.Collections.Generic;
using Kopernicus;
using UnityEngine;

namespace SciRev
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Applicator : MonoBehaviour
    {
        void Start()
        {
            // Create the logger
            Logger logger = new Logger();

            // Register our logger
            ParserOptions.Register("SciRev", new ParserOptions.Data
            {
                ErrorCallback = e => Logger.Active.LogException(e),
                LogCallback = e => Logger.Active.Log(e)
            });

            // Load the experiment nodes and get their RESULT configs
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("EXPERIMENT_DEFINITION");
            for (Int32 i = 0; i < nodes.Length; i++)
            {
                // Summon the logger
                String experimentName = nodes[i].GetValue("id");
                logger.SetFilename(experimentName);
                logger.SetAsActive();

                try
                {
                    // Grab the experiment results
                    ConfigNode results = nodes[i].GetNode("RESULTS");
                    ResultLoader loader = Parser.CreateObjectFromConfigNode<ResultLoader>(results, "SciRev");
                    ApplyDefinitions(loader.Situations, ref results);
                }
                catch (Exception e)
                {
                    logger.LogException(e);
                    logger.Close(); //implicit flush
                    Logger.Default.Log("Failed to load Experiment: " + experimentName + ": " + e.Message);
                    throw new Exception("Failed to load Experiment: " + experimentName);
                }
            }
        }

        private void ApplyDefinitions(List<SituationLoader> loaders, ref ConfigNode results, String prefix = "")
        {
            // If there is nothing we can apply, abort
            if (loaders == null)
            {
                return;
            }
            
            // Apply all the loaded definitions to the node
            for (Int32 i = 0; i < loaders.Count; i++)
            {
                String key = prefix + loaders[i].Name;

                // Recursively parse nodes
                ApplyDefinitions(loaders[i].Situations, ref results, key);

                // If there are no values, we have nothing to do anymore
                if (loaders[i].Values == null) continue;
                
                // Add the values
                for (Int32 j = 0; j < loaders[i].Values.Count; j++)
                {
                    results.AddValue(key, loaders[i].Values[j]);
                    Logger.Active.Log("Adding " + key + " to ScienceDefs");
                }
            }
        }
    }
}