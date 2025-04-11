using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utp;
using Mirror;
using UnityEngine.SceneManagement;

namespace BNG
{
    public class LobbyNetworkUI : NetworkBehaviour
    {
        [Header("Network Manager")]
        [SerializeField] private RelayNetworkManager networkManager;
        
        [Header("UI References")]
        [SerializeField] private InputField playerNameInput;
        [SerializeField] private InputField roomCodeInput;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private ScreenFader screenFader;
        [SerializeField] private SceneLoader sceneLoader;
        
        private bool clientConnected;

        private void Start()
        {
            if (screenFader == null)
                screenFader = FindObjectOfType<ScreenFader>();
        }

        /// <summary>
        /// Called when hosting a new lobby.
        /// </summary>
        public void OnHostButton()
        {
            statusText.text = "Hosting a new lobby...\n";
            
            string playerName = playerNameInput.text.Trim();
            if (string.IsNullOrEmpty(playerName))
            {
                statusText.text = "Please enter a player name.\n";
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName);

            if (!UnityAuthInitializer.IsAuthenticated)
            {
                Debug.LogError("The FUCKING AUTO AUTHENTICATION IS NOT WORKING!!! RQAAAAAAHHFH, i swear i am gonna tell my mom");
                statusText.text = "Not Authenticated... You cannot do anything about it yet!\n";
                return;
            }
            
            // Need to change this so i can show the players in the lobby tab and let people connect before the game starts :D
            // Also need to do a loading screen of some sort here
            networkManager.StartRelayHost(4);
        }

        /// <summary>
        /// Called when connecting to an existing lobby via join code.
        /// </summary>
        public void OnJoinButton()
        {
            statusText.text = "Joining lobby...\n";
            
            string joinCode = roomCodeInput.text.Trim();
            string playerName = playerNameInput.text;
            
            if (string.IsNullOrEmpty(joinCode) || joinCode.Length != 6)
            {
                statusText.text = "Please enter a valid join code.\n";
                return;
            }
            
            if (string.IsNullOrEmpty(playerName))
            {
                statusText.text = "Please enter a player name.\n";
                return;
            }
            
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.SetString("RoomCode", joinCode);
            
            if (!UnityAuthInitializer.IsAuthenticated)
            {
                Debug.LogError("The FUCKING AUTO AUTHENTICATION IS NOT WORKING!!! RQAAAAAAHHFH, i swear i am gonna tell my mom");
                statusText.text = "Not Authenticated... You cannot do anything about it yet!\n";
                return;
            }
            
            // Same jist as before, change this to work with the lobby tab and not just start the game
            // And ofc the loading screen
            networkManager.JoinRelayServer(joinCode);
        }
        
        /// <summary>
        /// Disconnects the client or host from the network session.
        /// </summary>
        public void OnDisconnectButton()
        {
            if (isClient) networkManager.StopClient();
            if (isServer) networkManager.StopHost();
        }
        
        /// <summary>
        /// Loads the offline single-player/test scene.
        /// </summary>
        public void OnOfflineButton()
        {
            // Should probably change the way im doing this cuz it works but it very janky... should probably do this with a sceneobject...
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(2));
            sceneLoader.LoadScene(sceneName);
        }

        /// <summary>
        /// Callback when client starts (Mirror hook).
        /// </summary>
        public override void OnStartClient()
        {
            Debug.Log("Client started.");
            clientConnected = true;
            statusText.text += "Client connected.\n";
        }
        
        /// <summary>
        /// Callback when server starts (Mirror hook).
        /// </summary>
        public override void OnStartServer()
        {
            Debug.Log("Server started.");
            statusText.text = "Server is live.\n";
        }

        /// <summary>
        /// Callback when the client disconnects.
        /// </summary>
        public override void OnStopClient()
        {
            Debug.Log("Client disconnected.");
            clientConnected = false;
            statusText.text += "Disconnected from lobby.\n";
        }
    }
}